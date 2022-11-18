﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using Arbor;

namespace ArborEditor
{
	internal sealed class ComponentEditorProcessor : IComponentProcessor
	{
		List<Object> _DelayDestroyObjects = new List<Object>();
		bool _DelayDestroyEnable = false;

		event ComponentUtility.DelayCallBack delayCallbacks;
		bool _DelayCallEnable = false;

		public Component AddComponent(GameObject gameObject, System.Type type)
		{
			Component component = Undo.AddComponent(gameObject, type);

#if UNITY_2018_1_OR_NEWER
			UnityEditor.Presets.Preset defaultPreset = Presets.PresetUtility.GetDefaultPreset(component);
			if (defaultPreset != null)
			{
#if ARBOR_DEBUG
				Debug.Log("ApplyDefaulPreset : " + defaultPreset);
#endif
				Undo.RecordObject(component, "Add Component");
				Presets.PresetUtility.ApplyPreset(defaultPreset, component);
			}
#endif

			return component;
		}

		public void DelayCall(ComponentUtility.DelayCallBack delayCall)
		{
			delayCallbacks += delayCall;

			if (!_DelayCallEnable)
			{
				EditorApplication.delayCall += OnDelayCall;
				_DelayCallEnable = true;
			}
		}

		public void DelayDestroy(Object obj)
		{
			_DelayDestroyObjects.Add(obj);

			if (!_DelayDestroyEnable)
			{
				EditorApplication.delayCall += OnDelayDestroyObjects;
				_DelayDestroyEnable = true;
			}
		}

		public void Destroy(Object objectToUndo)
		{
			Undo.DestroyObjectImmediate(objectToUndo);
		}

		public void MoveBehaviour(Node node, NodeBehaviour behaviour)
		{
			Clipboard.MoveBehaviour(node, behaviour);
		}

		public void MoveParameterContainer(NodeGraph nodeGraph)
		{
			Clipboard.MoveParameterContainer(nodeGraph);
		}

		public void MoveVariable(Parameter parameter, VariableBase variable)
		{
			Clipboard.MoveVariable(parameter, variable);
		}

		public void MoveVariableList(Parameter parameter, VariableListBase variableList)
		{
			Clipboard.MoveVariableList(parameter, variableList);
		}

		public void RecordObject(Object objectToUndo, string name)
		{
			Undo.RecordObject(objectToUndo, name);
		}

		public void RecordObjects(Object[] objectsToUndo, string name)
		{
			Undo.RecordObjects(objectsToUndo, name);
		}

		public void RegisterCompleteObjectUndo(Object objectToUndo, string name)
		{
			Undo.RegisterCompleteObjectUndo(objectToUndo, name);
		}

		public void SetDirty(Object obj)
		{
			EditorUtility.SetDirty(obj);
		}

		void OnDelayCall()
		{
			if (delayCallbacks != null)
			{
				delayCallbacks();
			}
			delayCallbacks = null;

			_DelayCallEnable = false;
		}

		void OnDelayDestroyObjects()
		{
			foreach (Object obj in _DelayDestroyObjects)
			{
				ComponentUtility.Destroy(obj);
			}
			_DelayDestroyObjects.Clear();

			_DelayDestroyEnable = false;
		}
	}

	[InitializeOnLoad]
	internal static class UndoComponentInitializer
	{
		private static readonly ComponentEditorProcessor _ComponentEditorProcessor = null;

		static UndoComponentInitializer()
		{
			_ComponentEditorProcessor = new ComponentEditorProcessor();

			ComponentUtility.editorProcessor = _ComponentEditorProcessor;
		}
	}
}
