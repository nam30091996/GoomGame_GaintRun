using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using KR.PrefsType;
namespace KR
{

	public sealed class PlayerPrefs : KR.Serializable<PlayerPrefs>
	{





		public IntPrefs Int = new IntPrefs();
		public StringPrefs String = new StringPrefs();
		public FloatPrefs Float = new FloatPrefs();
		public BoolPrefs Bool = new BoolPrefs();
		public QuaternionPrefs Quaternion = new QuaternionPrefs();
		public Vector2Prefs Vector2 = new Vector2Prefs();
		public Vector3Prefs Vector3 = new Vector3Prefs();
		public SpritePrefs Sprite = new SpritePrefs();
		public ColorPrefs Color = new ColorPrefs();
		public Color32Prefs Color32 = new Color32Prefs();

		/// <summary>
		/// Removes all keys and values from the preferences. Use with caution.
		/// Call this function in a script to delete all current settings in the PlayerPrefs. Any values or keys have previously been set up are then reset. Be careful when using this.
		/// </summary>
		public static void DeleteAll()
		{
			instance.Int.Clear();
			instance.Bool.Clear();
			instance.Float.Clear();
			instance.String.Clear();
			instance.Vector2.Clear();
			instance.Vector3.Clear();
			instance.Sprite.Clear();
			instance.Color.Clear();
			instance.Color32.Clear();

		}
		/// <summary>
		/// Removes key and its corresponding value from the preferences.
		/// </summary>
		/// <param name="key">Key.</param>
		public static void DeleteKey(string key)
		{
			instance.Int.Delete(key);
			instance.Float.Delete(key);
			instance.String.Delete(key);
			instance.Bool.Delete(key);
			instance.Vector2.Delete(key);
			instance.Vector3.Delete(key);
			instance.Sprite.Delete(key);
			instance.Color.Delete(key);
			instance.Color32.Delete(key);
		}

		/// <summary>
		/// Returns true if key exists in the preferences.
		/// </summary>
		/// <returns><c>true</c>, if key was hased, <c>false</c> otherwise.</returns>
		public static bool HasKey(string key)
		{
			return instance.Int.HasKey(key) ||
				   instance.Float.HasKey(key) ||
				   instance.String.HasKey(key) ||
				   instance.Bool.HasKey(key) ||
						   instance.Vector2.HasKey(key) ||
						   instance.Vector3.HasKey(key) ||
						   instance.Sprite.HasKey(key) ||
						   instance.Color.HasKey(key) 



						   ;

		}



		public static void SetInt(string key, int value)
		{
			instance.Int.InitKey(key);
			instance.Int[key] = value;
		}
		public static int GetInt(string key, int defaultValue = default(int))
		{
			instance.Int.InitKey(key, defaultValue);
			return instance.Int[key];
		}
		public static void SetFloat(string key, float value)
		{
			instance.Float.InitKey(key);
			instance.Float[key] = value;
		}
		public static float GetFloat(string key, float defaultValue = default(float))
		{
			instance.Float.InitKey(key, defaultValue);
			return instance.Float[key];
		}
		public static void SetBool(string key, bool value)
		{
			instance.Bool.InitKey(key);
			instance.Bool[key] = value;
		}
		public static bool GetBool(string key, bool defaultValue = default(bool))
		{
			instance.Bool.InitKey(key, defaultValue);
			return instance.Bool[key];
		}
		public static void SetString(string key, string value)
		{
			instance.String.InitKey(key);
			instance.String[key] = value;
		}
		public static string GetString(string key, string defaultValue = "")
		{
			instance.String.InitKey(key, defaultValue);
			return instance.String[key];
		}
	}
}
