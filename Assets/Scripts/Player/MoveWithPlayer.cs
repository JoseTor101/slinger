using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    public Transform player; // Reference to the player
    private Vector3 startPosition; // Initial position of the background
    private float offset; // Offset from the player's position

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player is not assigned");
            return;
        }

        startPosition = transform.position;
        
        offset = startPosition.x - player.position.x;
    }

    void Update()
    {

        float newPositionX = player.position.x + offset;

        transform.position = new Vector3(newPositionX, transform.position.y, transform.position.z);
    }
}
