using UnityEngine;
using UnityEngine.InputSystem;

public class A_Sword : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;


    // call movement singleten
    private A_Movement movementRef;



    private bool _canAttack = true;


    private void Start()
    {
        movementRef = A_Movement.instance;
    }
    private void HandleCantAttack()
    {
        _canAttack = true;

        animator.SetLayerWeight(1, 0.43f);

        movementRef.Canmove = true;

    }


    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && _canAttack && characterController.isGrounded)
        {
            animator.SetTrigger("Attack1");
            animator.SetLayerWeight(1, 0);
            _canAttack = false;
            movementRef.Canmove = false;
            Invoke("HandleCantAttack", 1);
        }
    }
}
