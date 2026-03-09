using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSize : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Renderer r = GetComponent<Renderer>();
        Bounds b = r.bounds;

        Debug.Log("Width (X): " + b.size.x);
        Debug.Log("Depth (Z): " + b.size.z);
        Debug.Log("Height (Y): " + b.size.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
