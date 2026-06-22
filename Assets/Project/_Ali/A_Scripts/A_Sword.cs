using UnityEngine;
using UnityEngine.InputSystem;

public class A_Sword : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private int attackNumber = 0;
    [SerializeField] private float newWeight;
    [SerializeField] private float bleendSpeed = 35;

    private A_Movement _movementRef;
    private int _lastAttackNumber;
    private bool _canAttack = true;
    private bool _startLerp = false;
    private float _returnToNormalAfterAttackTime = 0.5f;
    private float _sowrdWeight = 0.43f;
    private bool _firstAttack = true;

    [SerializeField] private float comboWindowDuration = 0.6f; // Time player has to chain next attack
 

    private void Start()
    {
        _movementRef = A_Movement.instance;
    }

    private void Update()
    {
        if (_startLerp)
        {
            newWeight = Mathf.Lerp(0, _sowrdWeight,Time.deltaTime * bleendSpeed);
            animator.SetLayerWeight(1, newWeight);
            if(newWeight >= _sowrdWeight)
            { 
            _startLerp = false;
            }
        }
    }

    private void HandleCantAttack()
    {
        if (_canAttack && (_lastAttackNumber == 2 || _firstAttack))
        {
            _firstAttack = false;

            _canAttack = false;
            attackNumber = 0;

            _movementRef.CanMove = false;
            animator.SetTrigger("Attack1");
            animator.SetLayerWeight(1, 0);
            Invoke("HnadleAnimitorWeight", 0.5f);

            _movementRef.CanMove = false;
            _lastAttackNumber = attackNumber;
        }



        else if (_lastAttackNumber == 0 && _canAttack)
        {
            _canAttack = false;
            attackNumber = 1;
            _movementRef.CanMove = false;

            animator.SetTrigger("Attack2");
            animator.SetLayerWeight(1, 0);
            Invoke("HnadleAnimitorWeight", 0.5f);

            _movementRef.CanMove = false;
            _lastAttackNumber = attackNumber;

        }
        else if (_lastAttackNumber == 1 && _canAttack)
        {
            _canAttack = false;
            attackNumber = 2;
            _movementRef.CanMove = false;

            animator.SetTrigger("Attack3");
            animator.SetLayerWeight(1, 0);
            Invoke("HnadleAnimitorWeight", 0.6f);

            _movementRef.CanMove = false;
            _lastAttackNumber = attackNumber;
        }

       }

       
    
    private void HnadleAnimitorWeight()
    {
        _startLerp = true;
        _movementRef.CanMove = true;
        _canAttack = true;
        _movementRef.isAttacking = false;


    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.performed && characterController.isGrounded && _canAttack)
        {
            HandleCantAttack();
            _movementRef.isAttacking = true;

        }
    }
}