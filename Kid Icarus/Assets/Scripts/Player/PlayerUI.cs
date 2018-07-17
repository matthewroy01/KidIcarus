using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	[Header("Feather display")]
	public GameObject[] feathers;

	[Header("First Aid Kit display")]
	public GameObject firstAidKit;

	private PlayerMovement refMovement;
	private PlayerCollision refCollision;
	private PlayerShoot refShoot;

	[Header("Hammer dipslay")]
	public Text textHammer;
	public Slider sliderHammer;

	void Start ()
	{
		refMovement = GetComponent<PlayerMovement>();
		refCollision = GetComponent<PlayerCollision>();
		refShoot = GetComponent<PlayerShoot>();

		// set feathers to inactive at first
		for (int i = 1; i < feathers.Length; ++i)
		{
			feathers[i].SetActive(false);
		}

		// update the size of the hammer slider for convenience
		sliderHammer.maxValue = refShoot.meleeChargeTotal;
	}

	void Update ()
	{
		DisplayFeathers();
		DisplayFirstAidKit();

		DisplayHammerValues();
	}

	private void DisplayHammerValues()
	{
		// hammer durability
		textHammer.text = refShoot.meleeUsesCurrent.ToString();

		// hammer recharge meter
		sliderHammer.value = refShoot.meleeChargeCurrent;
	}

	private void DisplayFeathers()
	{
		// enable feathers as more jumps are added
		for (int i = 0; i < refMovement.extraJumps; ++i)
		{
			feathers[i].SetActive(true);
		}
	}

	private void DisplayFirstAidKit()
	{
		if (refCollision.hasFirstAidKit == true)
		{
			firstAidKit.SetActive(true);
		}
		else
		{
			firstAidKit.SetActive(false);
		}
	}
}