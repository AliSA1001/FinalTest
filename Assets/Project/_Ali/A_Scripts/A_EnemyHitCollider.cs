using PixelCrushers.DialogueSystem;
using System;
using UnityEngine;

public class A_EnemyHitCollider : MonoBehaviour
{
    public event Action<int> OnTookHit;

     private A_Parry parry;
     private A_Dodge dodge;


    private void Start()
    {
        parry = gameObject.GetComponent<A_Parry>();
        dodge = gameObject.GetComponent<A_Dodge>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!parry.isInvincbal && !dodge.isInvincbal)
        {

            if (other.gameObject.CompareTag("Small Enemy"))
            {
                OnTookHit?.Invoke(1);
            }
            else if(other.gameObject.CompareTag("Big Enemy"))
            {
                OnTookHit?.Invoke(2);
            }
        }
    }
}
