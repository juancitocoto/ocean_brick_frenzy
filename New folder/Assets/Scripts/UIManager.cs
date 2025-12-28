using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI studCountText;
    public Slider sizeProgressBar;
    public Image fishSizeIcon;

    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    public GameObject levelStartPanel;

    [Header("Popups")]
    public GameObject studPopupPrefab;
    public Transform popupContainer;

    [Header("Fish Size Icons")]
    public Sprite[] fishSizeSprites; // Tiny to Boss

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null) scoreText.text = $"Score: {score:N0}";
    }

    public void UpdateStudCount(int studs)
    {
        if (studCountText != null) studCountText.text = $"x {studs}";
    }

    public void UpdateSizeProgress(float progress, BrickFish.FishType fishType)
    {
        if (sizeProgressBar != null) sizeProgressBar.value = progress;
        int iconIndex = (int)fishType;
        if (fishSizeIcon != null && iconIndex >= 0 && iconIndex < fishSizeSprites.Length)
            fishSizeIcon.sprite = fishSizeSprites[iconIndex];
    }

    public void ShowLevelStart(int level)
    {
        if (levelText != null) levelText.text = $"Level {level}";
        if (levelStartPanel != null)
        {
            levelStartPanel.SetActive(true);
        }
    }

    public void ShowLevelComplete(int level, int score)
    {
        if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
        Debug.Log($"Level {level} complete with score {score}");
    }

    public void ShowGameOver(int score, int level)
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Debug.Log($"GameOver: score={score} level={level}");
    }

    public void ShowPauseMenu(bool paused)
    {
        if (pausePanel != null) pausePanel.SetActive(paused);
    }

    public void ShowStudPopup(Vector3 position, int value)
    {
        if (studPopupPrefab == null || popupContainer == null)
        {
            Debug.Log($"Stud collected at {position} worth {value}");
            return;
        }

        var go = Instantiate(studPopupPrefab, popupContainer);
        go.transform.position = position;
        var txt = go.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null) txt.text = $"+{value}";
        Destroy(go, 1.5f);
    }
}
