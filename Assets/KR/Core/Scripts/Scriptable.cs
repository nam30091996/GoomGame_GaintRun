
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using KR.Editor;

namespace KR
{
	public static class Scriptable
	{

		public static void CreateAsset<T>(string dir, string name = null, bool multipler = false, int offset = 0) where T : ScriptableObject
		{
			string mult = multipler ? (Resources.LoadAll<T>("").Length + offset).ToString() : "";
			string rawAssetName = (name == null ? typeof(T).Name : name) + mult;
			string assetPathAndName = dir + rawAssetName + ".asset";
			if (!System.IO.File.Exists(assetPathAndName))
			{
				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);

				KR.Editor.Utility.DisplayProgressBar("Scriptable Initializing", "Creating " + typeof(T).FullName, 0.25f,
										   () =>
										   {
											   T asset = ScriptableObject.CreateInstance<T>();
											   AssetDatabase.CreateAsset(asset, assetPathAndName);
											   AssetDatabase.SaveAssets();
											   AssetDatabase.Refresh();
											   Selection.activeObject = asset;
										   });


			}
			else
			{
				KR.Editor.Utility.DisplayProgressBar("Scriptable Loading", "Loading " + typeof(T).FullName, 0.25f,
										   () =>
										   {
											   Selection.activeObject = Resources.Load<T>(rawAssetName);

										   });

			}

			//EditorUtility.FocusProjectWindow ();

		}

		public static void CreateInitiator<T>() where T : Component
		{
			KR.Editor.Utility.DisplayProgressBar("Initializing Manager", "Creating " + typeof(T).FullName, 0.25f, () =>
			{

				var ins = GameObject.FindObjectOfType<T>();
				if (ins == null)
					ins = new GameObject(typeof(T).Name).AddComponent<T>();

				Selection.activeObject = ins;
				EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
			});
		}
	}
}
#endif