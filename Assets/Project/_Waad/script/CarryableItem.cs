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
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Collider col = GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }
    }

    public void Drop()
    {
        transform.SetParent(null);

        transform.position =
            player.position + player.forward * 2f + Vector3.up;

        transform.rotation = Quaternion.identity;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Collider col = GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = true;
        }
    }
}