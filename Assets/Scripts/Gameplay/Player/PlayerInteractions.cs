using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    private HashSet<IInteractable> _availableInteractables = new();
    public event Action<bool> OnInteractionAvailableChanged;

    private void OnTriggerEnter(Collider other)
    {
        foreach (var interactable in other.gameObject.GetComponents<IInteractable>())
        {
            if (interactable.IsAutoInteract())
            {
                interactable.Interact();
            }
            else
            {
                _availableInteractables.Add(interactable);

            }

            if (_availableInteractables.Count > 0)
            {
                OnInteractionAvailableChanged?.Invoke(true);
            }
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            foreach (IInteractable interactable in _availableInteractables)
            {
                interactable.Interact();
                _availableInteractables.Remove(interactable);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable[] interactables = other.GetComponents<IInteractable>();
        _availableInteractables.ExceptWith(interactables);
        if (_availableInteractables.Count == 0)
        {
            OnInteractionAvailableChanged?.Invoke(false);
        }
    }
}
