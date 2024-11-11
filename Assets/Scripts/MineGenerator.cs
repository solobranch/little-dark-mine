using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MineGenerator : MonoBehaviour {
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float cellSize = 10f;

    [SerializeField] private int minRoomWidth = 5;
    [SerializeField] private int maxRoomWidth = 10;
    [SerializeField] private int minRoomLength = 5;
    [SerializeField] private int maxRoomLength = 10;

    [SerializeField] private int minRoomCount = 7;

    private bool[,] grid;

    private void Start() {
        grid = new bool[gridWidth, gridHeight];
        GenerateRooms();
    }

    private void GenerateRooms() {
        Stack<Vector2Int> cellStack = new Stack<Vector2Int>();
        Vector2Int currentCell = new Vector2Int(gridWidth / 2, gridHeight / 2);
        cellStack.Push(currentCell);

        int maxRetries = 100;

        while (cellStack.Count > 0) {
            currentCell = cellStack.Pop();

            List<Vector2Int> neighbours = GetUnvisitedNeighbours(currentCell); // return function

            neighbours = ShuffleList(neighbours);

            bool roomPlaced = false;

            foreach (Vector2Int neighbour in neighbours) {
                int retries = 0;
                while (retries < maxRetries) {
                    if (CreateRoom(neighbour.x, neighbour.y)) {
                        grid[neighbour.x, neighbour.y] = true;
                        cellStack.Push(neighbour);
                        roomPlaced = true;
                        break;
                    }

                    retries++;
                }

                if (roomPlaced) {
                    break;
                }
            }
        }
    }

    private List<Vector2Int> ShuffleList(List<Vector2Int> list) {
        for (int i = 0; i < list.Count; i++) {
            Vector2Int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }

        return list;
    }

    private List<Vector2Int> GetUnvisitedNeighbours(Vector2Int cell) {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        Vector2Int[] directions = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int direction in directions) {
            Vector2Int neighbour = cell + direction * 2;
            if (neighbour.x >= 0 && neighbour.x < gridWidth && neighbour.y >= 0 && neighbour.y < gridHeight &&
                !grid[neighbour.x, neighbour.y]) {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }

    private bool CanPlaceRoom(int startX, int startY, int roomWidth, int roomLength) {
        for (int x = 0; x < startX + roomWidth; x++) {
            for (int y = 0; y < startY + roomLength; y++) {
                if (x >= gridHeight || y >= gridHeight || grid[x, y]) {
                    return false;
                }
            }
        }

        return true;
    }

    private bool CreateRoom(int x, int y) {
        int roomWidth = Random.Range(minRoomWidth, maxRoomWidth);
        int roomLength = Random.Range(minRoomLength, maxRoomLength);

        if (!CanPlaceRoom(x, y, roomWidth, roomLength)) {
            return false;
        }

        MakeGridOccupied(x, y, roomWidth, roomLength);

        Vector3 roomPosition = new Vector3(x * cellSize, 0, y * cellSize);
        GameObject room = new GameObject("Room_" + x + "_" + y);
        room.transform.position = roomPosition;

        // get a random color for the tiles
        Color color = Random.ColorHSV();

        for (int i = 0; i < roomWidth; i++) {
            for (int j = 0; j < roomLength; j++) {
                Vector3 tilePosition = roomPosition + new Vector3(i * cellSize, 0, j * cellSize);
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);

                // set the color of the tile
                tile.GetComponent<Renderer>().material.color = color;

                tile.transform.position = tilePosition;
                tile.transform.localScale = new Vector3(cellSize, 1f, cellSize);
                tile.transform.parent = room.transform;
            }
        }

        return true;
    }


    private void MakeGridOccupied(int startX, int startY, int roomWidth, int roomLength) {
        for (int x = startX; x < startX + roomWidth; x++) {
            for (int y = startY; y < startY + roomLength; y++) {
                grid[x, y] = true;
            }
        }
    }
}