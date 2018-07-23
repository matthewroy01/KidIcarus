using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCollision : MonoBehaviour
{
	[Header("Score")]
	public int hearts;
	public int sale = 1;

	[Header("Health")]
	public int maxHealth;
	public int currentHealth;
	public bool isDead;
	public float invincibilityTime;
	private bool canGetHit = true;
	public bool inSafeZone = false;
	public bool constantHealthRegen = false;

	[Header("Drink of the Gods")]
	public int kiddieDrinkAmount;
	public int smallDrinkAmount;
	public int largeDrinkAmount;
	public float healthRestoreInterval;
	public int healthToRestore;

	[Header("Eggplant Curse")]
	public bool cursed;
	public bool hasFirstAidKit;

	[Header("Icons")]
	public GameObject iconOfLightPrefab;

	[Header("UI")]
	private int currentMeters = 0;
	public Text textMessages;

	private PlayerAudio refPlayerAudio;
	private PlayerMovement refPlayerMovement;
   private PlayerShoot refPlayerShoot;
	private ShopInfo refShopInfo;
	private UtilityMusicManager refMusicManager;

	void Start()
	{
		refPlayerAudio = GetComponent<PlayerAudio>();
		refPlayerMovement = GetComponent<PlayerMovement>();
      refPlayerShoot = GetComponent<PlayerShoot>();

		refShopInfo = GameObject.FindObjectOfType<ShopInfo>();
		refMusicManager = GameObject.FindObjectOfType<UtilityMusicManager>();

		currentHealth = maxHealth;
	}

	void Update()
	{
		UpdateMeters();
		CommunicateWithShop();

		// death
		if (currentHealth <= 0)
		{
			currentHealth = 0;
			SetConstantHealthRegen(false, 0);
			healthToRestore = 0;
			Death();
		}

		// curse and first aid
		if (hasFirstAidKit == true && cursed == true)
		{
			hasFirstAidKit = false;
			cursed = false;
			refPlayerAudio.PlayCursed(1.25f);
		}

		// stopping the health restoration coroutine
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

		if (other.CompareTag("Eggplant") && !cursed)
		{
			cursed = true;
			refPlayerAudio.PlayCursed(0.9f);
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
			else if (other.name == "DrinkKiddie(Clone)")
			{
				healthToRestore += kiddieDrinkAmount;
				StartCoroutine("RestoreHealth");
				Destroy(other.gameObject);
			}
			else if (other.name == "RocFeather(Clone)" && refPlayerMovement.extraJumps != refPlayerMovement.extraJumpsMax)
			{
				refPlayerMovement.extraJumps++;
				Destroy(other.gameObject);
			}
			else if (other.name == "CenturionAssist(Clone)")
			{
            refPlayerShoot.centurionsStored++;
				Destroy(other.gameObject);
			}
			else if (other.name == "DivineWard(Clone)")
			{
				GameObject tmpOrne = GameObject.Find("Orne");
				tmpOrne.transform.position = new Vector2(tmpOrne.transform.position.x, tmpOrne.transform.position.y - 50.0f);
				Destroy(other.gameObject);
			}
			else if (other.name == "FirstAidKit(Clone)" && !hasFirstAidKit)
			{
				hasFirstAidKit = true;
				Destroy(other.gameObject);
			}
			else if (other.name == "IconOfLight(Clone)")
			{
				GameObject tmpIcon = Instantiate(iconOfLightPrefab, transform.position, transform.rotation);
				tmpIcon.transform.parent = gameObject.transform;
				Destroy(other.gameObject);
			}
			else
			{
				// shop items
				ShopItem tmp = refShopInfo.getItem(other.name);

				if (tmp.cost / sale <= hearts)
				{
					if (tmp.name == "Small Drink" && currentHealth != maxHealth)
					{
						hearts -= tmp.cost / sale;
						sale = 1;
						healthToRestore += smallDrinkAmount;
						StartCoroutine("RestoreHealth");
						Destroy(other.gameObject);
					}

					if (tmp.name == "Large Drink" && currentHealth != maxHealth)
					{
						hearts -= tmp.cost / sale;
						sale = 1;
						healthToRestore += largeDrinkAmount;
						StartCoroutine("RestoreHealth");
						Destroy(other.gameObject);
					}

					if (tmp.name == "Kiddie Size Drink" && currentHealth != maxHealth)
					{
						hearts -= tmp.cost / sale;
						sale = 1;
						healthToRestore += kiddieDrinkAmount;
						StartCoroutine("RestoreHealth");
						Destroy(other.gameObject);
					}

					if (tmp.name == "Roc's Feather" && refPlayerMovement.extraJumps != refPlayerMovement.extraJumpsMax)
					{
						hearts -= tmp.cost / sale;
						sale = 1;
						refPlayerMovement.extraJumps++;
						Destroy(other.gameObject);
					}

					if (tmp.name == "Centurion Assist")
					{
						hearts -= tmp.cost / sale;
						sale = 1;
                  refPlayerShoot.centurionsStored++;
						Destroy(other.gameObject);
					}

					if (tmp.name == "Divine Ward")
					{
						hearts -= tmp.cost / sale;
						sale = 1;
						GameObject tmpOrne = GameObject.Find("Orne");
						tmpOrne.transform.position = new Vector2(tmpOrne.transform.position.x, tmpOrne.transform.position.y - 50.0f);
						Destroy(other.gameObject);
					}

					if (tmp.name == "First Aid Kit" && !hasFirstAidKit)
					{
						hearts -= tmp.cost / sale;
						sale = 1;
						hasFirstAidKit = true;
						Destroy(other.gameObject);
					}

					if (tmp.name == "Icon of Light")
					{
						hearts -= tmp.cost / sale;
						sale = 1;
						GameObject tmpIcon = Instantiate(iconOfLightPrefab, transform.position, transform.rotation);
						tmpIcon.transform.parent = gameObject.transform;
						Destroy(other.gameObject);
					}

					if (tmp.name == "Icon of Nature")
					{
						hearts -= tmp.cost / sale;
						sale = 1;
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

	private void CommunicateWithShop()
	{
		// first aid kit availability
		if (hasFirstAidKit == true)
		{
			refShopInfo.SetAvailability(false, "First Aid Kit");
		}
		else
		{
			refShopInfo.SetAvailability(true, "First Aid Kit");
		}

		// roc's feather availability
		if (refPlayerMovement.extraJumps >= refPlayerMovement.extraJumpsMax)
		{
			refShopInfo.SetAvailability(false, "Roc's Feather");
		}

		// centurion availability
		/*if (refPlayerShoot.hasCenturion == true)
		{
			refShopInfo.SetAvailability(false, "Centurion Assist");
		}
		else
		{
			refShopInfo.SetAvailability(true, "Centurion Assist");
		}*/
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

				if (constantHealthRegen == false)
				{
					healthToRestore--;
				}
				refPlayerAudio.PlayGetHealth();

				yield return new WaitForSeconds(healthRestoreInterval);
			}
			else
			{
				if (constantHealthRegen == false)
				{
					healthToRestore = 0;
				}

				yield return new WaitForSeconds(healthRestoreInterval);
			}
		}
	}

	public int getCurrentMeters()
	{
		return currentMeters;
	}

	public void SetConstantHealthRegen(bool regen, int duration)
	{
		constantHealthRegen = regen;
		healthToRestore++;
		StartCoroutine("RestoreHealth");
		Invoke("StopConstantHealthRegen", duration);
	}

	public void StopConstantHealthRegen()
	{
		constantHealthRegen = false;	
	}
}