using PixelCrushers.DialogueSystem;
using UnityEngine;

public class A_gunAttackEvents : MonoBehaviour
{
    [SerializeField] private A_Movement A_Movement;
    [SerializeField] private ParticleSystem ShotEffect1;
    [SerializeField] private ParticleSystem ShotEffect2;
    [SerializeField] private AudioSource gunSound;

    private void OnStartAttack()
    {
        A_Movement.CanMove = false;
    }
    private void OnEndAttack()
    {
        A_Movement.CanMove = true;
    }

    private void PlayEffect1()
    {
        ShotEffect1.Play();
        gunSound.pitch = Random.Range(1f, 1.5f);
        gunSound.Play();
    }


    private void PlayEffect2()
    {
        ShotEffect2 .Play();
        gunSound.pitch = Random.Range(1f, 1.5f);
        gunSound.Play();
    }
}
