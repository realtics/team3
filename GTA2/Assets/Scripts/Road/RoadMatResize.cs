using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoadMatResize : MonoBehaviour
{
    float oldX;

    void Start()
    {
        oldX = transform.localScale.x;
    }

    void Update()
    {
        if (transform.localScale.x == oldX)
            return;

        oldX = transform.localScale.x;
        GetComponent<Renderer>().sharedMaterial.mainTextureScale = new Vector2(oldX, 1);
    }
}
