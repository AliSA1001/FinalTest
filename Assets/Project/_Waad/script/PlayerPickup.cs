using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private Transform itemHolder;
    [SerializeField] private TMP_Text pickupText;

    [Header("Drop Settings")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float checkDistance = 2f;

    private CarryableItem nearbyItem;
    private CarryableItem carriedItem;

    private void Start()
    {
        if (pickupText != null)
            pickupText.gameObject.SetActive(false);
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

            if (pickupText != null)
            {
                if (canDrop)
                {
                    pickupText.gameObject.SetActive(true);
                    pickupText.text = "Press Q To Drop";
                }
                else
                {
                    pickupText.gameObject.SetActive(false);
                }
            }
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

                if (pickupText != null)
                    pickupText.gameObject.SetActive(false);

                return;
            }

            // أخذ
            if (nearbyItem != null)
            {
                nearbyItem.PickUp(itemHolder, transform);
                carriedItem = nearbyItem;

                if (pickupText != null)
                {
                    pickupText.gameObject.SetActive(true);
                    pickupText.text = "Press Q To Drop";
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CarryableItem item = other.GetComponentInParent<CarryableItem>();

        if (item != null)
        {
            nearbyItem = item;

            if (carriedItem == null && pickupText != null)
            {
                pickupText.gameObject.SetActive(true);
                pickupText.text = "Press Q To Pick Up";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CarryableItem item = other.GetComponentInParent<CarryableItem>();

        if (item == nearbyItem)
        {
            nearbyItem = null;

            if (pickupText != null && carriedItem == null)
                pickupText.gameObject.SetActive(false);
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