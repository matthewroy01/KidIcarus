using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReaper : MonoBehaviour
{
	[Header("Transforms for ground detection")]
	public Vector2 below;
	public Vector2 inFront;
	public LayerMask groundMask;
	public bool grounded;

	[Header("Movement")]
	public bool facingRight;
	public float movSpeed;

	[Header("Patrolling")]
	public bool doRandomWait;
	public int turnRate;
	public bool waiting = false;
	public float waitTime;

	[Header("Panic mode")]
	public float sightDistance;
	public GameObject lineOfSightObj;
	public float projectileInterval;
	public float projectileSpeed;
	public Transform projectilePos;
	public bool isPanicked;
	public float panicTime;
	public float panicSpeed;

	[Header("Reapettes")]
	public GameObject reapettePrefab;
	public bool spawnedReapettes = false;

	[Header("Sound")]
	public Sound cry;
	public float cryInterval;

	private Enemy refEnemy;
	private Rigidbody2D rb;
	private SpriteRenderer sr;
	private Animator refAnimator;
	private GameObject refPlayer;
	private UtilityMusicManager refMusicManager;
	private UtilityAudioManager refAudioManager;

	void Start ()
	{
		grounded = false;

		refEnemy = GetComponent<Enemy>();
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		refAnimator = GetComponent<Animator>();
		refPlayer = GameObject.Find("Pit");
		refMusicManager = GameObject.FindObjectOfType<UtilityMusicManager>();
		refAudioManager = GameObject.FindObjectOfType<UtilityAudioManager>();

		// if we spawn inside a wall, destroy
		if (Physics2D.OverlapCircle((Vector2)transform.position, 0.5f, groundMask) == true)
		{
			Destroy(gameObject);
		}
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
				// if we're not panicked, patrol and look for Pit
				if (isPanicked == false)
				{
					Patrol();

					// switch directions if we reach a wall or gap
					if (CheckForWalls() == true || CheckForGaps() == true)
					{
						facingRight = !facingRight;
					}
				}
				// otherwise chase the player
				else
				{
					Chase();
				}

				FlipSprite();
			}
		}
		else
		{
			// stop moving when we're dead
			rb.velocity = Vector2.zero;
		}

		// update animations
		refAnimator.SetBool("isPanicked", isPanicked);
		refAnimator.SetBool("isDead", refEnemy.isDead);
	}

	private bool CheckForWalls()
	{
		// look for walls in front of us and switch directions if there is one
		if ((facingRight == true && Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(inFront.x, 0.0f), 0.2f, groundMask) == true) ||
			(facingRight == false && Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(inFront.x * -1, 0.0f), 0.2f, groundMask) == true))
		{
			return true;
		}
		return false;
	}

	private bool CheckForGaps()
	{
		// look for gaps in front of us and switch directions if there is one
		if ((facingRight == true && Physics2D.OverlapCircle((Vector2)transform.position + inFront, 0.2f, groundMask) == false) ||
			(facingRight == false && Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(inFront.x * -1, inFront.y), 0.2f, groundMask) == false))
		{
			return true;
		}
		return false;
	}

	private void FlipSprite()
	{
		sr.flipX = !facingRight;
	}

	private void Falling()
	{
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

			// start looking for Pit
			StartCoroutine("FireProjectiles");
		}
	}

	private void Patrol()
	{
		// Debug.Log("Patrolling");

		if (doRandomWait == true)
		{
			// randomly decide to wait
			if (waiting == false && Random.Range(0, turnRate) == turnRate - 1)
			{
				waiting = true;
				rb.velocity = new Vector2(0.0f, 0.0f);
				facingRight = !facingRight;
				Invoke("StopWaiting", waitTime);
			}
		}

		// otherwise move
		if (waiting == false)
		{
			if (facingRight == true)
			{
				rb.velocity = new Vector2(movSpeed, 0.0f);
			}
			else
			{
				rb.velocity = new Vector2(movSpeed * -1, 0.0f);
			}
		}
	}

	private void Chase()
	{
		// Debug.Log("Chasing");
		// change direction based on where the player is
		facingRight = CheckForOptimalDirection();

		// only move if there's nothing in the way
		if (CheckForGaps() == false && CheckForWalls() == false)
		{
			refAnimator.SetBool("inPlace", false);

			// move
			if (facingRight == true)
			{
				rb.velocity = new Vector2(panicSpeed, 0.0f);
			}
			else
			{
				rb.velocity = new Vector2(panicSpeed * -1, 0.0f);
			}
		}
		else
		{
			refAnimator.SetBool("inPlace", true);
			rb.velocity = Vector2.zero;
		}
	}

	private bool CheckForOptimalDirection()
	{
		float normalDistance, wrapDistance;

		// depending on if Pit is to the right or left, calculate the wrap distance
		if (refPlayer.transform.position.x < transform.position.x)
		{
			// calculate direction with screen wrapping to the player
			wrapDistance = refPlayer.transform.position.x + Mathf.Abs(15.5f - transform.position.x);
		}
		else
		{
			// calculate direction with screen wrapping to the player
			wrapDistance = transform.position.x + Mathf.Abs(15.5f - refPlayer.transform.position.x);
		}

		// calculate direction normally to the player
		normalDistance = Mathf.Abs(transform.position.x - refPlayer.transform.position.x);

		// if we're closer normally, chase normally
		if (normalDistance <= wrapDistance)
		{
			if (transform.position.x < refPlayer.transform.position.x)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		// otherwise, go the opposite direction
		else
		{
			if (transform.position.x < refPlayer.transform.position.x)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}

	private void StopWaiting()
	{
		waiting = false;
		facingRight = !facingRight;
	}

	// but not that Panic...
	public void Panic()
	{
		// only panic if we're not already panicked
		if (isPanicked == false)
		{
			isPanicked = true;
			waiting = false;

			// start the coroutine to play the sound over and over
			StartCoroutine("ReaperCry");

			// change the music
			refMusicManager.SetMusicStatus(MusicStatus.reaperTheme);

			// if haven't yet, spawn reapettes
			if (spawnedReapettes == false)
			{
				Instantiate(reapettePrefab, Vector2.zero, Quaternion.identity);
				spawnedReapettes = true;
			}

			// stop panicking after a little bit
			Invoke("StopPanicking", panicTime);
		}
	}

	private IEnumerator FireProjectiles()
	{
		while (isPanicked == false)
		{
			GameObject tmp = Instantiate(lineOfSightObj, projectilePos.position, projectilePos.rotation);
			tmp.transform.parent = transform;

			if (facingRight == true)
			{
				tmp.GetComponent<Rigidbody2D>().velocity = Vector2.right * projectileSpeed;
			}
			else
			{
				tmp.GetComponent<Rigidbody2D>().velocity = Vector2.right * -1 * projectileSpeed;
			}

			yield return new WaitForSeconds(projectileInterval);
		}
	}

	void OnDestroy()
	{
		// set the music back to normal when we're destroyed
		refMusicManager.SetMusicStatus(MusicStatus.mainTheme);
	}

	private IEnumerator ReaperCry()
	{
		// as long as we're panicking, play the cry sound
		while (isPanicked == true)
		{
			refAudioManager.PlaySound(cry.clip, cry.volume, true);

			yield return new WaitForSeconds(cryInterval);
		}
	}

	private void StopPanicking()
	{
		// reset stuff
		isPanicked = false;
		StopCoroutine("ReaperCry");
		refAnimator.SetTrigger("stopPanicking");
		refAnimator.SetBool("inPlace", false);
		refAnimator.SetBool("isPanicked", false);

		// start looking for Pit again
		StartCoroutine("FireProjectiles");
	}
}