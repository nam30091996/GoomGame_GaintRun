//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;

namespace ArborEditor.BehaviourTree.Actions
{
	using Arbor;
	using Arbor.BehaviourTree.Actions;

	[CustomEditor(typeof(SubBehaviourTreeReference))]
	internal sealed class SubBehaviourTreeReferenceInspector : NodeBehaviourEditor
	{
		private FlexibleFieldProperty _ExternalBTProperty;

		private GraphArgumentListEditor _ArgumentListEditor = null;

		private GraphArgumentListEditor argumentListEditor
		{
			get
			{
				if (_ArgumentListEditor == null)
				{
					_ArgumentListEditor = new GraphArgumentListEditor(serializedObject.FindProperty("_ArgumentList"));
				}

				_ArgumentListEditor.nodeGraph = GetExternalGraph();

				return _ArgumentListEditor;
			}
		}

		NodeGraph GetExternalGraph()
		{
			FlexibleType type = _ExternalBTProperty.type;
			if (type == FlexibleType.Constant)
			{
				return _ExternalBTProperty.valueProperty.objectReferenceValue as NodeGraph;
			}

			return null;
		}

		private void OnEnable()
		{
			_ExternalBTProperty = new FlexibleFieldProperty(serializedObject.FindProperty("_ExternalBT"));
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(_ExternalBTProperty.property);
			if (EditorGUI.EndChangeCheck())
			{
				argumentListEditor.UpdateNodeGraph(GetExternalGraph());
			}

			EditorGUILayout.PropertyField(serializedObject.FindProperty("_UsePool"));

			SubBehaviourTreeReference subBehaviourTree = target as SubBehaviourTreeReference;
			NodeGraph nodeGraph = subBehaviourTree.runtimeBT;

			if (nodeGraph != null)
			{
				if (EditorGUITools.ButtonForceEnabled("Open " + nodeGraph.displayGraphName, ArborEditor.Styles.largeButton))
				{
					if (graphEditor != null)
					{
						graphEditor.hostWindow.ChangeCurrentNodeGraph(nodeGraph);
					}
				}
			}

			argumentListEditor.DoLayoutList();


			serializedObject.ApplyModifiedProperties();
		}
	}
}
