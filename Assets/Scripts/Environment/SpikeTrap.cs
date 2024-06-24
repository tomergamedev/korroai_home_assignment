using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] int _damage = 1;

    public void OnTriggerEnter(Collider other)
    {
        IDamagable damagable = other.gameObject.GetComponent<IDamagable>();
        damagable?.TakeDamage(_damage);
    }
}
