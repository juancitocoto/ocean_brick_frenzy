using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public int currentLevel = 1;
    public int score = 0;
    public int targetScore = 1000;
    public bool isGameOver = false;
    public bool isPaused = false;

    [Header("Spawning")]
    public FishSpawner fishSpawner;
    public float difficultyMultiplier = 1f;

    [Header("Level Themes")]
    public LevelTheme[] levelThemes;

    [Header("References")]
    public PlayerBrickFish player;
    public Transform spawnArea;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartLevel(currentLevel);
    }

    public void StartLevel(int level)
    {
        currentLevel = level;

        if (level <= levelThemes.Length && levelThemes.Length > 0)
        {
            ApplyTheme(levelThemes[level - 1]);
        }

        targetScore = 1000 * level;
        difficultyMultiplier = 1f + (level - 1) * 0.2f;

        if (fishSpawner != null) fishSpawner.SpawnWave(level);

        UIManager.Instance?.ShowLevelStart(level);
    }

    void ApplyTheme(LevelTheme theme)
    {
        if (theme == null) return;
        RenderSettings.skybox = theme.skyboxMaterial;
        AudioManager.Instance?.PlayMusic(theme.backgroundMusic);
    }

    public void AddScore(int points)
    {
        score += Mathf.RoundToInt(points * difficultyMultiplier);
        UIManager.Instance?.UpdateScore(score);

        if (score >= targetScore)
        {
            CompleteLevel();
        }
    }

    void CompleteLevel()
    {
        UIManager.Instance?.ShowLevelComplete(currentLevel, score);

        int bonus = player != null ? player.currentStuds * 10 : 0;
        score += bonus;

        currentLevel++;
        StartCoroutine(TransitionToNextLevel());
    }

    IEnumerator TransitionToNextLevel()
    {
        yield return new WaitForSeconds(3f);
        StartLevel(currentLevel);
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        UIManager.Instance?.ShowGameOver(score, currentLevel);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        UIManager.Instance?.ShowPauseMenu(isPaused);
    }
}

[System.Serializable]
public class LevelTheme
{
    public string themeName;
    public Material skyboxMaterial;
    public Color waterColor;
    public GameObject[] environmentPrefabs;
    public AudioClip backgroundMusic;
    public Color[] fishColorPalette;
}
