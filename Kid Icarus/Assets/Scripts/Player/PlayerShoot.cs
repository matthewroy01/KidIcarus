using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
	private PlayerMovement refPlayerMovement;
	private PlayerAudio refPlayerAudio;
	private PlayerCollision refPlayerCollision;

	[Header("Shooting arrows")]
	public float arrowProjectileSpeed;
	public GameObject arrowObject;
	public bool lookingUp;

	[Header("Melee attack")]
	public float meleeDuration;

	void Start ()
	{
		refPlayerMovement = GetComponent<PlayerMovement>();
		refPlayerAudio = GetComponent<PlayerAudio>();
		refPlayerCollision = GetComponent<PlayerCollision>();
	}

	void Update ()
	{
		// only allow shooting if we're alive
		if (refPlayerCollision.isDead == false)
		{
			CheckLookingUp();
			Shoot();
			Melee();
		}
	}

	void CheckLookingUp()
	{
		if (Input.GetKey("w"))
		{
			lookingUp = true;
		}
		else
		{
			lookingUp = false;
		}
	}

	void Shoot()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 shootDir;
			float zRotation;
			GameObject tmp;

			// if you're facing upwards
			if (lookingUp == true)
			{
				shootDir = Vector2.up * arrowProjectileSpeed;
				zRotation = 0;
			}
			// facing right
			else if (refPlayerMovement.facingRight)
			{
				shootDir = Vector2.right * arrowProjectileSpeed;
				zRotation = -90;
			}
			// facing left
			else
			{
				shootDir = Vector2.right * arrowProjectileSpeed * -1;
				zRotation = 90;
			}

			// shooting is not allowed while crouching
			if (refPlayerMovement.isCrouching == false)
			{
				// instantiate the arrow
				tmp = Instantiate(arrowObject, transform.position, Quaternion.Euler(0, 0, zRotation));
				tmp.GetComponent<Rigidbody2D>().velocity = shootDir;

				// play the shoot sound
				refPlayerAudio.PlayShoot();
			}
		}
	}

	void Melee()
	{
		if (Input.GetMouseButtonDown(1))
		{
			
		}
	}
}
