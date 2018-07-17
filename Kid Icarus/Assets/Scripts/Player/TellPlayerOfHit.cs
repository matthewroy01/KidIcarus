using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TellPlayerOfHit : MonoBehaviour
{
	public int IncreaseChargeBy = 1;

	void Start ()
	{
		GameObject.FindObjectOfType<PlayerShoot>().IncreaseMeleeCharge(IncreaseChargeBy);
	}
}
