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
	public bool inSafeZone = false;

	[Header("Drink of the Gods")]
	public int smallDrinkAmount;
	public int largeDrinkAmount;
	public float healthRestoreInterval;
	private int healthToRestore;

	[Header("Centurion")]
	public GameObject centurionPrefab;
	public bool hasCenturion = false;

	[Header("UI")]
	public Text textHearts;
	public Text textMeters;
	private int currentMeters = 0;
	public int startingMeterOffset;
	public Slider sliderHealth;
	public Text textMessages;

	private PlayerAudio refPlayerAudio;
	private PlayerMovement refPlayerMovement;
	private ShopInfo refShopInfo;
	private UtilityMusicManager refMusicManager;

	void Start()
	{
		refPlayerAudio = GetComponent<PlayerAudio>();
		refPlayerMovement = GetComponent<PlayerMovement>();

		refShopInfo = GameObject.FindObjectOfType<ShopInfo>();
		refMusicManager = GameObject.FindObjectOfType<UtilityMusicManager>();

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
			// normal items
			if (other.name == "Heart1(Clone)")
			{
				refPlayerAudio.PlayHeart();
				hearts += 1;
				Destroy(other.gameObject);
			}
			else if (other.name == "Heart5(Clone)")
			{
				refPlayerAudio.PlayHeart();
				hearts += 5;
				Destroy(other.gameObject);
			}
			else if (other.name == "Heart10(Clone)")
			{
				refPlayerAudio.PlayHeart();
				hearts += 10;
				Destroy(other.gameObject);
			}
			else if (other.name == "DrinkSmall(Clone)")
			{
				healthToRestore += smallDrinkAmount;
				StartCoroutine("RestoreHealth");
				Destroy(other.gameObject);
			}
			else if (other.name == "DrinkLarge(Clone)")
			{
				healthToRestore += largeDrinkAmount;
				StartCoroutine("RestoreHealth");
				Destroy(other.gameObject);
			}
			else
			{
				// shop items
				ShopItem tmp = refShopInfo.getItem(other.name);

				if (tmp.cost <= hearts)
				{
					if (tmp.name == "Small Drink")
					{
						hearts -= tmp.cost;
						healthToRestore += smallDrinkAmount;
						StartCoroutine("RestoreHealth");
						Destroy(other.gameObject);
					}

					if (tmp.name == "Large Drink")
					{
						hearts -= tmp.cost;
						healthToRestore += largeDrinkAmount;
						StartCoroutine("RestoreHealth");
						Destroy(other.gameObject);
					}

					if (tmp.name == "Roc's Feather")
					{
						hearts -= tmp.cost;
						refPlayerMovement.extraJumps++;
						Destroy(other.gameObject);
					}

					if (tmp.name == "Centurion Assist" && hasCenturion == false)
					{
						hearts -= tmp.cost;
						hasCenturion = true;
						Instantiate(centurionPrefab, other.transform.position, transform.rotation);
						Destroy(other.gameObject);
					}

					if (tmp.name == "Divine Ward")
					{
						hearts -= tmp.cost;
						GameObject tmpOrne = GameObject.Find("Orne");
						tmpOrne.transform.position = new Vector2(tmpOrne.transform.position.x, tmpOrne.transform.position.y - 50.0f);
						Destroy(other.gameObject);
					}
				}
			}
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

		if (other.CompareTag("SafeZone"))
		{
			refMusicManager.SetMusicStatus(MusicStatus.shopTheme);
			inSafeZone = true;
		}

		if (other.CompareTag("TextArea"))
		{
			textMessages.text = other.gameObject.GetComponent<TextArea>().text;
		}
		else
		{
			textMessages.text = "";
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("SafeZone"))
		{
			refMusicManager.SetMusicStatus(MusicStatus.mainTheme);
			inSafeZone = false;
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
			refMusicManager.SetMusicStatus(MusicStatus.death);

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