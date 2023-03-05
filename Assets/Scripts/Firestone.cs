using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Firestone : XRGrabInteractable
{
    [SerializeField] public GameObject fireWall;
    [SerializeField] public GameObject blocked_off_area;
    private bool hasRockInHand;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        hasRockInHand = true;
        Debug.Log("HOORAY!!");
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        base.OnSelectExiting(args);
        hasRockInHand = false;
    }

    void Start()
    {
        fireWall.SetActive(false);
        // hasRockInHand = false;
        // Debug.Log("HI");
    }

    void Update()
    {
        if (hasRockInHand == true)
        {
            fireWall.SetActive(false);
            blocked_off_area.SetActive(true);
        }
        else
        {
            fireWall.SetActive(true);
            blocked_off_area.SetActive(false);
        }
    }
}
