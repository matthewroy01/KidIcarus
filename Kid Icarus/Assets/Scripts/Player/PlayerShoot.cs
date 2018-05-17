using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
	private PlayerMovement refPlayerMovement;

	[Header("Shooting arrows")]
	public float arrowProjectileSpeed;
	public GameObject arrowObject;

	void Start ()
	{
		refPlayerMovement = GetComponent<PlayerMovement>();
	}

	void Update ()
	{
		Shoot();
	}

	void Shoot()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 shootDir;
			float zRotation;
			GameObject tmp;

			// if you're facing upwards
			if (Input.GetKey("w"))
			{
				shootDir = Vector2.up * arrowProjectileSpeed;
				zRotation = 0;
			}
			// facing right
			else if (refPlayerMovement.facingRight)
			{
				shootDir = Vector2.right * arrowProjectileSpeed;
				zRotation = -90;
			}
			// facing left
			else
			{
				shootDir = Vector2.right * arrowProjectileSpeed * -1;
				zRotation = 90;
			}

			// instantiate the arrow
			tmp = Instantiate(arrowObject, transform.position, Quaternion.Euler(0, 0, zRotation));
			tmp.GetComponent<Rigidbody2D>().velocity = shootDir;
		}
	}
}
