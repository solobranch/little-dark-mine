using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode] // Allows the script to run in edit mode
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
    private int minRoomCount = 7;

    [SerializeField] private int maxRetries = 5000;

    private bool[,] grid;
    private List<Vector2Int> roomList = new List<Vector2Int>();

    public void GenerateMine() {
        // Initialize the grid and clear previous rooms
        grid = new bool[gridWidth, gridHeight];
        roomList.Clear();

        // Clear previously generated rooms in the scene
        // Make sure to destroy the generated rooms and all their children
        foreach (Transform child in transform) {
            DestroyImmediate(child.gameObject); // Clear previously generated rooms
        }

        // Start room generation
        Vector2Int startCell = new Vector2Int(gridWidth / 2, gridHeight / 2);
        roomList.Add(startCell);
        grid[startCell.x, startCell.y] = true;
        CreateRoom(startCell.x, startCell.y);

        int roomsPlaced = 1;
        int retries = 0;

        // Try to place rooms until the desired room count is reached or retries exceed the limit
        while (roomsPlaced < minRoomCount && retries < maxRetries) {
            Vector2Int currentCell = roomList[Random.Range(0, roomList.Count)];
            List<Vector2Int> neighbors = GetNeighboringCells(currentCell);

            if (neighbors.Count > 0) {
                Vector2Int neighbor = neighbors[Random.Range(0, neighbors.Count)];

                if (CreateRoom(neighbor.x, neighbor.y)) {
                    grid[neighbor.x, neighbor.y] = true;
                    roomList.Add(neighbor);
                    roomsPlaced++;
                    retries = 0; // Reset retries on successful room placement
                }
            }
            else {
                retries++;
            }
        }

        if (roomsPlaced < minRoomCount) {
            Debug.LogWarning("Minimum room count not reached. Consider increasing grid size or adjusting parameters.");
        }
    }

    private List<Vector2Int> GetNeighboringCells(Vector2Int cell) {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int direction in directions) {
            for (int distance = 1; distance <= 2; distance++) {
                Vector2Int neighbor = cell + direction * distance;
                if (neighbor.x >= 0 && neighbor.x < gridWidth && neighbor.y >= 0 && neighbor.y < gridHeight &&
                    !grid[neighbor.x, neighbor.y]) {
                    neighbors.Add(neighbor);
                }
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
        room.transform.parent = transform; // Set the parent to keep the hierarchy clean

        Color roomColor = Random.ColorHSV(); // Generate a unique color for each room

        // Create a new material for each room to avoid material sharing
        Material roomMaterial = new Material(Shader.Find("Standard"));
        roomMaterial.color = roomColor;

        for (int i = 0; i < roomWidth; i++) {
            for (int j = 0; j < roomLength; j++) {
                Vector3 tilePosition = roomPosition + new Vector3(i * cellSize, 0, j * cellSize);
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);

                Renderer renderer = tile.GetComponent<Renderer>();
                renderer.material = roomMaterial; // Assign the unique material to the tile

                tile.transform.position = tilePosition;
                tile.transform.localScale = new Vector3(cellSize, 1f, cellSize);
                tile.transform.parent = room.transform;
            }
        }

        return true;
    }
}