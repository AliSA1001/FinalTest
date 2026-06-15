using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFill;

    private float health = 1f;
    private float targetHealth = 1f;

    private void Update()
    {
        healthFill.fillAmount = Mathf.Lerp(
            healthFill.fillAmount,
            targetHealth,
            Time.deltaTime * 5f
        );
    }

    public void TakeDamage()
    {
        health -= 0.1f;
        health = Mathf.Clamp01(health);

        targetHealth = health;
    }

    public void Heal()
    {
        health += 0.1f;
        health = Mathf.Clamp01(health);

        targetHealth = health;
    }
}