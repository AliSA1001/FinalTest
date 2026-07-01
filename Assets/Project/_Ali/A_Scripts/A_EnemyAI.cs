using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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
    [SerializeField] protected float timeBetweenAttacks;
    private bool _alreadyAttacked;
   [SerializeField] protected SphereCollider damageCollider;
    [SerializeField] protected A_EnemyCheckForParry hitColliderSystem;

    //States 
    [SerializeField] protected float sightRange, attackRange;
    [SerializeField] protected bool playerInSightRange, playerInAttackRange;
    // based on the set of States we will change the aniamtion 
    // 1- standing animtion
    // 2- walking - when patroling
    // 3- runing - when chesing
    // 4- attacking  - when attacking (:

    //Conection
    [SerializeField] private EnemyHealth enemyHealth;

    // we nned bool to check if we hit or not
    [SerializeField] protected float knockbackSpeed;
    [SerializeField] protected float knockbackDistance;
    protected bool isHit;
    protected Vector3 targetPostion;
    protected float hitTimer;

    // Parry
    protected bool isParryed = false;
    protected bool isParryWindow;
    [SerializeField] protected ParticleSystem stunEffect;
  

    [Header("animation")]
    [SerializeField] protected Animator animator;



    protected virtual void Awake()
    {
        player = GameObject.Find("Player").transform; // here we tell it to find my boy the player!!
        _agent = GetComponent<NavMeshAgent>();
    }
    protected virtual void Start()
    {
        enemyHealth.OnHit += OnHitEvent;
        hitColliderSystem.OnParry += OnParryEvent;
    }

    protected virtual void OnParryWindowStart()
    {
        isParryWindow = true;
        damageCollider.enabled = true;
    }
    protected virtual void OnParryWindowEnd()
    {
        isParryWindow = false;
        damageCollider.enabled = false;

    }


    protected virtual void OnParryEvent()
    {
        if (isParryWindow)
        {
            damageCollider.enabled = false;
            isParryed = true;
            animator.SetTrigger("Parry");
            stunEffect.Play();
            Invoke("HandleEndParry", 2);
        }

    }
    protected virtual void HandleEndParry()
    {
        isParryWindow = false ;
        isParryed = false;
        animator.SetTrigger("StunEnd");
    }

    protected virtual void OnHitEvent()
    {
        targetPostion = transform.position - (transform.forward * knockbackDistance);
        hitTimer = 0;
        isHit = true;

        _agent.isStopped = true;
        damageCollider.enabled = false;
        animator.SetTrigger("HitReaction");
    }

    protected virtual void Update()
    {
        if (isHit)
        {
            hitTimer += Time.deltaTime * knockbackSpeed;
            transform.position = Vector3.Lerp(transform.position, targetPostion, hitTimer);
            if(hitTimer >=1)
            {
                isHit = false;
                _agent.isStopped = false;
            }
        }


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

    protected virtual void HandleAnimationAndMovementSpeed(int stateNUM)
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


    protected virtual void Patroling()
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


    protected virtual void SearchWalkForPoint()
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

    protected virtual void ChasePlayer()
    {
        _agent.SetDestination(player.position);
    }

    protected virtual void AttackPlayer()
    {
        _agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!_alreadyAttacked)
        {
            animator.SetTrigger("Attack");
           // damageCollider.enabled = true;
            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack),timeBetweenAttacks);
        }
    }


    protected virtual void ResetAttack()
    {
        damageCollider.enabled = false;
        _alreadyAttacked = false;
    }

}






