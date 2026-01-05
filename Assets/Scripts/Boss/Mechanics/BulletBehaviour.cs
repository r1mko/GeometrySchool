using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public BulletType currentBulletType;

    // Wave
    [SerializeField] private float waveDuration = 2f;
    [SerializeField] private AnimationCurve waveCurve;
    private float waveAmplitude = 4f;

    // Z Bullet
    [SerializeField] private float ZBulletDuration = 2f;
    [SerializeField] private AnimationCurve zBulletCurve;
    private float zBulletAmplitude = 4f;

    // Сommon
    private Transform playerTransform;
    private float destroyOffset = 5f;
    private float xSpeed = 10f;
    private float bulletLifetimeFallback = 10f;
    private float elapsedTime = 0f;
    private float startY;

    public void Init(BulletType type, Transform player, float forceWaveAmplitude, float forceZBulletAmplitude)
    {
        currentBulletType = type;
        playerTransform = player;

        if (type == BulletType.Wave)
        {
            waveAmplitude = forceWaveAmplitude;
            startY = transform.position.y;
        }
        else if (type == BulletType.ZBullet)
        {
            zBulletAmplitude = forceZBulletAmplitude;
            startY = transform.position.y;
        }

        CancelInvoke(nameof(DestroyFallback));
        Invoke(nameof(DestroyFallback), bulletLifetimeFallback);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        switch (currentBulletType)
        {
            case BulletType.Straight:
                StraightBulletBehaviour();
                break;

            case BulletType.Wave:
                StraightBulletBehaviour();
                float normalizedWaveTime = Mathf.Repeat(elapsedTime, waveDuration) / waveDuration;
                float waveOffset = waveCurve.Evaluate(normalizedWaveTime) * waveAmplitude;
                transform.position = new Vector3(transform.position.x, startY + waveOffset, transform.position.z);
                break;
            case BulletType.ZBullet:
                StraightBulletBehaviour();
                float normalizedZTime = Mathf.Repeat(elapsedTime, ZBulletDuration) / ZBulletDuration;
                float zBulletOffset = zBulletCurve.Evaluate(normalizedZTime) * zBulletAmplitude;
                transform.position = new Vector3(transform.position.x, startY + zBulletOffset, transform.position.z);
                break;
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
            Debug.Log("Player hit by bullet!");
        }
    }
}