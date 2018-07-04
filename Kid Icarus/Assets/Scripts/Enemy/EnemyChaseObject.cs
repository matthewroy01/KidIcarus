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

	private Enemy refEnemy;
	private Rigidbody2D rb;
	private Animator refAnimator;

	void Start()
	{
		refEnemy = GetComponent<Enemy>();
		rb = GetComponent<Rigidbody2D>();
		refAnimator = GetComponent<Animator>();
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