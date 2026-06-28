using UnityEngine;

public class A_DamageTest : MonoBehaviour, IDamageable
{
    public void TakeDamage(float damage)
    {
        Debug.Log("Damage Taken" +  damage);    
    }
}
