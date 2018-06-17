using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyWithCamera : MonoBehaviour
{
	[Header("Points to lerp between relative to the camera")]
	public Vector2 topLeftOffset;
	public Vector2 topRightOffset;
	public Vector2 bottomOffset;

	private Vector2 topLeft;
	private Vector2 topRight;
	private Vector2 bottom;

	public enum CurrentPoint { topLeft, topRight, bottom };
	private CurrentPoint currentPoint;

	private bool justWasTopRight;

	[Header("Movement")]
	public float physicsSpeed;
	public float physicsAcceleration;
	public float checkDistance;

	[Header("Clones to follow behind")]
	public int numOfClones;
	public float spawnInterval;
	public bool isOriginal;

	// positions to base relative movement off of
	private Transform referencePoint;
	private Vector2 origin;

	// default values to pass on to clones
	private bool defaultTopRight;
	private CurrentPoint defaultCurrentPoint;
	private Vector2 defaultPosition;

	// other components
	private Rigidbody2D rb;
	private SpriteRenderer sr;
	private Enemy refEnemy;

	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		refEnemy = GetComponent<Enemy>();

		// find a reference point
		referencePoint = GameObject.Find("Camera").transform;

		// initialize the origin
		origin = new Vector2(referencePoint.position.x, referencePoint.position.y);

		// only randomize starting position and create clones if we're the original
		if (isOriginal == true)
		{
			// randomly set the starting point to either the top left or top right
			if (Random.Range(0, 2) == 1)
			{
				transform.position = origin + (topRightOffset * 2);
				currentPoint = CurrentPoint.topRight;
				justWasTopRight = true;
				defaultTopRight = true;
			}
			else
			{
				transform.position = origin + (topLeftOffset * 2);
				currentPoint = CurrentPoint.topLeft;
				justWasTopRight = false;
				defaultTopRight = false;
			}
			
			// save the default position for clones
			defaultPosition = transform.position;

			// save the default current position for clones
			defaultCurrentPoint = currentPoint;

			// spawn clones
			if (numOfClones > 0)
			{
				for (int i = 1; i < numOfClones + 1; ++i)
				{
					Invoke("SpawnClone", i * spawnInterval);
				}
			}
		}
	}

	void Update ()
	{
		if (refEnemy.isDead == false)
		{
			UpdatePointLocations();
			CheckDistance();
			FlipSprite();

			Movement();
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
	}

	private void Movement()
	{
		// use physics to move
		Vector2 tmp = (GetCurrentPointVec() - (Vector2)transform.position).normalized;
		rb.AddForce(tmp * physicsAcceleration);
		rb.velocity = rb.velocity.normalized * physicsSpeed;
	}

	private void FlipSprite()
	{
		// flip the sprite based on the direction we're facing
		if (rb.velocity.x <= 0)
		{
			sr.flipX = true;
		}
		else
		{
			sr.flipX = false;
		}
	}

	private void UpdatePointLocations()
	{
		// update the origin
		origin = new Vector2(8.0f, referencePoint.position.y);

		// update points to lerp between
		topRight = origin + topRightOffset;
		topLeft = origin + topLeftOffset;
		bottom = origin + bottomOffset;
	}

	private void CheckDistance()
	{
		// if we're in range of our current point
		if (Vector2.Distance(transform.position, GetCurrentPointVec()) < checkDistance)
		{
			// update the current point
			switch(currentPoint)
			{
				case CurrentPoint.topRight:
				{
					currentPoint = CurrentPoint.bottom;
					break;
				}
				case CurrentPoint.topLeft:
				{
					currentPoint = CurrentPoint.bottom;
					break;
				}
				case CurrentPoint.bottom:
				{
					if (justWasTopRight)
					{
						currentPoint = CurrentPoint.topLeft;
						justWasTopRight = false;
					}
					else
					{
						currentPoint = CurrentPoint.topRight;
						justWasTopRight = true;
					}
					break;
				}
			}
		}
	}

	Vector2 GetCurrentPointVec()
	{
		// if the enemy is within distance of the current target, switch to the next target
		switch(currentPoint)
		{
			case CurrentPoint.topRight:
			{
				return topRight;
			}
			case CurrentPoint.topLeft:
			{
				return topLeft;
			}
			case CurrentPoint.bottom:
			{
				return bottom;
			}
		}
		return Vector2.zero;
	}

	private void SpawnClone()
	{
		EnemyFlyWithCamera tmp = Instantiate(gameObject, defaultPosition, transform.rotation).GetComponent<EnemyFlyWithCamera>();
		tmp.isOriginal = false;
		tmp.currentPoint = defaultCurrentPoint;
		tmp.justWasTopRight = defaultTopRight;
		tmp.transform.position = defaultPosition;
	}
}