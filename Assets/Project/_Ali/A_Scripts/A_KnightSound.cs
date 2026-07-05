using UnityEngine;

public class A_KnightSound : MonoBehaviour
{
    [SerializeField] private AudioSource slashSound1;

    [SerializeField] private AudioSource slashSound2;

    [SerializeField] private AudioSource thrustSound;

    [SerializeField] private AudioSource dodgeSound;



    private void OnSlashAttack1()
    {
        slashSound1.Play();
    }

    private void OnSlashAttack2()
    {
        slashSound2.Play();
    }

    private void OnThrustAttack1()
    {
        thrustSound.Play();
    }
    private void OnDodge()
    {
        dodgeSound.Play();
    }
}
