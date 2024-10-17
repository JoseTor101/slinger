using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float fallMultiplier = 2.5f;
    [SerializeField] public float lowJumpMultiplier = 2f;

    private Vector3 customForce;
    private Rigidbody rb; // Reference to the player's Rigidbody

    private int score;
    private int level;
    public TextMeshProUGUI scoreCounter;
    public TextMeshProUGUI levelCounter;
    public TextMeshProUGUI deathMessage;

    private PlayerDeath playerDeath;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        score = 0;
        level = 1;
        deathMessage.gameObject.SetActive(false); // Hide death message at start

        playerDeath = GetComponent<PlayerDeath>();
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

        // Avoid issues with bouncing
        rb.AddForce(new Vector3(customForce.x, customForce.y, 0));
    }

    public void SetCustomForce(Vector3 newForce)
    {
        customForce = newForce;
    }

    public void IncreaseScore()
    {
        score += 1;
        SetCountText();

        if (score % 10 == 0)
        {
            level += 1;
            setLevelText();
        }
    }

    public void stopPlayer()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void SetCountText()
    {
        scoreCounter.text = score.ToString();
    }

    void setLevelText()
    {
        levelCounter.text = "LEVEL: " + level.ToString();
    }

    public int GetScore()
    {
        return score;
    }

    // Call this method when the player dies
    public void Die()
    {
        stopPlayer(); // Stop player movement
        deathMessage.text = "You Died!"; // Set death message
        deathMessage.gameObject.SetActive(true); // Show the death message

        playerDeath.OnPlayerDeath(); // Activate the dissolve effect
        // Optionally, restart or reset the game after a delay
        Invoke("RestartGame", 2f); // Restart the game after 2 seconds
    }

    // Method to restart the game or reset player state
    private void RestartGame()
    {
        // Reset the player position, score, level, and hide the death message
        transform.position = new Vector3(0, 23, -1); // Reset position (update as necessary)
        score = 0;
        level = 1;
        SetCountText();
        setLevelText();
        playerDeath.restartDeath();
        deathMessage.gameObject.SetActive(false); // Hide the death message

        PipeGenerator pipeGenerator = FindObjectOfType<PipeGenerator>();
    if (pipeGenerator != null)
    {
        pipeGenerator.RestartPipes();
    }
    }
}
