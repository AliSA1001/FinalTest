using UnityEngine;
using UnityEngine.InputSystem;

public class A_Parry : MonoBehaviour
{

    
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem parryEffect;
    [SerializeField] private CapsuleCollider parryCollider;


    private A_Movement _movementRef;
    private bool _canParry = true;
    private CharacterController _characterController;





    private void Start()
    {
        _movementRef = A_Movement.instance;
        _characterController = GetComponent<CharacterController>();
    }

    private void HnadleAnimitorWeight()
    {
        animator.SetLayerWeight(1, 0.43f);
        _canParry = true;
        _movementRef.CanMove = true;
        parryCollider.enabled = false;

    }

    public void OnParry(InputAction.CallbackContext context)
    {
        if(context.performed && _canParry && _characterController.isGrounded)
        {
            animator.SetLayerWeight(1, 0);
            animator.SetTrigger("Parry");
            _movementRef.CanMove = false;
            _canParry = false;
            parryEffect.Play();
            parryCollider.enabled = true;
            Invoke("HnadleAnimitorWeight", 0.5f);
           
        }
    }

}
