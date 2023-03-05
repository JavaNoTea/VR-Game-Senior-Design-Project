using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    // Fades out whatever object, in this case the sphere boundary
    private bool fadeOut;
    [SerializeField] private float fadeSpeed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(fadeOut)
        {
            Color objectColor = this.GetComponent<Renderer>().material.color;
            float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            this.GetComponent<Renderer>().material.color = objectColor;

            if (objectColor.a <= 0)
            {
                fadeOut = false;
                gameObject.SetActive(false);
            }
        }
        
    }

    public void FadeOutObject()
    {
        fadeOut = true;
    }
}
