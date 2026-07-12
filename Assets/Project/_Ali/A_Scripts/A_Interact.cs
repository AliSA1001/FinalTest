using UnityEngine;

public class A_Interact : MonoBehaviour
{
    [SerializeField] private A_Puzzle A_Puzzle;
    [SerializeField] private bool canInteract = false;
    [SerializeField] protected GameObject fire;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canInteract = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canInteract = false;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && canInteract && A_Puzzle.isHoldingTorch)
        {
             if (!fire.activeSelf)
            { 
                fire.SetActive(true);
            }
        }
    }
}
