//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using System.Collections.Generic;
using UnityEditor;

namespace ArborEditor
{
	public enum PropertyChangedType
	{
		UndoRedoPerformed,
		PostProcessModifications,
	}

	public interface IPropertyChanged
	{
		void OnPropertyChanged(PropertyChangedType propertyChangedType);
	}

	public interface IUpdateCallback
	{
		void OnUpdate();
	}

	public static class EditorCallbackUtility
	{
		static List<IPropertyChanged> s_PropertyChanged = new List<IPropertyChanged>();
		static List<IUpdateCallback> s_UpdateCallbackList = new List<IUpdateCallback>();

		static EditorCallbackUtility()
		{
			SetupPropertyChangedCallback();

			SetupUpdateCallback();
		}

		#region PropertyChanged

		static void SetupPropertyChangedCallback()
		{
			Undo.postprocessModifications += OnPostprocessModifications;
			Undo.undoRedoPerformed += OnUndoRedoPerformed;
		}

		static void OnUndoRedoPerformed()
		{
			PropertyChanged(PropertyChangedType.UndoRedoPerformed);
		}

		static UndoPropertyModification[] OnPostprocessModifications(UndoPropertyModification[] modifications)
		{
			PropertyChanged(PropertyChangedType.PostProcessModifications);
			return modifications;
		}

		static void PropertyChanged(PropertyChangedType propertyChangedType)
		{
			for (int i = s_PropertyChanged.Count - 1; i >= 0; i--)
			{
				var element = s_PropertyChanged[i];
				if (element == null)
				{
					continue;
				}

				element.OnPropertyChanged(propertyChangedType);
			}
		}

		public static void RegisterPropertyChanged(IPropertyChanged propertyChanged)
		{
			if (propertyChanged == null)
			{
				return;
			}

			if (!s_PropertyChanged.Contains(propertyChanged))
			{
				s_PropertyChanged.Add(propertyChanged);
			}
		}

		public static void UnregisterPropertyChanged(IPropertyChanged propertyChanged)
		{
			s_PropertyChanged.Remove(propertyChanged);
		}

		#endregion // PropertyChanged

		#region UpdateCallback

		static void SetupUpdateCallback()
		{
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			for (int i = s_UpdateCallbackList.Count - 1; i >= 0; i--)
			{
				var element = s_UpdateCallbackList[i];
				if (element == null)
				{
					continue;
				}

				element.OnUpdate();
			}
		}

		public static void RegisterUpdateCallback(IUpdateCallback callback)
		{
			if (callback == null)
			{
				return;
			}

			if (!s_UpdateCallbackList.Contains(callback))
			{
				s_UpdateCallbackList.Add(callback);
			}
		}

		public static void UnregisterUpdateCallback(IUpdateCallback callback)
		{
			s_UpdateCallbackList.Remove(callback);
		}

		#endregion // UpdateCallback
	}
}