using System.Collections;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public BulletType currentBulletType;
    // Straight
    [SerializeField] private float straightSpeedMultiplayer = 2f;

    // Wave
    [SerializeField] private float waveDuration = 2f;
    [SerializeField] private AnimationCurve waveCurve;
    private float waveAmplitude = 4.15f;

    // Z Bullet
    [SerializeField] private float zBulletDuration = 2f;
    [SerializeField] private AnimationCurve zBulletCurve;
    private float zBulletAmplitude = 4.15f;

    // Сommon
    private Transform playerTransform;
    private float destroyOffset = 20f;
    private float xSpeed = 10f;
    private float bulletLifetimeFallback = 10f;
    private float elapsedTime = 0f;
    private float startY;

    [Header("Appear Animation")]
    [SerializeField] private float appearDuration = 0.25f;
    [SerializeField]
    private AnimationCurve appearCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.6f, 1.2f),
        new Keyframe(1f, 1f)
    );

    public void Init(BulletType type, Transform player, float forceWaveAmplitude, float forceZBulletAmplitude, float waveDuration, float zBulletDuration)
    {
        currentBulletType = type;
        playerTransform = player;

        if (type == BulletType.Wave)
        {
            waveAmplitude = forceWaveAmplitude;
            this.waveDuration = waveDuration;
            startY = transform.position.y;
        }
        else if (type == BulletType.ZBullet)
        {
            zBulletAmplitude = forceZBulletAmplitude;
            this.zBulletDuration = zBulletDuration;
            startY = transform.position.y;
        }
        else if (type == BulletType.Straight)
        {
            xSpeed *= straightSpeedMultiplayer;
        }

        CancelInvoke(nameof(DestroyFallback));
        Invoke(nameof(DestroyFallback), bulletLifetimeFallback);

        StartCoroutine(AppearRoutine());
    }


    private IEnumerator AppearRoutine()
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        while (elapsed < appearDuration)
        {
            float t = elapsed / appearDuration;
            float scaleMultiplier = appearCurve.Evaluate(t);
            transform.localScale = startScale * scaleMultiplier;
            elapsed += Time.deltaTime;
            yield return null;
        }

        float finalScale = appearCurve.Evaluate(1f);
        transform.localScale = startScale * finalScale;
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
                float normalizedZTime = Mathf.Repeat(elapsedTime, zBulletDuration) / zBulletDuration;
                float zBulletOffset = zBulletCurve.Evaluate(normalizedZTime) * zBulletAmplitude;
                transform.position = new Vector3(transform.position.x, startY + zBulletOffset, transform.position.z);
                break;
        }

        if (playerTransform != null)
        {
            if (transform.position.x < (playerTransform.position.x - destroyOffset))
            {
                Destroy(gameObject);
            }
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
}