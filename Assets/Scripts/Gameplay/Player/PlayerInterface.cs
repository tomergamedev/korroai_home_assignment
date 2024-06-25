using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterface : MonoBehaviour
{
    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private PlayerHealth _health;
    [SerializeField] private PlayerInteractions _interactions;

    public PlayerInventory GetInventory() => _inventory;
    public PlayerHealth GetHealth() => _health;
    public PlayerInteractions GetInteractions() => _interactions;
}
