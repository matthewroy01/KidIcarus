using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInfo : MonoBehaviour
{
	public ShopItem[] shopItems;

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
		Debug.LogError("Item of name " + name + " does not exist. Returning the first item in the array instead.");
		return shopItems[0];
	}
}

[System.Serializable]
public struct ShopItem
{
	public GameObject obj;
	public string name;
	[TextArea(3, 10)]
	public string description;
	public int cost;
}