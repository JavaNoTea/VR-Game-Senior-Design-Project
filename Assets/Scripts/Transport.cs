using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class Transport : XRGrabInteractable
{
    [SerializeField] public int nextSceneGo;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        SceneManager.LoadScene(nextSceneGo);
    }
}