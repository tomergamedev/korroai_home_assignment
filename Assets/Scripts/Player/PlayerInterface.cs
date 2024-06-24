using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterface : MonoBehaviour
{
    public static PlayerInventory Inventory { get; private set; }

    void Awake()
    {
        Inventory = GetComponent<PlayerInventory>();
    }
}
