using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumpAndLand : MonoBehaviour
{
    [Header("Spawning")]
    public float yOffset;
    public float xRange;
    public Color spawnTint;

    [Header("Despawning")]
    public float destroyIfThisHigh;

    [Header("Jump")]
    public float initialJumpMultiplier;
    public Vector2 jumpVector;

    [Header("Landing")]
    public float jumpDelay;
    public LayerMask groundMask;
    public Vector2 groundCheck;
    private bool grounded;
    public GameObject landingParts;

    private Enemy refEnemy;
    private GameObject refPlayer;
    private Rigidbody2D rb;
    private SpriteRenderer refSpriteRenderer;
    private Animator refAnimator;
    private Collider2D refCollider;

    void Start()
    {
        refEnemy = GetComponent<Enemy>();
        refPlayer = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        refSpriteRenderer = GetComponent<SpriteRenderer>();
        refAnimator = GetComponent<Animator>();
        refCollider = GetComponent<Collider2D>();

        Vector2 camPos = Camera.main.transform.position;
        transform.position = new Vector2(camPos.x + Random.Range(-1 * xRange, xRange), camPos.y + yOffset);

        // disable collision for the first jump
        refCollider.enabled = false;
        // tint the color to show that the enemy isn't active yet
        refSpriteRenderer.color = spawnTint;

        Jump(initialJumpMultiplier);
        StartCoroutine(JumpingLoop());
    }

    private void Update()
    {
        if (refEnemy.isDead)
        {
            StopCoroutine(JumpingLoop());

            rb.gravityScale = 0.0f;
            rb.velocity = Vector2.zero;
        }

        if (transform.position.y - refPlayer.transform.position.y > destroyIfThisHigh)
        {
            Destroy(gameObject);
        }
    }

    private void CheckGround()
    {
        RaycastHit2D tmp = Physics2D.Linecast(transform.position, (Vector2)transform.position + groundCheck, groundMask);

        if (!grounded && rb.velocity.y < 0.0f && tmp)
        {
            // reenable collision for the first jump
            refCollider.enabled = true;
            // correct color now that the collider is enabled
            refSpriteRenderer.color = Color.white;

            grounded = true;
            rb.gravityScale = 0.0f;
            rb.velocity = Vector2.zero;

            transform.position = tmp.point - groundCheck;

            // spawn landing particles
            Instantiate(landingParts, tmp.point, Quaternion.identity);

            refAnimator.SetTrigger("Grounded");
        }
    }

    private IEnumerator JumpingLoop()
    {
        while (!refEnemy.isDead)
        {
            CheckGround();

            if (grounded)
            {
                yield return new WaitForSeconds(jumpDelay);

                grounded = false;
                Jump(1.0f);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void Jump(float mult)
    {
        rb.gravityScale = 1.0f;
        Vector2 tmp = jumpVector;

        if (refPlayer.transform.position.x < transform.position.x)
        {
            refSpriteRenderer.flipX = true;
            tmp = new Vector2(jumpVector.x * -1, jumpVector.y);
        }
        else
        {
            refSpriteRenderer.flipX = false;
        }

        refAnimator.SetTrigger("Jumping");
        rb.AddForce(tmp * mult);
    }
}
