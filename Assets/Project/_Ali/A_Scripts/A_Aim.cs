using UnityEngine;
using UnityEngine.InputSystem;
public class A_Aim : MonoBehaviour
{
    private Camera mainCamera;
    private Vector2 mouseScreenPosition;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        RotateTowardsMouseOrthographic();
    }

    // Link this to your Input Action Asset (<Mouse>/position)
    public void OnMousePosition(InputAction.CallbackContext context)
    {
        mouseScreenPosition = context.ReadValue<Vector2>();
    }

    private void RotateTowardsMouseOrthographic()
    {
        // 1. Convert the 2D mouse position to a world position using the camera
        // We give it a dummy Z depth (like 10 or the camera's near clip plane) 
        // because orthographic rays shoot straight forward regardless of depth.
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane));

        // 2. Project a ray straight down from that world position along the camera's viewing axis
        Ray ray = new Ray(mouseWorldPos, mainCamera.transform.forward);

        // 3. Create a plane at the player's feet height
        Plane playerPlane = new Plane(Vector3.up, transform.position);

        // 4. Find where the straight orthographic ray intersects the player's plane
        if (playerPlane.Raycast(ray, out float hitDistance))
        {
            Vector3 targetPoint = ray.GetPoint(hitDistance);

            // Calculate direction from player to the mouse intersection point
            Vector3 lookDirection = targetPoint - transform.position;

            // Keep rotation strictly flat on the Y axis
            lookDirection.y = 0f;

            if (lookDirection != Vector3.zero)
            {
                // Snap or smoothly rotate towards the target
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 25f);
            }
        }
    }
}
