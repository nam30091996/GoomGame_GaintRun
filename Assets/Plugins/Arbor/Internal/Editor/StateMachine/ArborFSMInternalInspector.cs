//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

using Arbor;

namespace ArborEditor
{
	[CustomEditor(typeof(ArborFSMInternal), true)]
	internal sealed class ArborFSMInternalInspector : Editor, IPropertyChanged
	{
		private static readonly GUIContent _NameContent = new GUIContent("Name");

		ArborFSMInternal _StateMachine;

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnEnable()
		{
			_StateMachine = target as ArborFSMInternal;
			ComponentUtility.RefreshNodeGraph(_StateMachine);

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
				_StateMachine = null;
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("graphName"), _NameContent);
			if (_StateMachine.rootGraph == _StateMachine)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("playOnStart"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("updateSettings"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("debugInfiniteLoopSettings"), true);
			}

			serializedObject.ApplyModifiedProperties();

			if (EditorGUITools.ButtonForceEnabled("Open Editor"))
			{
				ArborEditorWindow.Open(_StateMachine);
			}
#if false
			foreach( StateBehaviour behaviour in _StateMachine.GetComponents<StateBehaviour>() )
			{
				State state = behaviour.state;
				if( state == null )
				{
					EditorGUILayout.BeginHorizontal();

					EditorGUILayout.LabelField( "Missing Behaviour : " + behaviour.GetType () );
					if( GUILayout.Button( "Delete" ) )
					{
						DestroyImmediate( behaviour );
					}

					EditorGUILayout.EndHorizontal();
				}
			}
#endif
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnDestroy()
		{
			if (!target && (object)_StateMachine != null && !Application.isPlaying)
			{
				_StateMachine.OnDestroy();
			}
		}
	}
}
