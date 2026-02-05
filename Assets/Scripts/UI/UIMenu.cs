using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;

    private void Start()
    {
        playButton.onClick.AddListener(() => LoadScene(1));
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveAllListeners();
    }

    private void LoadScene(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
}
