using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCarSpawner : MonoBehaviour
{
    public GameObject testCarPrefab;
    public int spawnAmount;

    void Start()
    {
        GameObject[] allWP = GameObject.FindGameObjectsWithTag("waypoint");

        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject go = Instantiate(testCarPrefab);
            go.transform.position = allWP[Random.Range(0, allWP.Length)].transform.position;
            go.transform.parent = transform;
        }
    }
}
