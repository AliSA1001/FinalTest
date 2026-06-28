using UnityEngine;

public class A_DamageHit : MonoBehaviour
{
    [SerializeField] private float SwordDamage;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(SwordDamage);
        }
    }
}
