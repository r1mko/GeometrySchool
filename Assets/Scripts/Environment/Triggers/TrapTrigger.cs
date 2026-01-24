using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    [SerializeField] private TrapType currentTrapType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ActionBus.InvokeTriggeredTrap(currentTrapType);
        }
    }
}
