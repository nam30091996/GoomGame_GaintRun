//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
#if !ARBOR_DLL && UNITY_2018_1_OR_NEWER
#define ENABLE_UNITY_EDITOR_LANGUAGE
#endif

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Arbor;

namespace ArborEditor
{
	internal sealed class LanguageImporter : AssetPostprocessor
	{
		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			bool rebuild = false;

			foreach (string importAssetPath in importedAssets)
			{
				if (!importAssetPath.StartsWith("Assets/"))
				{
					continue;
				}

				string directory = Path.GetDirectoryName(importAssetPath);
				string languageName = Path.GetFileNameWithoutExtension(importAssetPath);
				string ext = Path.GetExtension(importAssetPath);

				if (ext == ".txt" && Localization.Contains(directory))
				{
					SystemLanguage language;
					rebuild = EnumUtility.TryParse<SystemLanguage>(languageName, out language);
				}
				else
				{
					LanguagePathInternal languagePath = AssetDatabase.LoadAssetAtPath<LanguagePathInternal>(importAssetPath);
					if (languagePath != null)
					{
						languagePath.Setup();
						Localization.AddLanguagePath(languagePath);
						rebuild = true;
					}
				}
			}

			if (deletedAssets != null && deletedAssets.Length > 0)
			{
				rebuild = true;
			}

			if (rebuild)
			{
				Localization.Rebuild();
			}
		}
	}

	[Obfuscation(Exclude = true)]
	[InitializeOnLoad]
	public static class Localization
	{
		static Localization()
		{
			Initialize();
		}

		private sealed class WordDictionary : Dictionary<string, GUIContent>
		{
		}

		private sealed class SystemLanguageCompare : IComparer<SystemLanguage>
		{
			public int Compare(SystemLanguage x, SystemLanguage y)
			{
				return ((int)x).CompareTo((int)y);
			}
		}

		private static List<LanguagePathInternal> _LanguagePaths = new List<LanguagePathInternal>();
		private static SortedDictionary<SystemLanguage, WordDictionary> _LanguageDics = new SortedDictionary<SystemLanguage, WordDictionary>(new SystemLanguageCompare());
		private static List<SystemLanguage> _Languages = new List<SystemLanguage>();
		private static GUIContent[] _LanguageLabels;
		private static SystemLanguage _LastLanguage;

		public static event System.Action onRebuild;

		const string k_DirectoryName = "Languages";

		public static string languageDirectory
		{
			get
			{
				return PathUtility.Combine(EditorResources.directory, k_DirectoryName);
			}
		}

		public static bool Contains(string path)
		{
			if (path == languageDirectory)
			{
				return true;
			}

			foreach (LanguagePathInternal languagePath in _LanguagePaths)
			{
				if (path == languagePath.path)
				{
					return true;
				}
			}

			return false;
		}

		internal static void Rebuild()
		{
			for (int i = _LanguagePaths.Count - 1; i >= 0; i--)
			{
				LanguagePathInternal languagePath = _LanguagePaths[i];
				if (languagePath == null)
				{
					_LanguagePaths.RemoveAt(i);
				}
			}

			_Languages.Clear();
			_LanguageDics.Clear();

			foreach (SystemLanguage language in EnumUtility.GetValues<SystemLanguage>())
			{
				Load(language);
			}

			_LanguageLabels = null;

			if (onRebuild != null)
			{
				onRebuild();
			}
		}

		internal static void AddLanguagePath(LanguagePathInternal languagePath)
		{
			if (!_LanguagePaths.Contains(languagePath))
			{
				_LanguagePaths.Add(languagePath);
			}
		}

		static void Initialize()
		{
			foreach (string guid in AssetDatabase.FindAssets("t:ArborEditor.LanguagePath"))
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				LanguagePathInternal languagePath = AssetDatabase.LoadAssetAtPath<LanguagePathInternal>(path);
				if (languagePath != null)
				{
					languagePath.Setup();
					AddLanguagePath(languagePath);
				}
			}

			Rebuild();
		}

		static T LoadAssetAtPath<T>(string path, string ext) where T : Object
		{
			T obj = AssetDatabase.LoadAssetAtPath<T>(path);
			if (obj != null)
			{
				return obj;
			}
			return AssetDatabase.LoadAssetAtPath<T>(Path.ChangeExtension(path, ext));
		}

		static void Load(SystemLanguage language)
		{
			if (_LanguageDics.ContainsKey(language))
			{
				return;
			}

			WordDictionary wordDic = new WordDictionary();

			string languageName = language.ToString();

			string path = PathUtility.Combine(k_DirectoryName, languageName);

			// load default words
			TextAsset languageAsset = EditorResources.Load<TextAsset>(path, ".txt");
			if (languageAsset != null)
			{
				Load(wordDic, languageAsset.text);
			}

			// sort language path order
			_LanguagePaths.Sort((a, b) =>
			{
				return a.order.CompareTo(b.order);
			});

			// load additional words
			foreach (LanguagePathInternal languagePath in _LanguagePaths)
			{
				path = languagePath.path;
				if (string.IsNullOrEmpty(path))
				{
					continue;
				}
				path = PathUtility.Combine(path, languageName);

				languageAsset = LoadAssetAtPath<TextAsset>(path, ".txt");
				if (languageAsset != null)
				{
					Load(wordDic, languageAsset.text);
				}
			}

			if (wordDic.Count > 0)
			{
				_LanguageDics.Add(language, wordDic);
				_Languages.Add(language);
			}
		}

		static void Load(WordDictionary wordDic, string text)
		{
			foreach (string line in text.Split('\n'))
			{
				if (line.StartsWith("//"))
				{
					continue;
				}

				int firstColonIndex = line.IndexOf(':');
				if (firstColonIndex < 0)
				{
					continue;
				}

				string key = line.Substring(0, firstColonIndex);
				string word = line.Substring(firstColonIndex + 1).Trim().Replace("\\n", "\n");

				wordDic[key] = new GUIContent(word);
			}
		}

		public static GUIContent GetTextContent(SystemLanguage language, string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return GUIContent.none;
			}

			WordDictionary wordDic = null;
			if (_LanguageDics.TryGetValue(language, out wordDic))
			{
				GUIContent content;
				if (wordDic.TryGetValue(key, out content))
				{
					return content;
				}
			}

			if (language != SystemLanguage.English)
			{
				return GetTextContent(SystemLanguage.English, key);
			}

			return new GUIContent(key);
		}

		public static GUIContent GetTextContent(string key)
		{
			return GetTextContent(ArborSettings.currnentLanguage, key);
		}

		public static string GetWord(SystemLanguage language, string key)
		{
			return GetTextContent(language, key).text;
		}

		public static string GetWord(string key)
		{
			return GetWord(ArborSettings.currnentLanguage, key);
		}

		public static void LanguagePopup(Rect position, GUIContent label, GUIStyle style)
		{
			using (new ProfilerScope("LanguagePopup"))
			{
				int languageCount = _Languages.Count;

				bool changed = _LastLanguage != ArborSettings.currnentLanguage;
				_LastLanguage = ArborSettings.currnentLanguage;

				int languageModeCount = 2;
#if ENABLE_UNITY_EDITOR_LANGUAGE
				languageModeCount++;
#endif

				if (_LanguageLabels == null || _LanguageLabels.Length != languageCount + languageModeCount)
				{
					_LanguageLabels = new GUIContent[languageCount + languageModeCount];

					changed = true;
				}

				if (changed)
				{
					_LanguageLabels[0] = EditorGUITools.GetTextContent(GetWord("System") + "(" + GetWord(GetSystemLanguage().ToString()) + ")");

#if ENABLE_UNITY_EDITOR_LANGUAGE
					_LanguageLabels[1] = EditorGUITools.GetTextContent(GetWord("UnityEditor") + "(" + GetWord(GetEditorLanguage().ToString()) + ")");
#endif

					// separator
					_LanguageLabels[languageModeCount - 1] = EditorGUITools.GetTextContent("");

					for (int i = 0; i < languageCount; i++)
					{
						SystemLanguage language = _Languages[i];
						_LanguageLabels[i + languageModeCount] = GetTextContent(language.ToString());
					}
				}

				int selectIndex = 0;
				switch (ArborSettings.languageMode)
				{
					case LanguageMode.Custom:
						{
							for (int i = 0; i < languageCount; i++)
							{
								SystemLanguage language = _Languages[i];
								if (language == ArborSettings.language)
								{
									selectIndex = i + languageModeCount;
									break;
								}
							}
						}
						break;
					case LanguageMode.System:
						selectIndex = 0;
						break;
#if ENABLE_UNITY_EDITOR_LANGUAGE
					case LanguageMode.UnityEditor:
						selectIndex = 1;
						break;
#endif
				}

				EditorGUI.BeginChangeCheck();
				selectIndex = EditorGUI.Popup(position, label, selectIndex, _LanguageLabels, style);
				if (EditorGUI.EndChangeCheck())
				{
					if (selectIndex == 0)
					{
						ArborSettings.languageMode = LanguageMode.System;
					}
#if ENABLE_UNITY_EDITOR_LANGUAGE
					else if (selectIndex == 1)
					{
						ArborSettings.languageMode = LanguageMode.UnityEditor;
					}
#endif
					else if (selectIndex == languageModeCount - 1)
					{
						// separator
					}
					else
					{
						ArborSettings.languageMode = LanguageMode.Custom;
						ArborSettings.language = _Languages[selectIndex - languageModeCount];
					}
				}
			}
		}

		public static void LanguagePopupLayout(GUIContent label, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, style, options);

			LanguagePopup(position, label, style);
		}

		public static bool ContainsLanguage(SystemLanguage language)
		{
			return _LanguageDics.ContainsKey(language);
		}

		public static SystemLanguage GetSystemLanguage()
		{
			SystemLanguage language = Application.systemLanguage;
			if (!ContainsLanguage(language))
			{
				language = SystemLanguage.English;
			}

			return language;
		}

		public static SystemLanguage GetEditorLanguage()
		{
			SystemLanguage language = Utility.GetEditorLanguage();
			if (!ContainsLanguage(language))
			{
				language = SystemLanguage.English;
			}

			return language;
		}

		private static class Utility
		{
			private static readonly System.Func<SystemLanguage> _GetEditorLanguage;

			static Utility()
			{
				var unityEditorAssembly = Assembly.Load("UnityEditor.dll");
				var localizationDatabaseType = unityEditorAssembly.GetType("UnityEditor.LocalizationDatabase");
				if (localizationDatabaseType != null)
				{
					var editorLanguageProperty = localizationDatabaseType.GetProperty("currentEditorLanguage", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					if (editorLanguageProperty != null)
					{
						var getEditorLanguageMethod = editorLanguageProperty.GetGetMethod(true);
						_GetEditorLanguage = (System.Func<SystemLanguage>)System.Delegate.CreateDelegate(typeof(System.Func<SystemLanguage>), getEditorLanguageMethod);
					}
				}
			}

			public static SystemLanguage GetEditorLanguage()
			{
				if (_GetEditorLanguage != null)
				{
					return _GetEditorLanguage();
				}

				return Application.systemLanguage;
			}
		}
	}
}
