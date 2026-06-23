using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Transform itemHolder;

    private GameObject currentItem;
    private GameObject originalPrefab;

    public bool HasItem()
    {
        return currentItem != null;
    }

    public void PickupItem(GameObject itemPrefab)
    {
        if (currentItem != null)
        {
            Debug.Log("Already carrying an item");
            return;
        }

        originalPrefab = itemPrefab;

        currentItem = Instantiate(itemPrefab);

        currentItem.transform.SetParent(itemHolder);

        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;
        currentItem.transform.localScale = new Vector3(0.44f, 0.44f, 0.44f);

        Debug.Log("Item Added To Back");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }
    }

    public void DropItem()
    {
        if (currentItem == null)
            return;

        Vector3 dropPos = transform.position + transform.forward * 2f;

        Destroy(currentItem);

        GameObject droppedItem =
            Instantiate(originalPrefab, dropPos, Quaternion.identity);

        currentItem = null;
        originalPrefab = null;

        Debug.Log("Item Dropped");
    }
}