using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnEnemies : MonoBehaviour
{
	[Header("Spawning")]
	public GameObject whatToSpawn;
	public int maxSpawns;
	public float spawnRate;
	private float spawnCount;
	public float spawnCooldown;
	public float minYDistanceToPlayer;
	private List<Enemy> spawns;

	[Header("If we're stuck in a wall")]
	public LayerMask wallLayer;

	[Header("Spawn this on destroy")]
	public GameObject destroyPrefab;

	private Enemy refEnemy;
	private Transform refPlayer;

	void Start ()
	{
		spawns = new List<Enemy>();

		// enemy status
		refEnemy = GetComponent<Enemy>();

		// the player
		refPlayer = GameObject.FindGameObjectWithTag("Player").transform;

		// if the object spawns in a wall, destroy it
		if (Physics2D.OverlapCircle(transform.position, 0.2f, wallLayer))
		{
			Destroy(gameObject);
		}

		StartCoroutine("SpawnEnemies");
	}

	void Update()
	{
		// check the status of the spawns
		CheckSpawns();
	}

	private IEnumerator SpawnEnemies()
	{
		// keep going until this object is dead
		while(refEnemy.isDead == false)
		{
			// if there are some spawns missing, spawn more
			if (spawns.Count < maxSpawns && transform.position.y - refPlayer.position.y < minYDistanceToPlayer)
			{
				spawns.Add(Instantiate(whatToSpawn, transform.position, Quaternion.identity).GetComponent<Enemy>());
			}

			if(spawnCount >= maxSpawns)
			{
				spawnCount = 0;
				yield return new WaitForSeconds(spawnCooldown);
			}
			else
			{
				spawnCount++;
				yield return new WaitForSeconds(spawnRate);
			}
		}
	}

	private void CheckSpawns()
	{
		// look for null spawns in the list
		for (int i = 0; i < spawns.Count; ++i)
		{
			if (spawns[i].isDead == true || spawns[i] == null)
			{
				spawns.Remove(spawns[i]);
			}
		}
	}

	void OnDestroy()
	{
		Instantiate(destroyPrefab, transform.position, Quaternion.identity);
	}
}
