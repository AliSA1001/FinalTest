using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light screenLight;
    public float minTime = 0.05f;
    public float maxTime = 0.3f;

    void Start()
    {
        StartCoroutine(Flicker());
    }

    System.Collections.IEnumerator Flicker()
    {
        while (true)
        {
            screenLight.enabled = !screenLight.enabled;
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));
        }
    }
}