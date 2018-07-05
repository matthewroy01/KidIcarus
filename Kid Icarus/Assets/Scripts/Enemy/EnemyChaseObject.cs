using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseObject : MonoBehaviour
{
	[Header("What are we trying to chase?")]
	public string tagName;

	[Header("Chase parameters")]
	public GameObject currentlyChasing;
	public float chaseDistance;
	public bool isChasing;

	[Header("Movement")]
	public float movSpeed;

	[Header("Sound")]
	public Sound flight;
	public float flightSoundInterval;

	private Enemy refEnemy;
	private Rigidbody2D rb;
	private Animator refAnimator;
	private UtilityAudioManager refAudioManager;

	void Start()
	{
		refEnemy = GetComponent<Enemy>();
		rb = GetComponent<Rigidbody2D>();
		refAnimator = GetComponent<Animator>();
		refAudioManager = GameObject.FindObjectOfType<UtilityAudioManager>();

		StartCoroutine("PlayFlightSound");
	}

	void Update()
	{
		if (refEnemy.isDead == false)
		{
			Chase();
			UpdateAnimation();
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
	}

	private void Chase()
	{
		// if there's something to chase, start moving
		if (currentlyChasing != null && Vector2.Distance(transform.position, currentlyChasing.transform.position) < chaseDistance)
		{
			isChasing = true;
			Movement();
		}
		else
		{
			// otherwise, stop chasing and set our gameobject to null
			isChasing = false;
			currentlyChasing = null;

			// stop moving
			rb.velocity = Vector2.zero;

			// look for another gameobject with the desired tag
			if (GameObject.FindGameObjectWithTag(tagName))
			{
				currentlyChasing = GameObject.FindGameObjectWithTag(tagName);
			}
		}
	}

	private IEnumerator PlayFlightSound()
	{
		while (refEnemy.isDead == false)
		{
			if (isChasing == true)
			{
				refAudioManager.PlaySound(flight.clip, flight.volume, true);
			}

			yield return new WaitForSeconds(flightSoundInterval);
		}
	}

	private void UpdateAnimation()
	{
		refAnimator.SetBool("isChasing", isChasing);
	}

	private void Movement()
	{
		// move towards our destination
		rb.velocity = (currentlyChasing.transform.position - transform.position).normalized * movSpeed;
	}
}