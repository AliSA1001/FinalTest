using UnityEngine;

public class CarryableItem : MonoBehaviour
{
    [Header("Hold Settings")]
    public Vector3 holdPosition;
    public Vector3 holdRotation;
    public Vector3 holdScale = Vector3.one;

    private Transform player;
    private Rigidbody rb;
    private Collider col;

    private Vector3 originalScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        originalScale = transform.localScale;
    }

    public void PickUp(Transform itemHolder, Transform playerTransform)
    {
        player = playerTransform;

        transform.SetParent(itemHolder);

        transform.localPosition = holdPosition;
        transform.localEulerAngles = holdRotation;
        transform.localScale = holdScale;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (col != null)
            col.enabled = false;
    }

    public void Drop()
    {
        transform.SetParent(null);

        // يرجع الحجم الأصلي
        transform.localScale = originalScale;

        // مكان الرمي
        Vector3 dropPosition =
            player.position +
            player.forward * 3f +
            Vector3.up * 4f;

        // Raycast عشان ما يدخل بالأرض
        RaycastHit hit;
        if (Physics.Raycast(dropPosition, Vector3.down, out hit, 5f))
        {
            dropPosition.y = hit.point.y + 0.1f;
        }

        transform.position = dropPosition + Vector3.up * 0.5f;

        transform.rotation = Quaternion.identity;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (col != null)
            col.enabled = true;
    }
}