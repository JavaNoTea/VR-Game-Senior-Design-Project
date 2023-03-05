using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Quiver : XRBaseInteractable
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject bowPrefab;
    [SerializeField] private bool bowActivated;
    public Material otherMaterial = null;
    public GameObject bowObject;

    private bool isBowDestroyable = false;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        //base.OnSelectEntered(args);

        if (bowActivated)
        {
            CreateAndSelectArrow(args);
            // if hand holding bow interacts with quiver, delete bow

        }
        else
        {
            CreateAndSelectBow(args);
            bowActivated = true;
            StartCoroutine(SetBowAsDestroyable());
        }
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("bow") && isBowDestroyable) {
            Destroy(bowObject);
            bowActivated = false;
            isBowDestroyable = false;
        }
    }

    IEnumerator SetBowAsDestroyable() {
        yield return new WaitForSeconds(1.0f);
        isBowDestroyable = true;
    }

    // void OnCollisionEnter(Collision collision)
    // {

    //     ApplyMaterial();

    //     if (collision.gameObject.CompareTag("bow"))
    //     {
    //         Destroy(bowObject);
    //         isBowDestroyable = false;
    //         bowActivated = false;
    //     }

    // }


    /*    protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            if (args.interactableObject.GetType().Equals(bowObject))
            {
                Destroy(bowObject);
                bowActivated = false;
            }

        }*/

    private void ApplyMaterial()
    {
        if (TryGetComponent(out MeshRenderer meshRenderer))
            meshRenderer.material = otherMaterial;
    }


    private void CreateAndSelectArrow(SelectEnterEventArgs args)
    {
        // Create arrow, force into interacting hand
        Arrow arrow = CreateArrow(args.interactorObject.transform);
        interactionManager.SelectEnter(args.interactorObject, arrow);
    }

    private void CreateAndSelectBow(SelectEnterEventArgs args)
    {
        // Create bow, force into interacting hand
        Bow bow = CreateBow(args.interactorObject.transform);
        interactionManager.SelectEnter(args.interactorObject, bow);
    }

    private Arrow CreateArrow(Transform orientation)
    {
        // Create arrow, and get arrow component
        GameObject arrowObject = Instantiate(arrowPrefab, orientation.position, orientation.rotation);
        return arrowObject.GetComponent<Arrow>();
    }

    private Bow CreateBow(Transform orientation)
    {
        // Create bow, and get bow component
        bowObject = Instantiate(bowPrefab, orientation.position, orientation.rotation);
        return bowObject.GetComponent<Bow>();
    }
}
