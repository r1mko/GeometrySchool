using UnityEngine;

public class LerpByRotate : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private Vector3 startRotation;
    [SerializeField] private Vector3 goalRotationl;
    [SerializeField] private float speed;
    private float current, target;

    void Update()
    {
        if (current == 1)
        {
            target = 0;
        }
        else if (current == 0)
        {
            target = 1;
        }

        current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);

        transform.rotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(goalRotationl), curve.Evaluate(current));
    }
}
