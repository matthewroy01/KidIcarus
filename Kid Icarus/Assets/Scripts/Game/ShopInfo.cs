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

    public GameObject GetRandomAvailableItem(GameObject[] exclusions)
    {
        int failsafe = 50;

        // keep a failsafe in case there are no available items
        while (failsafe > 0)
        {
            int rand = Random.Range(0, shopItems.Length);
            if (shopItems[rand].isAvailable)
            {
                bool excluded = false;

                // make sure this item isn't being excluded
                for (int i = 0; i < exclusions.Length; ++i)
                {
                    if (shopItems[rand].obj == exclusions[i])
                    {
                        excluded = true;
                    }
                }

                // if the item wasn't specifically excluded
                if (!excluded)
                {
                    // return item
                    return shopItems[rand].obj;
                }
            }
        }

        if (shopItems.Length > 0)
        {
            Debug.LogWarning("ShopInfo could not find an available item, and gave up looking for one. Returning the first item from the list.");
            return shopItems[0].obj;
        }
        else
        {
            Debug.LogWarning("ShopInfo could not find an available item, and gave up looking for one. Returning null since the list of items is empty");
            return null;
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