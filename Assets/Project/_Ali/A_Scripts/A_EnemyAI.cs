using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class A_EnemyAI : MonoBehaviour
{
  [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    private NavMeshAgent _agent;
    private Transform player;

    // patroling 
    [SerializeField]private Vector3 walkPoint;
    bool walkPointSet;
    [SerializeField] private float walkPointRange;

    //Attacking
    [SerializeField] private float timeBetweenAttacks;
    private bool _alreadyAttacked;
   [SerializeField] private SphereCollider damageCollider;
    [SerializeField] private A_EnemyHitCollider hitColliderSystem;

    //States 
    [SerializeField]private float sightRange, attackRange;
    [SerializeField] private bool playerInSightRange, playerInAttackRange;

    //Conection
    [SerializeField] private EnemyHealth enemyHealth;

    // Parry
    private bool isParryed = false;
    [SerializeField] private ParticleSystem stunEffect;
    // based on the set of States we will change the aniamtion 
    // 1- standing animtion
    // 2- walking - when patroling
    // 3- runing - when chesing
    // 4- attacking  - when attacking (:

    [Header("animation")]
    [SerializeField] private Animator animator;



    private void Awake()
    {
        player = GameObject.Find("Player").transform; // here we tell it to find my boy the player!!
        _agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        enemyHealth.OnHit += OnHitEvent;
        hitColliderSystem.OnParry += OnParryEvent;
    }

    private void OnParryEvent()
    {
        isParryed = true;
        animator.SetTrigger("Parry");
        stunEffect.Play();
        Invoke("HandleEndParry", 2);

    }
    private void HandleEndParry()
    {
        isParryed = false;
        animator.SetTrigger("StunEnd");
    }

    private void OnHitEvent()
    {
        damageCollider.enabled = false;
        animator.SetTrigger("HitReaction");
    }

    private void Update()
    {
        // we will use sphere to check around the ai 
        // first we use the checksphere to know if we are in sphere sightrange like the raycast
        playerInSightRange = Physics.CheckSphere(transform.position , sightRange , whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (isParryed)
        {
            _agent.SetDestination(transform.position);
        }
        else
        {

            if (!playerInSightRange && !playerInAttackRange)
            {
                Patroling();
                HandleAnimationAndMovementSpeed(0);
            }
            if (playerInSightRange && !playerInAttackRange)
            {
                ChasePlayer();
                HandleAnimationAndMovementSpeed(1);
            }
            if (playerInSightRange && playerInAttackRange)
            {
                AttackPlayer();
                HandleAnimationAndMovementSpeed(2);
            }
        }
    }

  private void HandleAnimationAndMovementSpeed(int stateNUM)
    {
        switch (stateNUM)
        {
            case 0:
                animator.SetBool("IsRuning", false);
                _agent.speed = 1;
                damageCollider.enabled = false;
                break;

                case 1:
                animator.SetBool("IsRuning", true);
                _agent.speed = 3.5f;
                damageCollider.enabled = false;
                break;
            case 2:
                _agent.speed = 0; // we will handle the attack logic in the attack method anyway sooo look there
                break;
                

        }

    }


    private void Patroling()
    {
        //if we dont set walk point we will look for point
        if (!walkPointSet) SearchWalkForPoint();

        if(walkPointSet)
        {
            _agent.SetDestination(walkPoint);

            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            // check if we got to the point 
            if( distanceToWalkPoint.magnitude < 1f)
            {
                walkPointSet = false;
            }
        }
    }

    
    private void SearchWalkForPoint()
    {
        // we look for random points
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        // we check if our walk point is on a ground
        if(Physics.Raycast(walkPoint, -transform.up, 2f , whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        _agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        _agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!_alreadyAttacked)
        {
            animator.SetTrigger("Attack");
            damageCollider.enabled = true;
            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack),timeBetweenAttacks);
        }
    }

    
    private void ResetAttack()
    {
        damageCollider.enabled = false;
        _alreadyAttacked = false;
    }

}






