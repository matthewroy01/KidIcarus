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
    private int heartsCollected = 0;
    private int heartsSpent = 0;

    [Header("Health")]
	public float maxHealth;
	public float currentHealth;
	public bool isDead;
	public float invincibilityTime;
    public GameObject hurtParts;
	public bool canGetHit = true;
	public bool inSafeZone = false;
    public bool constantHealthRegen = false;
    public GameObject refFinalResultsText;
    private FinalResults refFinalResults;
    private bool readyToRestart = false;

    [Header("Drink of the Gods")]
	public int kiddieDrinkAmount;
	public int smallDrinkAmount;
	public int largeDrinkAmount;
	public float healthRestoreInterval;
	public int healthToRestore;

    [Header("Divine Ward")]
    public int wardMeters;

	[Header("Eggplant Curse")]
	public bool cursed;
	public bool hasFirstAidKit;

	[Header("Icons")]
	public GameObject iconOfLightPrefab;

	[Header("UI")]
	public Text textMessages;
    public Text textTheft;
    public ParticleSystem moneyParts;
    private int currentMeters = 0;
    public LayerMask textAreaMask;
    private string previousMessage = "";

    private PlayerAudio refPlayerAudio;
	private PlayerMovement refPlayerMovement;
    private PlayerShoot refPlayerShoot;
    private PlayerAnimation refPlayerAnimation;
    private PlayerUI refPlayerUI;
	private ShopInfo refShopInfo;
	private UtilityMusicManager refMusicManager;
    private SpriteRenderer refSpriteRenderer;

	void Start()
	{
		refPlayerAudio = GetComponent<PlayerAudio>();
		refPlayerMovement = GetComponent<PlayerMovement>();
        refPlayerShoot = GetComponent<PlayerShoot>();
        refPlayerAnimation = GetComponent<PlayerAnimation>();
        refPlayerUI = GetComponent<PlayerUI>();

		refShopInfo = GameObject.FindObjectOfType<ShopInfo>();
		refMusicManager = GameObject.FindObjectOfType<UtilityMusicManager>();

        refSpriteRenderer = GetComponent<SpriteRenderer>();

        refFinalResultsText.SetActive(false);
        refFinalResults = GetComponent<FinalResults>();

		currentHealth = maxHealth;
	}

	void Update()
	{
		UpdateMeters();
		CommunicateWithShop();
        CheckForTextAreas();

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

        if (readyToRestart == true && Input.GetButtonDown("Jump"))
        {
            ReloadScene();
        }

        refMusicManager.SetWobble(cursed);
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
                heartsCollected += 1;
                refPlayerUI.DoEffectHeart();
				Destroy(other.gameObject);
			}
			else if (other.name == "Heart5(Clone)")
			{
				refPlayerAudio.PlayHeart();
				hearts += 5;
                heartsCollected += 5;
                refPlayerUI.DoEffectHeart();
                Destroy(other.gameObject);
			}
			else if (other.name == "Heart10(Clone)" || other.name == "FlyingHeart(Clone)")
			{
				refPlayerAudio.PlayHeart();
				hearts += 10;
                heartsCollected += 10;
                refPlayerUI.DoEffectHeart();
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
			else if (other.name == "AngelFeather(Clone)" && refPlayerMovement.extraJumps != refPlayerMovement.extraJumpsMax)
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
                EnemyOrne tmpOrne = GameObject.FindObjectOfType<EnemyOrne>();
                tmpOrne.SendBack(wardMeters);
                Destroy(other.gameObject);
			}
			else if (other.name == "FirstAidKit(Clone)")
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
            else if (other.name == "ChargeReticle(Clone)")
            {
                refPlayerShoot.IncreaseCharge();
                Destroy(other.gameObject);
            }
            else if (other.name == "Longbow(Clone)")
            {
                refPlayerShoot.IncreaseRange();
                Destroy(other.gameObject);
            }
            else if (other.name == "HomingBooster(Clone)")
            {
                refPlayerShoot.IncreaseHoming();
                Destroy(other.gameObject);
            }
            else if (other.name == "PowerupPack(Clone)")
            {
                refPlayerShoot.IncreaseAll();
                Destroy(other.gameObject);
            }
            else
			{
                // shop items
                ShopItem tmp = refShopInfo.getItem(other.name);

                if (tmp.cost / sale <= hearts)
                {
                    if (tmp.name == "Small Drink")
                    {
                        if (currentHealth != maxHealth)
                        {
                            healthToRestore += smallDrinkAmount;
                            StartCoroutine("RestoreHealth");
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (tmp.name == "Large Drink")
                    {
                        if (currentHealth != maxHealth)
                        {
                            healthToRestore += largeDrinkAmount;
                            StartCoroutine("RestoreHealth");
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (tmp.name == "Kiddie Size Drink")
                    {
                        if (currentHealth != maxHealth)
                        {
                            healthToRestore += kiddieDrinkAmount;
                            StartCoroutine("RestoreHealth");
                        }
                        else
                        {
                        return;
                        }
                    }

                    if (tmp.name == "Angel's Feather")
                    {
                        if (refPlayerMovement.extraJumps != refPlayerMovement.extraJumpsMax)
                        {
                            refPlayerMovement.extraJumps++;
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (tmp.name == "Centurion Assist")
                    {
                        refPlayerShoot.centurionsStored++;
                    }

                    if (tmp.name == "Divine Ward")
                    {
                        EnemyOrne tmpOrne = GameObject.FindObjectOfType<EnemyOrne>();
                        tmpOrne.SendBack(wardMeters);
                    }

                    if (tmp.name == "First Aid Kit")
                    {
                        if (!hasFirstAidKit)
                        {
                            hasFirstAidKit = true;
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (tmp.name == "Icon of Light")
                    {
                        GameObject tmpIcon = Instantiate(iconOfLightPrefab, transform.position, transform.rotation);
                        tmpIcon.transform.parent = gameObject.transform;
                    }

                    if (tmp.name == "Icon of Nature")
                    {

                    }

                    if (tmp.name == "Charge Reticle")
                    {
                        refPlayerShoot.IncreaseCharge();
                    }

                    if (tmp.name == "Longbow")
                    {
                        refPlayerShoot.IncreaseRange();
                    }

                    if (tmp.name == "Homing Booster")
                    {
                        refPlayerShoot.IncreaseHoming();
                    }

                    if (tmp.name == "Powerup Pack")
                    {
                        refPlayerShoot.IncreaseAll();
                    }

                    if (tmp.name == "Exit Tutorial")
                    {
                        SceneManager.LoadScene("mainScene");
                    }

                    hearts -= tmp.cost / sale;
                    sale = 1;
                    heartsSpent += tmp.cost;
                    Destroy(other.gameObject);

                    // play money particles
                    ParticleSystem tmpParts = Instantiate(moneyParts, transform.position, Quaternion.identity);
                    tmpParts.emission.SetBurst(0, new ParticleSystem.Burst(0.0f, tmp.cost / 2));
                    tmpParts.Play();
                    refPlayerAudio.PlayKaching();
                }
			}
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.CompareTag("Enemy") && canGetHit == true)
		{
			refPlayerAudio.PlayHurt();
            refPlayerUI.DoEffectHealth();
            Instantiate(hurtParts, transform.position, Quaternion.identity);

            // have centurion take damage as well
            if (refPlayerShoot.currentCenturion != null)
            {
                refPlayerShoot.currentCenturion.TakeDamage();
            }

            Enemy tmp = other.GetComponent<Enemy>();
            if (tmp != null)
            {
                currentHealth -= tmp.damage;
            }
            else
            {
                currentHealth -= 1.0f;
                Debug.LogWarning("This object with Enemy tag has no Enemy component.");
            }

            Time.timeScale = 0.1f;
            Invoke("ResetTime", 0.02f);

			canGetHit = false;
            refPlayerAnimation.StartBlink();
            Invoke("StopInvincibility", invincibilityTime);
		}

      if (other.CompareTag("Theif") && canGetHit == true)
      {
         List<int> candidates = new List<int>();

         if (refPlayerMovement.extraJumps != 1)
         {
            candidates.Add(0);
         }

         /*if (hearts > 0)
         {
            candidates.Add(1);
         }

         if (refPlayerShoot.hasChargeReticle || refPlayerShoot.hasLongbow)
         {
            candidates.Add(2);
         }*/

         if (candidates.Count > 0)
         {
            int rand = Random.Range(0, candidates.Count);

            switch (candidates[rand])
            {
               case 0:
               {
                  textTheft.text = "Angel's Feather stolen!";
                  if (refPlayerMovement.extraJumps != 1)
                  {
                     refPlayerMovement.extraJumps--;
                  }
                  break;
               }
               case 1:
               {
                  textTheft.text = "Half of your hearts stolen!";
                  hearts = hearts / 2;
                  break;
               }
               case 2:
               {
                  textTheft.text = "Bow upgrades stolen!";
                  refPlayerShoot.arrowChargeLevel = 0;
                  refPlayerShoot.arrowRangeLevel = 0;

                  break;
               }
               default:
               {
                  break;
               }
            }
         }
         else
         {
            textTheft.text = "Couldn't steal anything!";
         }

         refPlayerAudio.PlayTheif();

         CancelInvoke("ResetTheftMessage");
         Invoke("ResetTheftMessage", 3.0f);

         canGetHit = false;
         Invoke("StopInvincibility", invincibilityTime);
      }

      if (other.CompareTag("SafeZone"))
		{
			refMusicManager.SetMusicStatus(MusicStatus.shopTheme);
			inSafeZone = true;
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

    private void ResetTime()
    {
        Time.timeScale = 1.0f;
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
      refShopInfo.SetAvailability(!hasFirstAidKit, "First Aid Kit");

		// roc's feather availability
		if (refPlayerMovement.extraJumps >= refPlayerMovement.extraJumpsMax)
		{
			refShopInfo.SetAvailability(false, "Angel's Feather");
		}
      else
      {
         refShopInfo.SetAvailability(true, "Angel's Feather");
      }
   }

	private void StopInvincibility()
	{
		canGetHit = true;
	}

   private void ResetTheftMessage()
   {
      textTheft.text = "";
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
			Invoke("DisplayFinalResults", 3.5f);
		}
	}

    private void CheckForTextAreas()
    {
        Collider2D other = Physics2D.OverlapCircle(transform.position, 0.2f, textAreaMask);

        if (other != null && other.CompareTag("TextArea"))
        {
            TextArea textArea = other.gameObject.GetComponent<TextArea>();
            if (previousMessage != textArea.text)
            {
                textMessages.GetComponent<UIEffect>().DoEffect();
            }

            textMessages.text = textArea.text;
            previousMessage = textMessages.text;
        }
        else
        {
            textMessages.text = "";
            previousMessage = textMessages.text;
        }
    }

   private void DisplayFinalResults()
   {
      readyToRestart = true;
      refFinalResultsText.SetActive(true);
      refFinalResults.CalculateFinalResults();
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

   public int GetHeartsCollected()
   {
      return heartsCollected;
   }

   public int GetHeartsSpent()
   {
      return heartsSpent;
   }
}