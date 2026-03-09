using UnityEngine;

[ExecuteAlways]
public class GridOverlayGizmos : MonoBehaviour
{
    [Header("Grid Size")]
    [SerializeField] private float gridSizeX = 4f;   // world units across (X)
    [SerializeField] private float gridSizeZ = 4f;   // world units across (Z)
    [SerializeField] private float tileSize = 0.5f;  // each tile

    [Header("Lift above floor (avoid flicker)")]
    [SerializeField] private float yOffset = 0.02f;  // lift above floor


    void OnDrawGizmos()
    {
        int cellsX = Mathf.RoundToInt(gridSizeX / tileSize); // 8
        int cellsZ = Mathf.RoundToInt(gridSizeZ / tileSize); // 8

        Vector3 basePos = transform.position
                - new Vector3(gridSizeX * 0.5f, 0f, gridSizeZ * 0.5f)
                + Vector3.up * yOffset;

        Gizmos.color = Color.white;

        // lines parallel to Z
        for (int x = 0; x <= cellsX; x++)
        {
            Vector3 a = basePos + new Vector3(x * tileSize, 0f, 0f);
            Vector3 b = basePos + new Vector3(x * tileSize, 0f, gridSizeZ);
            Gizmos.DrawLine(a, b);
        }

        // lines parallel to X
        for (int z = 0; z <= cellsZ; z++)
        {
            Vector3 a = basePos + new Vector3(0f, 0f, z * tileSize);
            Vector3 b = basePos + new Vector3(gridSizeX, 0f, z * tileSize);
            Gizmos.DrawLine(a, b);
        }
    }
}