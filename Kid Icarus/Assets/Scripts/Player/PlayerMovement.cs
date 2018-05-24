using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("Movement variables")]
	public float movSpeed;
	public float maxSpeed;

	[Header("Jump variables")]
	public float jumpForce;
	public int numberOfJumps;
	public int currentJumps = 0;

	[Header("Ground variables")]
	public LayerMask groundMask;
	public Transform groundCheck;
	[Range(0.0f, 0.5f)]
	public float checkRadius;
	public bool grounded;

	[Header("Which direction are we facing?")]
	public bool facingRight;

	[Header("Crouching")]
	public bool isCrouching;

	[Header("QA")]
	public string QAURL;

	private Rigidbody2D rb;
	private SpriteRenderer sr;
	private PlayerShoot refPlayerShoot;
	private PlayerAudio refPlayerAudio;

	void Start ()
	{
		// rigidbody
		rb = GetComponent<Rigidbody2D>();

		// sprite renderer
		sr = GetComponent<SpriteRenderer>();
		facingRight = true;

		// player shoot script
		refPlayerShoot = GetComponent<PlayerShoot>();

		// audio manager
		refPlayerAudio = GetComponent<PlayerAudio>();
	}

	void Update()
	{
		CheckGround();

		DoJumping();
		DoCrouching();
		DoTerminalVelocities();

		CheckFlipSprite();
		ScreenWrapping();

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.OpenURL(QAURL);
			Application.Quit();
		}
	}

	void FixedUpdate ()
	{
		DoMovement();
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

		// movement is not allowed if you're looking up
		if (refPlayerShoot.lookingUp == false)
		{
			// add movement force
			rb.AddForce(movVec);
		}
	}

	private void DoJumping()
	{
		if (Input.GetButtonDown("Jump") && currentJumps < numberOfJumps)
		{
			// calculate jump vector
			Vector2 jumpVec = new Vector2(0.0f, jumpForce);

			// reset y velocity to 0
			rb.velocity = new Vector2(rb.velocity.x, 0.0f);
			// add jump force
			rb.AddForce(jumpVec);

			currentJumps++;

			if (grounded == false)
			{
				// play the flap sound
				refPlayerAudio.PlayFlap(currentJumps);
			}
			else
			{
				// play the jump sound
				refPlayerAudio.PlayJump();
			}
		}
	}

	private void DoCrouching()
	{
		if (Input.GetKey("s"))
		{
			isCrouching = true;
		}
		else
		{
			isCrouching = false;
		}
	}

	private void CheckGround()
	{
		// check if there is ground beneath the player
		if (Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundMask))
		{
			// if we've used some jumps and we were in the air
			if (grounded == false && currentJumps > 1)
			{
				// play the recharge sound
				refPlayerAudio.PlayRecharge();
			}

			// reset the number of jumps
			currentJumps = 0;

			// we are grounded
			grounded = true;
		}
		else
		{
			// we are not grounded
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

	private void ScreenWrapping()
	{
		if (transform.position.x > 16f)
		{
			transform.position = new Vector2(-0.5f, transform.position.y);
		}

		if (transform.position.x < -1.0f)
		{
			transform.position = new Vector2(15.5f, transform.position.y);
		}
	}
}
