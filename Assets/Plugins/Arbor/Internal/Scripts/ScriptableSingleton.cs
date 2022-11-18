//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// ScriptableObjectをシングルトンにするクラス。
	/// </summary>
#else
	/// <summary>
	/// Class that the ScriptableObject to Singleton.
	/// </summary>
#endif
	public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
	{
		private static T _Instance = null;

#if ARBOR_DOC_JA
		/// <summary>
		/// インスタンスを取得する。
		/// </summary>
#else
		/// <summary>
		/// To get an instance.
		/// </summary>
#endif
		public static T instance
		{
			get
			{
				if (_Instance == null)
				{
					T[] objects = Resources.FindObjectsOfTypeAll<T>();
					if (objects.Length > 0)
					{
						_Instance = objects[0];
					}
				}
				if (_Instance == null)
				{
					_Instance = ScriptableObject.CreateInstance<T>();
					_Instance.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
				}
				return _Instance;
			}
		}
	}
}
