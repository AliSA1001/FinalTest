using System;
using Unity.VisualScripting;
using UnityEngine;

public class A_Puzzle : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject torchPickUp;
    [SerializeField] private GameObject firstLight;
    [SerializeField] private GameObject secondLight;
    [SerializeField] private GameObject thirdLight;

    public event Action OnJumpPuuzle;

    private bool isWorng;

    public bool isHoldingTorch = false;

    private void Update()
    {
        if(torchPickUp.transform.IsChildOf(player))
        {
            isHoldingTorch= true;
        }
        else
        {
            isHoldingTorch= false;
        }

        if((secondLight.activeSelf || thirdLight.activeSelf) && !firstLight.activeSelf )
        {
            isWorng= true;
        }
        if(thirdLight.activeSelf && (!secondLight.activeSelf || !firstLight.activeSelf))
        {
           isWorng = true;

        }

        if(thirdLight.activeSelf && secondLight.activeSelf && firstLight.activeSelf)
        {
            if(!isWorng)
            {
                Debug.Log("oppppppppppppppen");
                OnJumpPuuzle?.Invoke();
            }
            else
            {
                thirdLight.SetActive(false);
                secondLight.SetActive(false);
                firstLight.SetActive(false);
                isWorng = false;
            }
        }
    }
    



}
