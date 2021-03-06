using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;
            
            _instance = FindObjectOfType<T>();

            if (_instance != null) return _instance;
            
            GameObject newGo = new GameObject();
            _instance = newGo.AddComponent<T>();
            return _instance;
        }
    }

    protected void Awake()
    {
        _instance = this as T;
    }
}
