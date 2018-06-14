using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { Idle = 0, Walk = 1 , Jump = 2, Fall = 3, Crouch = 4, LookUp = 5};

public class PlayerAnimation : MonoBehaviour
{

	private Animator refAnimator;
	private PlayerMovement refPlayerMovement;
	private PlayerShoot refPlayerShoot;
	private Rigidbody2D rb;
	private PlayerCollision refPlayerCollision;

	[Header("Current animation state")]
	public PlayerState animationState;
	public bool isShooting;

	[Header("Velocity thresholds for switching animations")]
	public float velocityThresholdHorizontal;
	public float velocityThresholdVertical;

	void Start ()
	{
		refAnimator = GetComponent<Animator>();
		refPlayerMovement = GetComponent<PlayerMovement>();
		refPlayerShoot = GetComponent<PlayerShoot>();
		rb = GetComponent<Rigidbody2D>();
		refPlayerCollision = GetComponent<PlayerCollision>();
	}

	void Update ()
	{
		refAnimator.SetInteger("State", (int)animationState);
		//refAnimator.SetBool("IsShooting", isShooting);

		CheckPlayer();
	}

	private void CheckPlayer()
	{
		if (refPlayerCollision.isDead == true)
		{
			refAnimator.SetBool("isDead", true);
		}
		else
		{
			if (refPlayerMovement.grounded == false && rb.velocity.y > velocityThresholdVertical)
			{
				animationState = PlayerState.Jump;
			}

			if (refPlayerMovement.grounded == false && rb.velocity.y < -1 * velocityThresholdVertical)
			{
				animationState = PlayerState.Fall;
			}

			if (refPlayerMovement.grounded == true && (rb.velocity.x > velocityThresholdHorizontal || rb.velocity.x < -1 * velocityThresholdHorizontal))
			{
				if (animationState != PlayerState.Walk)
					animationState = PlayerState.Walk;
			}

			if (refPlayerMovement.grounded == true && (rb.velocity.x < velocityThresholdHorizontal && rb.velocity.x > -1 * velocityThresholdHorizontal))
			{
				animationState = PlayerState.Idle;
			}

			if (refPlayerShoot.lookingUp == true)
			{
				animationState = PlayerState.LookUp;
			}

			if (refPlayerMovement.isCrouching == true)
			{
				animationState = PlayerState.Crouch;
			}
		}
	}
}
