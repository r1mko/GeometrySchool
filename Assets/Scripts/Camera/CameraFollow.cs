using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Offset & Dead Zone")]
    [SerializeField] private float xOffset = 0f;
    [SerializeField] private float deadZoneHalfWidth = 1f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        float targetX = target.position.x + xOffset;
        float currentX = transform.position.x;

        float delta = targetX - currentX;

        if (Mathf.Abs(delta) > deadZoneHalfWidth)
        {
            float desiredX = targetX - Mathf.Sign(delta) * deadZoneHalfWidth;
            float smoothedX = Mathf.SmoothDamp(currentX, desiredX, ref velocity.x, 0);
            transform.position = new Vector3(smoothedX, transform.position.y, transform.position.z);
        }
    }
}