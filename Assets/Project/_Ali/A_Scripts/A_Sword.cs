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
    private bool _StartLerp;

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
       
        if (_lastAttackNumber == 0 && _canAttack)
        {
            _canAttack = false;
            attackNumber = 1;
            _movementRef.Canmove = false;

            animator.SetTrigger("Attack2");
            animator.SetLayerWeight(1, 0);
            Invoke("HnadleAnimitorWeight", 0.2f);

            _movementRef.Canmove = false;
            _lastAttackNumber = attackNumber;
        }




       // if (_lastAttackNumber == attackNumber) { return; } // if the last attacknumber is the same number then we dont do anyting
        if (_canAttack)
        {
            _canAttack = false;
            attackNumber = 0;

            _movementRef.Canmove = false;
            animator.SetTrigger("Attack1");
            animator.SetLayerWeight(1, 0);
            Invoke("HnadleAnimitorWeight", 0.2f);

            _movementRef.Canmove = false;
            _lastAttackNumber = attackNumber;
        }
    }
    private void HnadleAnimitorWeight()
    {
        _startLerp = true;
        _movementRef.Canmove = true;
        _canAttack = true;

    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.performed && characterController.isGrounded && _canAttack)
        {
            HandleCantAttack();
        }
    }
}