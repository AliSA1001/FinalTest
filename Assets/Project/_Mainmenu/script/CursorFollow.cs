using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 mouse = Input.mousePosition;

        mouse.z = 10f;

        transform.position = Camera.main.ScreenToWorldPoint(mouse);
    }
}