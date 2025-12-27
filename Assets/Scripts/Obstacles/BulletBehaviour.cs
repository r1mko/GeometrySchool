using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public enum BulletType
    {
        Straight,
        Wave
    }

    public BulletType currentBulletType;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float destroyOffset = 5f;
    [SerializeField] private float xSpeed = 10f;

    // Wave-specific
    [SerializeField]
    private AnimationCurve waveCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.25f, 1f),
        new Keyframe(0.5f, 0f),
        new Keyframe(0.75f, -1f),
        new Keyframe(1f, 0f)
    );
    [SerializeField] private float waveAmplitude = 4f; // координаты границ волны
    [SerializeField] private float waveDuration = 2f;

    private float lifetimeFallback = 10f;
    private float elapsedTime = 0f;
    private float startY;

    public void Init(BulletType type, Transform player, float forceWaveAmplitude)
    {
        currentBulletType = type;
        playerTransform = player;
        waveAmplitude = forceWaveAmplitude;
        CancelInvoke(nameof(DestroyFallback));
        Invoke(nameof(DestroyFallback), lifetimeFallback);
    }

    private void Update()
    {
        if (currentBulletType == BulletType.Straight || currentBulletType == BulletType.Wave)
        {
            StraightBulletBehaviour();
        }

        if (currentBulletType == BulletType.Wave)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Repeat(elapsedTime, waveDuration) / waveDuration;
            float curveValue = waveCurve.Evaluate(normalizedTime);
            float offsetY = curveValue * waveAmplitude;
            transform.position = new Vector3(transform.position.x, startY + offsetY, transform.position.z);
        }

        if (transform.position.x < (playerTransform.position.x - destroyOffset))
        {
            Destroy(gameObject);
        }
    }

    private void StraightBulletBehaviour()
    {
        transform.Translate(Vector2.left * xSpeed * Time.deltaTime);
    }

    private void DestroyFallback()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player detected");
        }
    }
}