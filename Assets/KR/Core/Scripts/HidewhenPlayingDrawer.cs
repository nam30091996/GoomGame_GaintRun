using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HideWhenPlayingAttribute : PropertyAttribute
{
	public HideWhenPlayingAttribute() { }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(HideWhenPlayingAttribute))]
public class HidewhenPlayingDrawer : PropertyDrawer
{

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (!Application.isPlaying)
			EditorGUI.PropertyField(position, property, label, true);
	}
}
#endif