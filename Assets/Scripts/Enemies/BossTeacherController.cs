using System.Collections.Generic;
using UnityEngine;

public class BossTeacherController : MonoBehaviour
{
    // Common
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPointPosition;

    // Wave
    [SerializeField] private float waveAmplitudeStep = 4f;

    // Movement
    public PlayerController player;
    public float initialBossOffsetX = 17f;

    // Trap
    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private float closeOffsetX = 3f;
    [SerializeField] private float middleOffsetX = 6f;
    [SerializeField] private float longOffsetX = 9f;

    [SerializeField] private float minY = -4.2f;
    [SerializeField] private float maxY = 4.2f;
    [SerializeField] private float minTrapSpacingX = 2f; // ← теперь единый отступ по X

    // Теперь храним ВСЕ активные X-позиции ловушек (всех типов)
    private List<float> activeTrapXPositions = new();

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetTrap(TrapBehaviour.TrapType.Middle);
        }
    }

    private void FixedUpdate()
    {
        Vector3 bossPos = transform.position;
        bossPos.x = player.transform.position.x + initialBossOffsetX;
        transform.position = new Vector3(bossPos.x, transform.position.y, transform.position.z);
    }

    private void ShotProjectile(BulletBehaviour.BulletType bulletType)
    {
        GameObject projectile = Instantiate(projectilePrefab, spawnPointPosition);
        BulletBehaviour bulletBehaviour = projectile.GetComponent<BulletBehaviour>();
        Transform playerTransform = GameManager.Instance.Player.transform;
        bulletBehaviour.Init(bulletType, playerTransform, waveAmplitudeStep);
        waveAmplitudeStep = -waveAmplitudeStep;
    }

    public void SetTrap(TrapBehaviour.TrapType? trapType = null)
    {
        TrapBehaviour.TrapType actualType = trapType ?? GetRandomTrapType();

        float offsetX = actualType switch
        {
            TrapBehaviour.TrapType.Close => closeOffsetX,
            TrapBehaviour.TrapType.Middle => middleOffsetX,
            TrapBehaviour.TrapType.Long => longOffsetX,
            _ => middleOffsetX
        };

        // Случайный Y в строго заданных границах
        float spawnY = Random.Range(minY, maxY);

        // Базовая X-позиция (от игрока + offsetX)
        float baseX = player.transform.position.x + offsetX;

        // Найдём ближайшую X-позицию, удалённую минимум на minTrapSpacingX от всех существующих
        float finalX = FindValidXPosition(baseX, minTrapSpacingX);

        Vector3 spawnPos = new Vector3(finalX, spawnY, 0);

        GameObject trapObj = Instantiate(trapPrefab, spawnPos, Quaternion.identity);
        TrapBehaviour trap = trapObj.GetComponent<TrapBehaviour>();
        if (trap != null)
        {
            trap.Init(actualType, player.transform, this, spawnPos);
            activeTrapXPositions.Add(finalX);

            Debug.Log($"[Trap Spawned] Type: {actualType}, Position: X={spawnPos.x:F2}, Y={spawnPos.y:F2}");
        }
    }

    private float FindValidXPosition(float desiredX, float minSpacing)
    {
        float candidateX = desiredX;
        for (int attempt = 0; attempt < 20; attempt++)
        {
            bool valid = true;
            foreach (float x in activeTrapXPositions)
            {
                if (Mathf.Abs(candidateX - x) < minSpacing)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
                return candidateX;
            candidateX += minSpacing + 0.1f;
        }
        return desiredX;
    }

    public void OnTrapDestroyed(float trapX)
    {
        for (int i = activeTrapXPositions.Count - 1; i >= 0; i--)
        {
            if (Mathf.Abs(activeTrapXPositions[i] - trapX) < 0.1f)
            {
                activeTrapXPositions.RemoveAt(i);
                break;
            }
        }
    }

    private TrapBehaviour.TrapType GetRandomTrapType()
    {
        return Random.Range(0, 3) switch
        {
            0 => TrapBehaviour.TrapType.Close,
            1 => TrapBehaviour.TrapType.Middle,
            _ => TrapBehaviour.TrapType.Long
        };
    }

    // For subs
    private void StraigthShot()
    {
        ShotProjectile(BulletBehaviour.BulletType.Straight);
    }

    private void WaveShot()
    {
        ShotProjectile(BulletBehaviour.BulletType.Wave);
    }

    private void RayShot()
    {
        ShotProjectile(BulletBehaviour.BulletType.Ray);
    }
}