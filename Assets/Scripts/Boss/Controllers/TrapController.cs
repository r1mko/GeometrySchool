using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BossTeacherController))]
public class TrapController : MonoBehaviour
{
    [System.Serializable]
    public class SpawnCell
    {
        public Transform spawnPoint;
        public int row;
        public int column;
        public bool isBlocked = false;
    }

    [Header("References")]
    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private Transform spawnPointsContainer;

    [Header("Settings")]
    [SerializeField] private List<SpawnCell> spawnCells = new();
    [SerializeField] private float minPlayerDistance = 4f;
    [SerializeField] private float resetOffset = 1f;

    private PlayerController player;

    private Vector3 originalContainerLocalPosition;
    private bool isTrapSystemActive;
    private HashSet<SpawnCell> usedCells = new();
    private Dictionary<int, int> availableCellsPerRow = new();
    private float lastColumnWorldX;

    private void Start()
    {
        player = GameManager.Instance.Player;

        originalContainerLocalPosition = spawnPointsContainer.localPosition;
        CacheLastColumnPosition();
        ResetTrapSystem();
    }

    private void Update()
    {
        if (!isTrapSystemActive) return;

        if (player.transform.position.x > lastColumnWorldX + resetOffset)
        {
            ResetTrapSystem();
        }
    }

    private void FixedUpdate()
    {
        if (!isTrapSystemActive)
        {
            spawnPointsContainer.localPosition = originalContainerLocalPosition;
        }
    }

    public void TriggerTrapPlacement()
    {
        if (!isTrapSystemActive)
        {
            ActivateTrapSystem();
        }
        SpawnRandomTrap();
    }

    private void ActivateTrapSystem()
    {
        isTrapSystemActive = true;
        usedCells.Clear();
        ResetBlockedStates();
        DetachSpawnContainer();
        UpdateLastColumnWorldPosition();
        CacheAvailableCellsPerRow();
    }

    private void ResetTrapSystem()
    {
        isTrapSystemActive = false;
        usedCells.Clear();
        ResetBlockedStates();
        AttachSpawnContainer();
    }

    private void ResetBlockedStates()
    {
        foreach (var cell in spawnCells)
            cell.isBlocked = false;
    }

    private void DetachSpawnContainer()
    {
        spawnPointsContainer.SetParent(null);
    }

    private void AttachSpawnContainer()
    {
        spawnPointsContainer.SetParent(transform);
        spawnPointsContainer.localPosition = originalContainerLocalPosition;
    }

    private void SpawnRandomTrap()
    {
        UpdateBlockedStates();
        UpdateAvailableCellsPerRow();

        var eligibleRows = availableCellsPerRow
            .Where(kvp => kvp.Value >= 2) // Оставляем хотя бы 1 клетку свободной в ряду
            .Select(kvp => kvp.Key)
            .ToList();

        if (eligibleRows.Count == 0) return;

        int randomRow = eligibleRows[Random.Range(0, eligibleRows.Count)];
        var availableCells = GetAvailableCellsInRow(randomRow);

        if (availableCells.Count == 0) return;

        SpawnCell selectedCell = availableCells[Random.Range(0, availableCells.Count)];
        usedCells.Add(selectedCell);
        selectedCell.isBlocked = true; // Блокируем после использования

        GameObject trapObj = Instantiate(trapPrefab, selectedCell.spawnPoint.position, Quaternion.identity);
        if (trapObj.TryGetComponent<TrapBehaviour>(out var trap))
        {
            trap.Init(TrapBehaviour.TrapType.Middle, player.transform, null, selectedCell.spawnPoint.position);
        }

        availableCellsPerRow[randomRow]--;
    }

    private void UpdateBlockedStates()
    {
        float playerX = player.transform.position.x;
        foreach (var cell in spawnCells)
        {
            cell.isBlocked = usedCells.Contains(cell) ||
                             cell.spawnPoint.position.x < playerX + minPlayerDistance;
        }
    }

    private void UpdateAvailableCellsPerRow()
    {
        availableCellsPerRow.Clear();
        foreach (var cell in spawnCells)
        {
            if (!cell.isBlocked)
            {
                if (!availableCellsPerRow.ContainsKey(cell.row))
                    availableCellsPerRow[cell.row] = 0;
                availableCellsPerRow[cell.row]++;
            }
        }
    }

    private void CacheAvailableCellsPerRow()
    {
        availableCellsPerRow = spawnCells
            .Where(cell => !cell.isBlocked)
            .GroupBy(cell => cell.row)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private List<SpawnCell> GetAvailableCellsInRow(int row)
    {
        return spawnCells
            .Where(cell => cell.row == row && !cell.isBlocked && !usedCells.Contains(cell))
            .ToList();
    }

    private void CacheLastColumnPosition()
    {
        if (spawnCells.Count == 0) return;
        int lastColumn = spawnCells.Max(cell => cell.column);
        lastColumnWorldX = spawnCells
            .First(cell => cell.column == lastColumn)
            .spawnPoint.position.x;
    }

    private void UpdateLastColumnWorldPosition()
    {
        if (spawnCells.Count == 0) return;
        int lastColumn = spawnCells.Max(cell => cell.column);
        lastColumnWorldX = spawnPointsContainer.TransformPoint(
            spawnCells.First(cell => cell.column == lastColumn).spawnPoint.localPosition
        ).x;
    }
}