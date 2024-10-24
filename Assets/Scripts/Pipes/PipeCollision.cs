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
    private bool isColliding;

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
            isColliding = true;
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

                PrepareKillCube(pipeCollider, pipeHeight);
                dragAndShoot.SetCanShoot(true);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isColliding = false;
            // Aquí llamamos a StartVanishing al salir de la colisión
            if (killCube != null)
            {
                killCube.GetComponent<DeathCube>().StartVanishing(1f); // Cambia 0.5f por la velocidad que desees
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

    // Preparar el cubo de muerte
    private void PrepareKillCube(Collider pipeCollider, float pipeHeight)
    {
        if (!isRaising && killCube != null)
        {
            killCube.GetComponent<DeathCube>().RestartAlphaIntensity();

            killCube.GetComponent<DeathCube>().MoveCube(
                new Vector3(
                    transform.position.x,
                    pipeGenerator.soilLevel - (killCube.transform.localScale.y / 2),
                    transform.position.z
                )
            );

            killCube.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
            Vector3 pipeSize = pipeCollider.bounds.size;

            // Ajustar el tamaño del cubo de muerte al tamaño del collider de la tubería
            killCube.transform.localScale = new Vector3(
                pipeSize.x + 0.1f, 
                pipeSize.y + 0.1f, 
                pipeSize.z + 0.1f
            );

            Debug.Log("X: " + pipeSize.y + "XX: " + pipeHeight);

            float targetHeight = (pipeSize.y / 2) + 0.2f; 
            Vector3 startPosition = killCube.transform.position;

            StartCoroutine(RaiseKillCube(startPosition, targetHeight, pipeCollider));
        }
    }

    private IEnumerator RaiseKillCube(Vector3 startPosition, float targetHeight, Collider pipeCollider)
{
    isRaising = true;

    float elapsedTime = 0f;
    float duration = 5.0f;
    float stopSpeedFactor = 1f; // Factor para reducir la velocidad cuando no esté colisionando

    while (elapsedTime < duration)
    {
        // Si ya no está colisionando, reduce la velocidad gradualmente
        if (!isColliding)
        {
            stopSpeedFactor = Mathf.Max(0f, stopSpeedFactor - Time.deltaTime * 0.5f); // Reduce velocidad
        }

        // Lerp ajustado por el factor de reducción de velocidad
        killCube.transform.position = Vector3.Lerp(
            startPosition, 
            new Vector3(startPosition.x, targetHeight, startPosition.z), 
            (elapsedTime / duration) * stopSpeedFactor // Multiplicar por el factor
        );
        
        elapsedTime += Time.deltaTime;

        // Salir del ciclo si stopSpeedFactor llega a 0
        if (stopSpeedFactor == 0f)
        {
            break;
        }

        yield return null;
    }

    killCube.transform.position = new Vector3(startPosition.x, targetHeight, startPosition.z);

    // Verificar si el jugador está dentro del área del killCube para aplicar la lógica de muerte
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

    yield return new WaitForSeconds(0f);
}

}
