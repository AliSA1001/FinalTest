using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class A_Movement : MonoBehaviour
{
    private CharacterController characterController;

    public static A_Movement instance;

    [Header("Movement Settings")]
    public bool CanMove; // this is the key when we try to stop the player in attack
    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float jumpCooldown;
   


    [Header("Animation seetings")]
    [SerializeField] private Animator animator;
    private float _targetBlend;
    private float _currentBlend;
    private bool isInAir = false;


    // movement 
    private float _xMovement;
    private float _zMovement;
    private Vector3 velocity;
    private Vector3 moveDirection;
    private bool _isJumpCooldown;
    public bool isDodging;


    // Animation 
    private float _timeToSprint = 1;
   

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (CanMove)
        {
            Moving();
        }
        HandleAnimation();
        HandleJumpingCoolDown();
        if (isDodging)
        {
            HandleDodging(10);
        }

    }

    private void HandleJumpingCoolDown()
    {

        if (_isJumpCooldown)
        {
            jumpCooldown -= Time.deltaTime;
            if (jumpCooldown < 0)
            {
                jumpCooldown = 1;
                _isJumpCooldown = false;
            }
        }
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


    }
    // so we call this from the dodge script to make the player move in dir of the dodge
    public void HandleDodging(float dodgeSpeed)
    {
        if (isDodging)
        {

            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            moveDirection = (cameraForward * _zMovement) + (cameraRight * _xMovement);

            characterController.Move(moveDirection * Time.deltaTime * dodgeSpeed);
        }
    }

    private void HandleAnimation()
    {
       if (moveDirection.sqrMagnitude > 0)
        {
            _timeToSprint -= Time.deltaTime;
                _targetBlend = 1f;
        }
        else
        {
            _targetBlend = 0f;
            _timeToSprint = 1;
        }

        _currentBlend = Mathf.MoveTowards(_currentBlend, _targetBlend, 6f * Time.deltaTime);

        animator.SetFloat("WalkSpeed", _currentBlend);

        if (!characterController.isGrounded && Mathf.Abs(velocity.y) > 0.5f)
        {
            isInAir = true;
        }
        else
        {
            isInAir = false;

        }
        animator.SetBool("Inair", isInAir);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _xMovement = context.ReadValue<Vector2>().x;
        _zMovement = context.ReadValue<Vector2>().y;

    }
  public void OnJump(InputAction.CallbackContext context)
    {
        if(characterController.isGrounded && context.started && !_isJumpCooldown)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
             isInAir = true;
            _isJumpCooldown = true;
        }
    }
}
