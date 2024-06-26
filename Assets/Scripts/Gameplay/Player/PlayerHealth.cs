using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    [SerializeField] private int _initialHealth = 3;

    public event Action<int> OnPlayerHealthChanged;

    private int _health;
    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _health = _initialHealth;
        _playerMovement = GetComponent<PlayerMovement>();
    }

    public int GetHealth() => _health;

    public void TakeDamage(int damage, IDamagable.DamageType hitType)
    {
        _health -= damage;
        OnPlayerHealthChanged?.Invoke(_health);

        switch (hitType)
        {
            case IDamagable.DamageType.SpikeTrap:
                _playerMovement.PushFromDamage();
                break;
            case IDamagable.DamageType.KillPlane:
                _playerMovement.WarpToRespawnLocation();
                break;
            case IDamagable.DamageType.None:
                break;
            default:
                break;
        }
    }

}
