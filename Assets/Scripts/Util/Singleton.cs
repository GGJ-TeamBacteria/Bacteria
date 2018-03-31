using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance;

    protected void Awake()
    {
        if (instance == null)
        {
            instance = GetComponent<T>();
            SingletonAwake();
        }
        else
        {
            Debug.LogWarning("Warning: There is already an instance of " + typeof(T) + " in the scene!");
            Destroy(this);
        }
    }

    public abstract void SingletonAwake();
}