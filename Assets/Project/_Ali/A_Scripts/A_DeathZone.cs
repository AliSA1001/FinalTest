using UnityEngine;

public class A_DeathZone : MonoBehaviour
{
    [SerializeField] private float DeathDamage;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(DeathDamage);
        }
    }
}
