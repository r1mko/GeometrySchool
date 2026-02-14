using System.Collections;
using UnityEngine;

public class MechanicManager : MonoBehaviour
{
    public PlayerController player;
    public float initialBossOffsetX;

    [SerializeField] private TrapController trapController;
    [SerializeField] private RayController rayController;
    [SerializeField] private BulletController bulletController;

    private void Start()
    {
        if (trapController == null) trapController = GetComponent<TrapController>();
        if (rayController == null) rayController = GetComponent<RayController>();
        if (bulletController == null) bulletController = GetComponent<BulletController>();
        if (player == null) player = GameManager.Instance.Player;

    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        Vector3 bossPos = transform.position;
        bossPos.x = player.transform.position.x + initialBossOffsetX;
        transform.position = bossPos;
    }


    public void SpawnBullet(BulletType bulletType, int spawnPoint, float waveDuration, float zBulletDuration, int shotsAmount)
    {
        bulletController.SpawnBullet(bulletType, spawnPoint, waveDuration, zBulletDuration, shotsAmount);
    }

    public void SpawnTrap(TrapType trapType, float fallingTrapDuration)
    {
        trapController.TriggerTrapPlacement(trapType, fallingTrapDuration);
    }

    public void SpawnRay(RayType rayType, int spawnIndex, bool fromTop)
    {
        rayController.ShotRay(rayType, spawnIndex, fromTop);
    }
}