using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private Transform itemHolder;
    [SerializeField] private GameObject pickupText;

    private CarryableItem nearbyItem;
    private CarryableItem carriedItem;

    private void Start()
    {
        if (pickupText != null)
            pickupText.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            // إذا شايل شيء -> ارمه
            if (carriedItem != null)
            {
                carriedItem.Drop();
                carriedItem = null;

                if (pickupText != null && nearbyItem == null)
                    pickupText.SetActive(false);

                return;
            }

            // إذا قريب من شيء -> خذه
            if (nearbyItem != null)
            {
                nearbyItem.PickUp(itemHolder, transform);
                carriedItem = nearbyItem;

                if (pickupText != null)
                    pickupText.SetActive(false);
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
                pickupText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CarryableItem item = other.GetComponentInParent<CarryableItem>();

        if (item == nearbyItem)
        {
            nearbyItem = null;

            if (pickupText != null)
            {
                pickupText.SetActive(false);
            }
        }
    }
}