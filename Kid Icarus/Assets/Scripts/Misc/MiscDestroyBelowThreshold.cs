using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscDestroyBelowThreshold : MonoBehaviour
{
	public Transform playerTransform;
	private float threshold = 32;

	void Start ()
	{
		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
	}

	void Update ()
	{
		// if we're below the player and we're far enough away
		if (transform.position.y < playerTransform.position.y)
		{
			if (Vector2.Distance(transform.position, playerTransform.position) > threshold)
			{
				Destroy(gameObject);
			}
		}
	}
}
