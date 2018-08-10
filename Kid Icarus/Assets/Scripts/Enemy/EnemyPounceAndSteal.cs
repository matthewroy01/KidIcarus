using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPounceAndSteal : MonoBehaviour
{
   [Header("Transforms for ground detection")]
   public Vector2 below;
   public LayerMask groundMask;
   public bool grounded;

   [Header("Detection")]
   public float detectionRange;
   public bool facingRight;

   [Header("Pounce")]
   public Vector2 pounceVector;
   public bool pounced = false;
   public Sound pounceSound;
   public ParticleSystem pouncePartsLeft;
   public ParticleSystem pouncePartsRight;

   private GameObject refPlayer;
   private SpriteRenderer sr;
   private Rigidbody2D rb;
   private Enemy refEnemy;
   private UtilityAudioManager refAudioManager;
   private Animator refAnimator;

   void Start ()
   {
      refPlayer = GameObject.FindGameObjectWithTag("Player");
      sr = GetComponent<SpriteRenderer>();
      rb = GetComponent<Rigidbody2D>();
      refEnemy = GetComponent<Enemy>();
      refAudioManager = GameObject.FindObjectOfType<UtilityAudioManager>();
      refAnimator = GetComponent<Animator>();
   }

	void Update ()
   {
      if (refEnemy.isDead == false)
      {
         // if we're not grounded, fall
         if (grounded == false)
         {
            Falling();
         }
         else
         {
            if (!pounced)
            {
               CheckDirection();
               FacePlayer();
               CheckDistance();
            }
         }
      }
      else
      {
         rb.velocity = Vector2.zero;
      }
	}

   private void Falling()
   {
      // if there's nothing below us, fall
      if (Physics2D.OverlapCircle((Vector2)transform.position + below, 0.2f, groundMask) == false)
      {
         rb.velocity = Vector2.down * 7.5f;
      }
      else
      {
         // stop falling and set grounded to true
         rb.velocity = new Vector2(rb.velocity.x, 0.0f);
         grounded = true;
      }
   }

   private void CheckDirection()
   {
      facingRight = transform.position.x > refPlayer.transform.position.x;
   }

   private void FacePlayer()
   {
      sr.flipX = !facingRight;
   }

   private void CheckDistance()
   {
      float distance = transform.position.y - refPlayer.transform.position.y;

      if (distance < detectionRange && distance > -1 * detectionRange)
      {
         Pounce();
      }
   }

   private void Pounce()
   {
      pounced = true;

      // start ignoring collision with ground
      Physics2D.IgnoreLayerCollision(gameObject.layer, 8);

      // reactivate gravity
      rb.gravityScale = 1.0f;

      // make the enemy invicible
      refEnemy.immuneToArrows = true;
      refEnemy.immuneToHammer = true;

      // play jump sound
      refAudioManager.PlaySound(pounceSound.clip, pounceSound.volume, 0.5f);

      refAnimator.SetTrigger("Pounce");

      if (facingRight)
      {
         rb.velocity = new Vector2(pounceVector.x * -1, pounceVector.y);

         // play particles
         pouncePartsLeft.Play();
      }
      else
      {
         rb.velocity = new Vector2(pounceVector.x, pounceVector.y);

         // play particles
         pouncePartsRight.Play();
      }
   }
}
