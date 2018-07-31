using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
	[Header("Shooting arrows")]
	public float arrowProjectileSpeed;
	public GameObject arrowPrefabNormal;
   public GameObject arrowPrefabLongbow;
   public GameObject arrowPrefabCharged;
   public bool lookingUp;

   [Header("Power ups")]
   public bool hasLongbow;
   public bool hasChargeReticle;
   public bool isCharged;
   public float rechargeTime;
   public GameObject rechargePrefab;
   private bool rechargeInvoked = false;

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
   private CameraFollow refCameraFollow;

	[Header("Melee charge")]
	public int meleeUsesTotal;
	public int meleeUsesCurrent;
	public int meleeChargeTotal;
	public int meleeChargeCurrent;

   [Header("Centurions")]
   public bool hasCenturion = false;
   public int centurionsStored = 0;
   public GameObject centurionPrefab;

   private PlayerMovement refPlayerMovement;
	private PlayerAudio refPlayerAudio;
	private PlayerCollision refPlayerCollision;

	void Start ()
	{
		refPlayerMovement = GetComponent<PlayerMovement>();
		refPlayerAudio = GetComponent<PlayerAudio>();
		refPlayerCollision = GetComponent<PlayerCollision>();
      refCameraFollow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();

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
         SpawnCenturions();

         if (rechargeInvoked == false && !isCharged)
         {
            rechargeInvoked = true;
            Invoke("RechargeBow", rechargeTime);
         }

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
			GameObject tmp = null;

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
            if (hasChargeReticle && isCharged)
            {
               tmp = Instantiate(arrowPrefabCharged, transform.position, Quaternion.Euler(0, 0, zRotation));
               tmp.GetComponent<Rigidbody2D>().velocity = shootDir * 1.2f;

               isCharged = false;

               // stop charging a charge shot
               CancelInvoke("RechargeBow");
               rechargeInvoked = false;

               // play the shoot sound
               refPlayerAudio.PlayShoot(1.5f, 0.5f);
            }
            else
            {
               if (hasLongbow)
               {
                  tmp = Instantiate(arrowPrefabLongbow, transform.position, Quaternion.Euler(0, 0, zRotation));
               }
               else
               {
                  tmp = Instantiate(arrowPrefabNormal, transform.position, Quaternion.Euler(0, 0, zRotation));
               }
               // apply velocity to the arrow
               tmp.GetComponent<Rigidbody2D>().velocity = shootDir;

               // play the shoot sound
               refPlayerAudio.PlayShoot(1.0f, 1.0f);

               // stop charging a charge shot
               CancelInvoke("RechargeBow");
               rechargeInvoked = false;
            }
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
				//tmp.velocity = rb.velocity * 0.5f;
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

      // sound
		refPlayerAudio.PlayHammerHit();

      // particles
      partsHammer.Play();

      // camera shake
      refCameraFollow.shakeIntensity = Mathf.Lerp(refCameraFollow.shakeIntensity, 0.5f, 1.0f);

      Invoke("StopSwinging", meleeDuration);
	}

	void SwingLeft()
	{
		hammer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90f));
		hammer.enabled = true;

      //sound
		refPlayerAudio.PlayHammerHit();

      // particles
		partsHammer.Play();

      // camera shake
      refCameraFollow.shakeIntensity = Mathf.Lerp(refCameraFollow.shakeIntensity, 0.5f, 1.0f);

      Invoke("StopSwinging", meleeDuration);
	}

	void StopSwinging()
	{
		hammer.enabled = false;
		hammer.gameObject.SetActive(false);

		isSwinging = false;

      // stop camera shake
      refCameraFollow.shakeIntensity = Mathf.Lerp(refCameraFollow.shakeIntensity, 0.0f, 1.0f);

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

   private void SpawnCenturions()
   {
      if (Input.GetKeyDown(KeyCode.E))
      {
         // if we have centurions stored and we don't already have a centurion equipped, spawn one
         if (centurionsStored > 0 && hasCenturion == false)
         {
            Instantiate(centurionPrefab, transform.position, transform.rotation);
            hasCenturion = true;
            centurionsStored--;
         }
      }
   }

   private void RechargeBow()
   {
      if (hasChargeReticle)
      {
         isCharged = true;
         rechargeInvoked = false;

         GameObject tmp = Instantiate(rechargePrefab, transform.position, transform.rotation);
         tmp.transform.parent = transform;

         refPlayerAudio.PlayArrowRecharge();
      }
   }
}