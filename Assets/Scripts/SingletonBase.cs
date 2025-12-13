using UnityEngine;

/// <summary>
/// Awake‚ÆOnDestroyŽg‚Á‚Ä‚é‚Ì‚Åoverride‚µ‚Ä
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonBase<T> : MonoBehaviour
    where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance = (T)FindFirstObjectByType(typeof(T));

                if (_instance is null)
                {
                    Debug.Log("instance is null");
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            _instance = null;
        }
    }
}