using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricBolt : MonoBehaviour
{
    [SerializeField]
    float wavePerScale;
    [SerializeField]
    float waveSize;


    LineRenderer lineRenderer;
    float waveSizePerCount;
    float boltSize;
    int waveCount;


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        UpdateWave();
    }


    public void UpdateWave()
    {
        waveCount = (int)(transform.localScale.x * wavePerScale);
        lineRenderer.positionCount = waveCount;

        boltSize = transform.localScale.x;
        waveSizePerCount = boltSize * 2.0f / (waveCount - 1);

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
