using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInfo : MonoBehaviour
{
	public ShopItem[] shopItems;
	public ShopItem emptyItem;

	public int getItemCost(string name)
	{
		for (int i = 0; i < shopItems.Length; ++i)
		{
			// look for the item with the matching name
			if (shopItems[i].name == name)
			{
				return shopItems[i].cost;
			}
		}

		// otherwise return the maximum value for an integer
		return int.MaxValue;
	}

	public ShopItem getItem(string name)
	{
		for (int i = 0; i < shopItems.Length; ++i)
		{
			// look for the item with the matching name
			if (shopItems[i].name == name)
			{
				return shopItems[i];
			}
		}

		// otherwise return null
		Debug.LogWarning("Item of name " + name + " does not exist. Returning an empty item instead.");
		return emptyItem;
	}

	public void SetAvailability(bool available, string name)
	{
		for (int i = 0; i < shopItems.Length; ++i)
		{
			// look for the item with the matching name
			if (shopItems[i].name == name)
			{
				// set its availabilty accordingly
				shopItems[i].isAvailable = available;
			}
		}
	}
}

[System.Serializable]
public struct ShopItem
{
	public string name;
	public GameObject obj;
	[TextArea(3, 10)]
	public string description;
	public int cost;
	public bool isAvailable;
}