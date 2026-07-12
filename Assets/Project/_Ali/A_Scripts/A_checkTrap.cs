using UnityEngine;

public class A_checkTrap : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject torchPickUp;

    [SerializeField] private GameObject lava;

    public bool isHoldingTorch = false;
    private void Update()
    {
        if (torchPickUp.transform.IsChildOf(player))
        {
            isHoldingTorch = true;
            RenderSettings.ambientIntensity = 0f;
            lava.SetActive(false);
        }
       
    }
    }
