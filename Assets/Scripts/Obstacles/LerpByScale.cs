using UnityEngine;

public class LerpByScale : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private Vector2 startScale;
    [SerializeField] private Vector2 goalScale;
    [SerializeField] private float speed;
     private float current, target;


    private void Awake()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        if (transform.localScale.x == startScale.x && transform.localScale.y == startScale.y)
        {
            target = 1;
        }
        else if (transform.localScale.x == goalScale.x && transform.localScale.y == goalScale.y)
        {
            target = 0;
        }

        current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);

        transform.localScale = Vector2.Lerp(startScale,  goalScale, curve.Evaluate(current));
    }
}
