using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class UIMenu : MonoBehaviour
{
    public enum LevelDifficult
    {
        Easy,
        Medium,
        Hard
    }

    [Serializable]
    public class DifficultyIconConfig
    {
        public LevelDifficult Difficulty;
        public Sprite IconSprite;
    }

    [Serializable]
    public class LevelConfig
    {
        public int SceneIndex;
        public Color BackgroundColor;
        public LevelDifficult Difficult;
        public bool Completed;
    }

    [Header("UI Buttons")]
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button playButton;

    [Header("Common Visual Elements (Single Instance)")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image difficultyIconImage;
    [SerializeField] private GameObject completedMarker;

    [Header("Data Configuration")]
    [SerializeField] private LevelConfig[] levels;

    [SerializeField] private DifficultyIconConfig[] difficultyIcons;

    private int currentIndex;

    private void Awake()
    {
        currentIndex = SaveData.CurrentLevelIndex;

        if (currentIndex >= levels.Length || currentIndex < 0)
            currentIndex = 0;

        SaveData.CurrentLevelIndex = currentIndex;
    }

    private void Start()
    {
        rightButton.onClick.AddListener(OnRightClick);
        leftButton.onClick.AddListener(OnLeftClick);
        playButton.onClick.AddListener(OnPlayClick);

        UpdateLevelView();
    }

    private void OnDestroy()
    {
        rightButton.onClick.RemoveAllListeners();
        leftButton.onClick.RemoveAllListeners();
        playButton.onClick.RemoveAllListeners();
    }

    private void OnRightClick()
    {
        currentIndex = (currentIndex + 1) % levels.Length;

        SaveData.CurrentLevelIndex = currentIndex;
        UpdateLevelView();
    }

    private void OnLeftClick()
    {
        currentIndex = (currentIndex - 1 + levels.Length) % levels.Length;

        SaveData.CurrentLevelIndex = currentIndex;
        UpdateLevelView();
    }

    private void OnPlayClick()
    {
        int sceneIndex = levels[currentIndex].SceneIndex;
        LoadScene(sceneIndex);
    }

    private void UpdateLevelView()
    {
        if (levels == null || levels.Length == 0) return;

        LevelConfig currentLevel = levels[currentIndex];

        if (backgroundImage != null)
        {
            backgroundImage.color = currentLevel.BackgroundColor;
        }

        if (difficultyIconImage != null && difficultyIcons != null)
        {
            Sprite targetSprite = GetDifficultySprite(currentLevel.Difficult);
            if (targetSprite != null)
            {
                difficultyIconImage.sprite = targetSprite;
                difficultyIconImage.preserveAspect = true;
                difficultyIconImage.enabled = true;
            }
            else
            {
                difficultyIconImage.enabled = false;
            }
        }

        if (completedMarker != null)
        {
            completedMarker.SetActive(currentLevel.Completed);
        }
    }

    private Sprite GetDifficultySprite(LevelDifficult diff)
    {
        if (difficultyIcons == null) return null;

        foreach (var config in difficultyIcons)
        {
            if (config.Difficulty == diff)
                return config.IconSprite;
        }
        return null;
    }

    private void LoadScene(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
}