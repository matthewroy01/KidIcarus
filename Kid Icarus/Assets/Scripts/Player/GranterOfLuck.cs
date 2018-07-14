﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GranterOfLuck : MonoBehaviour
{
	[Header("Chance")]
	public int oneInThis;
	public int startingM;
	public int increaseMBy;
	public int targetM;
	public float destroyAfterTime;

	private bool alreadyDecided = false;
	private bool fadeOut = false;

	[Header("Message")]
	public Text goddessText;

	private PlayerCollision refPlayerCollision;

	void Start ()
	{
		refPlayerCollision = transform.parent.GetComponent<PlayerCollision>();
		startingM = refPlayerCollision.getCurrentMeters();
		targetM = startingM + increaseMBy;

		goddessText = GameObject.Find("textGoddess").GetComponent<Text>();;
	}

	void Update ()
	{
		if (refPlayerCollision.getCurrentMeters() >= targetM && alreadyDecided == false)
		{
			if (Random.Range(0, oneInThis) == 0)
			{
				goddessText.canvasRenderer.SetAlpha(0);//= new Color(goddessText.color.r, goddessText.color.g, goddessText.color.b, 0);
				alreadyDecided = true;
				DoRandomEffect();
			}
		}

		if (alreadyDecided == true && fadeOut == false)
		{
			goddessText.CrossFadeAlpha(1, 1.0f, false);
		}

		if (fadeOut == true)
		{
			goddessText.CrossFadeAlpha(0, 1.0f, false);
		}
	}

	private void DoRandomEffect()
	{
		int tmp = Random.Range(0, 3);

		switch(tmp)
		{
			case 0:
			{
				goddessText.text = "The next item you purchase in the\nshop will be half off...";
				refPlayerCollision.sale = 2;
				break;
			}
			case 1:
			{
				goddessText.text = "Health restoration for 10 seconds...";
				refPlayerCollision.SetConstantHealthRegen(true, 10);
				break;
			}
			case 2:
			{
				goddessText.text = "I've slowed down the Orne\nfor you, Pit...";
				EnemyOrne tmpOrne = GameObject.Find("Orne").GetComponent<EnemyOrne>();
				tmpOrne.SetDefaultMovSpeed(tmpOrne.GetDefaultMovSpeed() - tmpOrne.increaseMovSpeedBy * 2);
				break;
			}
			default:
			{
				goddessText.text = "Sorry Pit, the goddess screwed up her switch statement somehow.";
				break;
			}
		}

		Invoke("FadeOut", destroyAfterTime - 1.5f);
		Invoke("Dest", destroyAfterTime);
	}

	private void FadeOut()
	{
		fadeOut = true;
	}

	private void Dest()
	{
		goddessText.text = "";
		Destroy(gameObject);
	}
}