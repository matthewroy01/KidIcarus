using UnityEngine;

// tutorial by Brackeys: https://www.youtube.com/watch?v=B_Xp9pt8nRY

public class LevelGenerator : MonoBehaviour
{
	public Texture2D map;
	public ColorToPrefab[] colorMappings;

	void Start ()
	{
		GenerateLevel();
	}

	void GenerateLevel()
	{
		int x, y;

		// loop through all pixels in map
		for (x = 0; x < map.width; ++x)
		{
			for(y = 0; y < map.height; ++y)
			{
				GenerateTile(x, y);
			}
		}
	}

	void GenerateTile(int x, int y)
	{
		Color32 pixelColor = map.GetPixel(x, y);

		if (pixelColor.a == 0)
		{
			// the pixel is transparent, let's ignore it
			return;
		}

		Debug.Log(pixelColor.r + " " + pixelColor.g + " " + pixelColor.b + " " + pixelColor.a);

		foreach (ColorToPrefab colorMapping in colorMappings)
		{
			if (colorMapping.color.Equals(pixelColor))
			{
				Debug.Log(colorMapping.prefab.name);
				Vector2 position = new Vector2(x,y);
				Instantiate(colorMapping.prefab, position, Quaternion.identity, transform);
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