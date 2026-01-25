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
        player = GameManager.Instance.Player;
        trapController = GetComponent<TrapController>();
        rayController = GetComponent<RayController>();
        bulletController = GetComponent<BulletController>();

        //subs
        ActionBus.TriggeredTrap += SpawnTrap;
        ActionBus.TriggerSpawnRay += SpawnRay;
        ActionBus.TriggeredSpawnBullet += SpawnBullet;
    }

    private void OnDestroy()
    {
        ActionBus.TriggeredTrap -= SpawnTrap;
        ActionBus.TriggerSpawnRay -= SpawnRay;
        ActionBus.TriggeredSpawnBullet -= SpawnBullet;
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


    private void SpawnBullet(BulletType bulletType, int spawnPoint, float waveDuration, float zBulletDuration, int shotsAmount)
    {
        bulletController.SpawnBullet(bulletType, spawnPoint, waveDuration, zBulletDuration, shotsAmount);
    }

    private void SpawnTrap(TrapType trapType)
    {
        trapController.TriggerTrapPlacement(trapType);
    }

    private void SpawnRay(RayType rayType, int spawnIndex, bool fromTop)
    {
        rayController.ShotRay(rayType, spawnIndex, fromTop);
    }
}