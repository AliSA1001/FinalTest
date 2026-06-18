using UnityEngine;
using UnityEngine.InputSystem;

public class A_Parry : MonoBehaviour
{


    [SerializeField] private Animator animator;



    
    public void OnParry(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            animator.SetTrigger("Parry");
            animator.SetLayerWeight(1, 0);
        }
    }

}
