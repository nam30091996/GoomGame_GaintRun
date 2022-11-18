using UnityEngine;
using System.Collections;
using UnityEditor;
namespace KR.Editor
{
	[CustomEditor(typeof(KR.UI.Button))]
	public class ButtonEditor : UnityEditor.Editor
	{
		KR.UI.Button button;

		private SerializedProperty onPointerDownProperty, onPointerUpProperty, onPointerHoverProperty;


		private void OnEnable()
		{
			button = target as KR.UI.Button;
			onPointerDownProperty = this.serializedObject.FindProperty("onPointerDown");
			onPointerUpProperty = this.serializedObject.FindProperty("onPointerUp");
			onPointerHoverProperty = this.serializedObject.FindProperty("onPointerHover");



		}

		bool foldout;
		public override void OnInspectorGUI()
		{

			base.OnInspectorGUI();
			this.serializedObject.Update();
			foldout = EditorGUILayout.Foldout(foldout, "Custom Event");
			if (foldout)
			{
				GUI.enabled = false;
                button.isPointerDown = EditorGUILayout.Toggle("Is Pointer Down", button.isPointerDown);
                GUI.enabled = true;
				EditorGUILayout.PropertyField(this.onPointerDownProperty);
				EditorGUILayout.PropertyField(this.onPointerUpProperty);
				EditorGUILayout.PropertyField(this.onPointerHoverProperty);
			}
			this.serializedObject.ApplyModifiedProperties();

		}
	}
}


