using System;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public bool IsAutoInteract() => false;

    public static event Action OnDoorInteracted;

    public void Interact()
    {
        OnDoorInteracted?.Invoke();
    }
}
