using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MechanicManager))]
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
        [NonSerialized] public float lastColumnWorldX;
        [NonSerialized] public int spawnAttemptCount;
        [NonSerialized] public int successfulSpawnCount;
        [NonSerialized] public int lastSpawnedRow = -1;
    }

    [Header("Trap Systems")]
    [SerializeField] private TrapSystem staticTrapSystem = new() { type = TrapType.Static };
    [SerializeField] private TrapSystem fallingTrapSystem = new() { type = TrapType.Falling };
    [SerializeField] private TrapSystem columnTrapSystem = new() { type = TrapType.Column };
    [SerializeField] private float columnSpawnDelay;

    private PlayerController player;

    private void Start()
    {
        player = GameManager.Instance.Player;

        InitializeTrapSystem(staticTrapSystem);
        InitializeTrapSystem(fallingTrapSystem);
        InitializeTrapSystem(columnTrapSystem);
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
        CheckResetConditions(columnTrapSystem);
    }

    private void FixedUpdate()
    {
        if (!staticTrapSystem.isTrapSystemActive)
            staticTrapSystem.spawnPointsContainer.localPosition = staticTrapSystem.originalContainerLocalPosition;

        if (!fallingTrapSystem.isTrapSystemActive)
            fallingTrapSystem.spawnPointsContainer.localPosition = fallingTrapSystem.originalContainerLocalPosition;

        if (!columnTrapSystem.isTrapSystemActive)
            columnTrapSystem.spawnPointsContainer.localPosition = columnTrapSystem.originalContainerLocalPosition;
    }

    public void TriggerTrapPlacement(TrapType trapType)
    {
        TrapSystem targetSystem = trapType switch
        {
            TrapType.Static => staticTrapSystem,
            TrapType.Falling => fallingTrapSystem,
            TrapType.Column => columnTrapSystem,
            _ => null
        };

        if (targetSystem == null)
        {
            Debug.LogError($"[TrapController] Unsupported trap type: {trapType}");
            return;
        }

        targetSystem.spawnAttemptCount++;

        if (!targetSystem.isTrapSystemActive)
        {
            ActivateTrapSystem(targetSystem);
        }

        if (trapType == TrapType.Column)
        {
            StartCoroutine(SpawnNextAvailableRow(targetSystem));
        }
        else
        {
            StartCoroutine(SpawnRandomTrap(targetSystem));
        }

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
        system.lastSpawnedRow = -1;
        ResetBlockedStates(system);
        DetachSpawnContainer(system);
        UpdateLastColumnWorldPosition(system);
    }

    private void ResetTrapSystem(TrapSystem system)
    {
        system.isTrapSystemActive = false;
        system.usedCells.Clear();
        system.lastSpawnedRow = -1;
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

        Vector3 pos = system.spawnPointsContainer.position;
        pos.x = RoundUpToOdd(pos.x);
        system.spawnPointsContainer.position = pos;

        UpdateLastColumnWorldPosition(system);
    }

    private void AttachSpawnContainer(TrapSystem system)
    {
        system.spawnPointsContainer.SetParent(transform);
        system.spawnPointsContainer.localPosition = system.originalContainerLocalPosition;
    }

    // === Static / Falling ===
    private IEnumerator SpawnRandomTrap(TrapSystem system)
    {
        UpdateBlockedStates(system);

        int totalAvailable = system.spawnCells.Count(cell => !cell.isBlocked && !system.usedCells.Contains(cell));

        if (totalAvailable == 0)
        {
            Debug.LogWarning($"[{system.type} System] NO AVAILABLE CELLS FOR SPAWN!");
            yield break;
        }

        var eligibleRows = system.spawnCells
            .Where(cell => !cell.isBlocked && !system.usedCells.Contains(cell))
            .GroupBy(cell => cell.row)
            .Where(g => g.Any())
            .Select(g => g.Key)
            .ToList();

        if (eligibleRows.Count == 0)
        {
            Debug.LogWarning($"[{system.type} System] NO ELIGIBLE ROWS!");
            yield break;
        }

        int randomRow = eligibleRows[Random.Range(0, eligibleRows.Count)];
        var availableCells = system.spawnCells
            .Where(cell => cell.row == randomRow && !cell.isBlocked && !system.usedCells.Contains(cell))
            .ToList();

        if (availableCells.Count == 0) yield break;

        SpawnCell selectedCell = availableCells[Random.Range(0, availableCells.Count)];
        system.usedCells.Add(selectedCell);
        selectedCell.isBlocked = true;

        if (system.type == TrapType.Static)
        {
            ActionBus.InvokeSpawnStaticTrap(selectedCell.spawnPoint.position);
        }
        else if (system.type == TrapType.Falling)
        {
            ActionBus.InvokeSpawnFallingTrap();
        }

        GameObject trapObj = Instantiate(system.trapPrefab, selectedCell.spawnPoint.position, Quaternion.identity);

        if (trapObj.TryGetComponent<TrapBehaviour>(out var trap))
        {
            trap.Init(player.transform, null, selectedCell.spawnPoint.position, system.type);
        }

        system.successfulSpawnCount++;
    }

    // === COLUMN LOGIC ===
    private IEnumerator SpawnNextAvailableRow(TrapSystem system)
    {
        UpdateBlockedStates(system);

        var rowsOrdered = system.spawnCells
            .GroupBy(cell => cell.row)
            .Select(g => new { Row = g.Key, Cells = g.ToList() })
            .OrderBy(item => item.Row)
            .ToList();

        if (rowsOrdered.Count == 0)
        {
            Debug.LogWarning("[Column System] No rows found!");
            yield break;
        }

        var targetRowData = rowsOrdered
            .Where(rowGroup => rowGroup.Row > system.lastSpawnedRow)
            .FirstOrDefault(rowGroup =>
            {
                var availableCells = rowGroup.Cells
                        .Where(cell => !cell.isBlocked && !system.usedCells.Contains(cell))
                        .ToList();

                return availableCells.Count > 0;
            });

        if (targetRowData == null)
        {
            Debug.LogWarning("[Column System] No next row available!");
            yield break;
        }

        var availableCellsInRow = targetRowData.Cells
            .Where(cell => !cell.isBlocked && !system.usedCells.Contains(cell))
            .ToList();

        if (availableCellsInRow.Count == 0)
        {
            Debug.LogWarning($"[Column System] Row {targetRowData.Row} has no available cells!");
            system.lastSpawnedRow = targetRowData.Row;
            yield break;
        }

        SpawnCell skippedCell = availableCellsInRow[Random.Range(0, availableCellsInRow.Count)];

        int spawnedCount = 0;
        system.lastSpawnedRow = targetRowData.Row;
        system.successfulSpawnCount += spawnedCount;

        ActionBus.InvokeSpawnColumn();

        foreach (var cell in availableCellsInRow)
        {
            if (cell == skippedCell) continue;

            GameObject trapObj = Instantiate(system.trapPrefab, cell.spawnPoint.position, Quaternion.identity);
            if (trapObj.TryGetComponent<TrapBehaviour>(out var trap))
            {
                trap.Init(player.transform, null, cell.spawnPoint.position, TrapType.Column);
            }

            system.usedCells.Add(cell);
            cell.isBlocked = true;
            spawnedCount++;
            yield return new WaitForSeconds(columnSpawnDelay); //TO DO
        }

        Debug.Log($"[Column System] Spawned row {targetRowData.Row}: {spawnedCount} traps, skipped 1 random cell (total available: {availableCellsInRow.Count})");
    }

    private void UpdateBlockedStates(TrapSystem system)
    {
        float playerX = player.transform.position.x;
        foreach (var cell in system.spawnCells)
        {
            bool isUsed = system.usedCells.Contains(cell);
            bool isTooClose = cell.spawnPoint.position.x < playerX + system.minPlayerDistance;
            cell.isBlocked = isUsed || isTooClose;
        }
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

    private void CheckResetConditions(TrapSystem system)
    {
        if (!system.isTrapSystemActive) return;
        if (player != null && player.transform.position.x > system.lastColumnWorldX + system.resetOffset)
        {
            ResetTrapSystem(system);
        }
    }

    private float RoundUpToOdd(float value)
    {
        int rounded = Mathf.CeilToInt(value);
        return (rounded % 2 == 0) ? rounded + 1 : rounded;
    }
}