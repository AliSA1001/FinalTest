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


    [Header("Animation seetings")]
    [SerializeField] private Animator animator;
    [SerializeField] private float walkBlend;
    [SerializeField] private float sprintBlend;
    private float _targetBlend;
    private float _currentBlend;
    private bool isInAir = false;


    // movement 
    private float _xMovement;
    private float _zMovement;
    private Vector3 velocity;
    private Vector3 moveDirection;


    // Animation 
    private float _timeToSprint = 1;

    


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        
        Moving();
        if(!characterController.isGrounded && Mathf.Abs(velocity.y) > 0.5f)
        {
            isInAir = true;
        }
        else
        {
            isInAir = false;
           
        }
        animator.SetBool("Inair", isInAir);
    }


    private void Moving()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

         moveDirection = (cameraForward * _zMovement) + (cameraRight * _xMovement);

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
        HandleAnimation();


    }

    private void HandleAnimation()
    {
       if (moveDirection.sqrMagnitude > 0)
        {
            _timeToSprint -= Time.deltaTime;

            if(_timeToSprint > 0)
            {
                _targetBlend = 1f;

            }
           
        }
        else
        {
            _targetBlend = 0f;
            _timeToSprint = 1;
        }

        _currentBlend = Mathf.MoveTowards(_currentBlend, _targetBlend, 6f * Time.deltaTime);

        animator.SetFloat("WalkSpeed", _currentBlend);
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
            animator.SetTrigger("Jump");
             isInAir = true;
        }
    }
}
