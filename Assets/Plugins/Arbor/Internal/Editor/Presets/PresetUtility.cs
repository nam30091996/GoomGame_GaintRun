//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
#if UNITY_2018_1_OR_NEWER

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Presets;

namespace ArborEditor.Presets
{
	using Arbor;
	using Arbor.DynamicReflection;

	internal static class PresetUtility
	{
		static readonly DynamicField s_ReferencesField;
		static readonly DynamicField s_ReferenceField;

		static PresetUtility()
		{
			var unityEditorAssembly = System.Reflection.Assembly.Load("UnityEditor.dll");
			var presetEditorType = unityEditorAssembly.GetType("UnityEditor.Presets.PresetEditor");
			if (presetEditorType != null)
			{
				var referencesField = presetEditorType.GetField("s_References", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
				s_ReferencesField = DynamicField.GetField(referencesField);
			}

			var referenceCountType = presetEditorType.GetNestedType("ReferenceCount", System.Reflection.BindingFlags.NonPublic);
			if (referenceCountType != null)
			{
				var referenceField = referenceCountType.GetField("reference", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
				s_ReferenceField = DynamicField.GetField(referenceField);
			}
		}

		static void ApplyPresetBehaviour(Preset preset, StateBehaviour target)
		{
			if (target == null)
			{
				return;
			}

			bool expanded = target.expanded;

			ApplyPresetBehaviourInternal(preset, target);

			target.expanded = expanded;

			for (int i = 0, count = target.stateLinkCount; i < count; ++i)
			{
				StateLink s = target.GetStateLink(i);
				s.stateID = 0;
			}
		}

		static void ApplyPresetBehaviourInternal(Preset preset, NodeBehaviour target)
		{
			if (target == null)
			{
				return;
			}

			NodeGraph nodeGraph = target.nodeGraph;
			int nodeID = target.nodeID;

			Clipboard.DestroyChildGraphs(target);

			if (nodeGraph != null)
			{
				nodeGraph.DisconnectDataBranch(target);
			}

			preset.ApplyTo(target);

			target.Initialize(nodeGraph, nodeID);

			Clipboard.CopyChildGraphs(target);
		}

		public static void ApplyPreset(Preset preset, Object target)
		{
			NodeBehaviour nodeBehaviour = target as NodeBehaviour;
			if (nodeBehaviour != null)
			{
				StateBehaviour targetBehaviour = target as StateBehaviour;
				if (targetBehaviour != null)
				{
					ApplyPresetBehaviour(preset, targetBehaviour);
				}
				else
				{
					ApplyPresetBehaviourInternal(preset, nodeBehaviour);
				}
			}
			else
			{
				preset.ApplyTo(target);
			}
		}

		public static bool enableApplyDefaultPreset = true;

		public static bool IsObjectExcludedFromPresets(Object target)
		{
#if UNITY_2019_3_OR_NEWER
			return !new PresetType(target).IsValid();
#else
			return Preset.IsObjectExcludedFromPresets(target);
#endif
		}

		public static Preset GetDefaultPreset(Object target)
		{
			if (!enableApplyDefaultPreset || IsObjectExcludedFromPresets(target))
			{
				return null;
			}

#if UNITY_2019_3_OR_NEWER
			var defaults = Preset.GetDefaultPresetsForObject(target);
			return defaults.Length > 0 ? defaults[0] : null;
#else
			return Preset.GetDefaultForObject(target);
#endif

		}

		public static bool IsPreset(Object target)
		{
			if (s_ReferencesField == null || s_ReferenceField == null)
			{
				return false;
			}
			var referencesObj = s_ReferencesField.GetValue(null);

			IDictionary dicReferences = referencesObj as IDictionary;
			if (dicReferences == null)
			{
				return false;
			}

			foreach (var value in dicReferences.Values)
			{
				var reference = (Object)s_ReferenceField.GetValue(value);
				if (reference == target)
				{
					return true;
				}
			}

			return false;
		}
	}
}

#endif