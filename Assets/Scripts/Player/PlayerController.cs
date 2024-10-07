using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{

    //[SerializeField] public float jumpForce = 10f;
    //[SerializeField] public float gravityScale = 2f;
    [SerializeField] public float fallMultiplier = 2.5f;
    [SerializeField] public float lowJumpMultiplier = 2f;

    private Vector3 customForce;
    private Rigidbody rb; // Reference to the player's Rigidbody

    private int score;
    private int level;
    public TextMeshProUGUI scoreCounter;
    public TextMeshProUGUI levelCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        score = 0;
        level = 1;
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

    public void stopPlayer(){
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    void SetCountText()
    {
        scoreCounter.text = score.ToString();
    }

    void setLevelText()
    {
        levelCounter.text = "LEVEL: "+level.ToString();
    }

    public int GetScore()
    {
        return score;
    }
}
