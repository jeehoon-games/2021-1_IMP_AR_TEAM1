using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool appIsQuitting = false;
        private static T _instance;
        private static object _lock = new Object();

        public static T Instance
        {
            get
            {
                if (appIsQuitting) { return null; }
                
                if (!_instance)
                {
                    lock (_lock)
                    {
                        _instance = (T) FindObjectOfType(typeof(T));
                        if (_instance == null)
                        {
                            GameObject gameObject = new GameObject("NetworkCore");
                            _instance = (T) gameObject.AddComponent(typeof(T));
                            DontDestroyOnLoad(gameObject);
                        }
                    }
                }
                return _instance;
            }
        }

        private void OnDestroy()
        {
            appIsQuitting = true;
        }
    }
}
