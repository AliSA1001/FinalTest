using UnityEngine;
using UnityEngine.InputSystem;

public class A_Movement : MonoBehaviour
{
    private CharacterController characterController;



    [Header("Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float gravity;




    // movement 
    private float _xMovement;
    private float _zMovement;

    // Look 
    private Vector2 _lookValue;



    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 move = new Vector3 (_xMovement * speed, gravity , _zMovement * speed);
        characterController.Move (move * Time.deltaTime);

        
    }





    public void OnMove(InputAction.CallbackContext context)
    {
        _xMovement = context.ReadValue<Vector2>().x;
        _zMovement = context.ReadValue<Vector2>().y;

    }
  public void OnJump(InputAction.CallbackContext context)
    {

    }
}
