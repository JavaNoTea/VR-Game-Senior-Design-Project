using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class NewSceneTeleport : XRGrabInteractable
{
    [SerializeField] public int nextScene;

    // protected override void OnHoverEntered(HoverEnterEventArgs args)
    // {
    //     base.OnHoverEntered(args);
    //     SceneManager.LoadScene(nextScene);
    // }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("GameController") || other.CompareTag("Player"))
        {
            // Load the new scene
            SceneManager.LoadScene(nextScene);
        }
        
        
    }
}
