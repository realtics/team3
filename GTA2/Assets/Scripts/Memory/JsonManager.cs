using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JsonManager : MonoSingleton<JsonManager>
{
    public T Load<T>(string json)
    {
        // 문자열 불러오기


        return JsonUtility.FromJson<T>(json);
    }

    public void Save(object myObject)
    {
        string json = JsonUtility.ToJson(myObject);
        json = "";

        // 문자열 저장
    }
}
