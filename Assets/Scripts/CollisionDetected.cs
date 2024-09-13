using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public LayerMask pipeLayer;  // Assign this in the Inspector to identify pipe collisions
    private DragAndShoot dragAndShoot; 

    private void Start()
    {
        // Initialize dragAndShoot with the DragAndShoot component attached to the same GameObject
        dragAndShoot = GetComponent<DragAndShoot>();
        if (dragAndShoot == null)
        {
            Debug.LogError("DragAndShoot component not found on the GameObject!");
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pipe"))
        {
            // Calculate the contact point's position relative to the cube
            ContactPoint contact = collision.contacts[0];
            Vector3 contactPoint = contact.point;
            
            // Get the cube's collider bounds
            Collider pipeCollider = collision.collider;
            Bounds pipeBounds = pipeCollider.bounds;
            
            // Check if the contact point is on the upper face of the pipe
            if (contactPoint.y > pipeBounds.center.y)
            {
                Debug.Log("Player hit the upper face of the pipe!");
                if (dragAndShoot != null)
                {
                    dragAndShoot.SetCanShoot(true);
                }
                else
                {
                    Debug.LogError("DragAndShoot component is null when trying to set canShoot.");
                }
                
            }
        }
    }
}
