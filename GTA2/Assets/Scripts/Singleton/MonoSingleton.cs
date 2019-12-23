using UnityEngine;




// 유니티는 ​모노 싱글톤이 따로 필요하다.
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T ptr = null;
    public static T Instance
    {
        get
        {
            if (ptr == null)
            {
                ptr = FindObjectOfType(typeof(T)) as T;
                if (ptr == null)
                {
                    DebugX.Log("There's no active ManagerClass object");
                    // ptr = new GameObject("@" + typeof(T).ToString(),
                    //                            typeof(T)).AddComponent<T>();
                }
            }
           
            return ptr;
        }
    }
}