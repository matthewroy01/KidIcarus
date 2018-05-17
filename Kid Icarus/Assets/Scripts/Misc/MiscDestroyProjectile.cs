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
		Invoke("Dest", destroyAfterTime);
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
}
