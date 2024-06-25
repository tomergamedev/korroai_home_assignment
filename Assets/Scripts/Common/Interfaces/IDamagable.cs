using UnityEngine;

public interface IDamagable
{
    public enum DamageType
    {
        None,
        SpikeTrap,
        KillPlane
    }
    void TakeDamage(int damage, DamageType hitType);
}