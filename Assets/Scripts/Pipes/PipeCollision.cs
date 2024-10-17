using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeCollision : MonoBehaviour
{
    public PipeGenerator pipeGenerator;
    private GameObject killCube; // Referencia al KillCube
    private bool hasScored = false;
    private bool isRaising = false;

    [SerializeField] private AudioClip plop;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = plop;
        audioSource.playOnAwake = false;
        audioSource.loop = false;

         if (pipeGenerator != null)
    {
        killCube = pipeGenerator.killCube; // Acceder a killCube desde el PipeGenerator
        if (killCube != null)
        {
            // Inicializar la posición del KillCube al nivel del suelo
            killCube.transform.position = new Vector3(killCube.transform.position.x, pipeGenerator.soilLevel - (killCube.transform.localScale.y / 2), killCube.transform.position.z);
        }
    }
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

                raiseKillZone(pipeCollider);
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

void raiseKillZone(Collider pipeCollider)
{
    if (!isRaising && killCube != null)
    {
        Vector3 pipeSize = pipeCollider.bounds.size;
        killCube.transform.localScale = new Vector3(pipeSize.x + 0.1f, pipeSize.y + 0.1f, pipeSize.z + 0.1f);

        // Ajustar la posición inicial por debajo del suelo
        killCube.transform.position = new Vector3(pipeCollider.transform.position.x, pipeGenerator.soilLevel - (killCube.transform.localScale.y / 2), killCube.transform.position.z);

        // Calcular la altura objetivo correctamente
        float targetHeight = (pipeSize.y / 2) + 0.2f; // Ajustar para la posición sobre el suelo

        StartCoroutine(RaiseKillCube(targetHeight, pipeCollider));
    }
}

private IEnumerator RaiseKillCube(float targetHeight, Collider pipeCollider)
{
    isRaising = true;

    float elapsedTime = 0f;
    float duration = 5.0f;
    Vector3 startPosition = killCube.transform.position;

    while (elapsedTime < duration)
    {
        // Aquí usamos el Vector3.Lerp para elevar el killCube
        killCube.transform.position = Vector3.Lerp(startPosition, new Vector3(startPosition.x, targetHeight, startPosition.z), elapsedTime / duration);
        elapsedTime += Time.deltaTime;

        Debug.Log("Raising KillCube... Current Y: " + killCube.transform.position.y);

        yield return null;
    }

    // Asegúrate de que el killCube no exceda la altura de la tubería + 0.1f
    killCube.transform.position = new Vector3(startPosition.x, targetHeight, startPosition.z);
    Debug.Log("KillCube has reached the target height!");

     GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
         if (playerController != null)
        {
            // Obtener el rango de ancho del pipe
            float pipeWidth = pipeCollider.bounds.size.x;

            // Definir el rango en el que el jugador debe estar
            float halfPipeWidth = pipeWidth / 2f;

            // Comprobar si el jugador está dentro del rango de ancho del pipe
            if (player.transform.position.x >= killCube.transform.position.x - halfPipeWidth &&
                player.transform.position.x <= killCube.transform.position.x + halfPipeWidth &&
                killCube.transform.position.y >= player.transform.position.y) // Comprobación de que killCube alcance o supere la altura del jugador
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
    Vector3 killCubePosition = killCube.transform.position;
    float targetHeight = pipeGenerator.soilLevel - (killCube.transform.localScale.y / 2); // Volver por debajo del suelo

    float elapsedTime = 0f;
    float duration = 2.0f; // Duración de la bajada en segundos
    Vector3 startPosition = killCube.transform.position;

    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;

        // Calcular la posición actual usando Lerp
        float t = Mathf.Clamp01(elapsedTime / duration);
        Vector3 newPosition = Vector3.Lerp(startPosition, new Vector3(startPosition.x, targetHeight, startPosition.z), t);
        killCube.transform.position = newPosition;

        Debug.Log("Lowering KillCube... Current Y: " + killCube.transform.position.y);
        yield return null;
    }

    // Asegúrate de que la posición final sea exactamente el targetHeight
    killCube.transform.position = new Vector3(startPosition.x, targetHeight, startPosition.z);
    isRaising = false; // Permitir que vuelva a subir cuando sea necesario
}


}