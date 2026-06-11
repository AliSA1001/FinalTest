using UnityEngine;
using UnityEngine.InputSystem;

public class A_Movement : MonoBehaviour
{
    private CharacterController characterController;



    [Header("Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float gravity;




    // movement 
    private float xMovement;
    private float zMovement;



    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 move = new Vector3 (xMovement * speed, gravity , zMovement * speed);
        characterController.Move (move * Time.deltaTime);
    }





    public void OnMove(InputAction.CallbackContext context)
    {
        xMovement = context.ReadValue<Vector2>().x;
        zMovement = context.ReadValue<Vector2>().y;

    }
}
