using UnityEngine;
using Unity.Netcode;


public abstract class Singleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    public static T Instance {get; private set;}

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this as T;
    }

    protected virtual void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }
}
