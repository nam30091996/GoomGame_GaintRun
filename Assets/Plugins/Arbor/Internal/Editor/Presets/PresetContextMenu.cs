﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
#if UNITY_2018_1_OR_NEWER

using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;

namespace ArborEditor.Presets
{
	using Arbor;

	internal sealed class PresetContextMenu : PresetSelectorReceiver
	{
		private static class Style
		{
			public static readonly GUIStyle bottomBarBg = "ProjectBrowserBottomBarBg";
			public static readonly GUIStyle toolbarBack = "ObjectPickerToolbar";
			public static readonly GUIContent presetIcon = EditorGUIUtility.IconContent("Preset.Context");
		}

		Object _Target;
		Preset _InitialValue;

		System.Action _OnChanged;

		private bool _Applied = false;

		internal static bool HasPresetButton(Object target)
		{
			if (PresetUtility.IsObjectExcludedFromPresets(target)
				|| (target.hideFlags & HideFlags.NotEditable) != 0)
			{
				return false;
			}

			return true;
		}

		internal static bool PresetButton(Rect position, Object target, System.Action onChanged,GUIStyle style)
		{
			if (!HasPresetButton(target))
			{
				return false;
			}

			if (EditorGUI.DropdownButton(position, Style.presetIcon, FocusType.Passive, style))
			{
				CreateAndShow(target,onChanged);
			}

			return true;
		}

		internal static bool PresetButton(Rect position, Object target, System.Action onChanged)
		{
			return PresetButton(position, target, onChanged, Styles.iconButton);
		}

		static void CreateAndShow(Object target,System.Action onChanged)
		{
			var instance = CreateInstance<PresetContextMenu>();
			instance.Init(target, onChanged);
			PresetSelector.ShowSelector(target, null, true, instance);
		}

		internal void Init(Object target,System.Action onChanged)
		{
			_Target = target;
			_InitialValue = new Preset(target);
			_OnChanged = onChanged;
			_Applied = false;
		}

		public override void OnSelectionChanged(Preset selection)
		{
			if (selection != null)
			{
				Undo.RecordObject(_Target, "Apply Preset " + selection.name);

				PresetUtility.ApplyPreset(selection, _Target);

				_Applied = true;
			}
			else
			{
				if (_Applied)
				{
					Undo.RecordObject(_Target, "Cancel Preset");

					PresetUtility.ApplyPreset(_InitialValue, _Target);
				}
			}

			if (_OnChanged != null)
			{
				_OnChanged();
			}
		}

		public override void OnSelectionClosed(Preset selection)
		{
			OnSelectionChanged(selection);
			DestroyImmediate(this);
		}
	}
}

#endif