using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCube : MonoBehaviour
{
    private Material material;  // Cambiado de public a private

    void Start()
    {
        // Obtén el material del renderizador del objeto al que está adjunto este script
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;  // Asigna el material del renderizador
        }
        else
        {
            Debug.LogWarning("No Renderer found on the object. Material not assigned.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.Die();
            }
        }
    }

    public void StartVanishing(float vanishSpeed)
    {
        StartCoroutine(VanishCoroutine(vanishSpeed));
    }

    private IEnumerator VanishCoroutine(float vanishSpeed)
    {
        // Asegúrate de que el material fue asignado correctamente
        if (material != null)
        {
            float alphaIntensity = material.GetFloat("_alphaIntensity");
            float _Emission = material.GetFloat("_Emission");
            float _Alpha = material.GetFloat("_Alpha");

            while (alphaIntensity > 0f && _Emission > 0f && _Alpha > 0f)
            {
                alphaIntensity -= Time.deltaTime * vanishSpeed;
                _Emission -= Time.deltaTime * vanishSpeed;
                _Alpha -= Time.deltaTime * vanishSpeed;

                // Asegurarse de que los valores no sean menores a 0
                alphaIntensity = Mathf.Max(alphaIntensity, 0f);
                _Emission = Mathf.Max(_Emission, 0f);
                _Alpha = Mathf.Max(_Alpha, 0f);

                // Actualizar los valores en el material
                material.SetFloat("_alphaIntensity", alphaIntensity);
                material.SetFloat("_Emission", _Emission);
                material.SetFloat("_Alpha", _Alpha);

                yield return null;
            }
        }
        else
        {
            Debug.LogError("Material not assigned.");
        }
    }

    public void RestartAlphaIntensity()
    {
        if (material != null)
        {
            material.SetFloat("_alphaIntensity", 1f);
            material.SetFloat("_Emission", 1f);
            material.SetFloat("_Alpha", 1f);
        }
        else
        {
            Debug.LogError("Material not assigned.");
        }
    }

    public void MoveCube(Vector3 pos)
    {
        transform.position = pos;
    }
}
