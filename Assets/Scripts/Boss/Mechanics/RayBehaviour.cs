using System.Collections;
using UnityEngine;

public class RayBehaviour : MonoBehaviour
{
    public RayType currentRayType;

    [SerializeField] private float markedDuration = 1f;
    [SerializeField] private float rayDuration = 2f;
    [SerializeField] private float scaleUpDuration = 0.25f;
    [SerializeField] private float scaleDownDuration = 0.25f;
    [SerializeField] private float topStart = 30f;
    [SerializeField] private float topEnd = -8f;
    [SerializeField] private float bottomStart = -30f;
    [SerializeField] private float bottomEnd = 8f;
    [SerializeField] private Vector3 rayLength = new Vector3(16f, 0.1f, 1f);

    [SerializeField] private SpriteRenderer spriteRenderer;

    private Transform playerTransform;
    private bool isMarking;
    private float rayStartAngle;
    private float rayEndAngle;
    private float elapsedTime = 0f;

    public void Init(RayType type, Transform player, bool fromTop)
    {
        currentRayType = type;
        playerTransform = player;
        if (type == RayType.Dynamic)
        {
            rayStartAngle = fromTop ? topStart : bottomStart;
            rayEndAngle = fromTop ? topEnd : bottomEnd;
        }
        else if (type == RayType.Target)
        {
            Vector2 direction = player.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
            rayStartAngle = angle;
            rayEndAngle = angle;
            rayDuration += markedDuration; // lifetime = markedDuration + rayDuration
            isMarking = true;
            spriteRenderer.color = Color.red;
        }
        else if (type == RayType.Static)
        {
            rayDuration += markedDuration; // lifetime = markedDuration + rayDuration
            isMarking = true;
            spriteRenderer.color = Color.red;
        }

        transform.rotation = Quaternion.Euler(0, 0, rayStartAngle);
        transform.localScale = Vector3.zero;
        StartCoroutine(ScaleUpRoutine());

        CancelInvoke(nameof(DestroyFallback));
        Invoke(nameof(DestroyFallback), rayDuration);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        // Mark
        if (isMarking)
        {
            if (elapsedTime >= markedDuration)
            {
                isMarking = false;
                spriteRenderer.color = Color.white;
            }
        }

        // Lifetime
        if (elapsedTime <= rayDuration)
        {
            if (currentRayType == RayType.Dynamic)
            {
                float t = elapsedTime / rayDuration;
                float currentAngle = Mathf.Lerp(rayStartAngle, rayEndAngle, t);
                transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            }


            if (elapsedTime >= rayDuration - scaleDownDuration)
            {
                StartCoroutine(ScaleDownRoutine());
            }

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator ScaleUpRoutine()
    {
        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = rayLength;

        while (elapsed < scaleUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / scaleUpDuration);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private IEnumerator ScaleDownRoutine()
    {
        Vector3 startScale = rayLength;
        float elapsed = 0f;

        while (elapsed < scaleDownDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / scaleDownDuration);
            float newY = Mathf.Lerp(startScale.y, 0f, t);
            transform.localScale = new Vector3(startScale.x, newY, startScale.z);
            yield return null;
        }

        transform.localScale = new Vector3(startScale.x, 0f, startScale.z);
    }

    private void DestroyFallback()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isMarking)
            {
                Debug.Log("It's just marked or scalling. Ignoring");
                return;
            }
            collision.GetComponent<PlayerController>().LevelRestart();
            Debug.Log("Player hit by Ray!");
        }
    }
}