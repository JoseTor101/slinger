using UnityEngine;

[ExecuteInEditMode]
public class ImageEffectScript : MonoBehaviour
{
    public Material pixelationMaterial; // Aquí se asignará el material con el shader de pixelación

    // Este método aplica el efecto de pixelación cada vez que se renderiza una imagen
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Si el material de pixelación está asignado, aplica el efecto
        if (pixelationMaterial != null)
        {
            Graphics.Blit(source, destination, pixelationMaterial);
        }
        else
        {
            // Si no hay material asignado, simplemente renderiza sin cambios
            Graphics.Blit(source, destination);
        }
    }
}
