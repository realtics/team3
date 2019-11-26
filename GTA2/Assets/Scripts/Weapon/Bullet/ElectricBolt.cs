using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricBolt : MonoBehaviour
{
    [SerializeField]
    int waveCount;
    [SerializeField]
    float boltSize;
    [SerializeField]
    float waveSize;


    LineRenderer lineRenderer;
    float waveSizePerCount;


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = waveCount;

        waveSizePerCount = boltSize * 2.0f / (waveCount - 1);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWave();
    }

    void UpdateWave()
    {
        lineRenderer.SetPosition(0, new Vector3(boltSize * -1.0f, .0f, .0f));
        lineRenderer.SetPosition(waveCount - 1, new Vector3(boltSize * 1.0f, .0f, .0f));

        for (int i = 1; i < waveCount - 1; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(
                (boltSize * -1.0f) + waveSizePerCount * i,
                Random.Range(-waveSize, waveSize),
                .0f));
        }
    }
}
