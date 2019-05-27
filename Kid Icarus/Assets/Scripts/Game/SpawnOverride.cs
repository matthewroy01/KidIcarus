using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOverride : MonoBehaviour
{
    public bool doOverride;
    public SpOv[] overrides;
    public int currentOverrideID = 0;

    private InfiniteGenerator refInfiniteGenerator;
    private PlayerCollision refPlayerCollision;

    private void Start()
    {
        refInfiniteGenerator = FindObjectOfType<InfiniteGenerator>();
        refPlayerCollision = FindObjectOfType<PlayerCollision>();

        if (doOverride)
        {
            Debug.LogWarning("SpawnOverride script is overriding the number of enemies to spawn in the Infinite Generator.");
        }
    }

    private void Update()
    {
        if (doOverride && currentOverrideID != overrides.Length)
        {
            OverrideSpawns();
        }
    }

    private void OverrideSpawns()
    {
        if (refPlayerCollision.getCurrentMeters() >= overrides[currentOverrideID].meters)
        {
            refInfiniteGenerator.enemiesToSpawn = overrides[currentOverrideID].numToSpawn;
            currentOverrideID++;
        }
    }
}

[System.Serializable]
public struct SpOv
{
    public int meters;
    public int numToSpawn;
}