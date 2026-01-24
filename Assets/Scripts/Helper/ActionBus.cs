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

    public static event Action SpawnedStraightBullet;
    public static event Action SpawnedWaveBullet;
    public static event Action SpawnedZBullet;

    public static event Action TriggeredStraightBullet;
    public static event Action TriggeredWaveBullet;
    public static event Action TriggeredZBullet;

    public static void InvokeSpawnStraightBullet() => SpawnedStraightBullet?.Invoke();
    public static void InvokeSpawnWaveBullet() => SpawnedWaveBullet?.Invoke();
    public static void InvokeSpawnZBullet() => SpawnedZBullet?.Invoke();

    public static void InvokeTriggerStraightBullet() => TriggeredStraightBullet?.Invoke();
    public static void InvokeTriggerWaveBullet() => TriggeredWaveBullet?.Invoke();
    public static void InvokeTriggerZBullet() => TriggeredZBullet?.Invoke();
}