using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    public float interactionDistance = 2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            CheckForInteractable();
        }
    }

    void CheckForInteractable()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance))
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}


public class Chest : InteractableObject
{
    public override void Interact()
    {
        //chest interact scene
        Debug.Log("Chest opened!");
    }
}


