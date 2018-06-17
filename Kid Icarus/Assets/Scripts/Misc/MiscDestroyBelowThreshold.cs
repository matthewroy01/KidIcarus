using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscDestroyBelowThreshold : MonoBehaviour
{
	private Vector2 playerPos;
	private float threshold = 32;

	void Start ()
	{
		playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
	}

	void Update ()
	{
		// if we're below the player and we're far enough away
		if (transform.position.y < playerPos.y && transform.position.y - playerPos.y > threshold)
		{
			Destroy(gameObject);
		}
	}
}
