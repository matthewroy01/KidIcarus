using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	[Header("Health values")]
	public int maxHealth;
	public int currentHealth;
	public Sound hit;
	public GameObject ouch;

	[Header("Death values")]
	public bool isDead;
	public float deathTime;
	public Sound death;

	[Header("List of dropables")]
	public GameObject[] drops;

	// collider to turn off
	private Collider2D refCollider;
	private Animator refAnimator;
	private UtilityAudioManager refAudioManager;
	private SpriteRenderer refSpriteRenderer;

	void Start ()
	{
		// set up health
		currentHealth = maxHealth;

		// get the collider
		refCollider = GetComponent<Collider2D>();

		// get the animator
		refAnimator = GetComponent<Animator>();

		// get the audio manager
		refAudioManager = GameObject.FindObjectOfType<UtilityAudioManager>();

		// get the sprite renderer
		refSpriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		CheckHealth();
		ScreenWrapping();

		if (refAnimator != null)
		{
			refAnimator.SetBool("isDead", isDead);
		}
	}

	private void CheckHealth()
	{
		// if we're out of health
		if (currentHealth <= 0)
		{
			// disable the collider
			refCollider.enabled = false;

			// so this doesn't happen more than once, check if isDead is false
			if (isDead == false)
			{
				// invoke the gameobject destruction
				Invoke("Death", deathTime);
				isDead = true;
			}
		}
	}

	private void Death()
	{
		// drop something from the list of drops
		if (drops.Length != 0)
		{
			refAudioManager.PlaySound(death.clip, death.volume);
			Instantiate(drops[Random.Range(0, drops.Length)], transform.position, Quaternion.identity);
		}

		Destroy(gameObject);
	}

	private void ScreenWrapping()
	{
		if (transform.position.x > 15.75f)
		{
			transform.position = new Vector2(-0.5f, transform.position.y);
		}

		if (transform.position.x < -0.75f)
		{
			transform.position = new Vector2(15.5f, transform.position.y);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		// when colliding with an arrow, decrease the health
		if (other.CompareTag("Arrow") == true)
		{
			refAudioManager.PlaySound(hit.clip, hit.volume);
			currentHealth--;

			if (ouch != null)
			{
				Instantiate(ouch, other.transform.position, Quaternion.identity);
			}
		}

		// when colliding with the death floor, destroy the object
		if (other.CompareTag("InstantDeath") == true)
		{
			Destroy(gameObject);
		}
	}
}
