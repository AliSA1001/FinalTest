using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class A_checkTrap : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject torchPickUp;

    [SerializeField] private GameObject lava;
    [SerializeField] private Volume volume;
    

    public bool isHoldingTorch = false;
    private void Update()
    {
        if (torchPickUp.transform.IsChildOf(player))
        {
            isHoldingTorch = true;
            
            lava.SetActive(false);
        }
       
    }
    }
