using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
	[Header("Arrow prefabs")]
    public GameObject arrowPrefabNormal;
    public GameObject arrowPrefabCharged;
    
    [Header("Arrow stats")]
    public float arrowSpeedNormal;
    public float arrowSpeedCharged;
    public bool lookingUp;
    [Header("")]
    public int arrowRangeLevel = 0;
    public float arrowRangeBase;
    public float arrowRangeIncrement;
    [Header("")]
    public int arrowChargeLevel = 0;
    public float arrowChargeBase;
    public float arrowChargeIncrement;
    [Header("")]
    public int arrowHomingLevel = 0;
    public float arrowHomingBase;
    public float arrowHomingIncrement;
    [Header("")]
    public int arrowLevelMax;
    
    [Header("Plus one")]
    public GameObject chargePlusOnePrefab;
    public GameObject rangePlusOnePrefab;
    public GameObject homingPlusOnePrefab;
    
    [Header("Charge")]
    public bool isCharged;
    private float rechargeTime;
    public GameObject rechargePrefab;
    private bool rechargeInvoked = false;

    [Header("Eggplants")]
	public float eggplantProjecilteSpeed;
	public GameObject eggplantObject;
	public int eggplantNum;
	public float eggplantCooldown;
	public bool canFireEggplant = true;

	[Header("Melee")]
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
    private PlayerUI refPlayerUI;

	void Start ()
	{
		refPlayerMovement = GetComponent<PlayerMovement>();
		refPlayerAudio = GetComponent<PlayerAudio>();
		refPlayerCollision = GetComponent<PlayerCollision>();
        refPlayerUI = GetComponent<PlayerUI>();
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
        if (refPlayerCollision.isDead == false && refPlayerMovement.gameStarted == true)
        {
            CheckLookingUp();
            SpawnCenturions();

            rechargeTime = arrowChargeBase + (arrowChargeIncrement * arrowChargeLevel);

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
		if (Input.GetButton("Up"))
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
		if (Input.GetButtonDown("Shoot"))
		{
			Vector2 shootDir;
			float zRotation;
         float tmpSpeed = arrowSpeedNormal;
			GameObject tmp = null;

         // calculate velocity of the arrow here
         {
            // use increased velocity if we're charged
            if (isCharged == true && arrowChargeLevel > 0)
            {
               tmpSpeed = arrowSpeedCharged;
            }

            // if you're facing upwards
            if (lookingUp == true)
            {
               shootDir = Vector2.up * tmpSpeed;
               zRotation = 0;
            }
            // facing right
            else if (refPlayerMovement.facingRight)
            {
               shootDir = Vector2.right * tmpSpeed;
               zRotation = -90;
            }
            // facing left
            else
            {
               shootDir = Vector2.right * tmpSpeed * -1;
               zRotation = 90;
            }
         }

			// shooting is not allowed while crouching
			if (refPlayerMovement.isCrouching == false)
			{
            // instantiate the arrow
            if (arrowChargeLevel > 0 && isCharged)
            {
               tmp = Instantiate(arrowPrefabCharged, transform.position, Quaternion.Euler(0, 0, zRotation));
               isCharged = false;

               // play the charged shoot sound
               refPlayerAudio.PlayShoot(1.5f, 0.5f);
            }
            else
            {
               tmp = Instantiate(arrowPrefabNormal, transform.position, Quaternion.Euler(0, 0, zRotation));

               // play the normal shoot sound
               refPlayerAudio.PlayShoot(1.0f, 1.0f);
            }

            // apply velocity to the arrow
            tmp.GetComponent<Rigidbody2D>().velocity = shootDir;

            // set the arrow's range
            tmp.GetComponent<MiscDestroyProjectile>().destroyAfterTime = arrowRangeBase + (arrowRangeLevel * arrowRangeIncrement);

            // stop charging a charge shot
            CancelInvoke("RechargeBow");
            rechargeInvoked = false;

         }
      }
	}

	void ShootEggplant()
	{
		int tmpEggplantNum = eggplantNum - Random.Range(0, 1);

		if (Input.GetButtonDown("Shoot") && canFireEggplant)
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
		if (Input.GetButtonDown("Melee") && canHammer && meleeUsesCurrent > 0)
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
                    refPlayerUI.DoEffectHammer();
					meleeUsesCurrent++;
				}
			}
		}
	}

   private void SpawnCenturions()
   {
      if (Input.GetButtonDown("CallCenturion"))
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
      if (arrowChargeLevel > 0)
      {
         isCharged = true;
         rechargeInvoked = false;

         GameObject tmp = Instantiate(rechargePrefab, transform.position, transform.rotation);
         tmp.transform.parent = transform;

         refPlayerAudio.PlayArrowRecharge();
      }
   }

    public void IncreaseCharge()
    {
        if (arrowChargeLevel < arrowLevelMax)
        {
            ++arrowChargeLevel;
            refPlayerAudio.PlayPowerUp();
            Instantiate(chargePlusOnePrefab, new Vector2(transform.position.x, transform.position.y + 0.5f), transform.rotation);
        }
    }

    public void IncreaseRange()
    {
        if (arrowRangeLevel < arrowLevelMax)
        {
            ++arrowRangeLevel;
            refPlayerAudio.PlayPowerUp();
            Instantiate(rangePlusOnePrefab, new Vector2(transform.position.x, transform.position.y + 0.5f), transform.rotation);
        }
    }

    public void IncreaseHoming()
    {
        if (arrowHomingLevel < arrowLevelMax)
        {
            ++arrowHomingLevel;
            refPlayerAudio.PlayPowerUp();
            Instantiate(homingPlusOnePrefab, new Vector2(transform.position.x, transform.position.y + 0.5f), transform.rotation);
        }
    }

    public void IncreaseAll()
    {
        Invoke("IncreaseCharge", 0.0f);
        Invoke("IncreaseRange", 0.25f);
        Invoke("IncreaseHoming", 0.5f);
    }
}