using System;
using UnityEngine;

public static class ActionBus
{
    // ========== TRAPS ==========

    public static event Action SpawnedTrap;
    public static event Action<TrapType> TriggeredTrap;

    public static void InvokeSpawnedTrap() => SpawnedTrap?.Invoke();
    public static void InvokeTriggeredTrap(TrapType trapType) => TriggeredTrap?.Invoke(trapType);

    // ========== RAYS ==========

    public static event Action SpawnedRay;
    public static event Action<RayType,int, bool> TriggerSpawnRay;

    public static void InvokeSpawnRay() => SpawnedRay?.Invoke();
    public static void InvokeTriggerSpawnRay(RayType rayType, int spawnPointIndex, bool fromTop) => TriggerSpawnRay?.Invoke(rayType, spawnPointIndex, fromTop);

    // ========== BULLETS ==========

    public static event Action SpawnedBullet;
    public static event Action<BulletType,int, float, float, int> TriggeredSpawnBullet;

    public static void InvokeSpawnBullet() => SpawnedBullet?.Invoke();
    public static void InvokeTriggerBullet(BulletType bulletType, int spawnPoint, float waveDuration, float zBulletDuration, int shotsAmount) => TriggeredSpawnBullet?.Invoke(bulletType, spawnPoint, waveDuration, zBulletDuration, shotsAmount);
}