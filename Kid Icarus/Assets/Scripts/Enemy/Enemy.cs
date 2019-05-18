using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health values")]
    public int maxHealth;
    public int currentHealth;
    public Sound hit;
    public GameObject ouch;

    [Header("Damage to deal")]
    public float damage = 1.0f;

    [Header("Immunities")]
    public bool immuneToArrows;
    public bool immuneToHammer;

    [Header("Death values")]
    public bool isDead;
    public float deathTime;
    public Sound death;
    public float instantDeathTime = 0;

    [Header("Spawn this on destroy")]
    public GameObject spawnOnDeath;

    [Header("List of dropables")]
    public bool overrideWithShopItems = false;
    public GameObject[] drops;

    [Header("Screen wrapping")]
    public bool shouldWrap;

    // collider to turn off
    private Collider2D refCollider;
    private Animator refAnimator;
    private UtilityAudioManager refAudioManager;
    private ShopInfo refShopInfo;

	void Start ()
	{
		// set up health
		currentHealth = maxHealth;

		// get the collider
		refCollider = GetComponent<Collider2D>();

		// get the animator
		refAnimator = GetComponent<Animator>();

		// get the audio manager
		refAudioManager = GameObject.FindObjectOfType<UtilityAudioManager>();

        // get shop info in case an enemy needs to check for an appropriate item
        refShopInfo = GameObject.FindObjectOfType<ShopInfo>();
	}

    void Update()
    {
	    CheckHealth();

	    if (shouldWrap)
	    {
		    ScreenWrapping();
	    }

        // if the animator isn't null...
	    if (refAnimator != null)
	    {
            refAnimator.SetBool("isDead", isDead);
	    }
    }

	private void CheckHealth()
	{
		// if we're out of health
		if (currentHealth <= 0)
		{
			// disable the collider
			refCollider.enabled = false;

			// so this doesn't happen more than once, check if isDead is false
			if (isDead == false)
			{
				// invoke the gameobject destruction
				Invoke("Death", deathTime);
				isDead = true;
			}
		}
	}

	private void Death()
	{
        // get an item from the shop to spawn
        if (overrideWithShopItems)
        {
            Instantiate(refShopInfo.GetRandomAvailableItem(drops), transform.position, Quaternion.identity);
        }
        // drop something from the list of drops
        else if (drops.Length != 0)
        {
            refAudioManager.PlaySound(death.clip, death.volume);
            Instantiate(drops[Random.Range(0, drops.Length)], transform.position, Quaternion.identity);
        }

        if (spawnOnDeath != null)
		{
			Instantiate(spawnOnDeath, transform.position, Quaternion.identity);
		}

		Destroy(gameObject);
	}

	private void ScreenWrapping()
	{
		if (transform.position.x > 15.75f)
		{
			transform.position = new Vector2(-0.5f, transform.position.y);
		}

		if (transform.position.x < -0.75f)
		{
			transform.position = new Vector2(15.5f, transform.position.y);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		// when colliding with an arrow, decrease the health
		if (!immuneToArrows && other.CompareTag("Arrow") == true)
		{
			refAudioManager.PlaySound(hit.clip, hit.volume);
			currentHealth--;

			if (ouch != null)
			{
				Instantiate(ouch, other.transform.position, Quaternion.identity);
			}
		}

		// when colliding with a hammer, decrease the health
		if (!immuneToHammer && other.CompareTag("Hammer") == true)
		{
			refAudioManager.PlaySound(hit.clip, hit.volume);
			currentHealth -= 3;

			if (ouch != null)
			{
				Instantiate(ouch, transform.position, Quaternion.identity);
			}
		}

		// when colliding with the death floor, destroy the object
		if (other.CompareTag("InstantDeath") == true)
		{
            Invoke("Dest", instantDeathTime);
		}
	}

    private void Dest()
    {
        Destroy(gameObject);
    }
}
