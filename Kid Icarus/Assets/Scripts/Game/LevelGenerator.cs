using System.Collections.Generic;
using UnityEngine;

// tutorial by Brackeys: https://www.youtube.com/watch?v=B_Xp9pt8nRY
// edited by Matt Roy

public class LevelGenerator : MonoBehaviour
{
	[Header("Texture that represents the level layout")]
	public Map[] maps;
	public string defaultName;
	[Header("Texture that represents shop layout")]
	public Map[] shopMaps;
	public string defaultShopName;
	[Header("List of prefabs and their corresponding colors")]
	public ColorToPrefab[] colorMappings;
	[Header("Safe zone prefab")]
	public GameObject safeZone;
	[Header("Invisible wall prefab")]
	public GameObject invisibleWall;

	// private variables
	private bool makingCollider = false; // if we're making a collider currently
	private int offsetX = 0; // the current x offset, used to calculate the width and offset of the box collider
	private BoxCollider2D currentCollider = null; // the current collider we're changing the width and offset of
//	private int num = 0; // the iterator used for instantiating each map
	private Transform currentParent; // the current parent is where all blocks will be instantiated to as children
	private InfiniteGenerator refInfiniteGenerator; // a reference to the infinite generator which holds a list of all the levels
	private int offsetXDebug = 0; // another X offset used to instantiate each level off screen for debugging purposes
    private List<Vector2> propPlacement = new List<Vector2>(); // a temporary list of Vector2 to keep track of where props can be spawned

	void Start ()
	{
		// find the infinite generator, this is where we will store our level objects for gameplay later
		refInfiniteGenerator = FindObjectOfType<InfiniteGenerator>();

		// loop through each map
		LoopMaps();

		// once we're done, tell the infinite generator to start making levels
		refInfiniteGenerator.BeginGeneration();
	}

	void LoopMaps()
	{
		// generate the level
		for (int num = 0; num < maps.Length; num++)
		{
			// create a new gameobject that will serve as a parent for all the blocks
			currentParent = new GameObject(defaultName + num).transform;
			// read the texture, generate blocks, and generate collision
			GenerateLevel(num, maps, false);

			// move the level that was just created over a bit
			currentParent.position = new Vector2(offsetXDebug, -100);
			offsetXDebug += maps[num].main.width + 1;

			// add the current parent to the infinite generator's list of potential levels
			refInfiniteGenerator.AddLevel(new Level(currentParent.gameObject, maps[num].main.height, maps[num].main.width, propPlacement.ToArray()));

            // clear prop placement list
            propPlacement.Clear();
		}

		// generate shops
		for (int num = 0; num < shopMaps.Length; num++)
		{
			// create a new gameobject that will serve as a parent for all the blocks
			currentParent = new GameObject(defaultShopName + num).transform;
			// read the texture, generate blocks, and generate collision
			GenerateLevel(num, shopMaps, true);

			// move the level that was just created over a bit
			currentParent.position = new Vector2(offsetXDebug, -100);
			offsetXDebug += shopMaps[num].main.width + 1;

			// add the current parent to the infinite generator's list of potential levels
			refInfiniteGenerator.AddShop(new Level(currentParent.gameObject, shopMaps[num].main.height, shopMaps[num].main.width, propPlacement.ToArray()));

            // clear prop placement list
            propPlacement.Clear();
		}
	}

	void GenerateLevel(int num, Map[] list, bool safe)
	{
		int x = 0, y = 0;

		// loop through all pixels in map
		for (y = 0; y < maps[num].main.height; ++y)
		{
			for(x = 0; x < maps[num].main.width; ++x)
			{
				// generate the tile
				GenerateTile(list, num, x, y);
			}

			// once we've reached the end of the row, stop creating any colliders and set the offset back to 0
			makingCollider = false;
			offsetX = 0;
		}

		if (safe == true)
		{
			GameObject tmp = Instantiate(safeZone, new Vector2((x / 2.0f) - 0.5f, (y / 2.0f) - 0.5f), transform.rotation);
			tmp.transform.parent = currentParent;
			tmp.transform.localScale = new Vector3(maps[num].main.width, maps[num].main.height, 1.0f);
		}
	}

	void GenerateTile(Map[] list, int num, int x, int y)
	{
		Color32 pixelColor = list[num].main.GetPixel(x, y);

        // add position to list of potential prop placements if the props texture isn't null and the pixel isn't transparent
        if (list[num].props != null)
        {
            Color32 pixelColorProps = list[num].props.GetPixel(x, y);

            // TEMPORARY CODE FOR PROPS, may want to make props only spawn on specific tile types
            if (pixelColorProps.a != 0)
            {
                propPlacement.Add(new Vector2(x, y));
            }
        }

		// if the pixel's color is completely transparent
		if (pixelColor.a == 0)
		{
			// ignore it
			makingCollider = false;
			return;
		}

		// for each tile type, instantiate the correct tile
		foreach (ColorToPrefab colorMapping in colorMappings)
		{
			GameObject tmpPrefab;

			// if the color is equal to the pixel's color
			if (colorMapping.color.Equals(pixelColor))
			{
				// instantiate the tile
				Vector2 position = new Vector2(x,y);
				tmpPrefab = Instantiate(colorMapping.prefab, position, Quaternion.identity, currentParent);

				// if we're at either end, spawn an invisible wall opposite to it
				if (x == 0)
				{
					Instantiate(invisibleWall, new Vector2(16, position.y), Quaternion.identity, currentParent);
				}

				if (x == 15)
				{
					Instantiate(invisibleWall, new Vector2(-1, position.y), Quaternion.identity, currentParent);
				}

				// if we're not already making a collider
				if (makingCollider == false)
				{
					// start making a collider
					makingCollider = true;
					currentCollider = tmpPrefab.AddComponent<BoxCollider2D>();
					// set the offset to 0
					offsetX = 0;
				}

				// change the size of the collider
				currentCollider.size = new Vector2(offsetX + 1, 1);
				currentCollider.offset = new Vector2(0.5f * offsetX, 0);

				// increment the offset
				offsetX++;
			}
		}
	}
}

[System.Serializable]
public struct Map
{
    public Texture2D main;
    public Texture2D props;
}

[System.Serializable]
public struct ColorToPrefab
{
	public Color32 color;
	public GameObject prefab;
}