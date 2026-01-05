using UnityEngine;

public class BossTeacherController : MonoBehaviour
{
    // Common
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPointPosition;

    [SerializeField] private GameObject rayPrefab;

    // Movement
    public PlayerController player;
    public float initialBossOffsetX = 17f;

    // Wave
    [SerializeField] private float waveAmplitudeStep = 4f;

    // ZBullet
    [SerializeField] private float zBulletAmplitudeStep = 4f;

    // Trap
    [SerializeField] private TrapController trapController;
    // Ray
    [SerializeField] private RayController rayController;

    private void Start()
    {
        player = GameManager.Instance.Player;
        trapController = GetComponent<TrapController>();
        rayController = GetComponent<RayController>();
    }

    private void FixedUpdate()
    {
        Vector3 bossPos = transform.position;
        bossPos.x = player.transform.position.x + initialBossOffsetX;
        transform.position = bossPos;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 1; i++)
            {
                rayController.ShotRay(RayType.Dynamic);
            }
        }
    }

    private void ShotProjectile(BulletBehaviour.BulletType bulletType)
    {
        GameObject projectile = Instantiate(projectilePrefab, spawnPointPosition);
        if (projectile.TryGetComponent<BulletBehaviour>(out var bulletBehaviour))
        {
            Transform playerTransform = GameManager.Instance.Player.transform;
            bulletBehaviour.Init(bulletType, playerTransform, waveAmplitudeStep, zBulletAmplitudeStep);
            waveAmplitudeStep = -waveAmplitudeStep;
        }
    }

    private void StraigthShot() => ShotProjectile(BulletBehaviour.BulletType.Straight);
    private void WaveShot() => ShotProjectile(BulletBehaviour.BulletType.Wave);

}