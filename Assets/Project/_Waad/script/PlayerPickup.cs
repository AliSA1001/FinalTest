using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private Transform itemHolder;

    [Header("Prompt UI")]
    [Tooltip("The whole prompt panel (background + keycap + label) to show/hide.")]
    [SerializeField] private GameObject pickupPrompt;
    [Tooltip("The label text inside the panel. The script writes \"Pick Up\" / \"Drop\".")]
    [SerializeField] private TMP_Text pickupLabel;

    [Header("Drop Settings")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float checkDistance = 2f;

    private CarryableItem nearbyItem;
    private CarryableItem carriedItem;

    private void Start()
    {
        HidePrompt();
    }

    // Shows the prompt panel with the given short label ("Pick Up" / "Drop").
    private void ShowPrompt(string label)
    {
        if (pickupLabel != null) pickupLabel.text = label;
        if (pickupPrompt != null) pickupPrompt.SetActive(true);
    }

    private void HidePrompt()
    {
        if (pickupPrompt != null) pickupPrompt.SetActive(false);
    }

    private void Update()
    {
        bool canDrop = true;

        // إذا اللاعب شايل شيء نفحص قدامه
        if (carriedItem != null)
        {
            Vector3 origin = transform.position + Vector3.up * 1f;

            canDrop = !Physics.Raycast(
                origin,
                transform.forward,
                checkDistance,
                obstacleLayer
            );

            if (canDrop) ShowPrompt("Drop");
            else HidePrompt();
        }

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            // رمي
            if (carriedItem != null)
            {
                // إذا فيه جدار لا يرمي
                if (!canDrop)
                    return;

                carriedItem.Drop();
                carriedItem = null;

                HidePrompt();

                return;
            }

            // أخذ
            if (nearbyItem != null)
            {
                nearbyItem.PickUp(itemHolder, transform);
                carriedItem = nearbyItem;

                ShowPrompt("Drop");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CarryableItem item = other.GetComponentInParent<CarryableItem>();

        if (item != null)
        {
            nearbyItem = item;

            if (carriedItem == null)
                ShowPrompt("Pick Up");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CarryableItem item = other.GetComponentInParent<CarryableItem>();

        if (item == nearbyItem)
        {
            nearbyItem = null;

            if (carriedItem == null)
                HidePrompt();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 origin = transform.position + Vector3.up;

        Gizmos.DrawRay(origin, transform.forward * checkDistance);
    }
#endif
}