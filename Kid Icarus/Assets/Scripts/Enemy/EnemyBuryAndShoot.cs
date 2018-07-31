using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuryAndShoot : MonoBehaviour
{
   [Header("Checking for ground")]
   public uint checkDist;
   private Vector2 finalPos;
   public LayerMask groundLayer;
   public bool allSet = false;
   public Vector2 finalPosOffset;

   [Header("Shooting")]
   public GameObject projPrefab;
   public Vector2 projOrigin;
   public float projSpeed;
   public bool canShoot = true;
   public bool facingRight;
   public float timeStartup;
   public float timeCooldown;
   public float timeWait;
   public Sound soundPopOut;
   public Sound soundHide;

   [Header("Player detection")]
   public float detectionRadius;
   public float tooCloseRadius;

   [Header("Getting stunned")]
   public float stunDist;
   public float stunDuration;
   public bool isStunned = false;
   public GameObject stunnedEffect;
   public Sound soundStunned;

   private Enemy refEnemy;
   private GameObject refPlayer;
   private Animator refAnimator;
   private iamahammer[] tmpHammer;
   private Collider2D refHammer;
   private UtilityAudioManager refAudioManager;

	void Start ()
   {
      refEnemy = GetComponent<Enemy>();
      refPlayer = GameObject.FindGameObjectWithTag("Player");
      refAnimator = GetComponent<Animator>();
      tmpHammer = Resources.FindObjectsOfTypeAll<iamahammer>();
      refHammer = tmpHammer[0].GetComponent<Collider2D>();
      refAudioManager = GameObject.FindObjectOfType<UtilityAudioManager>();

      refEnemy.immuneToArrows = true;
      refEnemy.immuneToHammer = true;

      CheckBelow();
	}

   private void CheckBelow()
   {
      for (uint i = 0; i < checkDist; ++i)
      {
         Vector2 checkPos = new Vector2(transform.position.x, transform.position.y + i);
         // check if we're already on top of something and if the space above us is air 
         if (Physics2D.OverlapCircle(checkPos, 0.2f, groundLayer) && !Physics2D.OverlapCircle(new Vector2(checkPos.x, checkPos.y + 1.0f), 0.2f, groundLayer))
         {
            // save the final position
            finalPos = checkPos + finalPosOffset;
            transform.position = finalPos;
            allSet = true;
            canShoot = true;
            return;
         }
      }

      if (allSet == false)
      {
         // if we didn't find an appropriate position, destroy
         Destroy(gameObject);
      }
   }
	
	void Update ()
   {
      if (allSet && refEnemy.isDead == false)
      {
         if (CheckHammer() == true && isStunned == false)
         {
            // stun
            isStunned = true;
            refAnimator.SetTrigger("Stun");

            // cancel any invokes
            CancelInvoke("ActuallyShoot");
            CancelInvoke("RechargeShoot");
            CancelInvoke("WaitToShoot");

            // wait to stop being stunned
            Invoke("StopStun", stunDuration);

            // make stun effect
            GameObject tmp = Instantiate(stunnedEffect, (Vector2)transform.position + projOrigin, transform.rotation);
            tmp.transform.localScale *= 0.3f;
            tmp.transform.parent = transform;

            // play sound
            StartCoroutine("PlayStunSound");

            // make vulnerable
            Reposition(new Vector2(0, 0.5f));
            refEnemy.immuneToArrows = false;
            refEnemy.immuneToHammer = false;
         }

         if (isStunned == false && CheckRange() == true)
         {
            CheckDirection();
            Shoot();
         }
      }
	}

   private void StopStun()
   {
      isStunned = false;
      refAnimator.SetTrigger("Stun");

      // reset shooting
      canShoot = true;

      // make invulnerable
      Reposition(new Vector2(0, 0.0f));
      refEnemy.immuneToArrows = true;
      refEnemy.immuneToHammer = true;
   }

   private void Shoot()
   {
      if (canShoot)
      {
         canShoot = false;
         Invoke("ActuallyShoot", timeStartup);
      }
   }

   private void ActuallyShoot()
   {
      // change position
      Reposition(new Vector2(0, 0.5f));

      // do animation
      refAnimator.SetTrigger("Shooting");

      // make vulnerable
      refEnemy.immuneToArrows = false;
      refEnemy.immuneToHammer = false;

      // play sound
      refAudioManager.PlaySound(soundPopOut.clip, soundPopOut.volume, true);

      // spawn the projectile and add velocity
      GameObject tmp = Instantiate(projPrefab, (Vector2)transform.position + projOrigin, transform.rotation);
      if (facingRight == true)
      {
         tmp.GetComponent<Rigidbody2D>().velocity = Vector2.right * projSpeed;
      }
      else
      {
         tmp.GetComponent<Rigidbody2D>().velocity = Vector2.right * projSpeed * -1;
      }

      Invoke("RechargeShoot", timeCooldown);
   }

   private void RechargeShoot()
   {
      // change position
      Reposition(new Vector2(0, 0.0f));

      // do animation
      refAnimator.SetTrigger("Shooting");

      // make invulnerable
      refEnemy.immuneToArrows = true;
      refEnemy.immuneToHammer = true;

      // play sound
      refAudioManager.PlaySound(soundHide.clip, soundHide.volume, true);

      Invoke("WaitToShoot", timeWait);
   }

   private void WaitToShoot()
   {
      refAnimator.SetTrigger("Shooting");
      canShoot = true;
   }

   private bool CheckRange()
   {
      // check if the player is in range and also not too close
      if (refPlayer.transform.position.y > finalPos.y - detectionRadius && Vector2.Distance(finalPos, refPlayer.transform.position) > tooCloseRadius)
      {
         return true;
      }
      return false;
   }

   private bool CheckHammer()
   {
      return Vector2.Distance(finalPos, refHammer.transform.position) <= stunDist && refHammer.enabled == true;
   }

   private void CheckDirection()
   {
      facingRight = refPlayer.transform.position.x > finalPos.x;
   }

   private void Reposition(Vector2 offset)
   {
      // that meta Fire Emblem Heroes skill, essential for Voting Gauntlets
      transform.position = finalPos + offset;
   }

   private IEnumerator PlayStunSound()
   {
      while (isStunned == true)
      {
         refAudioManager.PlaySound(soundStunned.clip, soundStunned.volume, true);
         yield return new WaitForSeconds(0.5f);
      }
   }
}