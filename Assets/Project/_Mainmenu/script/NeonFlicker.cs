using UnityEngine;
using System.Collections;

public class NeonFlicker : MonoBehaviour
{
    public Renderer textRenderer;

    Material mat;

    void Start()
    {
        mat = textRenderer.material;
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            // انتظار عشوائي بين كل ومضة
            yield return new WaitForSeconds(Random.Range(3f, 8f));

            int flashes = Random.Range(2, 6);

            for (int i = 0; i < flashes; i++)
            {
                mat.DisableKeyword("_EMISSION");
                yield return new WaitForSeconds(Random.Range(0.02f, 0.08f));

                mat.EnableKeyword("_EMISSION");
                yield return new WaitForSeconds(Random.Range(0.03f, 0.12f));
            }
        }
    }
}