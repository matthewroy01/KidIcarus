using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowPath : MonoBehaviour
{
	public enum Type { Linear = 0, Cosine = 1 , Physics = 2};

	[Header("Movement")]
	[Range(0.0f, 1.0f)]
	public float interpolationSpeed;
	public Type interpolationType = 0; 
	[Header("Only used if type \"Physics\" is selected")]
	public float physicsSpeed;
	public float physicsAcceleration;

	[Header("List of points")]
	public int numOfPoints;
	public List<Vector2> points;

	[Header("Range at which to spawn points")]
	public float minRange;
	public float maxRange;

	[Header("Distance this object needs to be from a point to move to the next one")]
	public float checkDistance;

	// private variables
	private Vector2 scuttleBugHome; // this enemies original position
	private int target; // the ID of the current target position in the list of points
	private Rigidbody2D rb;
	private Enemy refEnemy;
	private Animator refAnimator;

	void Start()
	{
		// keep track of the default position
		scuttleBugHome = transform.position;

		// generate the points to inerpolate between
		GeneratePoints();

		if (interpolationType == Type.Physics)
		{
			rb = gameObject.AddComponent<Rigidbody2D>();
			rb.gravityScale = 0;
		}

		// enemy status
		refEnemy = GetComponent<Enemy>();

		// animation
		refAnimator = GetComponent<Animator>();
	}

	void Update ()
	{
		transform.position = InterpolateVector2(transform.position, scuttleBugHome + points[target]);

		CheckPoints();
	}

	void GeneratePoints()
	{
		for(int i = 0; i < numOfPoints; ++i)
		{
			// generate points
			Vector2 tmp = new Vector2(Random.Range(maxRange * -1, maxRange), Random.Range(maxRange * -1, maxRange));

			// clamp each point so that the X and Y values are between the min and max ranges
			// clamp X
			if (tmp.x > 0)
			{
				Mathf.Clamp(tmp.x, minRange, maxRange);
			}
			else
			{
				Mathf.Clamp(tmp.x, minRange * -1, maxRange * -1);
			}

			// clamp Y
			if (tmp.y > 0)
			{
				Mathf.Clamp(tmp.y, minRange, maxRange);
			}
			else
			{
				Mathf.Clamp(tmp.y, minRange * -1, maxRange * -1);
			}

			// convert to int
			tmp = new Vector2((int)tmp.x, (int)tmp.y);

			// add the point to the list
			points.Add(tmp);
		}
	}

	void CheckPoints()
	{
		// if the enemy is within distance of the current target, switch to the next target
		if (Vector2.Distance(transform.position, scuttleBugHome + points[target]) < checkDistance)
		{
			if (target == numOfPoints - 1)
			{
				target = 0;
			}
			else
			{
				target++;
			}
		}
	}

	Vector2 InterpolateVector2(Vector2 from, Vector2 to)
	{
		// if we're dead, just stay still
		if (refEnemy.isDead == true)
		{
			// if we were using physics, stop any velocity
			if (interpolationType == Type.Physics)
			{
				rb.velocity = Vector2.zero;
			}
			return transform.position;
		}
		else
		{
			// interpolate between from and to using the specified interpolation type
			switch((int)interpolationType)
			{
				case 0:
				{
					// linear interpolation
					return Vector2.Lerp(from, to, interpolationSpeed);
				}
				case 1:
				{
					// cosine interpolation
					return new Vector2(CosineInterpolate(from.x, to.x, interpolationSpeed), CosineInterpolate(from.y, to.y, interpolationSpeed));
				}
				case 2:
				{
					// physics
					Vector2 tmp = ((scuttleBugHome + points[target]) - (Vector2)transform.position).normalized;
					rb.AddForce(tmp * physicsAcceleration);
					rb.velocity = rb.velocity.normalized * physicsSpeed;
					return transform.position;
				}
				default:
				{
					// if the interpolation type is somehow invalid
					Debug.LogError("EnemyFollowPath: " + (int)interpolationType + "is not a valid interpolation type.");
					return transform.position;
				}
			}
		}
	}

	// interpolation algorithms found here: http://paulbourke.net/miscellaneous/interpolation/
	float CosineInterpolate(float val1, float val2, float interpolationValue)
	{
    	float interpolationValue2;

		interpolationValue2 = (1 - Mathf.Cos(interpolationValue * Mathf.PI)) / 2;
		return(val1 * (1 - interpolationValue2) + val2 * interpolationValue2);
	}

	void OnDrawGizmosSelected()
	{
		// draws the min and max ranges as wireframe spheres (because there is no Gizmos.DrawCircle)
		Gizmos.DrawWireSphere(scuttleBugHome, minRange);
		Gizmos.DrawWireSphere(scuttleBugHome, maxRange);

		Gizmos.color = Color.green;

		for (int i = 0 ; i < numOfPoints; ++i)
		{
			Gizmos.DrawSphere(scuttleBugHome + points[i], 0.1f);
		}
	}
}