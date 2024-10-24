using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCube : MonoBehaviour
{
    private Material material;  
    public float raiseSpeed = 1.0f; 
    public float maxRaiseSpeed = 5.0f; 
    private Coroutine raiseCoroutine; // Store the currently running coroutine
    private bool isRaising = false; // Track if the cube is currently raising

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material; 
        }
        else
        {
            Debug.LogWarning("No Renderer found on the object. Material not assigned.");
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Die();
            }
        }
    }*/

    public void StartVanishing(float vanishSpeed)
    {
        StartCoroutine(VanishCoroutine(vanishSpeed));
    }

    private IEnumerator VanishCoroutine(float vanishSpeed)
    {
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

                alphaIntensity = Mathf.Max(alphaIntensity, 0f);
                _Emission = Mathf.Max(_Emission, 0f);
                _Alpha = Mathf.Max(_Alpha, 0f);

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

    public void RaiseKillCube(Collider pipeCollider, PipeGenerator pipeGenerator)
    {
        if (raiseCoroutine != null) // Stop the previous coroutine if it's running
        {
            StopCoroutine(raiseCoroutine);
        }

        isRaising = true; // Set the raising flag
        raiseCoroutine = StartCoroutine(RaiseKillCubeCoroutine(pipeCollider, pipeGenerator));
    }

    private IEnumerator RaiseKillCubeCoroutine(Collider pipeCollider, PipeGenerator pipeGenerator)
    {
        Vector3 startPosition = new Vector3(transform.position.x, 
                                         pipeGenerator.soilLevel - (transform.localScale.y / 2), // Underground start level
                                         transform.position.z);

        float targetHeight = pipeCollider.bounds.center.y + (pipeCollider.bounds.size.y / 2) + 0.2f;

        float distanceToTravel = targetHeight - startPosition.y;

        float duration = Mathf.Abs(distanceToTravel / raiseSpeed);

        float elapsedTime = 0f;

        while (elapsedTime < duration && isRaising)
        {
            transform.position = Vector3.Lerp(
                startPosition,
                new Vector3(startPosition.x, targetHeight, startPosition.z),
                (elapsedTime / duration)
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (isRaising)
        {
            transform.position = new Vector3(startPosition.x, targetHeight, startPosition.z);
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                float pipeWidth = pipeCollider.bounds.size.x;
                float halfPipeWidth = pipeWidth / 2f;

                if (player.transform.position.x >= transform.position.x - halfPipeWidth &&
                    player.transform.position.x <= transform.position.x + halfPipeWidth &&
                    transform.position.y >= player.transform.position.y)
                {
                    playerController.Die();
                }
            }
        }

        raiseCoroutine = null; // Clear the coroutine reference when done
        isRaising = false; // Reset the raising flag
        yield return null;
    }

    // Method to stop raising smoothly
    public void StopRaising()
    {
        isRaising = false; // Stop raising
        if (raiseCoroutine != null)
        {
            StopCoroutine(raiseCoroutine);
            raiseCoroutine = null; // Clear the coroutine reference
        }
    }
}
