using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public Material enemyMaterial; // Assign the pulsating shader material
    public float spawnHeight = 10f; // Initial spawn height
    public float fallDelay = 5f; // Time after which gravity is enabled
    public float pulsateIncreaseRate = 5f; // Rate of pulsation speed increase
    public float initialPulsateSpeed = 1f; // Starting pulsation speed
    public float fallSpeed = 5f; // Speed at which the spike falls
    public float destroyYPosition = -5f; // Y position where the spike should be destroyed

    public Transform player; // Reference to the player Transform
    public float triggerDistanceOffset = 1f; // Distance from the spike to trigger the fall effect

    private Rigidbody rb;
    private float pulsateSpeed;
    private bool isFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
        pulsateSpeed = initialPulsateSpeed;

        enemyMaterial.SetFloat("_pulsateSpeed", pulsateSpeed);

        StartCoroutine(StartPulsatingAndFall());
    }

    private IEnumerator StartPulsatingAndFall()
{
    float elapsedTime = 0f;

    // Pulsate the enemy before it starts falling
    while (elapsedTime < fallDelay)
    {
        pulsateSpeed += Time.deltaTime * pulsateIncreaseRate;
        enemyMaterial.SetFloat("_pulsateSpeed", pulsateSpeed);

        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // After pulsating, check if the player is close enough to trigger the fall
    if (player != null)
    {
        Vector3 spikePosition = transform.position;
        float distanceToPlayer = Mathf.Abs(player.position.x - spikePosition.x); // Only consider the X distance

        // Check if the player is within the trigger distance (1f + segmentLength / 2)
        if (distanceToPlayer <= (1f + (GetComponent<PipeGenerator>().segmentLength / 2)))
        {
            rb.isKinematic = false; // Enable physics after pulsating
            isFalling = true; // Set falling to true
        }
    }
}

    void Update()
    {
        // Make the spike fall if it has started falling
        if (isFalling)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

            // Destroy the spike if it goes below the destroyYPosition
            if (transform.position.y < destroyYPosition)
            {
                Destroy(gameObject); // Destroy this spike
            }
        }
    }
}
