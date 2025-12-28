using System.Collections;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    public GameObject[] brickPrefabs;
    public GameObject[] enemyStructurePrefabs;
    public Transform[] spawnPoints;

    public int maxBricksInScene = 50;
    public int maxEnemiesInScene = 10;

    public float brickSpawnInterval = 2f;
    public float enemySpawnInterval = 10f;

    void Start()
    {
        StartCoroutine(SpawnBricks());
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnBricks()
    {
        while (true)
        {
            int current = GameObject.FindGameObjectsWithTag("Brick").Length;
            if (current < maxBricksInScene && brickPrefabs != null && brickPrefabs.Length > 0)
            {
                SpawnRandomBrick();
            }
            yield return new WaitForSeconds(brickSpawnInterval);
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            int current = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (current < maxEnemiesInScene && enemyStructurePrefabs != null && enemyStructurePrefabs.Length > 0)
            {
                SpawnRandomEnemy();
            }
            yield return new WaitForSeconds(enemySpawnInterval);
        }
    }

    void SpawnRandomBrick()
    {
        Vector3 randomPos = GetRandomSpawnPosition();
        GameObject prefab = brickPrefabs[Random.Range(0, brickPrefabs.Length)];
        var obj = Instantiate(prefab, randomPos, Quaternion.identity);
        if (obj != null) obj.tag = "Brick";
    }

    void SpawnRandomEnemy()
    {
        Vector3 pos = GetRandomSpawnPosition();
        GameObject prefab = enemyStructurePrefabs[Random.Range(0, enemyStructurePrefabs.Length)];
        var obj = Instantiate(prefab, pos, Quaternion.identity);
        if (obj != null) obj.tag = "Enemy";
    }

    Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform t = spawnPoints[Random.Range(0, spawnPoints.Length)];
            return t.position;
        }

        return new Vector3(Random.Range(-20f, 20f), Random.Range(-10f, 10f), 0f);
    }

    // Utility: allow external systems to request immediate enemy spawns
    public void SpawnImmediateEnemies(int count)
    {
        for (int i = 0; i < count; i++) SpawnRandomEnemy();
    }
}
