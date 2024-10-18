using System.Collections.Generic; // Required for List
using UnityEngine;

public class PipeGenerator : MonoBehaviour
{
    public GameObject pipePrefab;
    public Transform player;

    [SerializeField] int numberOfSegments = 10;
    [SerializeField] public float minSegmentHeight = 8f;
    [SerializeField] public float maxSegmentHeight = 20f;
    [SerializeField] public float soilLevel = 0f;
    [SerializeField] public float generationTriggerDistance = 30f;

    // Serialized fields to control pipe width (x) and depth (z)
    [SerializeField] public float xScale = 1.0f;
    [SerializeField] public float zScale = 1.0f;

    [SerializeField] public GameObject killCube; // For inspector visibility

    public float raiseSpeed = 0.5f;
    private Vector3 lastPipePosition;
    private bool generating = false;
    private float segmentLength = 10.0f;

    private PlayerController playerController;

    // List to hold all the generated pipe segments
    private List<GameObject> pipes = new List<GameObject>();

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (pipePrefab != null && player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            lastPipePosition = transform.position;
            lastPipePosition.y = soilLevel;

            GeneratePipes(numberOfSegments);
        }
    }

    void Update()
    {
        // Check if player is near the last generated pipe
        if (!generating && Vector3.Distance(player.position, lastPipePosition) < generationTriggerDistance)
        {
            generating = true; // Prevent multiple simultaneous generations
            GeneratePipes(numberOfSegments);
            generating = false;

            if (playerController.GetScore() % 10 == 0)
            {
                segmentLength += 2.0f;
            }
        }
    }

    public void RestartPipes() // Method to call on player restart
    {
        ClearPipes(); // Clear existing pipes
        lastPipePosition = transform.position; // Reset last pipe position
        lastPipePosition.y = soilLevel; // Ensure it starts from the correct height
        GeneratePipes(numberOfSegments); // Generate new pipes
    }

    void ClearPipes()
    {
        foreach (GameObject pipe in pipes)
        {
            Destroy(pipe); // Destroy each pipe in the list
        }
        pipes.Clear(); // Clear the list
    }

    void GeneratePipes(int segments)
    {
        for (int i = 0; i < segments; i++)
        {
            // New pipe in next position
            GameObject pipeSegment = Instantiate(pipePrefab, lastPipePosition, Quaternion.identity);
            pipeSegment.transform.SetParent(transform); // Set the parent to keep the hierarchy clean

            float segmentHeight = Random.Range(minSegmentHeight, maxSegmentHeight);

            // Set the scale of the pipe based on serialized values for xScale and zScale
            pipeSegment.transform.localScale = new Vector3(xScale, segmentHeight, zScale);

            // Position the segment so it grows up from the soil
            pipeSegment.transform.position = new Vector3(lastPipePosition.x, soilLevel, lastPipePosition.z);

            // Add collision detector
            PipeCollision pipeCollision = pipeSegment.AddComponent<PipeCollision>();  // Attaching a script to detect collisions
            pipeCollision.pipeGenerator = this;

            // Move last pipe position to account for the new segment's length
            lastPipePosition += new Vector3(segmentLength, 0, 0);

            // Add the newly created pipe segment to the list
            pipes.Add(pipeSegment);
        }
    }
}
