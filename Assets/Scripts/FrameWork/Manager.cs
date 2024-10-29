using UnityEngine;
/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : Singleton<MyClassName> {}
/// </summary>
public class Manager<T> : MonoBehaviour where T : MonoBehaviour
{
    // Check to see if we're about to be destroyed.
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static T m_Instance;

    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static T Instance
    {
        get
        {
#if UNITY_EDITOR
            if (m_ShuttingDown)
            {
                // DebugUtil.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed. Returning null.");
                //return null;
            }
#endif
            lock (m_Lock)
            {
                if (m_Instance == null)
                {
#if !BAN_FINDOBJECTOFTYPE
                    // Search for existing instance.
                    m_Instance = (T)FindObjectOfType(typeof(T));
#endif

                    // Create new instance if one doesn't already exist.
                    if (m_Instance == null)
                    {
                        // Need to create a new GameObject to attach the singleton to.
                        var singletonObject = new GameObject();

#if NORMALIZE_SINGLE_PARENT && UNITY_EDITOR
                        var root = GameObject.Find("SINGLETON_ROOT");
                        if (root == null)
                        {
                            root = new GameObject("SINGLETON_ROOT");
                        }

                        if (Application.isPlaying) DontDestroyOnLoad(root);
                        singletonObject.transform.SetParent(root.transform);
#endif

                        m_Instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T) + " (Singleton)";
                        //singletonObject.hideFlags = HideFlags.HideInHierarchy;
                        // Make instance persistent.
                        if (Application.isPlaying) DontDestroyOnLoad(singletonObject);
                        (m_Instance as Manager<T>).InitImmediately();
                    }
                }

                return m_Instance;
            }
        }
    }

    // 这个方法如果override，会在Instance创建完立刻调用, 派生类可以用来默认初始化一些东西
    protected virtual void InitImmediately()
    {
    }

    private void OnApplicationQuit()
    {
        m_ShuttingDown = true;
    }

    private void OnDestroy()
    {
        //m_ShuttingDown = true;
    }
}