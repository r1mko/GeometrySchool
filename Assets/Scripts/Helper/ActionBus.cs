using System;
using UnityEngine;

public static class ActionBus
{
    // ========== TRAPS ==========

    public static event Action<Vector3> SpawnedStaticTrap;
    public static event Action SpawnedColumnTrap;
    public static event Action SpawnedFallingTrap;

    public static event Action TriggeredStaticTrap;
    public static event Action TriggeredColumnTrap;
    public static event Action TriggeredFallingTrap;

    public static void InvokeSpawnStaticTrap(Vector3 worldPosition) => SpawnedStaticTrap?.Invoke(worldPosition);
    public static void InvokeSpawnColumn() => SpawnedColumnTrap?.Invoke();
    public static void InvokeSpawnFallingTrap() => SpawnedFallingTrap?.Invoke();

    public static void InvokeTriggerStaticTrap() => TriggeredStaticTrap?.Invoke();
    public static void InvokeTriggerColumnTrap() => TriggeredColumnTrap?.Invoke();
    public static void InvokeTriggerFallingTrap() => TriggeredFallingTrap?.Invoke();


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