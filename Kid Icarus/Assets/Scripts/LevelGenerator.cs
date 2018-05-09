using UnityEngine;

// tutorial by Brackeys: https://www.youtube.com/watch?v=B_Xp9pt8nRY
// edited by Matt Roy

public class LevelGenerator : MonoBehaviour
{
	[Header("Texture that represents the level layout")]
	public Texture2D map;
	[Header("List of prefabs and their corresponding colors")]
	public ColorToPrefab[] colorMappings;

	// private variables
	private bool makingCollider = false; // if we're making a collider currently
	private int offsetX = 0; // the current x offset, used to calculate the width and offset of the box collider
	private BoxCollider2D currentCollider = null; // the current collider we're changing the width and offset of

	void Start ()
	{
		// generate the level
		GenerateLevel();
	}

	void GenerateLevel()
	{
		int x, y;

		// loop through all pixels in map
		for (y = 0; y < map.height; ++y)
		{
			for(x = 0; x < map.width; ++x)
			{
				// generate the tile
				GenerateTile(x, y);
			}

			// once we've reached the end of the row, stop creating any colliders and set the offset back to 0
			makingCollider = false;
			offsetX = 0;
		}
	}

	void GenerateTile(int x, int y)
	{
		Color32 pixelColor = map.GetPixel(x, y);

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
				// instantiate the tiel
				Vector2 position = new Vector2(x,y);
				tmpPrefab = Instantiate(colorMapping.prefab, position, Quaternion.identity, transform);

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
public struct ColorToPrefab
{
	public Color32 color;
	public GameObject prefab;
}