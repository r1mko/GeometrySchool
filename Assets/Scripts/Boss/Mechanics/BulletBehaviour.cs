using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public enum BulletType
    {
        Straight,
        Wave,
        ZBullet,
        Ray
    }

    public BulletType currentBulletType;

    // Wave
    [SerializeField] private float waveDuration = 2f;
    [SerializeField] private AnimationCurve waveCurve;
    private float waveAmplitude = 4f;

    // Z Bullet
    [SerializeField] private float ZBulletDuration = 2f;
    [SerializeField] private AnimationCurve zBulletCurve;
    private float zBulletAmplitude = 4f;

    // Ray
    [SerializeField] private float rayDuration = 0.5f;
    [SerializeField] private float topStart = 12.5f;
    [SerializeField] private float topEnd = -8;
    [SerializeField] private float bottomStart = -12.5f;
    [SerializeField] private float bottomEnd = 8;
    private Vector3 rayLength = new Vector3(16f, 0.1f, 1);
    private float rayStartAngle;
    private float rayEndAngle;

    // Сommon
    private Transform playerTransform;
    private float destroyOffset = 5f;
    private float xSpeed = 10f;
    private float lifetimeFallback = 10f;
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
        else if (type == BulletType.Ray)
        {
            // Определяем, сверху или снизу
            bool isFromTop = Random.value > 0.5f;
            rayStartAngle = isFromTop ? topStart : bottomStart;
            rayEndAngle = isFromTop ? topEnd : bottomEnd;

            transform.localScale = rayLength;
            transform.rotation = Quaternion.Euler(0, 0, rayStartAngle);
        }

        CancelInvoke(nameof(DestroyFallback));
        float actualLifetime = (type == BulletType.Ray) ? rayDuration : lifetimeFallback;
        Invoke(nameof(DestroyFallback), actualLifetime);
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
            case BulletType.Ray:
                if (elapsedTime <= rayDuration)
                {
                    float t = elapsedTime / rayDuration;
                    float currentAngle = Mathf.Lerp(rayStartAngle, rayEndAngle, t);
                    transform.rotation = Quaternion.Euler(0, 0, currentAngle);
                }
                break;
        }

        if ((currentBulletType == BulletType.Straight || currentBulletType == BulletType.Wave || currentBulletType == BulletType.ZBullet) &&
            transform.position.x < (playerTransform.position.x - destroyOffset))
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