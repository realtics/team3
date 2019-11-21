
// 그냥 C#싱글톤
public abstract class Singleton<T> where T : class
{
    protected static T ptr = null;
    public static T Instance
    {
        get
        {
            if (ptr == null)
            {
                ptr = System.Activator.CreateInstance(typeof(T)) as T;
            }
            return ptr;
        }
    }
}