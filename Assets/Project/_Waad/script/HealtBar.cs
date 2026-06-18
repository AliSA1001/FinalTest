using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFill;

    private float targetHealth = 1f;

    private void Update()
    {
        healthFill.fillAmount = Mathf.Lerp(
            healthFill.fillAmount,
            targetHealth,
            Time.deltaTime * 5f
        );
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        targetHealth = currentHealth / maxHealth;
    }
}