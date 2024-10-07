using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeCollision : MonoBehaviour
{
    public PipeGenerator pipeGenerator;

    private bool hasScored = false;

    [SerializeField] private AudioClip plop;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = plop;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 contactPoint = contact.point;

            Collider pipeCollider = GetComponent<Collider>();

            if (contactPoint.y >= pipeCollider.bounds.center.y + (pipeCollider.bounds.size.y / 2) - 0.1f)
            {
                PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
                DragAndShoot dragAndShoot = playerController.GetComponent<DragAndShoot>();

                if (playerController != null && !hasScored)
                {
                    audioSource.Play();

                    StartCoroutine(CenterPlayerOnPipe(playerController));

                    playerController.IncreaseScore();
                    hasScored = true;
                }

                dragAndShoot.SetCanShoot(true);
            }
        }
    }


    private IEnumerator CenterPlayerOnPipe(PlayerController playerController)
    {
        Vector3 pipePosition = transform.position;

        float pipeHeight = GetComponent<Collider>().bounds.size.y;
        float playerHeight = playerController.GetComponent<Collider>().bounds.size.y;

        float targetY = pipePosition.y + (pipeHeight / 2) + (playerHeight / 2);

        Vector3 targetPosition = new Vector3(pipePosition.x, targetY, playerController.transform.position.z);
        float duration = 0.5f;
        float elapsedTime = 0f;

        playerController.stopPlayer();

        while (elapsedTime < duration)
        {
            playerController.transform.position = Vector3.Lerp(playerController.transform.position, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerController.transform.position = targetPosition;
    }

    void Update()
{
    if (Input.GetKeyDown(KeyCode.Space)) // Change the key as needed
    {
        audioSource.Play(); // Test playing the sound
    }
}


}