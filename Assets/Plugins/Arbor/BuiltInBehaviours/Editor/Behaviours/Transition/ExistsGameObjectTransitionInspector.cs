//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using Arbor.StateMachine.StateBehaviours;

namespace ArborEditor.StateMachine.StateBehaviours
{
	[CustomEditor(typeof(ExistsGameObjectTransition))]
	internal sealed class ExistsGameObjectTransitionInspector : Editor
	{
		private ReorderableList _TargetsList;

		void OnEnable()
		{
			SerializedProperty targetsProperty = serializedObject.FindProperty("_Targets");

			_TargetsList = new ReorderableList(serializedObject, targetsProperty, false, true, true, true)
			{
				drawHeaderCallback = DrawHeader,
				drawElementCallback = DrawElement,
				elementHeightCallback = ElementHeightListener,
				drawElementBackgroundCallback = DrawElementBackground,
				onRemoveCallback = OnRemove,
			};
		}

		void DrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, _TargetsList.serializedProperty.displayName);
		}

		void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty property = _TargetsList.serializedProperty.GetArrayElementAtIndex(index);

			float height = EditorGUI.GetPropertyHeight(property, EditorGUITools.GetTextContent(property.displayName), true);

			float maxHeight = Mathf.Max(rect.height, height);

			rect.yMin += (maxHeight - height) * 0.5f;

			EditorGUI.PropertyField(rect, property, true);
		}

		float ElementHeightListener(int index)
		{
			SerializedProperty property = _TargetsList.serializedProperty.GetArrayElementAtIndex(index);

			return Mathf.Max(EditorGUI.GetPropertyHeight(property, EditorGUITools.GetTextContent(property.displayName), true), _TargetsList.elementHeight);
		}

		private static class Defaults
		{
			public static readonly RectOffset entryBackPadding = new RectOffset(2, 2, 0, 0);
		}

		void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
		{
			if (Event.current.type == EventType.Repaint)
			{
				if (index >= 0)
				{
					rect.height = ElementHeightListener(index);
				}

				rect = Defaults.entryBackPadding.Remove(rect);

				GUIStyle style = ((index + 1) % 2 == 0) ? ArborEditor.Styles.entryBackEven : ArborEditor.Styles.entryBackOdd;
				style.Draw(rect, false, isActive, isFocused, false);
			}
		}

		private void OnRemove(ReorderableList list)
		{
			SerializedProperty property = _TargetsList.serializedProperty.GetArrayElementAtIndex(list.index);

			FlexibleSceneObjectProperty flexibleProperty = new FlexibleSceneObjectProperty(property);

			flexibleProperty.Disconnect();

			ReorderableList.defaultBehaviours.DoRemoveButton(_TargetsList);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			_TargetsList.DoLayoutList();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_InputTargets"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_CheckActive"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_CheckUpdate"));

			serializedObject.ApplyModifiedProperties();
		}
	}
}