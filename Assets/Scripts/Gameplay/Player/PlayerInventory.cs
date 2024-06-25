using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private int _score;
    private bool _hasKey;

    public int GetScore() => _score;
    public bool HasKey() => _hasKey;

    public void CollectPickup(PickupType pickup, int amount)
    {
        switch (pickup)
        {
            case PickupType.Coin:
                _score += amount;
                break;
            case PickupType.Key:
                _hasKey = true;
                break;
            default:
                break;
        }
    }


}
