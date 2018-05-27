using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
	[Header("Score")]
	public int hearts;

	[Header("Health")]
	public int maxHealth;
	public int currentHealth;
	public float invincibilityTime;
	private bool canGetHit = true;

	[Header("UI")]
	public Text textHearts;
	public Slider sliderHealth;

	private PlayerAudio refPlayerAudio;

	void Start()
	{
		refPlayerAudio = GetComponent<PlayerAudio>();

		currentHealth = maxHealth;
	}

	void Update()
	{
		UpdateUI();

		if (currentHealth <= 0)
		{
			currentHealth = 0;
			Death();
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("InstantDeath"))
		{
			Death();
		}

		if (other.CompareTag("Pickup"))
		{
			if (other.name == "Heart1(Clone)")
			{
				refPlayerAudio.PlayHeart();
				hearts += 1;
			}
			else if (other.name == "Heart5(Clone)")
			{
				refPlayerAudio.PlayHeart();
				hearts += 5;
			}
			else if (other.name == "Heart10(Clone)")
			{
				refPlayerAudio.PlayHeart();
				hearts += 10;
			}

			Destroy(other.gameObject);
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.CompareTag("Enemy") && canGetHit == true)
		{
			refPlayerAudio.PlayHurt();
			currentHealth--;
			canGetHit = false;
			Invoke("StopInvincibility", invincibilityTime);
		}
	}

	private void UpdateUI()
	{
		textHearts.text = hearts.ToString();
		if (currentHealth != 0)
		{
			sliderHealth.value = (float)currentHealth / (float)maxHealth;
		}
		else
		{
			sliderHealth.value = 0;
		}
	}

	private void StopInvincibility()
	{
		canGetHit = true;
	}

	private void Death()
	{
		Debug.Log("I'm finished!");
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}