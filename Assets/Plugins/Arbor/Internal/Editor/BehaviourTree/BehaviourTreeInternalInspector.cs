//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

namespace ArborEditor.BehaviourTree
{
	using Arbor;
	using Arbor.BehaviourTree;

	[CustomEditor(typeof(BehaviourTreeInternal), true)]
	internal sealed class BehaviourTreeInternalInspector : Editor, IPropertyChanged
	{
		private static readonly GUIContent _NameContent = new GUIContent("Name");

		BehaviourTreeInternal _BehaviourTree;

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnEnable()
		{
			_BehaviourTree = target as BehaviourTreeInternal;
			ComponentUtility.RefreshNodeGraph(_BehaviourTree);

			EditorCallbackUtility.RegisterPropertyChanged(this);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnDisable()
		{
			EditorCallbackUtility.UnregisterPropertyChanged(this);
		}

		void IPropertyChanged.OnPropertyChanged(PropertyChangedType propertyChangedType)
		{
			if (propertyChangedType != PropertyChangedType.UndoRedoPerformed)
			{
				return;
			}

			if (target == null)
			{
				_BehaviourTree = null;
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("graphName"), _NameContent);
			if (_BehaviourTree.rootGraph == _BehaviourTree)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("playOnStart"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("restartOnFinish"));

				EditorGUILayout.PropertyField(serializedObject.FindProperty("updateSettings"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("executionSettings"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("debugInfiniteLoopSettings"), true);
			}

			serializedObject.ApplyModifiedProperties();

			if (EditorGUITools.ButtonForceEnabled("Open Editor"))
			{
				ArborEditorWindow.Open(_BehaviourTree);
			}
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnDestroy()
		{
			if (!target && (object)_BehaviourTree != null && !Application.isPlaying)
			{
				_BehaviourTree.OnDestroy();
			}
		}
	}
}