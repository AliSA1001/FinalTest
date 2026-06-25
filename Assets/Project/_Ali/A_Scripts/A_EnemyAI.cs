using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class A_EnemyAI : MonoBehaviour
{
    [SerializeField] private GameObject player;


    private Vector3 _startingPostion;
    private NavMeshAgent agent;
    private Transform _playerPostion;




    private void Awake()
    {
        _startingPostion = transform.position;
    }

    private void Start()
    {
        // here we only give values to our agent and navmeash agent
        agent = GetComponent<NavMeshAgent>();
        _playerPostion = player.transform;
    }

    private void Update()
    {

    }

    private Vector3 GetRoamingPostion()
    {
        return _startingPostion + GetRandomDirection() * Random.Range(10f, 70f);
    }

    // get Random normalized direction 
    private Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-1,1), Random.Range(1,-1)).normalized;
    }
}
 

    



