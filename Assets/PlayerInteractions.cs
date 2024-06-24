using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        foreach (var interactable in hit.gameObject.GetComponents<IInteractable>())
        {
            interactable.Interact();
        }
    }
}
