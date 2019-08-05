using System;
using UnityEngine;

namespace STFEngine.Core
{
    public class STF_SingletonMonoBehaviour<T> : MonoBehaviour where T : STF_SingletonMonoBehaviour<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                return _instance;
            }
        }

        public static bool Exists
        {
            get;
            private set;
        }

        protected static GameObject prefGo;
        public static void OpenView(string path)
        {
            if (Instance == null)
            {
                GameObject go = Resources.Load(path) as GameObject;
                prefGo = GameObject.Instantiate(go);
                prefGo.AddComponent<T>();
            }
            else
            {
                if (Instance.gameObject.activeSelf)
                {
                    Instance.gameObject.SetActive(true);
                }
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
                Exists = true;
            }
            else if (_instance != this)
            {
                throw new InvalidOperationException("Can't have two instances of a view");
            }
        }
    }
}

