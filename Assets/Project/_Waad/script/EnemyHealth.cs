using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    public HealthBar healthBar;

    public GameObject blueHealPrefab;

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

            GameObject heal = Instantiate(
                blueHealPrefab,
                transform.position + Vector3.up * 2f,
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

                rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
            }

            Destroy(gameObject);
        }

        healthBar.UpdateHealthBar(maxHealth, currentHealth);
    }
}