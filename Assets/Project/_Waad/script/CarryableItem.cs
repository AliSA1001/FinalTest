using UnityEngine;

public class CarryableItem : MonoBehaviour
{
    private Transform player;

    public void PickUp(Transform itemHolder, Transform playerTransform)
    {
        player = playerTransform;

        transform.SetParent(itemHolder);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    public void Drop()
    {
        transform.SetParent(null);

        transform.position =
            player.position + player.forward * 1.5f;

        transform.rotation = Quaternion.identity;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}