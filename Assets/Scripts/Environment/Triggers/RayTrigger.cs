using System;
using UnityEngine;

public class RayTrigger : MonoBehaviour
{
    [SerializeField] private MechanicManager mechanicManager;

    [Serializable]
    public struct RayTriggerType
    {
        public RayType RayType;
        [Header("Bounds: [0, 1, 2]")]
        public int SpawnPoint;
        [Header("True: From Top; False: From Bottom")]
        public bool DynamicStartRay;
    }

    [SerializeField] private RayTriggerType currentRayTrigger;

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
                mechanicManager.SpawnRay(currentRayTrigger.RayType, currentRayTrigger.SpawnPoint, currentRayTrigger.DynamicStartRay);
                invoked = true;
            }

        }
    }
}
