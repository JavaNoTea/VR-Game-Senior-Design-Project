using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour {

    Vector3 lastPosition = Vector3.zero;
    RaycastHit hit;
    bool CheckForCollision(out RaycastHit hit) {
        if (lastPosition == Vector3.zero) {
            lastPosition = gameObject.transform.position;
        }
        bool collided = Physics.Linecast(lastPosition, gameObject.transform.position, out hit, ~0);
        lastPosition = collided ? lastPosition : gameObject.transform.position;

        return collided;
    }

    public IEnumerator LaunchRoutine() {
        while (!CheckForCollision(out hit)) {
            yield return null;
        }
        OnCollide(hit);
    }

    void OnCollide(RaycastHit hit) {
        if (hit.transform.TryGetComponent<IFireballHittable>(out IFireballHittable thing)) {
            thing.hit(this);
        }
    }
}

public interface IFireballHittable {
    void hit(Fireball fireball);
}