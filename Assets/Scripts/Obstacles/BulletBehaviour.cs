using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public enum BulletType
    {
        Straight
    }

    public BulletType currentBulletType;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float destroyOffset = 5f;
    [SerializeField] private float speed = 10f;

    private float lifetimeFallback = 10f;

    public void Init(BulletType type, Transform player)
    {
        currentBulletType = type;
        playerTransform = player;

        CancelInvoke(nameof(DestroyFallback));
        Invoke(nameof(DestroyFallback), lifetimeFallback);
    }

    private void Update()
    {
        if (currentBulletType == BulletType.Straight)
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            if (transform.position.x < (playerTransform.position.x - destroyOffset))
            {
                Destroy(gameObject);
            }
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
            Debug.Log("Player detected");
        }
    }
}