using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeCollision : MonoBehaviour
{
    [SerializeField] private AudioClip plop;
    public PipeGenerator pipeGenerator;

    private GameObject killCube;
    private AudioSource audioSource;
    private bool hasScored = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = plop;
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        if (pipeGenerator != null)
        {
            killCube = pipeGenerator.killCube;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 contactPoint = contact.point;
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            Collider pipeCollider = GetComponent<Collider>();
            float playerHeight = playerController.GetComponent<Collider>().bounds.size.y;
            float pipeHeight = pipeCollider.bounds.size.y;

            Debug.Log("Contact Point: " + contactPoint.y + " Pipe Height: " + pipeHeight);
            Debug.Log("Player Height: " + (pipeHeight - playerHeight / 2 - 0.1f));

            if (contactPoint.y >= pipeHeight - playerHeight / 2 - 0.1f)
            {
                DragAndShoot dragAndShoot = playerController.GetComponent<DragAndShoot>();

                if (playerController != null && !hasScored)
                {
                    audioSource.Play();
                    playerController.IncreaseScore();
                    hasScored = true;

                    // Increase raise speed if score is divisible by 5
                    if (playerController.GetScore() % 5 == 0 && playerController.GetScore() != 0)
                    {
                        float raiseSpeed = killCube.GetComponent<DeathCube>().raiseSpeed;
                        float maxRaiseSpeed = killCube.GetComponent<DeathCube>().maxRaiseSpeed;
                        killCube.GetComponent<DeathCube>().raiseSpeed = Mathf.Min(raiseSpeed + 0.5f, maxRaiseSpeed); // Increase but limit to maxRaiseSpeed
                    }

                    StartCoroutine(CenterPlayerOnPipe(playerController, pipeHeight, playerHeight));
                }

                PrepareKillCube(pipeCollider, pipeHeight, pipeGenerator);
                dragAndShoot.SetCanShoot(true);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (killCube != null)
            {
                killCube.GetComponent<DeathCube>().StopRaising(); // Stop raising when exiting the pipe
                killCube.GetComponent<DeathCube>().StartVanishing(1f); // Change 1f to your desired speed
            }
        }
    }

    private IEnumerator CenterPlayerOnPipe(PlayerController playerController, float pipeHeight, float playerHeight)
    {
        Vector3 pipePosition = transform.position;
        float targetY = pipeHeight + (playerHeight / 2);
        Vector3 targetPosition = new Vector3(pipePosition.x, targetY, playerController.transform.position.z);

        float duration = 0.5f;
        float elapsedTime = 0f;

        playerController.stopPlayer();

        while (elapsedTime < duration)
        {
            playerController.transform.position = Vector3.Lerp(
                playerController.transform.position,
                targetPosition,
                elapsedTime / duration
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerController.transform.position = targetPosition;
    }

    private void PrepareKillCube(Collider pipeCollider, float pipeHeight, PipeGenerator pipeGenerator)
    {
        if (killCube != null)
        {
            killCube.GetComponent<DeathCube>().RestartAlphaIntensity();

            Vector3 newPosition = new Vector3(
                transform.position.x,
                pipeGenerator.soilLevel - (killCube.transform.localScale.y / 2),
                transform.position.z
            );

            killCube.GetComponent<DeathCube>().MoveCube(newPosition);

            killCube.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
            Vector3 pipeSize = pipeCollider.bounds.size;

            // Adjust the size of the death cube to match the pipe collider size
            killCube.transform.localScale = new Vector3(
                pipeSize.x + 0.1f,
                pipeSize.y + 0.1f,
                pipeSize.z + 0.1f
            );

            // Move the cube and check if the player dies within the area
            killCube.GetComponent<DeathCube>().RaiseKillCube(pipeCollider, pipeGenerator);
        }
    }
}
