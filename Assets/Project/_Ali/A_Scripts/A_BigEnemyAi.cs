using UnityEngine;

public class A_BigEnemyAi : A_EnemyAI
{
    [SerializeField]  private SphereCollider attackHitBox;
    [SerializeField] private ParticleSystem attackEffect;
    private void OnAttackGroundSlam()
    {
        Debug.Log("We are INNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN");
        attackHitBox.enabled = true;
        attackEffect.Play();    
    }
    private void OnAtttackSlamEnd()
    {
        Debug.Log("We are outttttttttttttttttttt");

        attackHitBox.enabled = false;
    }
}
