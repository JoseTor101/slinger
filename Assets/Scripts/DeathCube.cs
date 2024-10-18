using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
             
             playerController.Die();
              
            }
        }
    }

    //Move kill cube to underground collided pipe
    public void MoveUnderground(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}
