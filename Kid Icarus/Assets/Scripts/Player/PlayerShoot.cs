using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
	[Header("Shooting arrows")]
	public float arrowProjectileSpeed;
	public GameObject arrowObject;
	public bool lookingUp;

	[Header("Shooting eggplants")]
	public float eggplantProjecilteSpeed;
	public GameObject eggplantObject;
	public int eggplantNum;
	public float eggplantCooldown;
	public bool canFireEggplant = true;

	[Header("Melee attack")]
	public float meleeStartup;
	public float meleeDuration;
	public Collider2D hammer;
	public float meleeCooldown;
	public bool isSwinging = false;
	private bool canHammer = true;
	public ParticleSystem partsHammer;

	[Header("Melee charge")]
	public int meleeUsesTotal;
	public int meleeUsesCurrent;
	public int meleeChargeTotal;
	public int meleeChargeCurrent;

	private PlayerMovement refPlayerMovement;
	private PlayerAudio refPlayerAudio;
	private PlayerCollision refPlayerCollision;
	private Rigidbody2D rb;

	void Start ()
	{
		refPlayerMovement = GetComponent<PlayerMovement>();
		refPlayerAudio = GetComponent<PlayerAudio>();
		refPlayerCollision = GetComponent<PlayerCollision>();
		rb = GetComponent<Rigidbody2D>();

		// set the hammer to inactive by default
		hammer.enabled = false;
		hammer.gameObject.SetActive(false);

		// set hammer charge values
		meleeUsesCurrent = meleeUsesTotal;
	}

	void Update ()
	{
		// only allow shooting if we're alive
		if (refPlayerCollision.isDead == false)
		{
			CheckLookingUp();

			if (refPlayerCollision.cursed == false)
			{
				Shoot();
				UpdateHammerPosition();
				Melee();
			}
			else
			{
				ShootEggplant();
			}
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

	void ShootEggplant()
	{
		int tmpEggplantNum = eggplantNum - Random.Range(0, 1);

		if (Input.GetMouseButtonDown(0) && canFireEggplant)
		{
			for (int i = 0; i < tmpEggplantNum; ++i)
			{
				Rigidbody2D tmp = Instantiate(eggplantObject, transform.position, transform.rotation).GetComponent<Rigidbody2D>();
				tmp.velocity = rb.velocity * 0.5f;
				tmp.AddForce(new Vector2(Random.Range(-1.0f, 1.0f), 1.0f) * eggplantProjecilteSpeed);
				canFireEggplant = false;

				Invoke("RechargeEggplant", eggplantCooldown);
			}
		}
	}

	void RechargeEggplant()
	{
		canFireEggplant = true;
	}

	private void UpdateHammerPosition()
	{
		if (refPlayerMovement.facingRight)
		{
			hammer.transform.position = (Vector2)transform.position + new Vector2(0.375f, 0.0f);
		}
		else
		{
			hammer.transform.position = (Vector2)transform.position + new Vector2(-0.375f, 0.0f);
		}
	}

	void Melee()
	{
		if (Input.GetMouseButtonDown(1) && canHammer && meleeUsesCurrent > 0)
		{
			meleeUsesCurrent--;

			hammer.enabled = false;
			hammer.gameObject.SetActive(true);

			canHammer = false;
			isSwinging = true;

			refPlayerAudio.PlayHammerSwing();

			if (refPlayerMovement.facingRight)
			{
				hammer.transform.rotation = Quaternion.Euler(Vector3.zero);
				Invoke("SwingRight", meleeStartup);
			}
			else
			{
				hammer.transform.rotation = Quaternion.Euler(Vector3.zero);
				Invoke("SwingLeft", meleeStartup);
			}
		}
	}

	void SwingRight()
	{
		hammer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90f));
		hammer.enabled = true;

		refPlayerAudio.PlayHammerHit();

		partsHammer.Play();

		Invoke("StopSwinging", meleeDuration);
	}

	void SwingLeft()
	{
		hammer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90f));
		hammer.enabled = true;

		refPlayerAudio.PlayHammerHit();

		partsHammer.Play();

		Invoke("StopSwinging", meleeDuration);
	}

	void StopSwinging()
	{
		hammer.enabled = false;
		hammer.gameObject.SetActive(false);

		isSwinging = false;

		Invoke("RechargeHammer", meleeCooldown);
	}

	void RechargeHammer()
	{
		canHammer = true;
	}

	public void IncreaseMeleeCharge(int increaseBy)
	{
		if (meleeUsesCurrent < meleeUsesTotal)
		{
			// increase charge
			if (meleeChargeCurrent < meleeChargeTotal)
			{
				meleeChargeCurrent += increaseBy;
			}

			// add a use
			if (meleeChargeCurrent >= meleeChargeTotal)
			{
				meleeChargeCurrent = 0;
				if (meleeUsesCurrent < meleeUsesTotal)
				{
					meleeUsesCurrent++;
				}
			}
		}
	}
}