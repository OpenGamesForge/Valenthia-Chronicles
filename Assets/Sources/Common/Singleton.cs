/*---------------------------------------------------------------------------------------------
 *  Copyright (c) OpenGamesForges. All rights reserved.
 *  Licensed under the MIT License. See LICENSE-MIT file in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using UnityEngine;

// TODO: Add documentation to know when use each singleton type
namespace Common
{
    // Credit: https://awesometuts.com/blog/singletons-unity/#elementor-toc__heading-anchor-0
    // Performance tests: init -> 1us, access -> 49ns
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        
        #if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        protected static void ResetInstance()
        {
            _instance = null;
        }
        #endif
        
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // find the generic instance
                    _instance = FindAnyObjectByType<T>();

                    // if it's null again create a new object
                    // and attach the generic instance
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject($"{typeof(T).Name}");
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        public virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            Destroy(this.gameObject);
            _instance = null; // Destroy is not immediate so to avoid race condition, we set the instance to null
        }
    }
    
    // Performance tests: init -> 0ns, access -> 106ns
    public abstract class LockSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object lockObj = new object();
        
        #if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        protected static void ResetInstance()
        {
            _instance = null;
        }
        #endif
        
        public static T Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (_instance == null)
                    {
                        _instance = FindAnyObjectByType<T>();
                        if (_instance == null)
                        {
                            GameObject obj = new GameObject($"{typeof(T).Name}");
                            _instance = obj.AddComponent<T>();
                        }
                    }
                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            Destroy(this.gameObject);
            _instance = null; // Destroy is not immediate so to avoid race condition, we set the instance to null
        }
    }
    
    // Performance tests: init -> 0ns, access -> 35ns
    public abstract class LazySingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly Lazy<T> LazyInstance = new Lazy<T>(() =>
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject($"{typeof(T).Name}");
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        });
        
        #if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        protected static void ResetInstance()
        {
            _instance = null;
        }
        #endif
        
        public static T Instance => LazyInstance.Value;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            Destroy(this.gameObject);
            _instance = null; // Destroy is not immediate so to avoid race condition, we set the instance to null
        }
    }
}
