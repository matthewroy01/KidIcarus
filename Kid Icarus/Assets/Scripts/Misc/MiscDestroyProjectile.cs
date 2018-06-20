using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscDestroyProjectile : MonoBehaviour
{
	[Header("How should this object be destroyed?")]
	public LayerMask destructableLayer;
	public float destroyAfterTime;

	void Start ()
	{
		if (destroyAfterTime >= 0)
		{
			Invoke("Dest", destroyAfterTime);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		// checking against all layers in a layer mask found here: https://answers.unity.com/questions/50279/check-if-layer-is-in-layermask.html
		if (destructableLayer == (destructableLayer | (1 << other.gameObject.layer)))
		{
			Destroy(gameObject);
		}
	}

	void Dest()
	{
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
}
