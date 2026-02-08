using System;
using UnityEngine;

public class BulletTrigger : MonoBehaviour
{
    [Serializable]
    public struct BulletTriggerType
    {
        public BulletType BulletType;
        public int ShotsAmount;
        [Header("Bounds: [0, 1, 2]")]
        public int StraightSpawnPoint;
        [Header("Bounds: [1.75 - Middle, 2.25 - Down]")]
        public float WaveDuration;
        [Header("Bounds: [2 - Middle, 3.5 - Down]")]
        public float ZBulletDuration;
    }

    [SerializeField] private BulletTriggerType currentBulletTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ActionBus.InvokeTriggerBullet(currentBulletTrigger.BulletType, currentBulletTrigger.StraightSpawnPoint,currentBulletTrigger.WaveDuration,currentBulletTrigger.ZBulletDuration, currentBulletTrigger.ShotsAmount);
        }
    }
}
