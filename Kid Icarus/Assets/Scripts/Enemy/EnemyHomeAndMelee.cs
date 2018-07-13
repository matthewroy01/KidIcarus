using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHomeAndMelee : MonoBehaviour
{
	[Header("Wandering")]
	public float wanderSpeed;
	public float switchDirectionTime;
	[Range(0.0f, 1.0f)]
	public float wanderLerpSpeed;
	public bool isWandering;

	[Header("Chasing")]
	public float chaseSpeed;
	public float detectionDistance;
	public float meleeDistance;

	private Vector2 currentDirection;
	private Vector2 currentTarget;

	[Header("Melee attack")]
	public bool canAttack;
	public float timeUntilAttack;
	public float attackDuration;
	public float attackCooldown;
	public bool isAttacking;
	public GameObject meleeHitbox;
	public Vector2 spawnLocation;

	[Header("Sound")]
	public Sound inhale;
	public Sound sneeze;

	private Enemy refEnemy;
	private GameObject refPlayer;
	private Rigidbody2D rb;
	private Animator refAnimator;
	private UtilityAudioManager refAudioManager;

	void Start ()
	{
		refEnemy = GetComponent<Enemy>();
		refPlayer = GameObject.FindGameObjectWithTag("Player");
		rb = GetComponent<Rigidbody2D>();
		refAnimator = GetComponent<Animator>();
		refAudioManager = GameObject.FindObjectOfType<UtilityAudioManager>();

		StartCoroutine("ChangeDirection");
	}

	void Update ()
	{
		if (refEnemy.isDead != true)
		{
			// wander
			if (isWandering)
			{
				Wander();
				// check the distance between you and the player
				CheckDistance();
			}
			else
			{
				// chase
				if (isAttacking == false)
				{
					Chase();
				}
			}
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
	}

	private void Wander()
	{
		// wander randomly
		rb.velocity = Vector2.Lerp(rb.velocity, currentDirection, wanderLerpSpeed);
	}

	private void CheckDistance()
	{
		// if we're in range, stop wandering
		if (Vector2.Distance(transform.position, refPlayer.transform.position) <= detectionDistance)
		{
			currentTarget = refPlayer.transform.position;
			isWandering = false;
		}
	}

	private void Chase()
	{
		// if we're in melee distance, attack
		if (Vector2.Distance(transform.position, currentTarget) < meleeDistance && canAttack == true)
		{
			rb.velocity = Vector2.zero;
			isAttacking = true;
			canAttack = false;
			Melee();

			return;
		}
		else if (Vector2.Distance(transform.position, currentTarget) >= meleeDistance)
		{
			// set the velocity in the right direction
			rb.velocity = (currentTarget - (Vector2)transform.position).normalized * chaseSpeed;
		}
		else
		{
			// stop moving if we're close up and can't attack yet
			rb.velocity = Vector2.zero;
		}
	}

	private IEnumerator ChangeDirection()
	{
		while (isWandering)
		{
			// randomize the current direction
			currentDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
			yield return new WaitForSeconds(switchDirectionTime);
		}
	}

	private void Melee()
	{
		refAnimator.SetTrigger("Sneeze");

		refAudioManager.PlaySound(inhale.clip, inhale.volume, true);

		Invoke("MeleeSpawn", timeUntilAttack);
	}

	private void MeleeSpawn()
	{
		if(refEnemy.isDead == false)
		{
			refAnimator.SetTrigger("Sneeze");

			refAudioManager.PlaySound(sneeze.clip, sneeze.volume, true);

			// spawn the attack hitbox
			Instantiate(meleeHitbox, (Vector2)transform.position + spawnLocation, transform.rotation);
			Invoke("MeleeStop", attackDuration);
		}
	}

	private void MeleeStop()
	{
		if(refEnemy.isDead == false)
		{
			refAnimator.SetTrigger("Sneeze");

			// stop attacking
			isAttacking = false;
			// change targets
			currentTarget = refPlayer.transform.position;

			Invoke("MeleeCooldown", attackCooldown);
		}
	}

	private void MeleeCooldown()
	{
		if(refEnemy.isDead == false)
		{
			canAttack = true;
		}
	}
}