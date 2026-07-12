using UnityEngine;

public class A_BigEnemyAi : A_EnemyAI
{
    [SerializeField]  private SphereCollider attackHitBox;
    [SerializeField] private ParticleSystem attackEffect;

    [SerializeField] private int hitsBeforeStun;
    [SerializeField] private int curentHits;


    protected override void OnHitEvent()
    {
        targetPostion = transform.position - (transform.forward * knockbackDistance);
        hitTimer = 0;
        isHit = true;

        damageCollider.enabled = false;
        if (curentHits <= 0)
        {
            curentHits = hitsBeforeStun;
            _agent.isStopped = true;
            animator.SetTrigger("HitReaction");
        }
        else
        {
            curentHits--;
        }
    }
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
