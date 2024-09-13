using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //[SerializeField] public float jumpForce = 10f;
    //[SerializeField] public float gravityScale = 2f;
    [SerializeField] public float fallMultiplier = 2.5f;
    [SerializeField] public float lowJumpMultiplier = 2f;

    private Vector3 customForce; 
    private Rigidbody rb; // Reference to the player's Rigidbody

    void Start()
    {
        // Get the Rigidbody component attached to the player
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        if (rb.velocity.y < 0)
        {
            // Player is falling, increase gravity
            rb.velocity += Vector3.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0)
        {
            rb.velocity += Vector3.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        
        //Avoid issues with bouncing
        rb.AddForce(new Vector3(customForce.x, customForce.y, 0));

    }

    public void SetCustomForce(Vector3 newForce)
    {
        customForce = newForce;
    }


    // Detect collision with the pipe
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Pipe") 
        {
            // Stop the player's movement by setting the velocity to zero
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Optionally, disable further movement if needed
            // Example: rb.isKinematic = true; // This stops all physics interaction
        }
    }
}
