using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    [Header("Grid Size")]
    [SerializeField] private float gridSizeX = 4f;   // world units across (X)
    [SerializeField] private float gridSizeZ = 4f;   // world units across (Z)

    [Header("Tile Size")]
    [SerializeField] private float tileSizeX;
    [SerializeField] private float tileSizeZ;

    [Header("Prefab Location")]
    [SerializeField] private GameObject prefabObject = null;
    [SerializeField] private int xIndex = 3;
    [SerializeField] private int zIndex = 3;


    public GameObject PlaceRoad(int zIndex, int xIndex, GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab parameter is null.");
            return null;
        }

        tileSizeX = gridSizeX / 8f;
        tileSizeZ = gridSizeZ / 8f;

        float worldX = transform.position.x + (gridSizeX * 0.5f) + (-xIndex * tileSizeX) - (tileSizeX * 0.5f);
        float worldZ = transform.position.z + (gridSizeZ * 0.5f) + (-zIndex * tileSizeZ) - (tileSizeZ * 0.5f);

        GameObject createdRoad = Instantiate(prefab, new Vector3(worldX, -0.15f, worldZ), transform.rotation);
        createdRoad.transform.localScale = new Vector3(tileSizeX, 0.02f, tileSizeZ);
        return createdRoad;
    }

    // Start is called before the first frame update
    void Start()
    {
        //PlaceRoad(1, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
