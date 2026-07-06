using UnityEngine;

public class A_LockHelathBarTransform : MonoBehaviour
{
    private Quaternion fixedLocation;


    private void Awake()
    {
        fixedLocation = transform.rotation;
    }


    private void Update()
    {
        transform.rotation = fixedLocation;
    }
}
