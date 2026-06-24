using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private Transform itemHolder;
    [SerializeField] private TMP_Text pickupText;

    private CarryableItem nearbyItem;
    private CarryableItem carriedItem;

    private void Start()
    {
        if (pickupText != null)
        {
            pickupText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            // رمي
            if (carriedItem != null)
            {
                carriedItem.Drop();
                carriedItem = null;

                if (pickupText != null)
                {
                    pickupText.gameObject.SetActive(false);
                }

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
            {
                pickupText.gameObject.SetActive(false);
            }
        }
    }
}