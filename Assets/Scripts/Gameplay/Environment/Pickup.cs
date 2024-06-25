using System;
using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{
    [SerializeField] private PickupType _pickupType;
    [SerializeField] private int _amount;
    [SerializeField] GameObject _effectPrefab;

    public static event Action<PickupType,int> OnCollected;

    public bool IsAutoInteract() => true;

    public void Interact()
    {
        OnCollected?.Invoke(_pickupType, _amount);
        if (_effectPrefab)
        {
            Instantiate(_effectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
     
}

public enum PickupType 
{
    Coin,
    Key,
}

