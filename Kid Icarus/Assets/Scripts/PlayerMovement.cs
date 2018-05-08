using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("Movement variables")]
	public float movSpeed;
	public float maxSpeed;
	public float jumpForce;
	public float gravForce;

	[Header("Ground variables")]
	public LayerMask groundMask;
	public bool grounded;

	[Header("Which direction are we facing?")]
	public bool facingRight;

	private Rigidbody2D rb;
	private SpriteRenderer sr;

	void Start ()
	{
		// rigidbody
		rb = GetComponent<Rigidbody2D>();
		rb.gravityScale = 0;

		// sprite renderer
		sr = GetComponent<SpriteRenderer>();
		facingRight = true;
	}

	void FixedUpdate ()
	{
		CheckGround();

		DoMovement();
		DoGravity();
		DoJumping();

		DoTerminalVelocities();

		CheckFlipSprite();
	}

	private void DoMovement()
	{
		float tmpAxis = Input.GetAxis("Horizontal");

		// calculate movement vector
		Vector2 movVec = new Vector2(tmpAxis * movSpeed, 0.0f);

		// check the axis to see which direction we should be facing
		if (tmpAxis > 0)
		{
			facingRight = true;
		}
		if (tmpAxis < 0)
		{
			facingRight = false;
		}

		// add movement force
		rb.AddForce(movVec);
	}

	private void DoJumping()
	{
		if (Input.GetButtonDown("Jump") && grounded)
		{
			// calculate jump vector
			Vector2 jumpVec = new Vector2(0.0f, jumpForce);

			// reset y velocity to 0
			rb.velocity = new Vector2(rb.velocity.x, 0.0f);
			// add jump force
			rb.AddForce(jumpVec);
		}
	}

	private void DoGravity()
	{
		// calculate gravity vector
		Vector2 gravVec = new Vector2(0.0f, gravForce * -1);

		// add gravity force
		rb.AddForce(gravVec);
	}

	private void CheckGround()
	{
		Vector2 endPos = new Vector2(transform.position.x, transform.position.y - 1.1f);

		Debug.DrawLine(transform.position, endPos, Color.green);

		// check if there is ground beneath the player
		if (Physics2D.Linecast(transform.position, endPos, groundMask))
		{
			grounded = true;
		}
		else
		{
			grounded = false;
		}
	}

	private void DoTerminalVelocities()
	{
		// x terminal velocities
		if (rb.velocity.x > maxSpeed)
		{
			rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
		}
		if (rb.velocity.x < maxSpeed * -1)
		{
			rb.velocity = new Vector2(maxSpeed * -1, rb.velocity.y);
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
