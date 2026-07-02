using UnityEngine;
using UnityEngine.InputSystem;

public class A_Dodge : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private Animator animatorAct1;
    [SerializeField] private Animator animatorAct2;

    [SerializeField] private float speed;


    private CharacterController _characterController;
    private A_Movement _movement;

    public bool isInvincbal = false;



    private void Start()
    {
        _characterController = GetComponent<CharacterController>();

        _movement = A_Movement.instance;
    }
    private void Update()
    {
        if (!animatorAct1.gameObject.active)
        {
            animator = animatorAct2;
        }
        else if (!animatorAct2.gameObject.active)
        {
            animator = animatorAct1;
        }
    }

    private void SwordWeight()
    {
        isInvincbal = false;
        _movement.CanMove = true;
        _movement.isDodging = false;


    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if(context.performed && _characterController.isGrounded )
        {
            _movement.CanMove = false;
            animator.SetLayerWeight(1, 0);
            animator.SetTrigger("Dodge");
            _movement.isDodging = true;
            isInvincbal = true;
            Invoke("SwordWeight", 0.65f);

        }
    }
}
