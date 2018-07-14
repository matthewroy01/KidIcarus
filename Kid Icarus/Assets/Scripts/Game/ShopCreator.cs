using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCreator : MonoBehaviour
{
	private ShopInfo refShopInfo;

	[Header("List of items we can spawn")]
	public ShopItem[] possibleItems;
	public List<int> alreadyUsed;

	[Header("Length of this array determines the number of items to spawn in the shop")]
	public Vector2[] spawnLocations;
	private int spawnCounter = 0;

	[Header("Must spawn this when cursed")]
	public int curseID;
	private PlayerCollision refPlayerCollision;
	private bool alreadySpawned = false;

	[Header("Price tag prefab")]
	public TextMesh priceTag;
	public Vector2 priceTagOffset;

	[Header("Text area prefab")]
	public TextArea textArea;
	public Vector2 textAreaOffset;
	
	void Start ()
	{
		// find the shop info
		refShopInfo = GameObject.FindObjectOfType<ShopInfo>();

		refPlayerCollision = GameObject.FindObjectOfType<PlayerCollision>();

		possibleItems = refShopInfo.shopItems;

		// initialize the list of already used numbers
		alreadyUsed = new List<int>();

		Invoke("RemoveCollider", 0.5f);

		// spawn items in the shop
		SpawnItems();
	}

	void SpawnItems()
	{
		for (int i = 0; i < spawnLocations.Length; ++i)
		{
			int toSpawn;

			// if cursed, spawn only the first aid hit
			if (refPlayerCollision.cursed == true && alreadySpawned == false)
			{
				toSpawn = curseID;
				alreadySpawned = true;
			}
			// otherwise, work as normal
			else
			{
				// prematurely add unavailable items to the "already used" list
				RemoveUnavailableItems();

				do
				{
					// if the lengths are equal, then there are no new items to spawn
					if (alreadyUsed.Count == possibleItems.Length)
					{
						Debug.LogWarning("Number of items to spawn in the shop was greater than the number of possible items.");
						return;
					}

					// randomly select a number
					toSpawn = Random.Range(0, possibleItems.Length);
				}
				// try again if the number selected was already spawned
				while(CheckIfAlreadyUsed(toSpawn));
			}

			// instantiate the item
			GameObject tmp = Instantiate(possibleItems[toSpawn].obj, (Vector2)transform.position + spawnLocations[spawnCounter], transform.rotation);
			spawnCounter++;

			// price tag
			{
				// instantiate a price tag over the item
				TextMesh tmpTM = Instantiate(priceTag, (Vector2)tmp.transform.position + priceTagOffset, transform.rotation);
				// display the cost
				tmpTM.text = possibleItems[toSpawn].cost.ToString();
				// set the price tag as a child of the item
				tmpTM.transform.parent = tmp.transform;
			}

			// text area
			{
				// instantiate a text area under the item
				TextArea tmpTA = Instantiate(textArea, (Vector2)tmp.transform.position + textAreaOffset, transform.rotation);
				// set the text area's text
				tmpTA.text = possibleItems[toSpawn].name + "\n" + possibleItems[toSpawn].description;
				// set the text area as a child of the item
				tmpTA.transform.parent = tmp.transform;
			}

			// change the name to differentiate it between items that can be picked up and bought
			tmp.name = possibleItems[toSpawn].name;

			// keep track of the items already used
			alreadyUsed.Add(toSpawn);
		}
	}

	bool CheckIfAlreadyUsed(int toCheck)
	{
		for (int i = 0; i < alreadyUsed.Count; ++i)
		{
			// if we've already used this item, return true
			if (toCheck == alreadyUsed[i])
			{
				return true;
			}
		}
		// otherwise we're good
		return false;
	}

	private void RemoveUnavailableItems()
	{
		for (int i = 0; i < possibleItems.Length; ++i)
		{
			// if the item isn't available, prematurely add it to the "alreadyUsed" list
			if (possibleItems[i].isAvailable == false)
			{
				alreadyUsed.Add(i);
			}
		}
	}

	void RemoveCollider()
	{
		if (GetComponent<Collider2D>() != null)
		{
			// remove the collider from the level generator...
			GetComponent<Collider2D>().enabled = false;
		}
	}
}