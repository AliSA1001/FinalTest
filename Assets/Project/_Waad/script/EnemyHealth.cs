using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    public HealthBar healthBar;

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
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            // حذف العدو عند الموت
            Destroy(gameObject);
        }

        healthBar.UpdateHealthBar(maxHealth, currentHealth);
    }
}