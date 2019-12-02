using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    public bool logStatus;
    public Transform root;

    private Dictionary<GameObject, ObjectPool<GameObject>> prefabLookup;
    private Dictionary<GameObject, ObjectPool<GameObject>> instanceLookup;

    private bool dirty = false;

    void Initialize()
    {
        prefabLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
        instanceLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
    }

    void warmPool(GameObject prefab, int size)
    {
        if (prefabLookup == null)
        {
            Initialize();
        }

        if (prefabLookup.ContainsKey(prefab))
        {
            throw new Exception("Pool for prefab " + prefab.name + " has already been created");
        }
        var pool = new ObjectPool<GameObject>(() => { return InstantiatePrefab(prefab); }, size);
        prefabLookup[prefab] = pool;

        dirty = true;
    }

    GameObject spawnObject(GameObject prefab)
    {
        return spawnObject(prefab, Vector3.zero, Quaternion.identity);
    }
    GameObject spawnObject(GameObject prefab, Vector3 position, Vector3 rotation)
    {
        var clone = spawnObject(prefab, Vector3.zero, Quaternion.identity);
        clone.transform.position = position;
        clone.transform.eulerAngles = rotation;
        return clone;
    }

    GameObject spawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!prefabLookup.ContainsKey(prefab))
        {
            WarmPool(prefab, 1);
        }

        var pool = prefabLookup[prefab];

        var clone = pool.GetItem();
        clone.transform.position = position;
        clone.transform.rotation = rotation;
        clone.SetActive(true);

        instanceLookup.Add(clone, pool);
        dirty = true;
        return clone;
    }

    void releaseObject(GameObject clone)
    {
        clone.SetActive(false);

        if (instanceLookup.ContainsKey(clone))
        {
            instanceLookup[clone].ReleaseItem(clone);
            instanceLookup.Remove(clone);
            dirty = true;
        }
        else
        {
            Debug.LogWarning("No pool contains the object: " + clone.name);
        }
    }

    void releaseAllObject(GameObject clone)
    {
        if (instanceLookup.ContainsKey(clone))
        {
            instanceLookup[clone].ReleaseAllItem(clone);
            instanceLookup.Remove(clone);
            dirty = true;
        }
        else if (prefabLookup.ContainsKey(clone))
        {
            prefabLookup[clone].ReleaseAllItem(clone);
            dirty = true;
        }
        else
        {
            Debug.LogWarning("No pool contains the object: " + clone.name);
        }
    }


    private GameObject InstantiatePrefab(GameObject prefab)
    {
        var go = GameObject.Instantiate(prefab) as GameObject;
        if (root != null) go.transform.parent = root;

        go.transform.position = new Vector3(10000.0f, 10000.0f, 10000.0f);
		go.name = prefab.name;
        return go;
    }

    #region Static API

    public static void WarmPool(GameObject prefab, int size)
    {
        Instance.warmPool(prefab, size);
    }

    public static GameObject SpawnObject(GameObject prefab)
    {
        return Instance.spawnObject(prefab);
    }

    public static GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return Instance.spawnObject(prefab, position, rotation);
    }

    public static GameObject SpawnObject(GameObject prefab, Vector3 position, Vector3 rotation)
    {
        return Instance.spawnObject(prefab, position, rotation);
    }


    public static void ReleaseObject(GameObject clone)
    {
        Instance.releaseObject(clone);
    }
    public static void ReleaseAllObject(GameObject clone)
    {
        Instance.releaseAllObject(clone);
    }

    #endregion
}