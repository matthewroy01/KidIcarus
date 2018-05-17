﻿using UnityEngine;

// tutorial by Brackeys: https://www.youtube.com/watch?v=B_Xp9pt8nRY
// edited by Matt Roy

public class LevelGenerator : MonoBehaviour
{
	[Header("Texture that represents the level layout")]
	public Texture2D[] maps;
	public string defaultName;
	[Header("List of prefabs and their corresponding colors")]
	public ColorToPrefab[] colorMappings;

	// private variables
	private bool makingCollider = false; // if we're making a collider currently
	private int offsetX = 0; // the current x offset, used to calculate the width and offset of the box collider
	private BoxCollider2D currentCollider = null; // the current collider we're changing the width and offset of
	private int num = 0; // the iterator used for instantiating each map
	private Transform currentParent;

	void Start ()
	{
		// generate the level
		for (num = 0; num < maps.Length; num++)
		{
			// create a new gameobject that will serve as a parent for all the blocks
			currentParent = new GameObject(defaultName + num).transform;
			// read the texture, generate blocks, and generate collision
			GenerateLevel();

			// move the level that was just created over a bit
			currentParent.position = new Vector2(-17 * num, 0);
		}
	}

	void GenerateLevel()
	{
		int x, y;

		// loop through all pixels in map
		for (y = 0; y < maps[num].height; ++y)
		{
			for(x = 0; x < maps[num].width; ++x)
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
		Color32 pixelColor = maps[num].GetPixel(x, y);

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
				tmpPrefab = Instantiate(colorMapping.prefab, position, Quaternion.identity, currentParent);

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