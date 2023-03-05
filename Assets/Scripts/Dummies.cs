using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Dummies : MonoBehaviour, IArrowHittable
{
    Rigidbody rb;
    [SerializeField] AudioSource impactSound;
    [SerializeField] BoxCollider HitBox;
    public float forceAmount = 1.0f;
    public UnityEvent OnCollision;

    void OnCollisionEnter(Collision other)
    {
        var proj = other.rigidbody.GetComponent<Arrow>();

        if (proj != null)
        {
            Hit(proj);
        }
    }

    public void Hit(Arrow arrow)
    {
        impactSound.Play();
        HitBox.enabled = false;
        OnCollision.Invoke();
        DisableCollider(arrow);
        Destroy(arrow.gameObject);
    }

    private void ApplyForce(Arrow arrow)
    {
        if (TryGetComponent(out Rigidbody rigidbody))
            rigidbody.AddForce(arrow.transform.forward * forceAmount);
    }

    private void DisableCollider(Arrow arrow)
    {
        if (arrow.TryGetComponent(out Collider collider))
            collider.enabled = false;
    }
}
