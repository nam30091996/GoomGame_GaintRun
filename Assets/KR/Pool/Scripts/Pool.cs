using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
using KR.Editor;
using System.Linq;
using Sirenix.OdinInspector;

#endif
namespace KR
{
    public class Pool : KR.ScriptableSingleton<Pool>
    {
#if UNITY_EDITOR
        [MenuItem("KR/Pool/Settings")]
        public static void PoolSetting()
        {
            KR.Scriptable.CreateAsset<Pool>("Assets/KR/Pool/Resources/");
        }



        [MenuItem("KR/Pool/Init")]
        public static void PoolInit()
        {
			var pool = FindObjectOfType<PoolManager>();

			pool = pool ?? (new GameObject(typeof(PoolManager).Name).AddComponent<PoolManager>());

            Selection.activeObject = pool;
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            //EditorUtility.SetDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }


         [Button]
        private void GeneratePoolList()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("namespace KR ");
            builder.AppendLine("{");
            builder.AppendLine("\tpublic enum PoolList");
            builder.AppendLine("\t{");
            //builder.AppendLine("\t{ ");
            for (int i = 0; i < initData.Length; i++)
            {
                if (initData[i] != null)
                {
					if (initData[i].source != null && !string.IsNullOrEmpty(initData[i].key))
                        builder.AppendLine("\t\t " + (initData[i].key) + ",");
                }
            }
            builder.AppendLine("\t} ");
            builder.AppendLine("} ");
			KR.Editor.Utility.DisplayProgressBar("Generating Pool", "Total items: " + initData.Length + " Total pools: " +  initData.Select(i=> i.poolAmount).Sum(), 0.25f, () => { 
				System.IO.File.WriteAllText(Application.dataPath + "/KR/Pool/Resources/PoolList.cs", builder.ToString());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
			});
            

         
        }

		
#endif
      
		public PoolInfo[] initData;
        private Dictionary<string, int> poolHolder = new Dictionary<string, int>();
        private List<List<GameObject>> poolList = new List<List<GameObject>>();
        private GameObject inst;


        public override void OnInitialized()
        {
            base.OnInitialized();

            int length = instance.initData.Length;
            for (int i = 0; i < length; i++)
            {
                instance.poolHolder.Add(instance.initData[i].key, i);
                instance.poolList.Add(new List<GameObject>());

                for (int init = 0; init < instance.initData[i].poolAmount; init++)
                {
                    instance.inst = Instantiate(instance.initData[i].source);
					instance.inst.gameObject.AddComponent<PoolEvent>();
                    instance.inst.gameObject.SetActive(false);
                    instance.poolList[i].Add(instance.inst);
                    DontDestroyOnLoad(instance.inst);

                }
            }
        }

        public static void DisableAll()
        {
            int l1 = instance.poolList.Count;
            for (int i = 0; i < l1; i++)
            {
                int l2 = instance.poolList[i].Count;
                for (int j = 0; j < l2; j++)
                {
                    if (instance.poolList[i][j] != null)
                        instance.poolList[i][j].SetActive(false);
                }
            }
        }
		public static Transform GetPool(PoolList key, Vector3 position, Quaternion rotation){
			var o = GetPool<Transform>(key);
			o.position = position;
			o.rotation = rotation;
			return o;

		}

		public static T GetPool<T>(PoolList key, Vector3 position, Quaternion rotation) where T : MonoBehaviour
        {
			var o = GetPool<T>(key);
			o.transform.position = position;
			o.transform.rotation = rotation;
            return o;

        }

        public static T GetPool<T>(PoolList key, bool active = true) where T : Component
        {
            int index = instance.poolHolder[key.ToString()];
            int length = instance.poolList[index].Count;
            for (int i = 0; i < length; i++)
            {
                if (!instance.poolList[index][i].activeSelf)
                {
                    instance.poolList[index][i].SetActive(active);
                    return instance.poolList[index][i].GetComponent<T>();
                }
            }
            instance.inst = Instantiate(instance.initData[index].source);
            instance.poolList[index].Add(instance.inst);
            DontDestroyOnLoad(instance.inst);

            return instance.inst.GetComponent<T>();
        }



        public static void CollectPool<T>(PoolList key, int amount, Func<T, bool> exclude) where T : Component
        {
            int index = instance.poolHolder[key.ToString()];
            int length = amount.clamp(max: instance.poolList[index].Count);
            //Debug.Log(length);

            for (int i = 0; i < length; i++)
            {
                var t = instance.poolList[index][i].GetComponent<T>();
                if (t.gameObject.activeSelf)
                {
                    if (exclude(t))
                    {
                        length = (length + 1).clamp(max: instance.poolList[index].Count);
                        continue;
                    }

                    instance.poolList[index][i].SetActive(false);
                }
            }
        }
		public static string GetKey(string input)
        {
            if (Regex.IsMatch(input, @"^\d"))
                input = "_" + input;

            Regex r = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return r.Replace(input, string.Empty).Replace(" ", "");

            //return input.Replace(" ", "").Replace("(", "").Replace(")", "");

        }

    }
    [Serializable]
    public class PoolInfo
    {

        
		 string Title {
			get{
                return string.IsNullOrEmpty(key)  ? string.Empty : Regex.Replace(Audio.GetKey(key), "([a-z])([A-Z])", "$1 $2");
			}
		}

		//[FoldoutGroup("$Title", Expanded = true)]
        public string key; 
		//[FoldoutGroup("$Title", Expanded = true)]
        public int poolAmount = 10;
		//[FoldoutGroup("$Title", Expanded = true)]
		//[ValidateInput("AutoSetName")]
        public GameObject source;
	

		private bool AutoSetName (GameObject s){
			if(string.IsNullOrEmpty(key) && source){
				key = Pool.GetKey(source.name);
			}
			return true;
		}

    }
}