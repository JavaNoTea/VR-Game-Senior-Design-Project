using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health_twilight_forest : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int numHearts;

    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private Animator healthAnimator;

    private bool healthIsVisible = false;
    private System.DateTime timeOfLastAttack;
    private int timeToHideUI = 5;


    // Update is called once per frame
    void Update()
    {
        if(health > numHearts)
        {
            health = numHearts;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }

            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < numHearts)
            {
                hearts[i].enabled = true;
            }

            else
            {
                hearts[i].enabled = false;
            }
        }

        // after taking damage let hearts be visible for timeToHideUI seconds before hiding
        if ((System.DateTime.Now - timeOfLastAttack).TotalSeconds > timeToHideUI)
        {
            healthIsVisible = false;
            healthAnimator.SetBool("wasHit", false);
        }

    }

    public int GetHealth() {
        return health;
    }

    public void TakeDamage(int n)
    {
        // make hearts visible if hearts not already visible
        if (!healthIsVisible)
        {
            healthAnimator.SetBool("wasHit", true);
            healthIsVisible = true;
        }
        health -= n;
        timeOfLastAttack = System.DateTime.Now;

        if (health <= 0)
        {
            //pause game
            //Time.timeScale = 0;
        }
    }

}
