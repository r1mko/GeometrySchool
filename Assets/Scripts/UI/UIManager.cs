using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform finish;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private Slider distanceSlider;
    [SerializeField] private Button exitButton;

    private bool isTracking = false;
    private bool isFinished = false;
    private float initialDistance;
    private const float FinishThreshold = 3f;

    private void Start()
    {
        exitButton.onClick.AddListener(LoadMenuScene);
    }

    private void OnDestroy()
    {
        exitButton.onClick.RemoveAllListeners();
    }

    private void LoadMenuScene()
    {
        if (SplatManager.Instance != null)
            SplatManager.Instance.DestroySelf();

        SceneManager.LoadScene("Menu");
    }

    public void StartTracking()
    {
        if (player == null || finish == null)
        {
            Debug.LogError("Player or Finish not assigned!");
            return;
        }

        initialDistance = Mathf.Abs(player.position.x - finish.position.x);
        isTracking = true;
        isFinished = false;
    }

    private void Update()
    {
        if (!isTracking || player == null || isFinished) return;

        float currentDistance = Mathf.Abs(player.position.x - finish.position.x);

        if (currentDistance <= FinishThreshold)
        {
            distanceSlider.value = 1f;
            distanceText.text = "100%";
            isFinished = true;
            return;
        }

        float progress = 1f - Mathf.Clamp01(currentDistance / initialDistance);
        distanceSlider.value = progress;

        int percentage = Mathf.RoundToInt(progress * 100);
        distanceText.text = percentage.ToString() + "%";
    }
}