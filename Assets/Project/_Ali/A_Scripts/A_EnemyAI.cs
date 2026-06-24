using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class A_EnemyAI : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform[] PatrolPoints;
    [SerializeField] private bool isTargetingPlayer;


    private NavMeshAgent agent;
    private Transform _playerPostion;






    private void Start()
    {
        // here we only give values to our agent and navmeash agent
        agent = GetComponent<NavMeshAgent>();
        _playerPostion = player.transform;
    }

    private void Update()
    {
        if(!isTargetingPlayer)
        {
            if(PatrolPoints == null)return;
            else
            {
                HandlePatrolPointsMovement();
            }
        }
    }

   private void HandlePatrolPointsMovement()
    {

        for (int i = 0; i < PatrolPoints.Length; i++)
        {
            Vector3 currentPoint = new Vector3(PatrolPoints[i].position.x, PatrolPoints[i].position.y, PatrolPoints[i].position.z);
            agent.SetDestination(currentPoint);
          
            
        }
        HandlePatrolPointsMovement();

    }


}
