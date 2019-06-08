using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomGeneration : MonoBehaviour
{
    private InfiniteGenerator refInfiniteGenerator;
    private SpawnOverride refSpawnOverride;

    [Header("Canvas to spawn objects under")]
    public GameObject canvas;

    [Header("List of enemies, used to replace the one in InfiniteGenerator")]
    public List<CustomEnemy> customEnemies = new List<CustomEnemy>();

    [Header("UI Prefabs")]
    public GameObject customEnemyUIPrefab;
    public GameObject customOverrideUIPrefab;
    public RectTransform defaultTransform;
    private float currX = 0, currY = 0;

    void Start()
    {
        refInfiniteGenerator = FindObjectOfType<InfiniteGenerator>();
        refSpawnOverride = FindObjectOfType<SpawnOverride>();

        CreateUI();
    }

    void Update()
    {
        for (int i = 0; i < customEnemies.Count; ++i)
        {
            UpdateCustomValues(customEnemies[i]);
        }
    }

    private void CreateUI()
    {
        CreateEnemyUI();
        CreateOverrideUI();
    }

    private void CreateEnemyUI()
    {
        bool nextRow = false;

        for (int i = 0; i < customEnemies.Count; ++i)
        {
            CustomEnemyRefs tmp = Instantiate(customEnemyUIPrefab, Vector2.zero, Quaternion.identity, canvas.transform).GetComponent<CustomEnemyRefs>();
            tmp.GetComponent<RectTransform>().localPosition = new Vector2(defaultTransform.localPosition.x + currX, defaultTransform.localPosition.y + currY);

            // set the enemy UI's name
            tmp.textDisplayName.text = customEnemies[i].dispName;

            // set the enemy UI's sprite
            if (customEnemies[i].enemy.obj.GetComponent<SpriteRenderer>() != null)
            {
                tmp.imageIcon.sprite = customEnemies[i].enemy.obj.GetComponent<SpriteRenderer>().sprite;
            }
            // hot fix for Rokman Hives not having a sprite, just have them use the sprites of the next enemy, the Rokman
            else
            {
                tmp.imageIcon.sprite = customEnemies[i + 1].enemy.obj.GetComponent<SpriteRenderer>().sprite;
            }

            // set the toggle value according to if we should spawn the enemy
            InitCustomValues(tmp, customEnemies[i]);

            // add the CustomEnemyRef to the list for later
            customEnemies[i].refs = tmp;

            // move the Y down for the next UI to spawn
            currY -= 40.0f;

            if (i > (customEnemies.Count / 2) - 1 && !nextRow)
            {
                currY = 0;
                currX = 70;
                nextRow = true;
            }
        }
    }

    private void CreateOverrideUI()
    {

    }

    private void InitCustomValues(CustomEnemyRefs refs, CustomEnemy en)
    {
        refs.toggleShouldSpawn.isOn = !en.enemy.shouldSpawn;
        refs.toggleOneAtATime.isOn = en.enemy.limitSpawns;
        refs.fieldDontSpawnBefore.text = en.enemy.dontSpawnBefore.ToString();
    }

    private void UpdateCustomValues(CustomEnemy en)
    {
        en.enemy.shouldSpawn = !en.refs.toggleShouldSpawn.isOn;
        en.enemy.limitSpawns = en.refs.toggleOneAtATime.isOn;
        en.enemy.dontSpawnBefore = System.Convert.ToInt32(en.refs.fieldDontSpawnBefore.text);
    }

    public void ApplyToGenerator()
    {
        List<EnemyToSpawn> tmp = new List<EnemyToSpawn>();

        for (int i = 0; i < customEnemies.Count; ++i)
        {
            tmp.Add(customEnemies[i].enemy);
        }

        refInfiniteGenerator.enemies = tmp.ToArray();
    }
}

[System.Serializable]
public class CustomEnemy
{
    public string dispName;
    [HideInInspector]
    public CustomEnemyRefs refs;
    public EnemyToSpawn enemy;
}