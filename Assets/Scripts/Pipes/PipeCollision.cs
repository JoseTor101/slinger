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
    private bool isRaising = false;

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

                    StartCoroutine(CenterPlayerOnPipe(playerController, pipeHeight, playerHeight));
                }

                RaiseKillZone(pipeCollider, pipeHeight);
                dragAndShoot.SetCanShoot(true);
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

    //Prepare to start raising up the kill cube
    private void RaiseKillZone(Collider pipeCollider, float pipeHeight)
    {
        if (!isRaising && killCube != null)
        {

            Vector3 pipeSize = pipeCollider.bounds.size;
            
            //Move kill cube to underground collided pipe
            killCube.GetComponent<DeathCube>().MoveUnderground(
                new Vector3(
                    transform.position.x,
                    //pipeGenerator.soilLevel - (killCube.transform.localScale.y / 2),
                    pipeGenerator.soilLevel - pipeSize.y/2,
                    killCube.transform.position.z
                )
            );

            //Match the kill cube size to the pipe collider size
            killCube.transform.localScale = new Vector3(
                pipeSize.x + 0.1f, 
                //pipeHeight,
                pipeSize.y + 0.1f, 
                pipeSize.z + 0.1f
            );

            Debug.Log("X: " + pipeSize.y + "XX: " +  pipeHeight);
            
            //Move kill cube to underground collided pipe
            /*killCube.transform.position = new Vector3(
                pipeCollider.transform.position.x, 
                pipeGenerator.soilLevel - (killCube.transform.localScale.y / 2), 
                killCube.transform.position.z
            );*/

            /*killCube.GetComponent<DeathCube>().MoveUnderground(
                new Vector3(
                    transform.position.x,
                    pipeGenerator.soilLevel - (killCube.transform.localScale.y / 2),
                    transform.position.z
                )
            );*/

            float targetHeight = (pipeSize.y / 2) + 0.2f; 
            Vector3 startPosition = killCube.transform.position;

            StartCoroutine(RaiseKillCube(startPosition, targetHeight, pipeCollider));
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 startPosition = killCube.transform.position;
            Vector3 pipeSize = pipeCollider.bounds.size;
            float targetHeight = (pipeSize.y / 2) + 0.2f; 

            RaiseKillCube(startPosition, targetHeight, pipeCollider);
        }
    }
    
    //Try with OnCollisionStay and Exit
    private IEnumerator RaiseKillCube(Vector3 startPosition, float targetHeight, Collider pipeCollider)
    {
        isRaising = true;

        float elapsedTime = 0f;
        float duration = 5.0f;

        while (elapsedTime < duration)
        {
            killCube.transform.position = Vector3.Lerp(
                startPosition, 
                new Vector3(startPosition.x, targetHeight, startPosition.z), 
                elapsedTime / duration
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        killCube.transform.position = new Vector3(startPosition.x, targetHeight, startPosition.z);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                float pipeWidth = pipeCollider.bounds.size.x;
                float halfPipeWidth = pipeWidth / 2f;

                if (player.transform.position.x >= killCube.transform.position.x - halfPipeWidth &&
                    player.transform.position.x <= killCube.transform.position.x + halfPipeWidth &&
                    killCube.transform.position.y >= player.transform.position.y)
                {
                    playerController.Die();
                }
            }
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine(LowerKillCube());
    }

    private IEnumerator LowerKillCube()
    {
        Vector3 startPosition = killCube.transform.position;
        float targetHeight = pipeGenerator.soilLevel - (killCube.transform.localScale.y / 2);

        float elapsedTime = 0f;
        float duration = 2.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            Vector3 newPosition = Vector3.Lerp(
                startPosition, 
                new Vector3(startPosition.x, targetHeight, startPosition.z), 
                Mathf.Clamp01(elapsedTime / duration)
            );
            killCube.transform.position = newPosition;
            yield return null;
        }

        killCube.transform.position = new Vector3(startPosition.x, targetHeight, startPosition.z);
        isRaising = false;
    }
}
