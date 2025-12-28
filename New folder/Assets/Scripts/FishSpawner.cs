using UnityEngine;
using System.Collections;

public class FishSpawner : MonoBehaviour
{
    [Header("Fish Prefabs")]
    public GameObject[] tinyFishPrefabs;
    public GameObject[] smallFishPrefabs;
    public GameObject[] mediumFishPrefabs;
    public GameObject[] largeFishPrefabs;
    public GameObject[] giantFishPrefabs;
    public GameObject[] bossFishPrefabs;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public Vector3 spawnAreaSize = new Vector3(40, 20, 40);
    public float minSpawnDistance = 10f;

    [Header("Population Control")]
    public int maxTinyFish = 30;
    public int maxSmallFish = 20;
    public int maxMediumFish = 10;
    public int maxLargeFish = 5;
    public int maxGiantFish = 2;

    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        StartCoroutine(ContinuousSpawning());
    }

    public void SpawnWave(int level)
    {
        int tinyCount = Mathf.Min(10 + level * 2, maxTinyFish);
        int smallCount = Mathf.Min(5 + level, maxSmallFish);
        int mediumCount = Mathf.Min(2 + level / 2, maxMediumFish);
        int largeCount = level >= 3 ? Mathf.Min(level / 2, maxLargeFish) : 0;
        int giantCount = level >= 5 ? 1 : 0;

        SpawnFishOfType(tinyFishPrefabs, tinyCount);
        SpawnFishOfType(smallFishPrefabs, smallCount);
        SpawnFishOfType(mediumFishPrefabs, mediumCount);
        SpawnFishOfType(largeFishPrefabs, largeCount);
        SpawnFishOfType(giantFishPrefabs, giantCount);

        if (level % 5 == 0)
        {
            SpawnBoss();
        }
    }

    void SpawnFishOfType(GameObject[] prefabs, int count)
    {
        if (prefabs == null || prefabs.Length == 0) return;
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = GetValidSpawnPosition();
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            if (prefab == null) continue;

            GameObject fish = Instantiate(prefab, spawnPos, Random.rotation);
            fish.transform.SetParent(transform);
        }
    }

    Vector3 GetValidSpawnPosition()
    {
        Vector3 pos;
        int attempts = 0;

        do
        {
            pos = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );
            attempts++;
        }
        while (player != null && Vector3.Distance(pos, player.position) < minSpawnDistance && attempts < 50);

        return pos;
    }

    void SpawnBoss()
    {
        Vector3 spawnPos = GetValidSpawnPosition();
        spawnPos = new Vector3(spawnPos.x, 0, spawnPos.z);

        if (bossFishPrefabs == null || bossFishPrefabs.Length == 0) return;
        GameObject boss = Instantiate(bossFishPrefabs[Random.Range(0, bossFishPrefabs.Length)], spawnPos, Quaternion.identity);
        boss.AddComponent<BossIndicator>();
    }

    IEnumerator ContinuousSpawning()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            int tinyCount = CountFishOfType(BrickFish.FishType.Tiny);
            int smallCount = CountFishOfType(BrickFish.FishType.Small);
            int mediumCount = CountFishOfType(BrickFish.FishType.Medium);

            if (tinyCount < maxTinyFish / 2)
                SpawnFishOfType(tinyFishPrefabs, 5);

            if (smallCount < maxSmallFish / 2)
                SpawnFishOfType(smallFishPrefabs, 3);

            if (mediumCount < maxMediumFish / 2)
                SpawnFishOfType(mediumFishPrefabs, 2);
        }
    }

    int CountFishOfType(BrickFish.FishType type)
    {
        BrickFish[] allFish = FindObjectsOfType<BrickFish>();
        return System.Array.FindAll(allFish, f => f.fishType == type).Length;
    }
}
