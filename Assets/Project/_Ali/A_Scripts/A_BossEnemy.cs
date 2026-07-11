using UnityEngine;

public class A_BossEnemy : A_EnemyAI
{
    [SerializeField] private int hitsBeforeStun;
    [SerializeField] private int curentHits;

    [SerializeField] private SphereCollider attackHitBox;
    [SerializeField] private ParticleSystem attackEffect;

    private void Awake()
    {
        curentHits = hitsBeforeStun;
    }
    protected  override void OnHitEvent()
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

