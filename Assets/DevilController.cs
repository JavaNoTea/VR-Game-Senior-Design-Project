using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Basic implementations of the cubic bezier and its derivative for smoothing out movement
static class BezierCurves {
    public static float CubicBezier(float t, float y0, float y1, float y2, float y3) {
        float tm = 1 - t;
        return tm*tm*tm*y0 + 3*tm*tm*t*y1 + 3*tm*t*t*y2 + t*t*t*y3;
    }

    public static float CubicBezierDeriv(float t, float y0, float y1, float y2, float y3) {
        float tm = 1 - t;
        return -3*tm*tm*y0 + 3*y1*(3*t*t-4*t+1) + 3*y2*(-3*t*t+2*t) + 3*t*t*y3;
    }
}

public class DevilController : MonoBehaviour, IArrowHittable
{
    Rigidbody rb;
    [SerializeField] DevilState currentState = DevilState.Idle;
    [SerializeField] float trackingTimeSec = 5.0f;
    [SerializeField] Transform target;

    //[SerializeField] bool isArrowClose = true;
    [SerializeField] float playerCloseDistance = 20.0f;
    [SerializeField] bool isRecentlyInjured = false;
    [SerializeField] float fireballLaunchRange = 12.0f;
    [SerializeField] float closingDistance = 10.0f;
    [SerializeField] float speed = 1;
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] GameObject stone;
    [SerializeField] float fireballSpeed = 300.0f;
    Transform actualTransform;
    Vector3 originalPosition;

    [SerializeField] bool patrolling = true;
    //[SerializeField] float wingSpeedBoost = 1.0f;

    float startTime;

    private IEnumerator fireballCoroutine = null;
    private IEnumerator seekingCoroutine = null;

    private Animator animator;

    private Renderer spriteRenderer;

    [SerializeField] private float arrowImpactForce = 10.0f;
    [SerializeField] AudioSource impactSound;
    [SerializeField] AudioSource fireballSound;
    [SerializeField] int health = 3;
    public void Hit(Arrow arrow)
    {
        health--;
        isRecentlyInjured = true;
        StartCoroutine(SetUninjured());
        Destroy(arrow);
        if (health <= 0) {
            animator.SetTrigger("Die");
            animator.applyRootMotion = false;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.AddForce(arrow.transform.forward * arrowImpactForce, ForceMode.Impulse);

            if (stone)
            {
                stone.SetActive(true);
            }
        }
        impactSound.Play();
        currentState = DevilState.Evading;
    }

    IEnumerator SetUninjured() {
        yield return new WaitForSeconds(30.0f);
        isRecentlyInjured = false;
    }

    void OnDeath() {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    NavMeshAgent agent;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();
        var cuteDevil = transform.GetChild(0);
        spriteRenderer = cuteDevil.GetComponent<Renderer>();
        agent = GetComponent<NavMeshAgent>();
        originalPosition = transform.position;
        actualTransform = transform.Find("Collider");
    }

    bool isArrowCloseMethod() {
        var collisions = Physics.OverlapSphere(this.actualTransform.position, 2, 1 << 2, QueryTriggerInteraction.Ignore);
        return collisions.Length > 0;
    }

    IEnumerator wanderCoroutine = null;

    float turnSpeedScale = 3.0f;
    void turnTo(Vector3 position)
    {
        var direction = Vector3.RotateTowards(transform.forward, position - transform.position, turnSpeedScale * speed * Time.deltaTime, 0.0f);
        Debug.DrawRay(transform.position, direction, Color.red);
        transform.rotation = Quaternion.LookRotation(direction);
    }
    void turnToPlayer() {
        var playerLocation = new Vector3(target.position.x, target.position.y + 1.1f, target.position.z);
        turnTo(playerLocation);
    }

    void Update()
    {
        var isArrowClose = isArrowCloseMethod();
        var distanceFromPlayer = Vector3.Distance(transform.position, target.transform.position);
        var isPlayerClose = distanceFromPlayer <= playerCloseDistance;
        var isPlayerInRange = distanceFromPlayer <= fireballLaunchRange;
        var isVisibleToPlayer = spriteRenderer.isVisible;
        // Debug.Log("=======");
        // Debug.Log(distanceFromPlayer);
        // Debug.Log(isPlayerClose);
        // Debug.Log(isPlayerInRange);
        // Debug.Log(isVisibleToPlayer);
        // Debug.Log(isRecentlyInjured);
        // Debug.Log("=======");
        switch (currentState)
        {
            case DevilState.Idle:
                if (isArrowClose)
                {
                    currentState = DevilState.Seeking;
                    startTime = Time.time;
                    StartCoroutine(seekingCoroutine = seek());
                }
                else if (isPlayerClose)
                {
                    currentState = DevilState.Seeking;
                    startTime = Time.time;
                    StartCoroutine(seekingCoroutine = seek());
                }
                else {
                    if (patrolling && wanderCoroutine == null) {
                        StartCoroutine(wanderCoroutine = wander());
                    }
                }
                break;
            case DevilState.Seeking:
                if (wanderCoroutine != null) {
                    StopCoroutine(wanderCoroutine);
                    wanderCoroutine = null;
                }
                if (isPlayerInRange)
                {
                    currentState = DevilState.Attacking;
                    StopCoroutine(seekingCoroutine);
                    seekingCoroutine = null;
                }
                else if (seekingCoroutine == null)
                {
                    currentState = DevilState.Idle;
                }
                else
                {
                    agent.destination = new Vector3(target.position.x, transform.position.y, target.position.z);
                    agent.stoppingDistance = closingDistance;
                }
                break;
            case DevilState.Attacking:
                turnToPlayer();
                if (!isPlayerInRange)
                {
                    currentState = DevilState.Seeking;
                    startTime = Time.time;
                    if (fireballCoroutine != null) {
                        StopCoroutine(fireballCoroutine);
                        fireballCoroutine = null;
                    }
                }
                else
                {
                    if (fireballCoroutine == null) {
                        StartCoroutine(fireballCoroutine = attack());
                    }
                }
                break;
            case DevilState.Evading:
                if (fireballCoroutine != null) {
                    StopCoroutine(fireballCoroutine);
                    fireballCoroutine = null;
                }
                if (wanderCoroutine != null) {
                    StopCoroutine(wanderCoroutine);
                    wanderCoroutine = null;
                }
                if (!isVisibleToPlayer)
                {
                    currentState = DevilState.Attacking;
                }
                else
                {
                    evade();
                }
                break;
            default:
                Debug.Log("Invalid state was reached in DevilController.cs");
                break;
        }
    }
    IEnumerator attack() {
        while (true) {
            animator.SetTrigger("Projectile Attack");
            yield return new WaitForSeconds(2.0f);
        }
    }

    int wanderCount = 0;
    IEnumerator wander() {
        if (wanderCount >= 3) {
            agent.destination = originalPosition;
            agent.stoppingDistance = 1.0f;
            wanderCount = 0;
        } else {
            var randomXPos = Random.Range(rangeLower, rangeUpper) * (Random.Range(0, 2) == 0 ? 1 : -1);
            var randomZPos = Random.Range(rangeLower, rangeUpper) * (Random.Range(0, 2) == 0 ? 1 : -1);
            moveToward(transform.position + new Vector3(randomXPos, 0, randomZPos));
            agent.stoppingDistance = 0.0f;
        }
        yield return new WaitForSeconds(Random.Range(2.0f, 5.0f));
        wanderCount++;
        wanderCoroutine = null;
    }

    void OnProjectileAttack() {
        shootFireball(target.transform.position);
    }

    private Vector3 targetPositionAfterEvasion = Vector3.zero;
    [SerializeField] float rangeLower = 2.0f;
    [SerializeField] float rangeUpper = 5.0f;
    IEnumerator evasionCoroutine = null;
    void evade()
    {
        if (targetPositionAfterEvasion == Vector3.zero) {
            var randomXPos = Random.Range(rangeLower, rangeUpper) * (Random.Range(0, 2) == 0 ? 1 : -1);
            var randomZPos = Random.Range(rangeLower, rangeUpper) * (Random.Range(0, 2) == 0 ? 1 : -1);
            targetPositionAfterEvasion = new Vector3(target.position.x + randomXPos, transform.position.y, target.position.z + randomZPos);
            StartCoroutine(evasionCoroutine = evasionTimer());
        }
        Debug.DrawLine(transform.position, targetPositionAfterEvasion, Color.blue);
        moveToward(targetPositionAfterEvasion, 2.0f * speed);
        if ((transform.position - targetPositionAfterEvasion).magnitude < 0.01f) {
            currentState = DevilState.Attacking;
            targetPositionAfterEvasion = Vector3.zero;
        }
    }

    IEnumerator evasionTimer() {
        yield return new WaitForSeconds(2.0f);
        currentState = DevilState.Attacking;
        targetPositionAfterEvasion = Vector3.zero;
        evasionCoroutine = null;
    }

    void moveToward(Vector3 place) {
        moveToward(place, speed);
    }
    void moveToward(Vector3 place, float speed)
    {
        var t = ((Time.time - startTime) % 1.33f) / 1.33f;
        var progressPercent = BezierCurves.CubicBezier(t, 0, .42f, .58f, 1);
        var bezierSpeed = BezierCurves.CubicBezierDeriv(t, 0, 0, 1, 1);
        agent.destination = new Vector3(place.x, transform.position.y, place.z);
        agent.stoppingDistance = 0.0f;
        agent.speed = speed * (1 + bezierSpeed);
    }

    void shootFireball(Vector3 target) {
        var fireballGameobject = Instantiate(fireballPrefab, actualTransform.position, actualTransform.rotation);
        var fireballRb = fireballGameobject.AddComponent<Rigidbody>();
        var fireball = fireballGameobject.AddComponent<Fireball>();
        fireballRb.useGravity = false;
        fireballRb.AddForce(fireballSpeed * (target - actualTransform.position).normalized);
        fireballSound.Play();
        StartCoroutine(fireball.LaunchRoutine());
    }

    IEnumerator seek() {
        yield return new WaitForSeconds(trackingTimeSec);
        currentState = DevilState.Idle;
        agent.destination = transform.position;
        agent.stoppingDistance = 0.0f;
        seekingCoroutine = null;
    }

}

enum DevilState
{
    Idle, Seeking, Attacking, Evading
}
