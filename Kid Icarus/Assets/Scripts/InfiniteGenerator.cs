using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteGenerator : MonoBehaviour
{
	[Header("List of possible levels")]
	public List<Level> levels;
	private List<int> candidates;

	[Header("List of enemies")]
	public GameObject[] enemies;
	public int enemiesToSpawn;

	[Header("Maximum value for keeping track of priority")]
	public int maxAge;

	[Header("Spawn values")]
	public int currentY; // Y value to spawn a new level at
	public int defaultX; // X value to spawn a new level at

	// private variables
	private bool isGenerating; // if the game has begun generating yet
	private Transform refPlayer; // a reference to the player's transform to see how high they've made it for the purposes of spawning more levels

	void Start()
	{
		candidates = new List<int>();
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
			CheckCandidates();

			// instantiate one of the levels that was chosen
			int randomChoice = candidates[Random.Range(0, candidates.Count)];

			Instantiate(levels[randomChoice].obj, new Vector2(defaultX, currentY), Quaternion.identity);

			// spawn enemies in this part of the level
			SpawnEnemies(levels[randomChoice].height, levels[randomChoice].width);

			// increase the current Y for checking for the next instantiation
			currentY += levels[randomChoice].height;

			// increase the age of the level that was chosen
			levels[randomChoice].age = maxAge;
		}
	}

	public void AddLevel(Level newLevel)
	{
		levels.Add(newLevel);
	}

	private void CheckCandidates()
	{
		candidates.Clear();

		for (int i = 0; i < levels.Count; ++i)
		{
			// if the level hasn't been used, add it to the list of potential candidates
			if (levels[i].age == 0)
			{
				candidates.Add(i);
			}
			// otherwise, decrease the age
			else if (levels[i].age > 0)
			{
				--levels[i].age;
			}
		}
	}

	private void SpawnEnemies(int height, int width)
	{
		int randX, randY;
		randX = Random.Range(defaultX, defaultX + width);
		randY = Random.Range(currentY, currentY + height);

		if (enemiesToSpawn != 0 && enemies.Length != 0)
		{
			for (int i = 0; i < enemiesToSpawn; ++i)
			{
				Instantiate(enemies[Random.Range(0, enemies.Length)], new Vector2(randX, randY), Quaternion.identity);
			}
		}
	}
}

[System.Serializable]
public class Level
{
	public Level(GameObject myObj, int myHeight, int myWidth)
	{
		obj = myObj;
		age = 0;
		height = myHeight;
		width = myWidth;
	}

	public GameObject obj;
	public int age;
	public int height, width;
}