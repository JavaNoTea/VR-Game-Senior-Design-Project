using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Controller : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("Level 0");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
