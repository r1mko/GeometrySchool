using System;
using UnityEngine;

public static class ActionBus
{
    // ========== TRAPS ==========

    public static event Action<TrapType, float> TriggeredTrap;
    public static void InvokeTriggeredTrap(TrapType trapType, float fallingTrapDuration) => TriggeredTrap?.Invoke(trapType, fallingTrapDuration);

    // ========== RAYS ==========

    public static event Action<RayType, int, bool> TriggerSpawnRay;
    public static void InvokeTriggerSpawnRay(RayType rayType, int spawnPointIndex, bool fromTop) => TriggerSpawnRay?.Invoke(rayType, spawnPointIndex, fromTop);

    // ========== BULLETS ==========

    public static event Action<BulletType, int, float, float, int> TriggeredSpawnBullet;
    public static void InvokeTriggerBullet(BulletType bulletType, int spawnPoint, float waveDuration, float zBulletDuration, int shotsAmount) => TriggeredSpawnBullet?.Invoke(bulletType, spawnPoint, waveDuration, zBulletDuration, shotsAmount);
}