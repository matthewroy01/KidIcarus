using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteGenerator : MonoBehaviour
{
	[Header("List of possible levels")]
	public List<Level> levels;

	[Header("Maximum value for keeping track of priority")]
	public int lowestPriority; // 0 is highest priority, -1 means that level will never be generated again

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
		levels[0].priorityNum = -1;
	}

	void Update ()
	{
		if (isGenerating == true)
		{
			// do something, idk
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
		priorityNum = 0;
		height = myHeight;
	}

	public GameObject obj;
	public int priorityNum;
	public int height;
}