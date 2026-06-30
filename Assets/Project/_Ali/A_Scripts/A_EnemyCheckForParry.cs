using PixelCrushers.DialogueSystem;
using System;
using UnityEngine;

public class A_EnemyCheckForParry : MonoBehaviour
{
    public event Action OnParry;



    private void OnTriggerEnter(Collider other)
    {
        // we check for parry first then we check for damage

        if (other.gameObject.CompareTag("Parry Collider"))
        {
            OnParry?.Invoke();
        }
    }
    }
