//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace ArborEditor
{
	using Arbor;

	internal abstract class ListParameterEditorBase : PropertyEditor
	{
		protected SerializedProperty _ListProperty;
		protected ReorderableList _ReorderableList;
		protected GUIContent _Label;

		protected abstract SerializedProperty GetListProperty();

		protected override sealed void OnInitialize()
		{
			_ListProperty = GetListProperty();
			_ReorderableList = new ReorderableList(_ListProperty.serializedObject, _ListProperty)
			{
				drawHeaderCallback = OnDrawHeader,
				drawElementCallback = OnDrawElement,
				elementHeightCallback = GetElementHeight,
				drawElementBackgroundCallback = DrawElementBackground,
			};
		}

		protected override sealed float GetHeight(GUIContent label)
		{
			_Label = label;
			return _ReorderableList.GetHeight();
		}

		protected override sealed void OnGUI(Rect position, GUIContent label)
		{
			_Label = label;
			_ReorderableList.DoList(position);
		}

		protected virtual void OnDrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, _Label);
		}

		protected virtual void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty valueProperty = _ReorderableList.serializedProperty.GetArrayElementAtIndex(index);

			EditorGUI.PropertyField(rect, valueProperty, true);
		}

		protected virtual float GetElementHeight(int index)
		{
			SerializedProperty property = _ReorderableList.serializedProperty.GetArrayElementAtIndex(index);

			return Mathf.Max(EditorGUI.GetPropertyHeight(property), _ReorderableList.elementHeight);
		}

		void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
		{
			if (Event.current.type == EventType.Repaint)
			{
				if (index >= 0)
				{
					rect.height = GetElementHeight(index);
				}

				rect = Defaults.entryBackPadding.Remove(rect);

				GUIStyle style = ((index + 1) % 2 == 0) ? ArborEditor.Styles.entryBackEven : ArborEditor.Styles.entryBackOdd;
				style.Draw(rect, false, isActive, isFocused, false);
			}
		}

		private static class Defaults
		{
			public static readonly RectOffset entryBackPadding = new RectOffset(2, 2, 0, 0);
		}
	}

	internal class ListParameterEditor : ListParameterEditorBase
	{
		protected override SerializedProperty GetListProperty()
		{
			return property.FindPropertyRelative("list");
		}
	}

	[CustomPropertyDrawer(typeof(ListParameterBase), true)]
	internal sealed class ListParameterPropertyDrawer : PropertyEditorDrawer<ListParameterEditor>
	{
	}
}