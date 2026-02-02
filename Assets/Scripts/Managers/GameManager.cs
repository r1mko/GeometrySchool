using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerController Player;
    public SoundManager SoundManager;
    public UIManager UIManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (Player == null)
        {
            Player = FindFirstObjectByType<PlayerController>();
        }
        if (SoundManager == null)
        {
            SoundManager = FindFirstObjectByType<SoundManager>();
        }
        if (UIManager == null)
        {
            UIManager = FindFirstObjectByType<UIManager>();
        }
    }

    private void Start()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        yield return null;
        Player.StartMove();
        SoundManager.PlayBackgroundCurrentMusic();
        UIManager.StartTracking();
    }
}