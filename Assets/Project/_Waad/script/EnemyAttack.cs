using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Something Hit: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Hit!");

            HeartsHealthVisual heartsHealthVisual =
                FindObjectOfType<HeartsHealthVisual>();

            if (heartsHealthVisual != null)
            {
                heartsHealthVisual.Damage1();
            }
        }
    }
}