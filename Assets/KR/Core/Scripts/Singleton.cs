using UnityEngine;
using System.Collections.Generic;

namespace KR
{
    public class Singleton<T> : MonoBehaviour
    {

        public static T instance;
        protected virtual void Awake()
        {
            instance = GetComponent<T>();

        }

    }
    public class ManagerSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T instance;
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = gameObject.GetComponent<T>();
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
    }
    public class Scripton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T _instance;
		const string pattern = "----------";
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
					_instance = new GameObject( pattern + typeof(T).Name + pattern).AddComponent<T>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
        public static void Init()
        {
            if (_instance == null)
                Debug.Log((instance.name + " Initialized.").color(Color.blue));

        }
    }


    public abstract class ScriptableSingleton : ScriptableObject
    {
        public abstract void OnInitialized();
    }

    public class ScriptableSingleton<T> : ScriptableSingleton where T : ScriptableSingleton
    {
        public static string resourceName { get { return typeof(T).Name; } }


	    
        private static T _instance;
        public static T instance {
			get {
				

				return _instance ??  (_instance = Resources.Load<T>(resourceName));

			} 
		}
        public static bool IsInitialized;

		const string pattern = "----------";
		public string Nickname {
			get{
				return pattern + name + pattern;
			}
		}

        public static void Init()
        {
            if (IsInitialized)
                return;


            IsInitialized = true;
            instance.OnInitialized();
        }
        public override void OnInitialized()
        {
			Debug.Log("{0} Initialized".format(Nickname).color(Color.blue));

        }
    }
}