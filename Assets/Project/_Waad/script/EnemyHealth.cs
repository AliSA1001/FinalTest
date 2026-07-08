using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour , IDamageable
{
    public float maxHealth = 200f;
    public float currentHealth = 200f;

    public HealthBar healthBar;

    public GameObject blueHealPrefab;

    //Ali
    // we added the abilty to send event to the enemy ai 
    public event Action OnHit;
    //Ali

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.UpdateHealthBar(maxHealth, currentHealth);
    }

    private void Update()
    {
        // اختبار مؤقت
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(25);
        }
    }

    public void TakeDamage(float damage)
    {
        // Ali
        OnHit?.Invoke();
        // Ali
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            // 30% chance to drop the heal pickup on death.
            if (blueHealPrefab != null && UnityEngine.Random.value <= 0.30f)
            {
                GameObject heal = Instantiate(
                    blueHealPrefab,
                    transform.position + new Vector3(0f, 2.5f, 0f),
                    Quaternion.identity
                );

                HealPickup healPickup = heal.GetComponent<HealPickup>();

                if (healPickup != null)
                {
                    healPickup.destroyAfterTime = true;
                }

                Rigidbody rb = heal.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;

                    // نطة خفيفة
                    rb.AddForce(Vector3.up * 3f, ForceMode.Impulse);
                }
            }

            Destroy(gameObject);
        }

        healthBar.UpdateHealthBar(maxHealth, currentHealth);
    }
}