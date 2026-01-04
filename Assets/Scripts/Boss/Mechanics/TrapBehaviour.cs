using UnityEngine;

public class TrapBehaviour : MonoBehaviour
{
    [Header("Common Settings")]
    public TrapType currentTrapType = TrapType.Static;
    public float SpawnX { get; private set; }

    [SerializeField] private float destroyOffset = 5f;
    [SerializeField] private float lifetimeFallback = 10f;

    [Header("Falling Trap Settings")]
    [SerializeField] private float fallTargetY = -3.85f;
    [SerializeField] private float fallDuration = 1.2f;
    [SerializeField]
    private AnimationCurve fallCurve = new AnimationCurve(
        new Keyframe(0f, 0f, 0f, 0f),
        new Keyframe(1f, 1f, 0f, 0f)
    );

    private Transform playerTransform;
    private bool initialized;
    private float startY;
    private float fallStartTime;

    public void Init(Transform player, BossTeacherController boss, Vector3 spawnPos, TrapType type)
    {
        currentTrapType = type;
        playerTransform = player;
        SpawnX = spawnPos.x;
        initialized = true;

        CancelInvoke(nameof(DestroyFallback));
        Invoke(nameof(DestroyFallback), lifetimeFallback);

        if (currentTrapType == TrapType.Falling)
        {
            startY = spawnPos.y;
            fallStartTime = Time.time;
        }
    }

    private void Update()
    {
        if (!initialized) return;

        switch (currentTrapType)
        {
            case TrapType.Static:
                break;

            case TrapType.Falling:
                float fallProgress = (Time.time - fallStartTime) / fallDuration;
                if (fallProgress >= 1f)
                {
                    transform.position = new Vector3(transform.position.x, fallTargetY, transform.position.z);
                }
                else
                {
                    float t = fallCurve.Evaluate(fallProgress);
                    float currentY = Mathf.Lerp(startY, fallTargetY, t);
                    transform.position = new Vector3(transform.position.x, currentY, transform.position.z);
                }
                break;
        }

        if (currentTrapType == TrapType.Static &&
            transform.position.x < playerTransform.position.x - destroyOffset)
        {
            Destroy(gameObject);
        }
    }

    private void DestroyFallback()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit by trap!");
        }
    }
}