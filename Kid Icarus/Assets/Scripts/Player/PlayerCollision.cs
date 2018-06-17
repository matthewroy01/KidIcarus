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
	public bool isDead;
	public float invincibilityTime;
	private bool canGetHit = true;

	[Header("Drink of the Gods")]
	public int smallDrinkAmount;
	public int largeDrinkAmount;
	public float healthRestoreInterval;
	private int healthToRestore;

	[Header("UI")]
	public Text textHearts;
	public Text textMeters;
	private int currentMeters = 0;
	public int startingMeterOffset;
	public Slider sliderHealth;

	private PlayerAudio refPlayerAudio;
	private PlayerMovement refPlayerMovement;

	void Start()
	{
		refPlayerAudio = GetComponent<PlayerAudio>();
		refPlayerMovement = GetComponent<PlayerMovement>();

		currentHealth = maxHealth;
	}

	void Update()
	{
		UpdateMeters();
		UpdateUI();

		if (currentHealth <= 0)
		{
			currentHealth = 0;
			Death();
		}

		if (healthToRestore == 0)
		{
			StopCoroutine("RestoreHealth");
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
			else if (other.name == "DrinkSmall(Clone)")
			{
				healthToRestore += smallDrinkAmount;
				StartCoroutine("RestoreHealth");
			}
			else if (other.name == "DrinkLarge(Clone)")
			{
				healthToRestore += largeDrinkAmount;
				StartCoroutine("RestoreHealth");
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

	private void UpdateMeters()
	{
		// update the current meters climbed
		if (transform.position.y > currentMeters)
		{
			currentMeters = (int)transform.position.y;
		}
	}

	private void UpdateUI()
	{
		textHearts.text = hearts.ToString();
		textMeters.text = (currentMeters + startingMeterOffset).ToString() + "m";
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
		if (isDead == false)
		{
			Debug.Log("I'm finished!");
			isDead = true;
			refPlayerAudio.PlayDead();

			// do a little jump
			refPlayerMovement.Jump();

			// disable collision
			refPlayerMovement.defaultCollider.enabled = false;
			refPlayerMovement.crouchedCollider.enabled = false;

			// reload the scene
			Invoke("ReloadScene", 3.5f);
		}
	}

	private void ReloadScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	private IEnumerator RestoreHealth()
	{
		while (healthToRestore > 0)
		{
			if (currentHealth < maxHealth && currentHealth > 0)
			{
				currentHealth++;
				healthToRestore--;
				refPlayerAudio.PlayGetHealth();

				yield return new WaitForSeconds(healthRestoreInterval);
			}
			else
			{
				healthToRestore = 0;
			}
		}
	}

	public int getCurrentMeters()
	{
		return currentMeters;
	}
}