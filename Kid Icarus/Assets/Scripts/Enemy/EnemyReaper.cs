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

	private Enemy refEnemy;
	private Rigidbody2D rb;
	private SpriteRenderer sr;

	void Start ()
	{
		grounded = false;

		refEnemy = GetComponent<Enemy>();
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();

		if (Physics2D.OverlapCircle((Vector2)transform.position, 0.5f, groundMask) == true)
		{
			Destroy(gameObject);
		}

		StartCoroutine("Falling");
	}

	private void CheckForWalls()
	{
		if ((facingRight == true && Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(inFront.x, 0.0f), 0.2f, groundMask) == true) ||
			(facingRight == false && Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(inFront.x * -1, 0.0f), 0.2f, groundMask) == true))
		{
			facingRight = !facingRight;
		}
	}

	private void CheckForGaps()
	{
		if ((facingRight == true && Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(inFront.x, 0.0f), 0.2f, groundMask) == true) ||
			(facingRight == false && Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(inFront.x * -1, 0.0f), 0.2f, groundMask) == true))
		{
			facingRight = !facingRight;
		}
	}

	private void FlipSprite()
	{
		sr.flipX = !facingRight;
	}

	private IEnumerator Falling()
	{
		while (grounded == false)
		{
			if (Physics2D.OverlapCircle((Vector2)transform.position + below, 0.2f, groundMask) == false)
			{
				rb.velocity = Vector2.down * 5;
			}
			else
			{
				rb.velocity = new Vector2(rb.velocity.x, 0.0f);
				grounded = true;
			}

			yield return new WaitForSeconds(0.0f);
		}

		StartCoroutine("Patrol");
	}

	private IEnumerator Patrol()
	{
		while (grounded == true)
		{
			if ((facingRight == true && Physics2D.OverlapCircle((Vector2)transform.position + inFront, 0.2f, groundMask) == false) ||
				(facingRight == false && Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(inFront.x * -1, inFront.y), 0.2f, groundMask) == false))
			{
				facingRight = !facingRight;
			}

			CheckForWalls();
			CheckForGaps();
			FlipSprite();

			if (facingRight == true)
			{
				rb.velocity = new Vector2(movSpeed, 0.0f);
			}
			else
			{
				rb.velocity = new Vector2(movSpeed * -1, 0.0f);
			}

			yield return new WaitForSeconds(0.0f);
		}
	}
}