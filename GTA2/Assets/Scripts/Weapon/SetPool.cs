using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPool : MonoBehaviour
{
    static public GameObject poolMother;

    static public void Init()
    {
        poolMother = new GameObject("[SetPool]");
    }

    static public List<GameObject> PoolMemory(
        GameObject target, GameObject motherObj, int count, string name)
    {
        List<GameObject> returnList = new List<GameObject>(count);
        for (int i = 0; i < count; i++)
        {
            GameObject tmpObject = Instantiate(target);
            tmpObject.name = name + i.ToString();
            tmpObject.transform.parent = motherObj.transform;
            tmpObject.SetActive(false);
            returnList.Add(tmpObject);
        }

        motherObj.transform.parent = poolMother.transform;
        return returnList;
    }
}


public class GetPool<T> : MonoBehaviour
{
    static public List<T> GetListComponent(List<GameObject> target)
    {
        List<T> returnList = new List<T>(target.Count);
        foreach (var i in target)
        {
            returnList.Add(i.GetComponent<T>());
        }

        return returnList;
    }

    static public int PlusListIdx(List<T> target, int idx)
    {
        int returnIdx = idx;

        returnIdx += 1;
        if (target.Count <= returnIdx)
        {
            returnIdx = 0;
        }

        return returnIdx;
    }
}