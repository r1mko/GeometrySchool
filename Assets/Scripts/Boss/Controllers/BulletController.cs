using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public PlayerController player;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform[] spawnPointPosition; // 0 1 2 from top
    private float waveAmplitudeStep = 4.15f;
    private float zBulletAmplitudeStep = 4.15f;

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    public void SpawnBullet(BulletType bulletType, int spawnPoint, float waveDuration, float zBulletDuration, int shotsAmount)
    {
        StartCoroutine(ShootRoutine(bulletType, spawnPoint, waveDuration, zBulletDuration, shotsAmount));
    }

    private IEnumerator ShootRoutine(BulletType bulletType, int spawnPoint, float waveDuration, float zBulletDuration, int shotsAmount)
    {
        for (int i = 0; i < shotsAmount; i++)
        {
            ShotProjectile(bulletType, spawnPoint, waveDuration, zBulletDuration);
            if (bulletType == BulletType.Straight)
            {
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                if ((i + 1) % 2 == 0)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }

    public void ShotProjectile(BulletType bulletType, int spawnPoint, float waveDuration, float zBulletDuration)
    {
        if (bulletType == BulletType.Wave || bulletType == BulletType.ZBullet)
        {
            spawnPoint = 1;
        }
        GameObject projectile = Instantiate(projectilePrefab, spawnPointPosition[spawnPoint]);
        if (projectile.TryGetComponent<BulletBehaviour>(out var bulletBehaviour))
        {
            Transform playerTransform = player.transform;
            bulletBehaviour.Init(bulletType, playerTransform, waveAmplitudeStep, zBulletAmplitudeStep, waveDuration, zBulletDuration);
            waveAmplitudeStep = -waveAmplitudeStep;
            zBulletAmplitudeStep = -zBulletAmplitudeStep;
        }
    }
}
