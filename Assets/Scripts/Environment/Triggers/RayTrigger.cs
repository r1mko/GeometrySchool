using System;
using UnityEngine;

public class RayTrigger : MonoBehaviour
{
    [Serializable]
    public struct RayTriggerType
    {
        public RayType RayType;
        [Header("Bounds: [0, 1, 2]")]
        public int SpawnPoint;
        [Header("Just for dynamic")]
        public bool FromTop;
    }

    [SerializeField] private RayTriggerType currentRayTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ActionBus.InvokeTriggerSpawnRay(currentRayTrigger.RayType, currentRayTrigger.SpawnPoint, currentRayTrigger.FromTop);
            Debug.Log($"Спавним луч: {currentRayTrigger.RayType}; в точке {currentRayTrigger.SpawnPoint}");
        }
    }
}
