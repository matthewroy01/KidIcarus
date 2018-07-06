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
	public int turnRate;
	public bool waiting = false;
	public float waitTime;

	[Header("Panic mode")]
	public float sightDistance;
	public LayerMask playerMask;
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

		if (Physics2D.OverlapCircle((Vector2)transform.position, 0.5f, groundMask) == true)
		{
			Destroy(gameObject);
		}
	}

	void FixedUpdate()
	{
		if (refEnemy.isDead == false)
		{
			if (grounded == false)
			{
				Falling();
			}
			else
			{
				if (isPanicked == false)
				{
					Patrol();
					LineOfSight();
					if (CheckForWalls() == true || CheckForGaps() == true)
					{
						facingRight = !facingRight;
					}
				}
				else
				{
					Chase();
				}

				FlipSprite();
			}
		}
		else
		{
			rb.velocity = Vector2.zero;
		}

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
			rb.velocity = Vector2.down * 5;
		}
		else
		{
			// stop falling and set grounded to true
			rb.velocity = new Vector2(rb.velocity.x, 0.0f);
			grounded = true;
		}
	}

	private void Patrol()
	{
		Debug.Log("Patrolling");
		// randomly decide to wait
		if (waiting == false && Random.Range(0, turnRate) == turnRate - 1)
		{
			waiting = true;
			rb.velocity = new Vector2(0.0f, 0.0f);
			facingRight = !facingRight;
			Invoke("StopWaiting", waitTime);
		}

		if (waiting == false)
		{
			// move
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
		Debug.Log("Chasing");
		// change direction based on where the player is
		if (transform.position.x <= refPlayer.transform.position.x)
		{
			facingRight = true;
		}
		else
		{
			facingRight = false;
		}

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

	private void StopWaiting()
	{
		waiting = false;
		facingRight = !facingRight;
	}

	private void LineOfSight()
	{
		if ((facingRight == true && Physics2D.Linecast(transform.position, new Vector2(transform.position.x + sightDistance, transform.position.y), playerMask)) ||
			(facingRight == false && Physics2D.Linecast(transform.position, new Vector2(transform.position.x - sightDistance, transform.position.y), playerMask)))
		{
			if (isPanicked == false)
			{
				isPanicked = true;
				waiting = false;

				StartCoroutine("ReaperCry");

				refMusicManager.SetMusicStatus(MusicStatus.reaperTheme);

				if (spawnedReapettes == false)
				{
					Instantiate(reapettePrefab, Vector2.zero, Quaternion.identity);
				}

				spawnedReapettes = true;

				Invoke("StopPanicking", panicTime);
			}
		}
	}

	void OnDestroy()
	{
		refMusicManager.SetMusicStatus(MusicStatus.mainTheme);
	}

	private IEnumerator ReaperCry()
	{
		while (isPanicked == true)
		{
			refAudioManager.PlaySound(cry.clip, cry.volume, true);

			yield return new WaitForSeconds(cryInterval);
		}
	}

	private void StopPanicking()
	{
		isPanicked = false;
		StopCoroutine("ReaperCry");
		refAnimator.SetTrigger("stopPanicking");
		refAnimator.SetBool("inPlace", false);
		refAnimator.SetBool("isPanicked", false);
	}
}