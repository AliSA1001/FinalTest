using PixelCrushers.DialogueSystem;
using System;
using UnityEngine;

public class A_EnemyHitCollider : MonoBehaviour
{
    public event Action<int> OnTookHit;

  
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Small Enemy"))
        {
            OnTookHit?.Invoke(1);
        }
    }
}
