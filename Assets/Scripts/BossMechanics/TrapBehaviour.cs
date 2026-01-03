using UnityEngine;

public class TrapBehaviour : MonoBehaviour
{
    public enum TrapType
    {
        Close,
        Middle,
        Long
    }

    public TrapType CurrentType { get; private set; }
    public float SpawnX { get; private set; }

    [SerializeField] private float destroyOffset = 5f;
    [SerializeField] private float lifetimeFallback = 10f;

    private Transform playerTransform;
    private BossTeacherController bossController;

    public void Init(TrapType type, Transform player, BossTeacherController boss, Vector3 spawnPos)
    {
        CurrentType = type;
        playerTransform = player;
        bossController = boss;
        SpawnX = spawnPos.x;

        Invoke(nameof(DestroyFallback), lifetimeFallback);
    }

    private void Update()
    {
        if (transform.position.x < playerTransform.position.x - destroyOffset)
        {
            DestroySelf();
        }
    }

    private void DestroyFallback()
    {
        DestroySelf();
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}