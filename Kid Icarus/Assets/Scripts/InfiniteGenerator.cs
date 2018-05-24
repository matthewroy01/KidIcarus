using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteGenerator : MonoBehaviour
{
	[Header("List of possible levels")]
	public List<Level> levels;

	[Header("Maximum value for keeping track of priority")]
//	private int lowestPriority = 0; // 0 is lowest priority, -1 means that level will never be generated again
//	public int highestPriority;

	[Header("Spawn values")]
	public int currentY; // Y value to spawn a new level at
	public int defaultX; // X value to spawn a new level at

	// private variables
	private bool isGenerating; // if the game has begun generating yet
	private Transform refPlayer; // a reference to the player's transform to see how high they've made it for the purposes of spawning more levels

	void Start()
	{
		refPlayer = GameObject.FindGameObjectWithTag("Player").transform;
	}

	public void BeginGeneration ()
	{
		// set is generating to true to begin checking the player's height
		isGenerating = true;

		// create the default starting point
		Instantiate(levels[0].obj, new Vector2(defaultX, currentY), Quaternion.identity);
		levels[0].age = -1;
		currentY = levels[0].height;
	}

	void Update ()
	{
		if (isGenerating == true && refPlayer.position.y > currentY - 16)
		{
			int tmpAge = 0;

			// look for the oldest level
			for (int i = 0; i < levels.Count; ++i)
			{
				if (levels[i].age >= tmpAge)
				{
					tmpAge = i;
				}
			}

			// increase the age of the levels that weren't chosen and reset the age of the one that was
			for (int i = 0; i < levels.Count; ++i)
			{
				if (i != tmpAge)
				{
					levels[i].age++;
				}
				else
				{
					levels[i].age = 0;
				}
			}

			// instantiate the level that was chosen
			Instantiate(levels[tmpAge].obj, new Vector2(defaultX, currentY), Quaternion.identity);

			// increase the current Y for checking for the next instantiation
			currentY += levels[tmpAge].height;
		}
	}

	public void AddLevel(Level newLevel)
	{
		levels.Add(newLevel);
	}
}

[System.Serializable]
public class Level
{
	public Level(GameObject myObj, int myHeight)
	{
		obj = myObj;
		age = 0;
		height = myHeight;
	}

	public GameObject obj;
	public int age;
	public int height;
}