using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteGenerator : MonoBehaviour
{
    [Header("List of possible levels")]
    public List<Level> levels;
    private List<int> candidates;
    public bool randomFlipEnabled;

    [Header ("List of possible shops")]
    public List<Level> shops;
    public int waitToSpawnEnemies;
    private int waitToSpawnEnemiesCount = 0;

    [Header("List of enemies")]
    public int enemiesToSpawn;
    public EnemyToSpawn[] enemies;

    [Header("List of treasures")]
    public int spawnTreasureEveryMax;
    public int spawnTreasureEveryMin;
    private int treasureSpawnCounter;
    public GameObject[] treasures;

    [Header("List of props")]
    [Range(0.0f, 100.0f)]
    public float propSpawnProbability;
    public float propZ;
    private float propX, propY;
    public Color propTint;
    public GameObject[] props;

    [Header("Maximum value for keeping track of priority")]
    public int maxAge;

    [Header("Spawn values")]
    public int currentY; // Y value to spawn a new level at
    public int defaultX; // X value to spawn a new level at
    public int roomCount; // how many rooms have been spawned
    public int spawnShopEvery; // spawn a shop every this many levels

    // private variables
    private bool isGenerating; // if the game has begun generating yet
    private Transform refPlayer; // a reference to the player's transform to see how high they've made it for the purposes of spawning more levels
    private PlayerCollision refPlayerCollision;

    void Start()
    {
	    candidates = new List<int>();
	    refPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        refPlayerCollision = GameObject.FindObjectOfType<PlayerCollision>();

        treasureSpawnCounter = Random.Range(spawnTreasureEveryMin, spawnTreasureEveryMax);
    }

    public void BeginGeneration()
    {
	    // set is generating to true to begin checking the player's height
	    isGenerating = true;

	    // create the default starting point
	    GameObject firstLevel = Instantiate(levels[0].obj, new Vector2(defaultX, currentY), Quaternion.identity);
        SpawnProps(levels[0], firstLevel.transform);
	    levels[0].age = -1;
	    currentY = levels[0].height;

	    // increase room count
	    roomCount++;
    }

    void Update ()
    {
	    if (isGenerating == true && refPlayer.position.y > currentY - 16)
	    {
		    // spawn a shop every certain number of rooms
		    if (roomCount % spawnShopEvery == 0)
		    {
                SpawnShops();
		    }
		    // spawn rooms
		    else
		    {
                // spawning enemies and treasures also happens here
                SpawnRooms();
		    }
	    }
    }

    private void SpawnShops()
    {
        // instantiate one of the levels that was chosen
        int randomChoice = Random.Range(0, shops.Count);

        GameObject tmp = Instantiate(shops[randomChoice].obj, new Vector2(defaultX, currentY), Quaternion.identity);

        // add this component so the levels will destroy themselves after reaching a certain point below the player
        tmp.AddComponent<MiscDestroyBelowThreshold>();

        // increase the current Y for checking for the next instantiation
        currentY += shops[randomChoice].height;

        // increase the age of the level that was chosen
        levels[randomChoice].age = maxAge;

        // reset room count
        roomCount++;

        // prevent enemies from spawning if a shop spawned
        waitToSpawnEnemiesCount = waitToSpawnEnemies;
    }

    private void SpawnRooms()
    {
        // update list of potential rooms to spawn
        CheckCandidates();

        // instantiate one of the levels that was chosen
        int randomChoice = candidates[Random.Range(0, candidates.Count)];

        GameObject tmp = Instantiate(levels[randomChoice].obj, new Vector2(defaultX, currentY), Quaternion.identity);

        // add this component so the levels will destroy themselves after reaching a certain point below the player
        MiscDestroyBelowThreshold destThresh = tmp.AddComponent<MiscDestroyBelowThreshold>();
        destThresh.threshold += levels[randomChoice].height - 16;

        // spawn props in this level
        SpawnProps(levels[randomChoice], tmp.transform);

        if (randomFlipEnabled && Random.Range(0, 2) == 1)
        {
            tmp.transform.rotation = Quaternion.Euler(0, 180, 0);
            tmp.transform.position = new Vector2(15, currentY);
        }

        if (waitToSpawnEnemiesCount == 0)
        {
            // spawn enemies in this part of the level
            SpawnEnemies(levels[randomChoice].height, levels[randomChoice].width);
        }
        else
        {
            waitToSpawnEnemiesCount--;
        }

        // try to spawn treasures
        SpawnTreasure(levels[randomChoice].height, levels[randomChoice].width);

        // increase the current Y for checking for the next instantiation
        currentY += levels[randomChoice].height;

        // increase the age of the level that was chosen
        levels[randomChoice].age = maxAge;

        // increment number of rooms that has been spawned
        roomCount++;
    }

    private void SpawnProps(Level level, Transform parent)
    {
        // spawn props at the given positions
        for (int i = 0; i < level.propPositions.Length; ++i)
        {
            if (Random.Range(0, 100) <= propSpawnProbability)
            {
                propX = defaultX + level.propPositions[i].x;
                propY = currentY + level.propPositions[i].y;

                SpriteRenderer tmp = Instantiate(props[Random.Range(0, props.Length)], new Vector3(propX, propY - propZ, propZ), Quaternion.identity, parent).GetComponent<SpriteRenderer>();
                tmp.color = propTint;
            }
        }
    }

    public void AddLevel(Level newLevel)
    {
	    levels.Add(newLevel);
    }

	public void AddShop(Level newLevel)
	{
		shops.Add(newLevel);
	}

    private void CheckCandidates()
    {
	    candidates.Clear();

	    for (int i = 1; i < levels.Count; ++i)
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

	    List<int> limitedSpawn;
	    limitedSpawn = new List<int>();

	    int tmp;

	    if (enemiesToSpawn != 0 && enemies.Length != 0)
	    {
		    for (int i = 0; i < enemiesToSpawn; ++i)
		    {
			    // keep checking until there are no repeats
			    do
			    {
				    tmp = Random.Range(0, enemies.Length);
			    }
			    while(CheckForRepeats(tmp, limitedSpawn) == true || enemies[tmp].dontSpawnBefore > refPlayerCollision.getCurrentMeters());

			    Instantiate(enemies[tmp].obj, new Vector2(randX, randY), Quaternion.identity);

			    // if the enemy that was spawned is linked to something else, prevent that thing from spawning
			    if (enemies[tmp].linkedTo >= 0)
			    {
				    // Debug.Log("Enemy at " + tmp + " prevent enemy at " + enemies[tmp].linkedTo + " from spawning.");
				    limitedSpawn.Add(enemies[tmp].linkedTo);
			    }

                // if spawns are limited, add it to the list so we don't spawn more
                if (enemies[tmp].limitSpawns == true)
			    {
				    limitedSpawn.Add(tmp);
			    }
		    }
	    }
    }

    private void SpawnTreasure(int height, int width)
    {
        if (treasureSpawnCounter <= 0)
        {
            int randX, randY, tmp;
            randX = Random.Range(defaultX, defaultX + width);
            randY = Random.Range(currentY, currentY + height);
            tmp = Random.Range(0, treasures.Length);

            Instantiate(treasures[tmp], new Vector2(randX, randY), transform.rotation);

            treasureSpawnCounter = Random.Range(spawnTreasureEveryMin, spawnTreasureEveryMax);
        }
        else
        {
            treasureSpawnCounter--;
        }
    }

    private bool CheckForRepeats(int rand, List<int> list)
    {
	    // if for some reason, all enemies have been used, just return false
	    if (list.Count == enemies.Length)
	    {
		    return false;
	    }

	    // check the list to see if our random number has already been used
	    for (int i = 0; i < list.Count; ++i)
	    {
		    if (list[i] == rand)
		    {
			    return true;
		    }
	    }
	    return false;
    }
}

[System.Serializable]
public class Level
{
    public Level(GameObject myObj, int myHeight, int myWidth, Vector2[] myPropPositions)
    {
	    obj = myObj;
	    age = 0;
	    height = myHeight;
	    width = myWidth;
        propPositions = myPropPositions;
    }

    public GameObject obj;
    public int age;
    public int height, width;
    public Vector2[] propPositions;
}

[System.Serializable]
public class EnemyToSpawn
{
    public GameObject obj;
    public bool limitSpawns;
    public int linkedTo = -1;
    public int dontSpawnBefore = 0;
}