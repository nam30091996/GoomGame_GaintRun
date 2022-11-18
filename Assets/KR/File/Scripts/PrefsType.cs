using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace KR.PrefsType
{

        [Serializable]
    public class KeyValuePair<T>
    {
        public void Clear()
        {
            Keys.Clear();
            Values.Clear();
        }
        public void Delete(string key)
        {
            for (int i = Keys.Count - 1; i >= 0; i--)
            {
                if (Keys[i] == key)
                {
                    Keys.RemoveAt(i);
                    Values.RemoveAt(i);
                    return;
                }
            }

        }
        public bool HasKey(string key)
        {
            for (int i = Keys.Count - 1; i >= 0; i--)
                if (Keys[i] == key)
                    return true;

            return false;
        }
        public List<string> Keys = new List<string>();
        public List<T> Values = new List<T>();
        public void InitKey(string key, T defaultValue = default(T))
        {

            if (!Keys.Contains(key))
            {

                Keys.Add(key);
                Values.Add(defaultValue);

                if ((Device.instance.logType & LogType.Info) == LogType.Info)
                    Debug.LogFormat("[PlayerPrefs]Created key [{0}]", key);
            }
        }
        public T this[string key]
        {
            get
            {
                for (int i = 0; i < Keys.Count; i++)
                {
                    if (Keys[i] == key)
                    {
                        return Values[i];
                    }
                }
                //throw new KeyNotFoundException();
                //No Key, Add New Key
                throw new KeyNotFoundException();


            }
            set
            {
                for (int i = 0; i < Keys.Count; i++)
                {
                    if (Keys[i] == key)
                    {
                        Values[i] = value;
                    }
                }
            }
        }

    }

	[Serializable]
	public class IntPrefs : KeyValuePair<int> { }
	[Serializable]
	public class StringPrefs : KeyValuePair<string> { }
	[Serializable]
	public class BoolPrefs : KeyValuePair<bool> { }
	[Serializable]
	public class FloatPrefs : KeyValuePair<float> { }
	[Serializable]
	public class Vector2Prefs : KeyValuePair<Vector2> {}
	[Serializable]
    public class Vector3Prefs : KeyValuePair<Vector3> { }
	[Serializable]
	public class QuaternionPrefs : KeyValuePair<Quaternion> { }
	[Serializable]
	public class SpritePrefs : KeyValuePair<Sprite>{}
	[Serializable]
	public class ColorPrefs : KeyValuePair<Color>{}
	[Serializable]
	public class Color32Prefs : KeyValuePair<Color32>{}
	[Serializable]
	public class MaterialPrefs : KeyValuePair<Material>{}
}
