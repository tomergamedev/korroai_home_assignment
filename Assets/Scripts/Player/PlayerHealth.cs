using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    [SerializeField] private int _initialHealth = 3;
    
    private int _health;

    void Start()
    {
        _health = _initialHealth;
    }

    public void TakeDamage(int damage)
    {
        //TODO: play damage effect
        _health -= damage;
        GetComponent<PlayerMovement>().PushFromDamage();
        if(_health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //TODO: replace with proper animation and effects
        Destroy(gameObject);
    }
}
