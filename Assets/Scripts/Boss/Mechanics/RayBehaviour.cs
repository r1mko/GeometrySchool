using UnityEngine;

public class RayBehaviour : MonoBehaviour
{
    public RayType currentRayType;

    [SerializeField] private float markedDuration = 1f;
    [SerializeField] private float rayDuration = 2f;
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

    public void Init(RayType type, Transform player)
    {
        currentRayType = type;
        playerTransform = player;
        if (type == RayType.Dynamic)
        {
            bool isFromTop = Random.value > 0.5f;
            rayStartAngle = isFromTop ? topStart : bottomStart;
            rayEndAngle = isFromTop ? topEnd : bottomEnd;
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

        transform.localScale = rayLength;
        transform.rotation = Quaternion.Euler(0, 0, rayStartAngle);

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
        }
        else
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
            if (isMarking)
            {
                Debug.Log("It's just marked. Ignoring");
                return;
            }
            Debug.Log("Player hit by Ray!");
        }
    }
}