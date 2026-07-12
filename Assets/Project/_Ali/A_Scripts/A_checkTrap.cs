using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Required for Exposure

public class A_checkTrap : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject torchPickUp;
    [SerializeField] private GameObject lava;
    [SerializeField] private AudioSource soundtrack;

    public bool isHoldingTorch = false;
    private bool firstTime;

    private List<GameObject> lightObjects;
    private List<GameObject> darkObjects;

    private void Update()
    {
        if (torchPickUp.transform.IsChildOf(player))
        {
            isHoldingTorch = true;
            RenderSettings.reflectionIntensity = 0; 
            lava.SetActive(false);
            foreach (GameObject obj in lightObjects)
            {
                obj.SetActive(false);
            }
            foreach (GameObject obj in darkObjects)
            {
                obj?.SetActive(true);
            }
            soundtrack.pitch = 0.3f;


        }
        else
        {
            isHoldingTorch = false;
            RenderSettings.reflectionIntensity = 1;
            lava.SetActive(true);
            foreach (GameObject obj in darkObjects)
            {
                obj.SetActive(false);
            }
            foreach(GameObject obj in lightObjects)
            {
                obj.SetActive(true);
            }
            soundtrack.pitch = 1f;
        }
    
    }

   
    private void Start()
    {
        GameObject[] foundLights = GameObject.FindGameObjectsWithTag("LightObject");
        lightObjects = new List<GameObject>(foundLights);

        GameObject[] foundDark = GameObject.FindGameObjectsWithTag("DarkObject");
        darkObjects = new List<GameObject>(foundDark);
    }
}