using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OriginHittable : MonoBehaviour, IFireballHittable
{
    [SerializeField] Health_twilight_forest healthSystem;
    int health = 5;
    [SerializeField] AudioSource sound;
    public GameOverScript GameOverScript;

    public void hit(Fireball fb) {
        sound.Play();
        Destroy(fb.gameObject);
        //Debug.Log("oh no, we got hit!");
        healthSystem.TakeDamage(1);
        // if (healthSystem.GetHealth() <= 0) {
        //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // }
        health--;
        if (health <= 0) {

            //pause game
            Time.timeScale = 0;

            // trigger game over ui
            GameOverScript.Setup();
        }
    }
}
