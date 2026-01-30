using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    private const string SceneLevelPrefix = "Level";
    [SerializeField] private Button playButton;

    private void Start()
    {
        playButton.onClick.AddListener(() => LoadScene(1)); //to do поменять на индексы
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveAllListeners();
    }

    private void LoadScene(int levelIndex)
    {
        var levelName = SceneLevelPrefix + levelIndex.ToString();
        SceneManager.LoadScene(levelName);
    }
}
