using UnityEngine;

public class RandomMirroredPlacer : MonoBehaviour
{
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private int numberOfObjectsToPlace = 3;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private GameObject bearPrefab;

    private bool[,] occupiedGrid;

    private void PlaceRandomBears()
    {
        if (tileManager == null)
        {
            Debug.LogError("TileManager is not assigned.");
            return;
        }

        if (bearPrefab == null)
        {
            Debug.LogError("Bear prefab is not assigned.");
            return;
        }

        if (width % 2 != 0)
        {
            Debug.LogError("Width must be even for mirrored placement.");
            return;
        }

        int placedCount = 0;
        int attempts = 0;
        int maxAttempts = 200;

        while (placedCount < numberOfObjectsToPlace && attempts < maxAttempts)
        {
            attempts++;

            int halfWidth = width / 2;
            int xIndex = Random.Range(0, halfWidth);
            int zIndex = Random.Range(0, height);

            int mirrorXIndex = width - 1 - xIndex;

            if (occupiedGrid[xIndex, zIndex] || occupiedGrid[mirrorXIndex, zIndex])
            {
                continue;
            }

            tileManager.PlaceBear(zIndex, xIndex, bearPrefab);
            tileManager.PlaceBear(zIndex, mirrorXIndex, bearPrefab);

            occupiedGrid[xIndex, zIndex] = true;
            occupiedGrid[mirrorXIndex, zIndex] = true;

            placedCount++;
        }

        if (placedCount < numberOfObjectsToPlace)
        {
            Debug.LogWarning($"Only placed {placedCount} mirrored bear pairs.");
        }
    }


    private void Start()
    {
        occupiedGrid = new bool[width, height];
        PlaceRandomBears();
    }

}