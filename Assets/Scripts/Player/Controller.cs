using UnityEngine;

public class Controller : MonoBehaviour
{
    // wave movement
    [SerializeField] private Vector2 waveForceDirectionUp;
    [SerializeField] private Vector2 waveForceDirectionDown;
    [SerializeField] private float rotationWaveForce;
    [SerializeField] private float waveSpeed;
    [SerializeField] private AnimationCurve moveTransitionCurve;

    private Rigidbody2D playerRb;
    private float current, target;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        target = Input.GetKey(KeyCode.Mouse0) ? 1 : 0;
    }

    private void FixedUpdate()
    {
        if (target == 1)
        {
            WaveUp();
        }
        else
        {
            WaveDown();
        }

        current = Mathf.MoveTowards(current, target, waveSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Lerp(
            Quaternion.Euler(0, 0, -rotationWaveForce),
            Quaternion.Euler(0, 0, rotationWaveForce),
            moveTransitionCurve.Evaluate(current)
        );
    }

    private void WaveUp()
    {
        playerRb.linearVelocity = waveForceDirectionUp;
    }

    private void WaveDown()
    {
        playerRb.linearVelocity = waveForceDirectionDown;
    }
}