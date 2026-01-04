using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BossTeacherController))]
public class TrapController : MonoBehaviour
{
    [Serializable]
    public class SpawnCell
    {
        public Transform spawnPoint;
        public int row;
        public int column;
        public bool isBlocked = false;
    }

    [Serializable]
    public class TrapSystem
    {
        [Header("Trap Type Settings")]
        public TrapType type;
        public GameObject trapPrefab;
        public Transform spawnPointsContainer;
        public List<SpawnCell> spawnCells = new();
        public float resetOffset;
        public float minPlayerDistance;

        [Header("Runtime Data")]
        [NonSerialized] public Vector3 originalContainerLocalPosition;
        [NonSerialized] public bool isTrapSystemActive;
        [NonSerialized] public HashSet<SpawnCell> usedCells = new();
        [NonSerialized] public Dictionary<int, int> availableCellsPerRow = new();
        [NonSerialized] public float lastColumnWorldX;
        [NonSerialized] public int spawnAttemptCount;
        [NonSerialized] public int successfulSpawnCount;
    }

    [Header("Trap Systems")]
    [SerializeField] private TrapSystem staticTrapSystem = new() { type = TrapType.Static };
    [SerializeField] private TrapSystem fallingTrapSystem = new() { type = TrapType.Falling };

    private PlayerController player;

    private void Start()
    {
        player = GameManager.Instance.Player;

        InitializeTrapSystem(staticTrapSystem);
        InitializeTrapSystem(fallingTrapSystem);
    }

    private void InitializeTrapSystem(TrapSystem system)
    {
        if (system.spawnPointsContainer == null)
        {
            Debug.LogError($"[TrapController] {system.type} trap system missing spawn points container!");
            return;
        }

        system.originalContainerLocalPosition = system.spawnPointsContainer.localPosition;
        CacheLastColumnPosition(system);
        ResetTrapSystem(system);
    }

    private void Update()
    {
        CheckResetConditions(staticTrapSystem);
        CheckResetConditions(fallingTrapSystem);
    }

    private void CheckResetConditions(TrapSystem system)
    {
        if (!system.isTrapSystemActive) return;

        if (player.transform.position.x > system.lastColumnWorldX + system.resetOffset)
        {
            ResetTrapSystem(system);
        }
    }

    private void FixedUpdate()
    {
        if (!staticTrapSystem.isTrapSystemActive)
            staticTrapSystem.spawnPointsContainer.localPosition = staticTrapSystem.originalContainerLocalPosition;

        if (!fallingTrapSystem.isTrapSystemActive)
            fallingTrapSystem.spawnPointsContainer.localPosition = fallingTrapSystem.originalContainerLocalPosition;
    }

    public void TriggerTrapPlacement(TrapType trapType)
    {
        TrapSystem targetSystem = trapType == TrapType.Static ? staticTrapSystem : fallingTrapSystem;
        targetSystem.spawnAttemptCount++;

        if (!targetSystem.isTrapSystemActive)
        {
            ActivateTrapSystem(targetSystem);
        }

        SpawnRandomTrap(targetSystem);

        if (targetSystem.spawnAttemptCount >= 18)
        {
            targetSystem.spawnAttemptCount = 0;
            targetSystem.successfulSpawnCount = 0;
        }
    }

    private void ActivateTrapSystem(TrapSystem system)
    {
        system.isTrapSystemActive = true;
        system.usedCells.Clear();
        ResetBlockedStates(system);
        DetachSpawnContainer(system);
        UpdateLastColumnWorldPosition(system);
        CacheAvailableCellsPerRow(system);
    }

    private void ResetTrapSystem(TrapSystem system)
    {
        system.isTrapSystemActive = false;
        system.usedCells.Clear();
        ResetBlockedStates(system);
        AttachSpawnContainer(system);
    }

    private void ResetBlockedStates(TrapSystem system)
    {
        foreach (var cell in system.spawnCells)
            cell.isBlocked = false;
    }

    private void DetachSpawnContainer(TrapSystem system)
    {
        system.spawnPointsContainer.SetParent(null);
    }

    private void AttachSpawnContainer(TrapSystem system)
    {
        system.spawnPointsContainer.SetParent(transform);
        system.spawnPointsContainer.localPosition = system.originalContainerLocalPosition;
    }

    private void SpawnRandomTrap(TrapSystem system)
    {
        UpdateBlockedStates(system);
        UpdateAvailableCellsPerRow(system);

        int totalAvailable = system.spawnCells.Count(cell => !cell.isBlocked && !system.usedCells.Contains(cell));

        if (totalAvailable == 0)
        {
            Debug.LogWarning($"[{system.type} System] NO AVAILABLE CELLS FOR SPAWN!");
            return;
        }

        var eligibleRows = system.availableCellsPerRow
            .Where(kvp => kvp.Value >= 1)
            .Select(kvp => kvp.Key)
            .ToList();

        if (eligibleRows.Count == 0)
        {
            Debug.LogWarning($"[{system.type} System] NO ELIGIBLE ROWS! Availability: {GetRowAvailabilityString(system)}");
            return;
        }

        int randomRow = eligibleRows[Random.Range(0, eligibleRows.Count)];
        var availableCells = GetAvailableCellsInRow(system, randomRow);

        if (availableCells.Count == 0)
        {
            Debug.LogWarning($"[{system.type} System] No available cells in row {randomRow} despite count: {system.availableCellsPerRow[randomRow]}");
            return;
        }

        SpawnCell selectedCell = availableCells[Random.Range(0, availableCells.Count)];
        system.usedCells.Add(selectedCell);
        selectedCell.isBlocked = true;

        GameObject trapObj = Instantiate(system.trapPrefab, selectedCell.spawnPoint.position, Quaternion.identity);
        if (trapObj.TryGetComponent<TrapBehaviour>(out var trap))
        {
            TrapType behaviourType =
                system.type == TrapType.Static
                    ? TrapType.Static
                    : TrapType.Falling;

            trap.Init(player.transform, null, selectedCell.spawnPoint.position, behaviourType);
        }

        system.availableCellsPerRow[randomRow]--;
        system.successfulSpawnCount++;
    }

    private void UpdateBlockedStates(TrapSystem system)
    {
        float playerX = player.transform.position.x;

        foreach (var cell in system.spawnCells)
        {
            bool wasBlocked = cell.isBlocked;
            bool isUsed = system.usedCells.Contains(cell);
            bool isTooClose = cell.spawnPoint.position.x < playerX + system.minPlayerDistance;

            cell.isBlocked = isUsed || isTooClose;
        }
    }

    private void UpdateAvailableCellsPerRow(TrapSystem system)
    {
        system.availableCellsPerRow.Clear();
        foreach (var cell in system.spawnCells)
        {
            if (!cell.isBlocked)
            {
                if (!system.availableCellsPerRow.ContainsKey(cell.row))
                    system.availableCellsPerRow[cell.row] = 0;
                system.availableCellsPerRow[cell.row]++;
            }
        }
    }

    private void CacheAvailableCellsPerRow(TrapSystem system)
    {
        system.availableCellsPerRow = system.spawnCells
            .Where(cell => !cell.isBlocked)
            .GroupBy(cell => cell.row)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private List<SpawnCell> GetAvailableCellsInRow(TrapSystem system, int row)
    {
        return system.spawnCells
            .Where(cell => cell.row == row && !cell.isBlocked && !system.usedCells.Contains(cell))
            .ToList();
    }

    private void CacheLastColumnPosition(TrapSystem system)
    {
        if (system.spawnCells.Count == 0) return;

        int lastColumn = system.spawnCells.Max(cell => cell.column);
        system.lastColumnWorldX = system.spawnCells
            .First(cell => cell.column == lastColumn)
            .spawnPoint.position.x;
    }

    private void UpdateLastColumnWorldPosition(TrapSystem system)
    {
        if (system.spawnCells.Count == 0) return;

        int lastColumn = system.spawnCells.Max(cell => cell.column);
        system.lastColumnWorldX = system.spawnPointsContainer.TransformPoint(
            system.spawnCells.First(cell => cell.column == lastColumn).spawnPoint.localPosition
        ).x;
    }

    private string GetRowAvailabilityString(TrapSystem system)
    {
        return string.Join(", ", system.availableCellsPerRow.Select(kvp => $"Row {kvp.Key}: {kvp.Value}"));
    }
}