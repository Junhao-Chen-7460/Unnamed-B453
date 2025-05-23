using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int width = 50;
    public int height = 30;
    public float cellSize = 1f;
    public GameObject wallPrefab;

    [Header("Base Settings")]
    public GameObject redBasePrefab;
    public GameObject blueBasePrefab;
    public GameObject greenBasePrefab;
    public GameObject yellowBasePrefab;

    [Header("Walls Inside")]
    [Range(0f, 1f)]
    public float obstacleRate = 0.1f;

    [Header("Base limit")]
    public int baseSafeDistanceFromWall = 5;
    public float baseMinDistanceBetweenBases = 5f;

    [Header("Camera Settings")]
    public Camera mainCamera;

    private int[,] map;

    void Start()
    {
        GenerateMap();
        PlaceBases();
        CenterCamera();
    }

    void GenerateMap()
    {
        map = new int[width, height];
        FillOuterWalls();
        PlaceRandomObstacles();
        CreateRandomBigOpenArea();
        CleanSmallObstacleClusters();
        InstantiatePrefabs();
    }

    void FillOuterWalls()
    {
        for (int x = 0; x < width; x++)
        {
            map[x, 0] = 1;
            map[x, height - 1] = 1;
        }
        for (int y = 0; y < height; y++)
        {
            map[0, y] = 1;
            map[width - 1, y] = 1;
        }
    }

    void PlaceRandomObstacles()
    {
        int targetObstacles = Mathf.FloorToInt(width * height * obstacleRate);
        int placed = 0;

        while (placed < targetObstacles)
        {
            int x = Random.Range(1, width - 1);
            int y = Random.Range(1, height - 1);

            if (map[x, y] == 0)
            {
                map[x, y] = 1;
                if (!IsMapFullyConnected())
                {
                    map[x, y] = 0;
                }
                else
                {
                    placed++;
                }
            }
        }
    }
    bool IsClear(int cx, int cy)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int nx = cx + dx;
                int ny = cy + dy;
                if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                    return false;
                if (map[nx, ny] != 0)
                    return false;
            }
        }
        return true;
    }

    void CreateRandomBigOpenArea()
    {
        int targetClearTiles = Mathf.FloorToInt(width * height * 0.1f);
        bool[,] cleared = new bool[width, height];

        Vector2Int start = new Vector2Int(-1, -1);
        for (int tries = 0; tries < 1000; tries++)
        {
            int x = Random.Range(5, width - 5);
            int y = Random.Range(5, height - 5);

            if (map[x, y] == 0)
            {
                start = new Vector2Int(x, y);
                break;
            }
        }

        if (start.x == -1) return;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        cleared[start.x, start.y] = true;

        int clearedCount = 0;

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        while (queue.Count > 0 && clearedCount < targetClearTiles)
        {
            Vector2Int current = queue.Dequeue();

            if (current.x == 0 || current.x == width - 1 || current.y == 0 || current.y == height - 1)
                continue;

            if (map[current.x, current.y] == 1)
            {
                map[current.x, current.y] = 0;
            }
            clearedCount++;

            for (int dir = 0; dir < 4; dir++)
            {
                int nx = current.x + dx[dir];
                int ny = current.y + dy[dir];
                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    if (!cleared[nx, ny])
                    {
                        cleared[nx, ny] = true;
                        queue.Enqueue(new Vector2Int(nx, ny));
                    }
                }
            }
        }
    }

    void CleanSmallObstacleClusters()
    {
        bool[,] visited = new bool[width, height];

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (!visited[x, y] && map[x, y] == 1)
                {
                    List<Vector2Int> cluster = new List<Vector2Int>();
                    Queue<Vector2Int> queue = new Queue<Vector2Int>();
                    queue.Enqueue(new Vector2Int(x, y));
                    visited[x, y] = true;
                    cluster.Add(new Vector2Int(x, y));

                    while (queue.Count > 0)
                    {
                        Vector2Int current = queue.Dequeue();
                        for (int dir = 0; dir < 4; dir++)
                        {
                            int nx = current.x + dx[dir];
                            int ny = current.y + dy[dir];
                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                if (!visited[nx, ny] && map[nx, ny] == 1)
                                {
                                    visited[nx, ny] = true;
                                    queue.Enqueue(new Vector2Int(nx, ny));
                                    cluster.Add(new Vector2Int(nx, ny));
                                }
                            }
                        }
                    }

                    if (cluster.Count < 4)
                    {
                        foreach (var pos in cluster)
                        {
                            map[pos.x, pos.y] = 0;
                        }
                    }
                }
            }
        }
    }

    void InstantiatePrefabs()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                {
                    Instantiate(wallPrefab, GetWorldPosition(x, y), Quaternion.identity);
                }
            }
        }
    }

    void PlaceBases()
    {
        List<Vector2> candidates = new List<Vector2>();

        for (int x = baseSafeDistanceFromWall; x < width - baseSafeDistanceFromWall; x++)
        {
            for (int y = baseSafeDistanceFromWall; y < height - baseSafeDistanceFromWall; y++)
            {
                if (IsClear(x, y))
                {
                    candidates.Add(new Vector2(x, y));
                }
            }
        }

        List<Vector2> selected = new List<Vector2>();

        for (int i = 0; i < 4; i++)
        {
            Vector2 best = Vector2.zero;
            float bestMinDist = -1f;

            foreach (var c in candidates)
            {
                float minDist = float.MaxValue;
                foreach (var s in selected)
                {
                    float dist = Vector2.Distance(c, s);
                    if (dist < minDist) minDist = dist;
                }
                if (selected.Count == 0 || minDist >= baseMinDistanceBetweenBases)
                {
                    if (minDist > bestMinDist)
                    {
                        bestMinDist = minDist;
                        best = c;
                    }
                }
            }
            selected.Add(best);
            candidates.Remove(best);
        }

        Instantiate(redBasePrefab, GetWorldPosition((int)selected[0].x, (int)selected[0].y), Quaternion.identity);
        Instantiate(blueBasePrefab, GetWorldPosition((int)selected[1].x, (int)selected[1].y), Quaternion.identity);
        Instantiate(greenBasePrefab, GetWorldPosition((int)selected[2].x, (int)selected[2].y), Quaternion.identity);
        Instantiate(yellowBasePrefab, GetWorldPosition((int)selected[3].x, (int)selected[3].y), Quaternion.identity);
    }

    bool IsMapFullyConnected()
    {
        bool[,] visited = new bool[width, height];

        Vector2Int start = new Vector2Int(-1, -1);
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (map[x, y] == 0)
                {
                    start = new Vector2Int(x, y);
                    break;
                }
            }
            if (start.x != -1) break;
        }

        if (start.x == -1) return false;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        int visitedCount = 1;
        int emptyCount = 0;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (map[x, y] == 0)
                    emptyCount++;

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            for (int dir = 0; dir < 4; dir++)
            {
                int nx = current.x + dx[dir];
                int ny = current.y + dy[dir];
                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    if (!visited[nx, ny] && map[nx, ny] == 0)
                    {
                        visited[nx, ny] = true;
                        queue.Enqueue(new Vector2Int(nx, ny));
                        visitedCount++;
                    }
                }
            }
        }

        return visitedCount == emptyCount;
    }

    Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(
            (x - width / 2f) * cellSize,
            (y - height / 2f) * cellSize,
            0f
        );
    }

    void CenterCamera()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0, 0, -10);
            float mapAspect = (float)width / height;
            if (mapAspect > 1)
            {
                mainCamera.orthographicSize = (width * cellSize) / 2f / mapAspect;
            }
            else
            {
                mainCamera.orthographicSize = (height * cellSize) / 2f;
            }
        }
    }
}
