using UnityEngine;
using UnityEngine.InputSystem;

public class A_Dodge : MonoBehaviour
{

    [SerializeField] private Animator animator;


    private CharacterController _characterController;
    private A_Movement _movement;


    private void Start()
    {
        _characterController = GetComponent<CharacterController>();

        _movement = A_Movement.instance;
    }

    private void SwordWeight()
    {
        _movement.CanMove = true;

    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if(context.performed && _characterController.isGrounded )
        {
            _movement.CanMove = false;
            animator.SetLayerWeight(1, 0);
            
            animator.SetTrigger("Dodge");

        }
    }
}
