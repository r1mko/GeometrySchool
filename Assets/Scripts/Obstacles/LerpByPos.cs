using UnityEngine;

public class LerpByPos : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 goalPosition;
    [SerializeField] private float speed;
    private float current, target;


    private void Awake()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (transform.position.x == startPosition.x && transform.position.y == startPosition.y)
        {
            target = 1;
        }
        else if (transform.position.x == goalPosition.x && transform.position.y == goalPosition.y)
        {
            target = 0;
        }

        current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);

        transform.position = Vector2.Lerp(startPosition, goalPosition, curve.Evaluate(current));
    }

}
