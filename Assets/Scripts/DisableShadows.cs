using UnityEngine;

public class DisableShadows : MonoBehaviour
{
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        // Desactivar emisión y recepción de sombras
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }
}
