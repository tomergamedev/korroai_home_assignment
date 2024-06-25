using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private PlayerInterface _playerInterface;
    [SerializeField] private LevelCanvas _levelCanvas;

    public static LevelManager Instance;

    private void Awake()
    {
        Instance = this;
        _playerInterface.GetHealth().OnPlayerHealthChanged += OnPlayerHealthChange;
        _playerInterface.GetInteractions().OnInteractionAvailableChanged += _levelCanvas.UpdateInteractGUIActive;
        Pickup.OnCollected += PickupCollected;

        GameCursor.ToggleGameMode();
    }

    public PlayerInventory GetPlayerInventory() => _playerInterface.GetInventory();

    private void OnPlayerHealthChange(int Health)
    {
        if (Health <= 0)
        {
            EndGame();
        }

        _levelCanvas.UpdateHealthText(Health);
    }

    private void EndGame()
    {
        SceneLoader.LoadScene(0);
    }

    private void PickupCollected(PickupType pickupType, int amount)
    {
        _playerInterface.GetInventory().CollectPickup(pickupType, amount);

        switch (pickupType)
        {
            case PickupType.Coin:
                _levelCanvas.UpdateScoreText(_playerInterface.GetInventory().GetScore());
                break;
            case PickupType.Key:
                _levelCanvas.UpdateKeyCollected();
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        _playerInterface.GetHealth().OnPlayerHealthChanged -= OnPlayerHealthChange;
        _playerInterface.GetInteractions().OnInteractionAvailableChanged -= _levelCanvas.UpdateInteractGUIActive;
        Pickup.OnCollected -= PickupCollected;
    }
}
