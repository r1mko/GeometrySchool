using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    [SerializeField] private TrapType currentTrapType;
    [SerializeField] private float fallingTrapDuration = 2.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ActionBus.InvokeTriggeredTrap(currentTrapType, fallingTrapDuration);
        }
    }
}
