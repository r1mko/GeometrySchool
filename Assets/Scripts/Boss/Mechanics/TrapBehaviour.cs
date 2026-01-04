using UnityEngine;

public class TrapBehaviour : MonoBehaviour
{
    public float SpawnX { get; private set; }

    [SerializeField] private float destroyOffset = 5f;
    [SerializeField] private float lifetimeFallback = 10f;

    private Transform playerTransform;

    public void Init(Transform player, BossTeacherController boss, Vector3 spawnPos)
    {
        playerTransform = player;
        SpawnX = spawnPos.x;
        Invoke(nameof(DestroyFallback), lifetimeFallback);
    }

    private void Update()
    {
        if (transform.position.x < playerTransform.position.x - destroyOffset)
        {
            Destroy(gameObject);
        }
    }

    private void DestroyFallback()
    {
        Destroy(gameObject);
    }
}