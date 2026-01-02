using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Offset & Dead Zone")]
    [SerializeField] private float xOffset;
    [SerializeField] private float deadZoneHalfWidth;

    [Header("Smoothing")]
    [SerializeField] private float smoothTime;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        float targetX = target.position.x + xOffset;
        float currentX = transform.position.x;

        float delta = targetX - currentX;

        float desiredX = currentX;
        if (Mathf.Abs(delta) > deadZoneHalfWidth)
        {
            desiredX = targetX - Mathf.Sign(delta) * deadZoneHalfWidth;
        }

        float smoothedX = Mathf.SmoothDamp(currentX, desiredX, ref velocity.x, smoothTime);
        transform.position = new Vector3(smoothedX, transform.position.y, transform.position.z);
    }
}