public class DebugX
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(object msg)
    {
        UnityEngine.Debug.Log(msg);
    }
}