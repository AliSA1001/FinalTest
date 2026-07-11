using UnityEngine;

public class A_Lava : MonoBehaviour
{
  
    // Adjust these in the Inspector
    public float scrollX = 0.1f;
    public float scrollY = 0.2f;

    private Material lavaMaterial;

    void Start()
    {
        // Get the material from the Renderer
        Renderer rend = GetComponent<Renderer>();
        lavaMaterial = rend.material;
    }

    void Update()
    {
        // Calculate the new offset based on time
        float offsetX = Time.time * scrollX;
        float offsetY = Time.time * scrollY;

        // Apply it to the material
        lavaMaterial.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}

