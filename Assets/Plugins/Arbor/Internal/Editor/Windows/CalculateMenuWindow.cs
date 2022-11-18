//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

using Arbor;

namespace ArborEditor
{
	[System.Reflection.Obfuscation(Exclude = true)]
	[InitializeOnLoad]
	internal sealed class CalculateMenuWindow : NodeBehaviourMenuWindow
	{
		private static CalculateMenuWindow _Instance;

		public static CalculateMenuWindow instance
		{
			get
			{
				if (_Instance == null)
				{
					CalculateMenuWindow[] objects = Resources.FindObjectsOfTypeAll<CalculateMenuWindow>();
					if (objects.Length > 0)
					{
						_Instance = objects[0];
					}
				}
				if (_Instance == null)
				{
					_Instance = ScriptableObject.CreateInstance<CalculateMenuWindow>();
				}
				return _Instance;
			}
		}

		private Vector2 _Position;

		protected override string searchWord
		{
			get
			{
				return ArborEditorCache.calculatorSearch;
			}

			set
			{
				ArborEditorCache.calculatorSearch = value;
			}
		}

		protected override System.Type GetClassType()
		{
			return typeof(Calculator);
		}

		protected override string GetRootElementName()
		{
			return "Calculators";
		}

		protected override void OnSelect(System.Type classType)
		{
			graphEditor.CreateCalculator(_Position, classType);
		}

		public void Init(NodeGraphEditor graphEditor, Vector2 position, Rect buttonRect)
		{
			_Position = position;

			Open(graphEditor, buttonRect);
		}
	}
}
