using UnityEngine;
using UnityEngine.InputSystem;

public class A_Movement : MonoBehaviour
{
    private CharacterController characterController;



    [Header("Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float rotationSpeed;
    




    // movement 
    private float _xMovement;
    private float _zMovement;
    private Vector3 velocity;

    


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        
        Moving();
        
    }

    private void Moving()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * _zMovement) + (cameraRight * _xMovement);

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if(moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.RotateTowards(transform.rotation,targetRotation,rotationSpeed * Time.deltaTime);
        }

        characterController.Move(moveDirection * Time.deltaTime * speed);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);


    }



    public void OnMove(InputAction.CallbackContext context)
    {
        _xMovement = context.ReadValue<Vector2>().x;
        _zMovement = context.ReadValue<Vector2>().y;

    }
  public void OnJump(InputAction.CallbackContext context)
    {
        if(characterController.isGrounded && context.started)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        }
    }
}
