using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int currentLevel = 1;
    public int targetSize = 50;

    public PlayerBrick player;

    void Start()
    {
        if (player == null) player = FindObjectOfType<PlayerBrick>();
        targetSize = CalculateTargetSize(currentLevel);
    }

    void Update()
    {
        CheckLevelProgress();
    }

    void CheckLevelProgress()
    {
        if (player != null && player.currentSize >= targetSize)
        {
            CompleteLevel();
        }
    }

    void CompleteLevel()
    {
        currentLevel++;
        targetSize = CalculateTargetSize(currentLevel);

        if (player != null)
        {
            player.currentSize = Mathf.Max(1, currentLevel * 5);
        }

        SpawnLevelEnemies();

        if (UIManager.Instance != null) UIManager.Instance.ShowLevelComplete(currentLevel);
    }

    int CalculateTargetSize(int level)
    {
        return 50 + (level * 25);
    }

    void SpawnLevelEnemies()
    {
        // Inform the spawner to increase difficulty and spawn a few immediate enemies
        var spawner = FindObjectOfType<BrickSpawner>();
        if (spawner != null)
        {
            spawner.maxEnemiesInScene += Mathf.Clamp(currentLevel, 1, 5);
            spawner.SpawnImmediateEnemies(Mathf.Clamp(currentLevel, 1, 5));
        }
    }
}
