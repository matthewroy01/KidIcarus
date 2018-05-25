using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalkAndFall : MonoBehaviour
{
	[Header("Checking for ground")]
	public float centerY;
	public float boxWidth;
	public float boxHeight;
	public LayerMask groundLayer;
	public bool grounded;

	[Header("Checking for walls")]
	public float offsetX;
	public float radius;

	[Header("Movement")]
	public float walkSpeed;
	public bool facingRight;
	public float fallSpeed;

	// rigidbody
	private Rigidbody2D rb;

	// the player
	private Transform refPlayer;

	// enemy status
	private Enemy refEnemy;

	// sprite renderer
	private SpriteRenderer sr;

	void Start()
	{
		// find the rigidbody
		rb = GetComponent<Rigidbody2D>();

		// find the player
		refPlayer = GameObject.Find("Player").transform;

		// find the enemy status
		refEnemy = GetComponent<Enemy>();

		// decide whether or not we should face left or right
		facingRight = CheckFacingRight();

		// sprite renderer
		sr = GetComponent<SpriteRenderer>();
	}

	void Update ()
	{
		CheckGround();
		CheckWalls();
		WalkOrFall();
		CheckFlipSprite();
	}

	private void CheckGround()
	{
		if (Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y + centerY), new Vector2(boxWidth, boxHeight), 0.0f, groundLayer))
		{
			if (grounded == false)
			{
				facingRight = CheckFacingRight();
			}
			grounded = true;
		}
		else
		{
			grounded = false;
		}
	}

	private void WalkOrFall()
	{
		if (refEnemy.isDead == false)
		{
			// walk
			if (grounded == true)
			{
				if (facingRight)
				{
					rb.velocity = new Vector2(walkSpeed, 0);	
				}
				else
				{
					rb.velocity = new Vector2(walkSpeed * -1, 0);
				}
			}
			// fall
			else
			{
				rb.velocity = new Vector2(0, fallSpeed * -1);
			}
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
	}

	private bool CheckFacingRight()
	{
		if (transform.position.x >= refPlayer.position.x)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	void CheckWalls()
	{
		// upon hitting a wall, change direction
		if ((Physics2D.OverlapCircle(new Vector2(transform.position.x + offsetX, transform.position.y), radius, groundLayer) && facingRight == true)||
			Physics2D.OverlapCircle(new Vector2(transform.position.x + offsetX * -1.0f, transform.position.y), radius, groundLayer) && facingRight == false)
		{
			facingRight = !facingRight;
		}
	}

	private void CheckFlipSprite()
	{
		// flip the sprite according to which direction we're facing
		if (facingRight == true)
		{
			sr.flipX = false;
		}
		else
		{
			sr.flipX = true;
		}
	}
}