using UnityEngine;

public class RayBehaviour : MonoBehaviour
{
    public enum RayType
    {
        Dynamic,
        Static
    }

    public RayType currentRayType;

    [SerializeField] private float rayDuration = 2f;
    [SerializeField] private float topStart = 30f;
    [SerializeField] private float topEnd = -8f;
    [SerializeField] private float bottomStart = -30f;
    [SerializeField] private float bottomEnd = 8f;
    [SerializeField] private Vector3 rayLength = new Vector3(16f, 0.1f, 1f);

    private float rayStartAngle;
    private float rayEndAngle;
    private float elapsedTime = 0f;

    public void Init(RayType type)
    {
        currentRayType = type;
        bool isFromTop = Random.value > 0.5f;
        rayStartAngle = isFromTop ? topStart : bottomStart;
        rayEndAngle = isFromTop ? topEnd : bottomEnd;

        transform.localScale = rayLength;
        transform.rotation = Quaternion.Euler(0, 0, rayStartAngle);

        CancelInvoke(nameof(DestroyFallback));
        Invoke(nameof(DestroyFallback), rayDuration);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime <= rayDuration)
        {
            float t = elapsedTime / rayDuration;
            float currentAngle = Mathf.Lerp(rayStartAngle, rayEndAngle, t);
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
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
            Debug.Log("Player hit by Ray!");
        }
    }
}