using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // wave movement
    [SerializeField] private Vector2 waveForceDirectionUp;
    [SerializeField] private Vector2 waveForceDirectionDown;
    // wave angle
    [SerializeField] private float waveAngleRotation;
    [SerializeField] private float waveLerpBackSpeed;
    [SerializeField] private AnimationCurve moveTransitionCurve;

    // border detection
    [SerializeField] private LayerMask borderLayer;
    [SerializeField] private float borderCheckRadius;

    private Rigidbody2D playerRb;
    private float current, target;

    private bool gameStarted;
    private bool atTopBorder;
    private bool atBottomBorder;

    public void StartMove()
    {
        gameStarted = true;
    }

    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        target = Input.GetKey(KeyCode.Mouse0) ? 1 : 0;
        CheckBorders();
    }

    private void FixedUpdate()
    {
        if (!gameStarted)
        {
            return;
        }

        if (target == 1)
        {
            WaveUp();
        }
        else
        {
            WaveDown();
        }

        current = Mathf.MoveTowards(current, target, waveLerpBackSpeed * Time.fixedDeltaTime);

        if (atTopBorder || atBottomBorder)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(0, 0, -waveAngleRotation),
                Quaternion.Euler(0, 0, waveAngleRotation),
                moveTransitionCurve.Evaluate(current)
            );
        }
    }

    private void WaveUp()
    {
        if (atTopBorder)
        {
            playerRb.linearVelocity = new Vector2(waveForceDirectionUp.x, 0f);
        }
        else
        {
            playerRb.linearVelocity = waveForceDirectionUp;
        }
    }

    private void WaveDown()
    {
        if (atBottomBorder)
        {
            playerRb.linearVelocity = new Vector2(waveForceDirectionDown.x, 0f);
        }
        else
        {
            playerRb.linearVelocity = waveForceDirectionDown;
        }
    }

    private void CheckBorders()
    {
        Vector2 topCheckPos = (Vector2)transform.position + Vector2.up * borderCheckRadius;
        Collider2D[] topColliders = Physics2D.OverlapCircleAll(topCheckPos, borderCheckRadius, borderLayer);
        atTopBorder = false;
        foreach (var col in topColliders)
        {
            if (col.CompareTag("Border"))
            {
                atTopBorder = true;
                break;
            }
        }

        Vector2 bottomCheckPos = (Vector2)transform.position + Vector2.down * borderCheckRadius;
        Collider2D[] bottomColliders = Physics2D.OverlapCircleAll(bottomCheckPos, borderCheckRadius, borderLayer);
        atBottomBorder = false;
        foreach (var col in bottomColliders)
        {
            if (col.CompareTag("Border"))
            {
                atBottomBorder = true;
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 topPos = (Vector2)transform.position + Vector2.up * borderCheckRadius;
        Gizmos.DrawWireSphere(topPos, borderCheckRadius);

        Gizmos.color = Color.cyan;
        Vector2 bottomPos = (Vector2)transform.position + Vector2.down * borderCheckRadius;
        Gizmos.DrawWireSphere(bottomPos, borderCheckRadius);
    }
}