using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	[Header("Health values")]
	public int maxHealth;
	public int currentHealth;

	[Header("Death values")]
	public bool isDead;
	public float deathTime;

	[Header("List of dropables")]
	public GameObject[] drops;

	// collider to turn off
	private Collider2D refCollider;
	private Animator refAnimator;

	void Start ()
	{
		// set up health
		currentHealth = maxHealth;

		// get the collider
		refCollider = GetComponent<Collider2D>();

		// get the animator
		refAnimator = GetComponent<Animator>();
	}

	void Update()
	{
		CheckHealth();

		refAnimator.SetBool("isDead", isDead);
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
			Instantiate(drops[Random.Range(0, drops.Length)], transform.position, Quaternion.identity);
		}

		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		// when colliding with an arrow, decrease the health
		if (other.CompareTag("Arrow") == true)
		{
			currentHealth--;
		}
	}
}
