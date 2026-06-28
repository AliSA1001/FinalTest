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
    bool walkPoints;
    [SerializeField] private float walkPointRange;

    //Attacking
    [SerializeField] private float timeBetweenAttacks;
    private bool _alreadyAttacked;

    //States 
    [SerializeField]private float sightRange, attackRange;
    [SerializeField] private bool playerInSightRange, playerInAttackRange;


    private void Awake()
    {
        player = GameObject.Find("player").transform; // here we tell it to find my boy the player!!
    }


}






