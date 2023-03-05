using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour, IHittableInterface
{
    public void targetShot()
    {
        Destroy(gameObject);
    }

    public void playAnimation()
    {
        // Maybe to Do later, just depends on time
    }

    public void playSound()
    {
        // Maybe to Do later, just depends on time
    }
}
