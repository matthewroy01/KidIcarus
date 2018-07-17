using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscIgnoreCollision : MonoBehaviour
{
	public int layer1, layer2;

	void Start ()
	{
		Physics2D.IgnoreLayerCollision(layer1, layer2);
	}
}