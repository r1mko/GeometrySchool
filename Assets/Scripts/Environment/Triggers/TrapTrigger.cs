using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    [SerializeField] private MechanicManager mechanicManager;
    [SerializeField] private TrapType currentTrapType;
    [SerializeField] private float fallingTrapDuration = 2.5f;

    private bool invoked;

    private void OnValidate()
    {
        if (mechanicManager == null) mechanicManager = FindFirstObjectByType<MechanicManager>();
    }

    private void Start()
    {
        if (mechanicManager == null) mechanicManager = FindFirstObjectByType<MechanicManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!invoked)
            {
                mechanicManager.SpawnTrap(currentTrapType, fallingTrapDuration);
                invoked = true;
            }
        }
    }
}
