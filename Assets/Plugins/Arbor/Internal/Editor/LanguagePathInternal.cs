//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.IO;

namespace ArborEditor
{
	public class LanguagePathInternal : ScriptableObject
	{
		[System.NonSerialized]
		private string _Path = null;
		public string path
		{
			get
			{
				return _Path;
			}
		}

		public int order = 0;

		internal void Setup()
		{
			if (string.IsNullOrEmpty(_Path))
			{
				string assetPath = AssetDatabase.GetAssetPath(this);
				if (!string.IsNullOrEmpty(assetPath))
				{
					_Path = Path.GetDirectoryName(assetPath);
				}
				else
				{
					_Path = null;
				}
			}
		}
	}
}