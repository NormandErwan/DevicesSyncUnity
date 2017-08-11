// Based on: http://wiki.unity3d.com/index.php/Singleton

using UnityEngine;

namespace DeviceSyncUnity
{
    /// <remarks>
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// </remarks>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Properties

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                        "' already destroyed on application quit." +
                        " Won't create again - returning null.");
                    return null;
                }

                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                " - there should never be more than 1 singleton!" +
                                " Reopening the scene might fix it.");
                            return instance;
                        }

                        if (instance == null)
                        {
                            GameObject singleton = new GameObject();
                            instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();
                            DontDestroyOnLoad(singleton);
                        }
                    }

                    return instance;
                }
            }
        }

        // Variables

        private static T instance;

        private static object instanceLock = new object();

        private static bool applicationIsQuitting = false;

        // Methods

        protected virtual void Awake()
        {
            lock (instanceLock)
            {
                if (instance == null)
                {
                    T that = this as T;
                    if (that != null)
                    {
                        instance = that;
                    }
                }
            }
        }

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        protected virtual void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }
}
