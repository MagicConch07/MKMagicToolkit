using UnityEngine;

namespace MKMagicToolkit.Utility
{
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType(typeof(T)) as T;
                    if (_instance == null)
                    {
                        SetupInstnace();
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            RemoveDuplicates();
            Initialized();
        }

        protected void OnDisable()
        {
            _instance = null;
        }

        protected virtual void Initialized()
        {
            SetupInstnace();
        }

        protected static void SetupInstnace()
        {
            _instance = FindAnyObjectByType(typeof(T)) as T;
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                _instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }
        }

        protected void RemoveDuplicates()
        {
            if (_instance == null)
            {
                _instance = this as T;

                if (transform.parent != null)
                    transform.SetParent(null);

                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                DestroyImmediate(this.gameObject);
            }
        }
    }
}