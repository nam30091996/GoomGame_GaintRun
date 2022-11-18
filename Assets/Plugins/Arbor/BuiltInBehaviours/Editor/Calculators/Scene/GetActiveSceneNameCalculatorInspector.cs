//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;

namespace ArborEditor.Calculators
{
	using Arbor.Calculators;

	[CustomEditor(typeof(GetActiveSceneNameCalculator))]
	public class GetActiveSceneNameCalculatorInspector : CalculatorBehaviourEditor
	{
		public override bool IsResizableNode()
		{
			return false;
		}

		public override float GetNodeWidth()
		{
			return 200f;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("_Output"));

			serializedObject.ApplyModifiedProperties();
		}
	}
}