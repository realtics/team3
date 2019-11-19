
using UnityEngine;




// 유니티는 ​모노 싱글톤이 따로 필요하다.
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance = null;
    public static T Instance
    {
        get
        {
            instance = FindObjectOfType(typeof(T)) as T;
            if (instance == null)
            {
                instance = new GameObject("@" + typeof(T).ToString(),
                                           typeof(T)).AddComponent<T>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }
}