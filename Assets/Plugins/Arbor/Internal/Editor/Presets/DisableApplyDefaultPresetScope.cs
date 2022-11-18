//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
#if UNITY_2018_1_OR_NEWER

using UnityEngine;

namespace ArborEditor.Presets
{
	public struct DisableApplyDefaultPresetScope : System.IDisposable
	{
		bool _EnableApplyDefaultPreset;
		bool _Disposed;

		public DisableApplyDefaultPresetScope(bool disabled)
		{
			_Disposed = false;

			_EnableApplyDefaultPreset = PresetUtility.enableApplyDefaultPreset;
			PresetUtility.enableApplyDefaultPreset = !disabled;
		}

		public void Dispose()
		{
			if (_Disposed)
			{
				return;
			}

			_Disposed = true;
			PresetUtility.enableApplyDefaultPreset = _EnableApplyDefaultPreset;
		}
	}
}

#endif