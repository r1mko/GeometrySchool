using UnityEngine;

public static class SaveData
{
    private const string LevelCompletedPrefix = "LEVEL_COMPLETED_";

    public static int CurrentLevelIndex { get; set; } = 0;

    public static void MarkLevelAsCompleted(int levelIndex)
    {
        string key = LevelCompletedPrefix + levelIndex;
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();

        Debug.Log($"[SaveData] Level {levelIndex} marked as completed. Key: {key}");
    }

    public static bool IsLevelCompleted(int levelIndex)
    {
        string key = LevelCompletedPrefix + levelIndex;
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    public static void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();

        Debug.Log("[SaveData] All progress reset.");
    }

    public static void ResetLevel(int levelIndex)
    {
        string key = LevelCompletedPrefix + levelIndex;
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
    }
}