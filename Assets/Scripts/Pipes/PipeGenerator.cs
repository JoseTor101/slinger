
using UnityEngine;

public class PipeGenerator : MonoBehaviour
{

    public GameObject pipePrefab; 
    public Transform player;
    [SerializeField] int numberOfSegments = 10;
    [SerializeField] public float segmentLength = 1.0f; 
    [SerializeField] public float minSegmentHeight = 8f;
    [SerializeField] public float maxSegmentHeight = 20f; 
    [SerializeField] public float soilLevel = 0f; 
    [SerializeField] public float generationTriggerDistance = 30f;
    [SerializeField] public float lavaSpeed = 0.5f;
    private Vector3 lastPipePosition;
    private bool generating = false;
    //Check if player has got alredy a point from the pipe

    void Start()
    {
        
        if (pipePrefab == null || player == null)
        {
            Debug.LogError("Pipe prefab or player is not assigned.");
            return;
        }

        lastPipePosition = transform.position;
        lastPipePosition.y = soilLevel;


        GeneratePipes(numberOfSegments);
    }

    public Vector3 boxCenterOffset = Vector3.up; 
    public Vector3 boxSize = new Vector3(1.5f, 0.1f, 1.5f); 

    void Update()
    {
        // Check if player is near the last generated pipe
        if (!generating && Vector3.Distance(player.position, lastPipePosition) < generationTriggerDistance)
        {
            generating = true; // Prevent multiple simultaneous generations
            GeneratePipes(numberOfSegments); 
            generating = false;
        }

        /*foreach (Transform child in transform)
        {
            Renderer pipeRenderer = child.GetComponent<Renderer>();
            if (pipeRenderer != null)
            {
                pipeRenderer.material.SetFloat("_LavaSpeed", lavaSpeed);
            }
        }*/ 

    }

    void GeneratePipes(int segments)
    {
        for (int i = 0; i < segments; i++)
        {
          
            GameObject pipeSegment = Instantiate(pipePrefab, lastPipePosition, Quaternion.identity);
            pipeSegment.transform.SetParent(transform); // Set the parent to keep the hierarchy clean

            float segmentHeight = Random.Range(minSegmentHeight, maxSegmentHeight);
            pipeSegment.transform.localScale = new Vector3(pipeSegment.transform.localScale.x, segmentHeight, pipeSegment.transform.localScale.z);

            // Position the segment so it grows up from the soil
            pipeSegment.transform.position = new Vector3(lastPipePosition.x, soilLevel + segmentHeight / 2, lastPipePosition.z);

            // Add collision detector
            PipeCollision pipeCollision = pipeSegment.AddComponent<PipeCollision>();  // Attaching a script to detect collisions
            pipeCollision.pipeGenerator = this;

            lastPipePosition += new Vector3(segmentLength, 0, 0);
        }
    }

    
}