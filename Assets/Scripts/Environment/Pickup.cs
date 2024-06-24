using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{
    [SerializeField] private PickupType _pickupType;
    [SerializeField] private int _amount;

    public void Interact()
    {
        PlayerInterface.Inventory.CollectPickup(_pickupType, _amount);
        Destroy(gameObject);
    }
}

public enum PickupType 
{
    Coin,
    Key,
}

