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
	public int extraJumps;
	private int currentJumps = 0;

	[Header("Ground variables")]
	public LayerMask groundMask;
	public Transform groundCheck;
	[Range(0.0f, 0.5f)]
	public float checkRadius;
	public bool grounded;
	[Range(0.0f, 0.2f)]
	public float groundLeeway;

	[Header("Which direction are we facing?")]
	public bool facingRight;

	[Header("Crouching")]
	public Transform headCheck;
	public bool isCrouching;
	public Collider2D defaultCollider;
	public Collider2D crouchedCollider;

	[Header("Particle systems")]
	public ParticleSystem partsFeathers;

	[Header("QA")]
	public string QAURL;

	private Rigidbody2D rb;
	private SpriteRenderer sr;
	private PlayerShoot refPlayerShoot;
	private PlayerAudio refPlayerAudio;
	private PlayerCollision refPlayerCollision;

	void Start ()
	{
		// rigidbody
		rb = GetComponent<Rigidbody2D>();

		// sprite renderer
		sr = GetComponent<SpriteRenderer>();
		facingRight = true;

		// player shoot script
		refPlayerShoot = GetComponent<PlayerShoot>();

		// player collision script
		refPlayerCollision = GetComponent<PlayerCollision>();

		// audio manager
		refPlayerAudio = GetComponent<PlayerAudio>();

		// 2D colliders
		defaultCollider.enabled = true;
		crouchedCollider.enabled = false;
	}

	void Update()
	{
		// only allow movement if we're alive
		if (refPlayerCollision.isDead == false)
		{
			CheckGround();

			DoJumping();
			DoCrouching();
			DoTerminalVelocities();

			CheckFlipSprite();
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.OpenURL(QAURL);
			Application.Quit();
		}
	}

	void FixedUpdate ()
	{
		// only allow movement if we're alive
		if (refPlayerCollision.isDead == false)
		{
			ScreenWrapping();
			DoMovement();
		}
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
			rb.velocity = new Vector2(movVec.x, rb.velocity.y);
		}
	}

	private void DoJumping()
	{
		if (Input.GetButtonDown("Jump") && currentJumps < extraJumps)
		{
			Jump();

			if (grounded == false)
			{
				currentJumps++;

				// play the flap sound
				refPlayerAudio.PlayFlap(currentJumps);

				// play the feather particle effect
				partsFeathers.Play();
			}
			else
			{
				// play the jump sound
				refPlayerAudio.PlayJump();

				// set grounded to false to avoid potential super jumps from leeway
				grounded = false;
			}
		}
	}

	public void Jump()
	{
		// calculate jump vector
		Vector2 jumpVec = new Vector2(0.0f, jumpForce);

		// reset y velocity to 0
		rb.velocity = new Vector2(rb.velocity.x, 0.0f);
		// add jump force
		rb.AddForce(jumpVec);
	}

	private void DoCrouching()
	{
		if (Input.GetKey("s"))
		{
			isCrouching = true;
		}
		else if (!Physics2D.OverlapCircle(headCheck.position, checkRadius, groundMask))
		{
			isCrouching = false;
		}

		if (isCrouching == true)
		{
			defaultCollider.enabled = false;
			crouchedCollider.enabled = true;
		}
		else
		{
			defaultCollider.enabled = true;
			crouchedCollider.enabled = false;
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

			CancelInvoke("NotGrounded");
		}
		else
		{
			// we are not grounded
			Invoke("NotGrounded", groundLeeway);
		}
	}

	private void NotGrounded()
	{
		grounded = false;
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
		/*if (transform.position.x > 15f)
		{
			CheckWrapCollision(0.0f);
			{
				PushAway(true);
				return;
			}
		}

		if (transform.position.x < 0.0f)
		{
			if (CheckWrapCollision(15.0f) == true)
			{
				PushAway(false);
				return;
			}
		}*/

		// if we're too far off the right side
		if (transform.position.x > 15.75f)
		{
			// check for walls on the opposite side
			transform.position = new Vector2(-0.5f, transform.position.y);
		}

		// if we're too far off the left side
		if (transform.position.x < -0.75f && !CheckWrapCollision(0))
		{
			// check for walls on the opposite side
			transform.position = new Vector2(15.5f, transform.position.y);
		}
	}

	private bool CheckWrapCollision(float xPos)
	{
		/*if (isCrouching == true)
		{
			if (Physics2D.OverlapBox(new Vector2(xPos, transform.position.y) + (Vector2)crouchedCollider.bounds.center, crouchedCollider.bounds.extents * 0.0f, 0, groundMask))
			{
				return true;
			}
		}
		else
		{
			if (Physics2D.OverlapBox(new Vector2(xPos, transform.position.y) + (Vector2)defaultCollider.bounds.center, defaultCollider.bounds.extents * 0.75f, 0, groundMask))
			{
				return true;
			}
		}*/
		return false;
	}

	private void PushAway(bool fromRightSide)
	{
		// push Pit horizontally according to the multiplier
		if (fromRightSide == true)
		{
			transform.position = new Vector2(15.0f, transform.position.y);
		}
		else
		{
			transform.position = new Vector2(-0.0f, transform.position.y);
		}
	}
}