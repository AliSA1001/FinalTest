using UnityEngine;
using UnityEngine.InputSystem;

public class A_Sword : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private int AttackNumber = 0;

    private A_Movement _movementRef;

    [SerializeField] private float comboWindowDuration = 0.6f; // Time player has to chain next attack
    private float _comboTimer = 0f;
    private bool _canAttack = true;

    private void Start()
    {
        _movementRef = A_Movement.instance;
    }

    private void Update()
    {
        // Constantly tick down the combo timer over time
        if (_comboTimer > 0)
        {
            _comboTimer -= Time.deltaTime;

            // If the player waits too long, break the combo and reset to Attack 0
            if (_comboTimer <= 0)
            {
                AttackNumber = 0;
            }
        }
    }

    private void HandleCantAttack()
    {
        _canAttack = true;
        animator.SetLayerWeight(1, 0.43f);
        _movementRef.Canmove = true;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed || !characterController.isGrounded) return;

        // CRITICAL FIX: Allow input if they are allowed to attack generally OR if they are executing a combo chain
        if (_canAttack)
        {
            // Cancel previous recovery invoke so the new attack gets its full duration
            CancelInvoke("HandleCantAttack");

            switch (AttackNumber)
            {
                case 0:
                    // First Attack
                    animator.SetTrigger("Attack1");
                    animator.SetLayerWeight(1, 0);
                    _canAttack = false;
                    _movementRef.Canmove = false;

                    AttackNumber = 1; // Prepare next stage
                    _comboTimer = comboWindowDuration; // Open combo timer
                    Invoke("HandleCantAttack", 1f);
                    break;

                case 1:
                    // Second Attack (Only triggers if still within the combo timer window)
                    if (_comboTimer > 0)
                    {
                        animator.SetTrigger("Attack2");
                        animator.SetLayerWeight(1, 0);
                        _canAttack = false;
                        _movementRef.Canmove = false;

                        AttackNumber = 2; // Prepare final stage
                        _comboTimer = comboWindowDuration; // Refresh combo timer
                        Invoke("HandleCantAttack", 1f);
                    }
                    break;

                case 2:
                    // Third Attack (Finisher)
                    if (_comboTimer > 0)
                    {
                        animator.SetTrigger("Attack3");
                        animator.SetLayerWeight(1, 0);
                        _canAttack = false;
                        _movementRef.Canmove = false;

                        AttackNumber = 0; // Reset combo back to start
                        _comboTimer = 0;   // Clear combo window
                        Invoke("HandleCantAttack", 1f);
                    }
                    break;
            }
        }
    }
}