using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private PlayerInterface _playerInterface;
    [SerializeField] private LevelCanvas _levelCanvas;

    private void Awake()
    {
        _playerInterface.GetHealth().OnPlayerHealthChanged += OnPlayerHealthChange;
        _playerInterface.GetInteractions().OnInteractionAvailableChanged += _levelCanvas.UpdateInteractGUIActive;
        Pickup.OnCollected += OnPickupCollected;
        Door.OnDoorInteracted += OnDoorInteracted;

        GameCursor.ToggleGameMode();

        _levelCanvas.UpdateHealthText(_playerInterface.GetHealth().GetHealth());
        _levelCanvas.UpdateScoreText(0);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Quit"))
        {
            SceneLoader.LoadScene(0);
        }
    }

    public PlayerInventory GetPlayerInventory() => _playerInterface.GetInventory();

    private void OnPlayerHealthChange(int Health)
    {
        if (Health <= 0)
        {
            RestartLevel();
        }

        _levelCanvas.UpdateHealthText(Health);
    }

    private void OnDoorInteracted()
    {
        if (_playerInterface.GetInventory().HasKey())
        {
            SceneLoader.LoadNextScene();
        }
        else
        {
            _levelCanvas.DisplayKeyRequiredMessage();
        }
    }

    private void RestartLevel()
    {
        SceneLoader.ReloadCurrentScene();
    }

    private void OnPickupCollected(PickupType pickupType, int amount)
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
        Pickup.OnCollected -= OnPickupCollected;
        Door.OnDoorInteracted -= OnDoorInteracted;
    }
}
