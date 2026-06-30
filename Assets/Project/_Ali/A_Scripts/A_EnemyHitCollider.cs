using PixelCrushers.DialogueSystem;
using System;
using UnityEngine;

public class A_EnemyHitCollider : MonoBehaviour
{
    public event Action<int> OnTookHit;

    public event Action OnParry;

  
    private void OnTriggerEnter(Collider other)
    {
        // we check for parry first then we check for damage

        if(other.gameObject.CompareTag("Parry Collider"))
        {
            OnParry?.Invoke();

        }

        if (other.gameObject.CompareTag("Small Enemy"))
        {
            OnTookHit?.Invoke(1);
        }
        
    }

   
}
