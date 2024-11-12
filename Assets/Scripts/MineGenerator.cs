using UnityEngine;
using System.Collections.Generic;

public class MineGenerator : MonoBehaviour {
    [Header("Grid Settings")] [SerializeField]
    private int gridWidth = 10;

    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float cellSize = 10f;

    [Header("Room Size Settings")] [SerializeField]
    private int minRoomWidth = 2;

    [SerializeField] private int maxRoomWidth = 4;
    [SerializeField] private int minRoomLength = 2;
    [SerializeField] private int maxRoomLength = 4;


    [Header("Room Generation Settings")] [SerializeField]
    private int minRoomCount = 7; // Minimum number of rooms to generate

    private bool[,] grid; // Tracks which cells have rooms
    private List<Vector2Int> roomList = new List<Vector2Int>();

    private void Start() {
        grid = new bool[gridWidth, gridHeight];
        GenerateMineWithGrowingTree();
    }

    private void GenerateMineWithGrowingTree() {
        // Start in the center of the grid
        Vector2Int startCell = new Vector2Int(gridWidth / 2, gridHeight / 2);
        roomList.Add(startCell);
        grid[startCell.x, startCell.y] = true;
        CreateRoom(startCell.x, startCell.y); // Initial room

        int roomsPlaced = 1;

        while (roomsPlaced < minRoomCount && roomList.Count > 0) {
            // Choose a random cell from the list
            Vector2Int currentCell = roomList[Random.Range(0, roomList.Count)];

            // Get available neighbors
            List<Vector2Int> neighbors = GetValidNeighbors(currentCell);

            if (neighbors.Count > 0) {
                // Pick a random neighbor to grow a new room
                Vector2Int neighbor = neighbors[Random.Range(0, neighbors.Count)];

                if (CreateRoom(neighbor.x, neighbor.y)) // Attempt to place a room
                {
                    grid[neighbor.x, neighbor.y] = true;
                    roomList.Add(neighbor);
                    roomsPlaced++;
                }
            }
            else {
                // No valid neighbors left, remove this cell from the list
                roomList.Remove(currentCell);
            }
        }

        if (roomsPlaced < minRoomCount) {
            Debug.LogWarning("Minimum room count not reached. Consider increasing grid size.");
        }
    }

    private List<Vector2Int> GetValidNeighbors(Vector2Int cell) {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int direction in directions) {
            Vector2Int neighbor = cell + direction * 2; // Use a gap of two to allow room sizes
            if (neighbor.x >= 0 && neighbor.x < gridWidth && neighbor.y >= 0 && neighbor.y < gridHeight &&
                !grid[neighbor.x, neighbor.y]) {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private bool CanPlaceRoom(int startX, int startY, int roomWidth, int roomLength) {
        for (int x = startX; x < startX + roomWidth; x++) {
            for (int y = startY; y < startY + roomLength; y++) {
                if (x >= gridWidth || y >= gridHeight || grid[x, y])
                    return false;
            }
        }

        return true;
    }

    private void MarkGridOccupied(int startX, int startY, int roomWidth, int roomLength) {
        for (int x = startX; x < startX + roomWidth; x++) {
            for (int y = startY; y < startY + roomLength; y++) {
                grid[x, y] = true;
            }
        }
    }

    private bool CreateRoom(int x, int y) {
        int roomWidth = Random.Range(minRoomWidth, maxRoomWidth);
        int roomLength = Random.Range(minRoomLength, maxRoomLength);

        if (!CanPlaceRoom(x, y, roomWidth, roomLength))
            return false;

        MarkGridOccupied(x, y, roomWidth, roomLength);

        Vector3 roomPosition = new Vector3(x * cellSize, 0, y * cellSize);
        GameObject room = new GameObject("Room_" + x + "_" + y);
        room.transform.position = roomPosition;

        Color color = Random.ColorHSV();

        for (int i = 0; i < roomWidth; i++) {
            for (int j = 0; j < roomLength; j++) {
                Vector3 tilePosition = roomPosition + new Vector3(i * cellSize, 0, j * cellSize);
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);

                Renderer renderer = tile.GetComponent<Renderer>();
                renderer.material.color = color;

                tile.transform.position = tilePosition;
                tile.transform.localScale = new Vector3(cellSize, 1f, cellSize);
                tile.transform.parent = room.transform;
            }
        }

        return true;
    }
}