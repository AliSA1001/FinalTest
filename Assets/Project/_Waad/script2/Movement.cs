using UnityEngine;

public class Movement : MonoBehaviour
{
 [Header("Camera")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSmoothTime = 0.03f;
    [SerializeField] private bool cursorLock = true;
    [SerializeField] private float mouseSensitivity = 3.5f;

    [Header("Movement")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float moveSmoothTime = 0.3f;
    [SerializeField] private float gravity = -30f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask ground;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 6f;

    private float velocityY;
    private bool isGrounded;

    private float cameraCap;

    private Vector2 currentMouseDelta;
    private Vector2 currentMouseDeltaVelocity;

    private CharacterController controller;

    private Vector2 currentDir;
    private Vector2 currentDirVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        UpdateMouse();
        UpdateMovement();
    }

    void UpdateMouse()
    {
        Vector2 targetMouseDelta = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(
            currentMouseDelta,
            targetMouseDelta,
            ref currentMouseDeltaVelocity,
            mouseSmoothTime);

        cameraCap -= currentMouseDelta.y * mouseSensitivity;
        cameraCap = Mathf.Clamp(cameraCap, -90f, 90f);

        playerCamera.localEulerAngles = Vector3.right * cameraCap;

        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            0.2f,
            ground);

        Vector2 targetDir = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"));

        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(
            currentDir,
            targetDir,
            ref currentDirVelocity,
            moveSmoothTime);

        velocityY += gravity * Time.deltaTime;

        Vector3 velocity =
            (transform.forward * currentDir.y +
             transform.right * currentDir.x) * speed +
             Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (isGrounded && controller.velocity.y < -1f)
        {
            velocityY = -8f;
        }
    }
}