using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEggplant : MonoBehaviour
{
	[Header("Transforms for ground detection")]
	public Vector2 below;
	public Vector2 inFront;
	public LayerMask groundMask;
	public bool grounded;
    public float maxFallDistance;

    [Header("Movement")]
	public bool facingRight;
	public bool goingRight;
	public float movSpeed;

	[Header("Projectiles")]
	public GameObject eggplantPrefab;
	public float fireRate;
	public Vector2 force;
	public float detectionDistance;

	[Header("Sound")]
	public Sound fire;

	private Enemy refEnemy;
	private Rigidbody2D rb;
	private SpriteRenderer sr;
	private Animator refAnimator;
	private GameObject refPlayer;
	private UtilityAudioManager refAudioManager;

	void Start ()
	{
		grounded = false;

		refEnemy = GetComponent<Enemy>();
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		refAnimator = GetComponent<Animator>();
		refPlayer = GameObject.Find("Pit");
		refAudioManager = GameObject.FindObjectOfType<UtilityAudioManager>();
	}

	void FixedUpdate()
	{
		if (refEnemy.isDead == false)
		{
			// if we're not grounded, fall
			if (grounded == false)
			{
				Falling();
			}
			else
			{
				Patrol();

				// switch directions if we reach a wall or gap
				if (CheckForWalls() == true || CheckForGaps() == true)
				{
					goingRight = !goingRight;
				}

				FlipSprite();
			}
		}
		else
		{
			// stop moving when we're dead
			rb.velocity = Vector2.zero;
		}

		refAnimator.SetBool("isDead", refEnemy.isDead);
	}

	private bool CheckForWalls()
	{
		// look for walls in front of us and switch directions if there is one
		if ((goingRight == true && Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(inFront.x, 0.0f), 0.2f, groundMask) == true) ||
			(goingRight == false && Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(inFront.x * -1, 0.0f), 0.2f, groundMask) == true))
		{
			return true;
		}
		return false;
	}

	private bool CheckForGaps()
	{
		// look for gaps in front of us and switch directions if there is one
		if ((goingRight == true && Physics2D.OverlapCircle((Vector2)transform.position + inFront, 0.2f, groundMask) == false) ||
			(goingRight == false && Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(inFront.x * -1, inFront.y), 0.2f, groundMask) == false))
		{
			return true;
		}
		return false;
	}

	private void FlipSprite()
	{
		if (refPlayer.transform.position.x > transform.position.x)
		{
			facingRight = true;
			sr.flipX = false;
		}
		else
		{
			facingRight = false;
			sr.flipX = true;
		}
	}

	private void Falling()
	{
        // don't fall too far
        if (!Physics2D.Linecast(transform.position, new Vector2(transform.position.x, transform.position.y - maxFallDistance), groundMask))
        {
            Destroy(gameObject);
        }

        // if there's nothing below us, fall
        if (Physics2D.OverlapCircle((Vector2)transform.position + below, 0.2f, groundMask) == false)
		{
			rb.velocity = Vector2.down * 7.5f;
		}
		else
		{
			// stop falling and set grounded to true
			rb.velocity = new Vector2(rb.velocity.x, 0.0f);
			grounded = true;

			// start firing projectiles
			StartCoroutine("FireProjectiles");
		}
	}

	private void Patrol()
	{
		// otherwise move
		if (goingRight == true)
		{
			rb.velocity = new Vector2(movSpeed, 0.0f);
		}
		else
		{
			rb.velocity = new Vector2(movSpeed * -1, 0.0f);
		}
	}

	private IEnumerator FireProjectiles()
	{
		// don't shoot if we're dead
		// only shoot if we're in range or the player is above us
		while (refEnemy.isDead == false)
		{
			if (transform.position.y - refPlayer.transform.position.y <= detectionDistance || transform.position.y < refPlayer.transform.position.y)
			{
				// fire the projectile and move it
				GameObject tmp = Instantiate(eggplantPrefab, transform.position, transform.rotation);

				// play a sound
				refAudioManager.PlaySound(fire.clip, fire.volume, true);

				// flip the direction of the projectile depending on the direction you're facing
				if (facingRight == true)
				{
					tmp.GetComponent<Rigidbody2D>().AddForce(force);
				}
				else
				{
					tmp.GetComponent<Rigidbody2D>().AddForce(new Vector2(force.x * -1, force.y));
				}
			}

			yield return new WaitForSeconds(fireRate);
		}
	}
}