
// 그냥 C#싱글톤
public abstract class Singleton<T> where T : class
{
    protected static T instance = null;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = System.Activator.CreateInstance(typeof(T)) as T;
            }
            return instance;
        }
    }
}