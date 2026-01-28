using UnityEngine;

public class FinishZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("<color=green>You reached the finish!</color>");
        }
    }
}
