using System.Collections.Generic;
using UnityEngine;

public class CreateFixedPath : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private int width = 8;   // x (horizontal)
    [SerializeField] private int height = 8;  // z (vertical)
    [SerializeField] private GameObject debugPathPrefab;
    [SerializeField] private TileManager tileManager;

    private TileInfo[,] grid; // grid[z, x]

    public enum Dir
    {
        None,
        Up,    // +z
        Down,  // -z
        Left,  // -x
        Right  // +x
    }

    public struct TileInfo
    {
        public int g;
        public int h;
        public bool isVisited;
        public bool inOpen;
        public Dir cameFrom;
    }

    // start = (x, z) stored as Vector2Int(x, z) -> z is in .y
    [Header("Start / Goal (z first, then x)")]
    [SerializeField] private int startZ = 0;
    [SerializeField] private int startX = 0;

    [SerializeField] private int goalZ = 7;
    [SerializeField] private int goalX = 7;

    private List<Vector2Int> openList = new List<Vector2Int>();

    private void InitGrid()
    {
        grid = new TileInfo[height, width]; // [z, x]

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[z, x] = new TileInfo
                {
                    g = int.MaxValue,   // infinity initially
                    h = 0,
                    isVisited = false,
                    inOpen = false,
                    cameFrom = Dir.None
                };
            }
        }
    }

    private int Manhattan(Vector2Int a, Vector2Int b)
    {
        // a.y and b.y represent z
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private Vector2Int PickLowestF()
    {
        Vector2Int best = openList[0];

        int bestX = best.x;
        int bestZ = best.y; // remember: Vector2Int.y = z
        //Add pertubations to encourage randomized path
        float bestF = grid[bestZ, bestX].g + grid[bestZ, bestX].h 
            //+Random.Range(0f, 0.1f);

        for (int i = 1; i < openList.Count; i++)
        {
            Vector2Int p = openList[i];
            int x = p.x;
            int z = p.y;

            int f = grid[z, x].g + grid[z, x].h;
            if (f < bestF)
            {
                bestF = f;
                best = p;
            }
        }

        return best;
    }

    private void TryRelaxNeighbors(int newX, int newZ, Dir cameFromDir, Vector2Int current, Vector2Int goal) {
        if (newX < 0 || newZ < 0 || newX >= width || newZ >= height) {
            return;
        }

        int prevX = current.x;
        int prevZ = current.y;

        int tentativeG = grid[prevZ, prevX].g + 1; // cost to move to neighbor

        if(tentativeG < grid[newZ, newX].g) {
            grid[newZ, newX].g = tentativeG;
            grid[newZ, newX].h = Manhattan(new Vector2Int(newX, newZ), goal);
            grid[newZ, newX].cameFrom = cameFromDir;
            if (!grid[newZ, newX].inOpen) {
                openList.Add(new Vector2Int(newX, newZ));
                grid[newZ, newX].inOpen = true;
            }
        }
    }

    private void UpdateNeighbors(Vector2Int current, Vector2Int goal) {
        int startX = current.x;
        int startZ = current.y;

        TryRelaxNeighbors(startX, startZ + 1, Dir.Up, current, goal);   // up
        TryRelaxNeighbors(startX, startZ - 1, Dir.Down, current, goal);   // down
        TryRelaxNeighbors(startX - 1, startZ, Dir.Left, current, goal);   // left
        TryRelaxNeighbors(startX + 1, startZ, Dir.Right, current, goal);   // right
    }

    private bool RunAStar(Vector2Int start, Vector2Int goal)
    {
        // safety: if openList isn't initialized
        if (openList.Count == 0)
            openList.Add(start);

        while (openList.Count > 0)
        {
            // 1) pick best current (lowest f)
            Vector2Int current = PickLowestF();
            int cx = current.x;
            int cz = current.y;

            // 2) if goal reached, stop
            if (cx == goal.x && cz == goal.y)
            {
                Debug.Log("Reached goal!");
                return true;
            }

            // 3) remove current from open, mark as closed
            openList.Remove(current);

            TileInfo curInfo = grid[cz, cx];
            curInfo.inOpen = false;
            curInfo.isVisited = true;
            grid[cz, cx] = curInfo;

            // 4) update neighbors (relax edges)
            UpdateNeighbors(current, goal);
        }

        Debug.Log("No path found (openList empty).");
        return false;
    }

    private List<Vector2Int> ReconstructPath(Vector2Int start, Vector2Int goal)
    {
        var path = new List<Vector2Int>();

        int x = goal.x;
        int z = goal.y;

        int maxSteps = width * height + 5;

        for (int step = 0; step < maxSteps; step++)
        {
            path.Add(new Vector2Int(x, z));

            // reached start
            if (x == start.x && z == start.y)
            {
                path.Reverse(); // start -> goal
                return path;
            }

            Dir came = grid[z, x].cameFrom;

            if (came == Dir.None)
            {
                Debug.LogError($"Reconstruct failed at (x={x}, z={z}) because cameFrom=None");
                return null;
            }

            // step back to parent (opposite direction)
            switch (came)
            {
                case Dir.Up: z -= 1; break;
                case Dir.Down: z += 1; break;
                case Dir.Left: x += 1; break;
                case Dir.Right: x -= 1; break;
            }

            if (x < 0 || x >= width || z < 0 || z >= height)
            {
                Debug.LogError("Reconstruct stepped out of bounds. Something is inconsistent.");
                return null;
            }
        }

        Debug.LogError("Reconstruct failed: exceeded maxSteps (likely a loop).");
        return null;
    }


    void Start()
    {
        InitGrid();
        // Vector2Int is (x, y). We use y as z.
        Vector2Int start = new Vector2Int(startX, startZ);
        Vector2Int goal = new Vector2Int(goalX, goalZ);

        // grid is [z, x]
        grid[startZ, startX].g = 0;
        grid[startZ, startX].inOpen = true;
        grid[startZ, startX].h = Manhattan(start, goal);
        grid[startZ, startX].cameFrom = Dir.None;

        openList.Clear();
        openList.Add(start);


        bool found = RunAStar(start, goal);
        Debug.Log($"A* finished. Found path? {found}");

        List<Vector2Int> createdPath = ReconstructPath(start, goal);
        for (int i = 0; i < createdPath.Count; i++)
        {
            Vector2Int p = createdPath[i];
            Debug.Log($"Step {i}:  z={p.y} x={p.x},)");

            if (tileManager != null)
                tileManager.PlaceRoad(p.y, p.x, debugPathPrefab); // p.y is z
            else
                Debug.LogError("TileManager not assigned!");
        }
    }

    void Update() { }
}