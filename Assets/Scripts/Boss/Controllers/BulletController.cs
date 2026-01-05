using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPointPosition;
    public PlayerController player;

    // Wave
    [SerializeField] private float waveAmplitudeStep = 4f;

    // ZBullet
    [SerializeField] private float zBulletAmplitudeStep = 4f;

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    public void ShotProjectile(BulletType bulletType)
    {
        GameObject projectile = Instantiate(projectilePrefab, spawnPointPosition);
        if (projectile.TryGetComponent<BulletBehaviour>(out var bulletBehaviour))
        {
            Transform playerTransform = GameManager.Instance.Player.transform;
            bulletBehaviour.Init(bulletType, playerTransform, waveAmplitudeStep, zBulletAmplitudeStep);
            waveAmplitudeStep = -waveAmplitudeStep;
        }
    }
}
