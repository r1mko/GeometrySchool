using UnityEngine;

public class BossTeacherController : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPointPosition;
    private float waveAmplitudeStep = 4;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            WaveShot();
        }
    }

    private void ShotProjectile(BulletBehaviour.BulletType bulletType)
    {
        GameObject projectile = Instantiate(projectilePrefab, spawnPointPosition);
        BulletBehaviour bulletBehaviour = projectile.GetComponent<BulletBehaviour>();
        Transform playerTransform = GameManager.Instance.Player.transform;
        bulletBehaviour.Init(bulletType, playerTransform, waveAmplitudeStep);
        waveAmplitudeStep = -waveAmplitudeStep;
    }

    //for subs
    private void StraigthShot()
    {
        ShotProjectile(BulletBehaviour.BulletType.Straight);
    }

    private void WaveShot()
    {
        ShotProjectile(BulletBehaviour.BulletType.Wave);
        ShotProjectile(BulletBehaviour.BulletType.Wave);
    }
}
