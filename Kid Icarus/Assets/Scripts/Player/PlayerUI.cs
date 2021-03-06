﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	[Header("Health display")]
	public Slider sliderHealth;

	[Header("Heart display")]
	public Text textHeart;
    public Image iconSale;

	[Header("Hammer dipslay")]
	public Text textHammer;
	public Slider sliderHammer;

	[Header("Feather display")]
	public GameObject[] feathers;

	[Header("First Aid Kit display")]
	public GameObject firstAidKit;

    [Header("Centurions display")]
    public Text textCenturions;

	[Header("Meters display")]
	public Text textMeters;
	public int startingMeterOffset;
    public Text textHighScore;
    public bool resetScore = false;

    [Header("Start Screen display")]
    public GameObject startScreen;

	private PlayerMovement refMovement;
	private PlayerCollision refCollision;
	private PlayerShoot refShoot;

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

        if (resetScore || !PlayerPrefs.HasKey("HighScore"))
        {
            Debug.LogWarning("High score being reset, turn off for builds.");
            PlayerPrefs.SetInt("HighScore", 0);
        }
	}

	void Update ()
	{
		DisplayHealth();
		DisplayHearts();
		DisplayHammerValues();
		DisplayFeathers();
		DisplayFirstAidKit();
        DisplayCenturions();
		DisplayMeters();
        DisplayHighScore();
        DisplayStartScreen();
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
		for (int i = 0; i < feathers.Length; ++i)
		{
            feathers[i].SetActive(i < refMovement.extraJumps);
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

	private void DisplayHealth()
	{
		// get values from collision script
		float tmpCurrentHealth = (float)refCollision.currentHealth;
		float tmpMaxHealth =  (float)refCollision.maxHealth;

		// set values of the slider
		if (tmpCurrentHealth != 0)
		{
			sliderHealth.value = tmpCurrentHealth / tmpMaxHealth;
		}
		else
		{
			sliderHealth.value = 0;
		}
	}

	private void DisplayHearts()
	{
		textHeart.text = refCollision.hearts.ToString();

        // display the 50% symbol when the next item will be on sale
        if (refCollision.sale == 2)
        {
            iconSale.enabled = true;
        }
        else
        {
            iconSale.enabled = false;
        }
	}

   private void DisplayCenturions()
   {
      textCenturions.text = refShoot.centurionsStored.ToString();
   }

	private void DisplayMeters()
	{
		textMeters.text = (refCollision.getCurrentMeters() + startingMeterOffset).ToString() + "m";
	}

   private void DisplayHighScore()
   {
      int tmpScore = refCollision.getCurrentMeters() + startingMeterOffset;

      if (PlayerPrefs.GetInt("HighScore") < tmpScore)
      {
         PlayerPrefs.SetInt("HighScore", tmpScore);
      }

      textHighScore.text = "HIGH SCORE\n" + PlayerPrefs.GetInt("HighScore").ToString() + "m";
   }

    private void DisplayStartScreen()
    {
        startScreen.SetActive(!refMovement.gameStarted);
    }
} 