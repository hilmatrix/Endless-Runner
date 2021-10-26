using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour {

    [Header("Movement")]
    public float moveAccel;
    public float maxSpeed;

    [Header("Jump")]
    public float jumpAccel;

    private bool isJumping;
    private bool isDiving;
    public bool isOnGround = false;

    [Header("Ground Raycast")]
    public float groundRaycastDistance;
    public LayerMask groundLayerMask;

    private Animator anim;
    private Rigidbody2D rig;
    private Collider2D coll;
    private CharacterSoundController sound;

    [Header("Scoring")]
    public ScoreController score;
    public float scoringRatio;
    private float lastPositionX;

    [Header("GameOver")]
    public GameObject gameOverScreen;
    public float fallPositionY;

    [Header("Camera")]
    public CameraMoveController gameCamera;

    // Start is called before the first frame update
    void Start() {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sound = GetComponent<CharacterSoundController>();
        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.UpArrow)) {
            if (isOnGround) {
                isJumping = true;
                sound.PlayJump();
            }
        }
        else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.DownArrow)) {
            if (!isOnGround && !isDiving) {
                isDiving = true;
                sound.PlayDive(); 
            }
        }

        anim.SetBool("isOnGround", isOnGround);

        int distancePassed = Mathf.FloorToInt(transform.position.x - lastPositionX);
        int scoreIncrement = Mathf.FloorToInt(distancePassed / scoringRatio);

        if (scoreIncrement > 0) {
            score.IncreaseCurrentScore(scoreIncrement);
            lastPositionX += distancePassed;
        }

        if (transform.position.y < fallPositionY) {
            GameOver();
        }
    }

    private void FixedUpdate() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastDistance, groundLayerMask);

        if (hit) {
            if (!isOnGround && rig.velocity.y <= 0) {
                isOnGround = true;
            }
        }
        else {
            isOnGround = false;
        }

        Vector2 velocityVector = rig.velocity;

        if (isJumping) {
            velocityVector.y += jumpAccel;
            isJumping = false;
        } else if (isDiving) {
            velocityVector.y -= jumpAccel;
            isDiving = false;
        }

        velocityVector.x = Mathf.Clamp(velocityVector.x + moveAccel * Time.deltaTime, 0.0f, maxSpeed);
        rig.velocity = velocityVector;
    }

    private void OnDrawGizmos() {
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * groundRaycastDistance), Color.white);
    }

    private void GameOver() {
        // set high score
        score.FinishScoring();

        // stop camera movement
        gameCamera.enabled = false;

        // show gameover
        gameOverScreen.SetActive(true);

        // disable this too
        this.enabled = false;
    }
}
