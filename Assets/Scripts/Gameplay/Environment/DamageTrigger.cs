using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] IDamagable.DamageType _hitType;
    [SerializeField] int _damage = 1;
    [SerializeField] GameObject _effectPrefab;

    public void OnTriggerEnter(Collider other)
    {
        IDamagable damagable = other.gameObject.GetComponent<IDamagable>();
        if (damagable != null)
        {
            damagable.TakeDamage(_damage, _hitType);
            if (_effectPrefab)
            {
                Instantiate(_effectPrefab, transform.position, Quaternion.identity);
            }

        }
    }
}
