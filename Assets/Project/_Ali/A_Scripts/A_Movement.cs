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
    [SerializeField] private Animator animatorAct1;
    [SerializeField] private Animator animatorAct2;

    private float _targetBlend;
    private float _currentBlend;
    private bool isInAir = false;


    [Header("Attack And Dodge Speed")]
    [SerializeField] private float AttackMoveSpeed;
    [SerializeField] private float DodgeMoveSpeed;

    [Header("Effects")]
    [SerializeField] private GameObject footStep;

    // movement 
    private float _xMovement;
    private float _zMovement;
    private Vector3 velocity;
    private Vector3 moveDirection;
    private bool _isJumpCooldown;
    public bool isDodging;
    public bool isAttacking;


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
        // we just change the animator we work on base on the Chr we workOn
        if(animatorAct1 == null)
        {
            animator = animatorAct2;
        }
        else
        {
            animator = animatorAct1;
        }


        if (CanMove)
        {
            Moving();
        }
        else
        {
            HandleLookingWhileCantMove();
        }
      
        HandleAnimation();
        HandleJumpingCoolDown();
        if (isDodging)
        {
            HandleDodging();
        }
        if(isAttacking)
        {
            HandleAttackingMovement();
        }

    }
    private void HandleLookingWhileCantMove()
    {
        HandleLooking();
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
        HandleLooking();

        characterController.Move(moveDirection * Time.deltaTime * speed);

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);


    }

  private void HandleLooking()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection = (cameraForward * _zMovement) + (cameraRight * _xMovement);

        // here we will check for the move direction if it is 0 then we dont do footstep effect
        if ((_zMovement > 0 || _xMovement > 0) && characterController.isGrounded)
        {
            footStep.SetActive(true);
        }
        else
        {
            footStep.SetActive(false);
        }

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleAttackingMovement()
    {
        moveDirection = transform.forward;

        characterController.Move(moveDirection * Time.deltaTime * AttackMoveSpeed);
    }

    // so we call this from the dodge script to make the player move in dir of the dodge
    private void HandleDodging()
    {
        if (isDodging)
        {

          //  Vector3 cameraForward = Camera.main.transform.forward;
          //  Vector3 cameraRight = Camera.main.transform.right;

        //    cameraForward.y = 0f;
         //   cameraRight.y = 0f;

         //   cameraForward.Normalize();
          //  cameraRight.Normalize();


            moveDirection = transform.forward; //(cameraForward * _zMovement) + (cameraRight * _xMovement);

            characterController.Move(moveDirection * Time.deltaTime * DodgeMoveSpeed);
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
