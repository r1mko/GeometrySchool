using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerController Player;
    public SoundManager SoundManager;

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
    }

    private void Start()
    {
        SoundManager.PlayBackgroundCurrentMusic();
        Player.StartMove();
    }
}