using UnityEngine;
using TMPro;
using System.Collections;


public class PlayerController : MonoBehaviour
{
    [SerializeField] public float fallMultiplier = 2.5f;
    [SerializeField] public float lowJumpMultiplier = 2f;
    [SerializeField] public AudioClip deathSound;
    [SerializeField] public AudioClip backgroundMusic; // Background music clip

    private Vector3 customForce;
    private Rigidbody rb; // Reference to the player's Rigidbody

    private int score;
    private int level;
    public TextMeshProUGUI scoreCounter;
    public TextMeshProUGUI levelCounter;
    public TextMeshProUGUI deathMessage;

    private PlayerDeath playerDeath;
    private AudioSource audioSource;
    private AudioSource musicSource; // AudioSource for background music

    private GameObject killCube;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        score = 0;
        level = 1;
        deathMessage.gameObject.SetActive(false); // Hide death message at start

        playerDeath = GetComponent<PlayerDeath>();

        // AudioSource for death sound
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = deathSound;
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        // AudioSource for background music
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.loop = true; // Loop the background music
        musicSource.volume = 0.25f; // Set initial volume (adjust as needed)
        musicSource.Play(); // Start playing the background music

        killCube = GameObject.Find("KillCube");
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
        audioSource.Play(); // Play death sound
        FadeOutMusic(); // Fade out background music

        DeathCube deathCube = killCube.GetComponent<DeathCube>();
        if (deathCube != null)
        {
            deathCube.MoveCube(new Vector3(0, -20f, 0)); 
        }

        Invoke("RestartGame", 4f); // Restart the game after 4 seconds
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

        killCube.transform.position = new Vector3(killCube.transform.position.x, -20f, killCube.transform.position.z); // Reset killCube position

        killCube.GetComponent<DeathCube>().raiseSpeed = 1f; // Reset raise speed
        PipeGenerator pipeGenerator = FindObjectOfType<PipeGenerator>();
        if (pipeGenerator != null)
        {
            pipeGenerator.RestartPipes();
        }

        // Restart background music (if needed)
        musicSource.volume = 0.5f; // Reset volume (or adjust as necessary)
        musicSource.Play();
    }

    private void FadeOutMusic()
    {
        StartCoroutine(FadeOutCoroutine(1f)); // Fade out over 1 second
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = musicSource.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }

        musicSource.Stop(); // Stop the music after fading out
        musicSource.volume = startVolume; // Reset volume for next play
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Die();
        }

        
    }


}
