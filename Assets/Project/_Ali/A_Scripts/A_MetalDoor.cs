using UnityEngine;

public class A_MetalDoor : MonoBehaviour
{
    [SerializeField] private A_Puzzle A_Puzzle;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        A_Puzzle.OnJumpPuuzle += A_Puzzle_OnJumpPuuzle;
    }

    private void A_Puzzle_OnJumpPuuzle()
    {
        animator.SetBool("IsOpen" , true);
    }
}
