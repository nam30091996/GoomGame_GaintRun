﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
#if !ARBOR_DLL && UNITY_2018_1_OR_NEWER
#define ENABLE_UNITY_EDITOR_LANGUAGE
#endif

using UnityEngine;
using UnityEngine.Serialization;

namespace ArborEditor
{
	[System.Reflection.Obfuscation(Exclude = true)]
	[FilePathAttribute("ArborSettings.asset", FilePathAttribute.Location.PreferencesFolder)]
	public sealed class ArborSettings : EditorSettings<ArborSettings>, ISerializationCallbackReceiver
	{
		private const bool kDefaultShowGrid = true;
		private const bool kDefaultSnapGrid = true;
		internal const float kDefaultGridSize = 120.0f;
		internal const int kDefaultGridSplitNum = 10;
		private const LanguageMode kDefaultLanguageMode = LanguageMode.System;
		private const SystemLanguage kDefaultLanguage = SystemLanguage.English;
		private const bool kDefaultOpenSidePanel = true;
		private const float kDefaultSidePanelWidth = 200.0f;
		private const LogoShowMode kDefaultShowLogo = LogoShowMode.FadeOut;
		private const bool kDefaultLiveTracking = true;
		private const bool kDefaultLiveTrackingHierarchy = true;
		private const bool kDefaultDockingOpen = true;
		private const bool kDefaultShowDataValue = false;
		private const bool kDefaultNodeCommentAffectsZoom = false;
		private const NodeCommentViewMode kDefaultNodeCommentViewMode = NodeCommentViewMode.Normal;
		private const MouseWheelMode kDefaultMouseWheelMode = MouseWheelMode.Zoom;
		private const SidePanelTab kDefaultSidePanelTab = SidePanelTab.Graph;
		private const DataSlotShowMode kDefaultDataSlotShowMode = DataSlotShowMode.Outside;
		private const StateLinkShowMode kDefaultStateLinkShowMode = StateLinkShowMode.BehaviourBottom;

		public delegate void OnChangedLanguage();

		public static event OnChangedLanguage onChangedLanguage;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private bool _ShowGrid = kDefaultShowGrid;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private bool _SnapGrid = kDefaultSnapGrid;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private float _GridSize = kDefaultGridSize;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private int _GridSplitNum = kDefaultGridSplitNum;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		[FormerlySerializedAs("_AutoLanguage")]
		private LanguageMode _LanguageMode = kDefaultLanguageMode;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private SystemLanguage _Language = kDefaultLanguage;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		[FormerlySerializedAs("_OpenStateList")]
		private bool _OpenSidePanel = kDefaultOpenSidePanel;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		[FormerlySerializedAs("_StateListWidth")]
		private float _SidePanelWidth = kDefaultSidePanelWidth;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private LogoShowMode _ShowLogo = kDefaultShowLogo;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private bool _LiveTracking = kDefaultLiveTracking;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private bool _LiveTrackingHierarchy = kDefaultLiveTrackingHierarchy;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private bool _DockingOpen = kDefaultDockingOpen;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private bool _NodeCommentAffectsZoom = kDefaultNodeCommentAffectsZoom;


		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private NodeCommentViewMode _NodeCommentViewMode = kDefaultNodeCommentViewMode;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private MouseWheelMode _MouseWheelMode = kDefaultMouseWheelMode;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private bool _ShowDataValue = kDefaultShowDataValue;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private SidePanelTab _SidePanelTab = kDefaultSidePanelTab;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private DataSlotShowMode _DataSlotShowMode = kDefaultDataSlotShowMode;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private StateLinkShowMode _StateLinkShowMode = kDefaultStateLinkShowMode;

		public static bool showGrid
		{
			get
			{
				return instance._ShowGrid;
			}
			set
			{
				if (instance._ShowGrid != value)
				{
					instance._ShowGrid = value;

					Save();
				}
			}
		}

		public static bool snapGrid
		{
			get
			{
				return instance._SnapGrid;
			}
			set
			{
				if (instance._SnapGrid != value)
				{
					instance._SnapGrid = value;

					Save();
				}
			}
		}

		public static float gridSize
		{
			get
			{
				return instance._GridSize;
			}
			set
			{
				value = Mathf.Clamp(value, 1.0f, 1000.0f);
				if (instance._GridSize != value)
				{
					instance._GridSize = value;

					Save();
				}
			}
		}

		public static int gridSplitNum
		{
			get
			{
				return instance._GridSplitNum;
			}
			set
			{
				value = Mathf.Clamp(value, 1, 100);
				if (instance._GridSplitNum != value)
				{
					instance._GridSplitNum = value;

					Save();
				}
			}
		}

		public static LanguageMode languageMode
		{
			get
			{
				return instance._LanguageMode;
			}
			set
			{
				value = ValidLanguageMode(value);
				if (instance._LanguageMode != value)
				{
					SystemLanguage preLanguage = currnentLanguage;

					instance._LanguageMode = value;

					if (preLanguage != currnentLanguage && onChangedLanguage != null)
					{
						onChangedLanguage();
					}

					Save();
				}
			}
		}

		public static SystemLanguage language
		{
			get
			{
				return instance._Language;
			}
			set
			{
				if (instance._Language != value)
				{
					SystemLanguage preLanguage = currnentLanguage;

					instance._Language = value;

					if (preLanguage != currnentLanguage && onChangedLanguage != null)
					{
						onChangedLanguage();
					}

					Save();
				}
			}
		}

		public static bool openSidePanel
		{
			get
			{
				return instance._OpenSidePanel;
			}
			set
			{
				if (instance._OpenSidePanel != value)
				{
					instance._OpenSidePanel = value;

					Save();
				}
			}
		}

		public static float sidePanelWidth
		{
			get
			{
				return instance._SidePanelWidth;
			}
			set
			{
				if (instance._SidePanelWidth != value)
				{
					instance._SidePanelWidth = value;

					Save();
				}
			}
		}

		public static LogoShowMode showLogo
		{
			get
			{
#if ARBOR_TRIAL
				return LogoShowMode.AlwaysShow;
#else
				return instance._ShowLogo;
#endif
			}
			set
			{
#if !ARBOR_TRIAL
				if (instance._ShowLogo != value)
				{
					instance._ShowLogo = value;

					Save();
				}
#endif
			}
		}

		public static bool liveTracking
		{
			get
			{
				return instance._LiveTracking;
			}
			set
			{
				if (instance._LiveTracking != value)
				{
					instance._LiveTracking = value;

					Save();
				}
			}
		}

		public static bool liveTrackingHierarchy
		{
			get
			{
				return instance._LiveTrackingHierarchy;
			}
			set
			{
				if (instance._LiveTrackingHierarchy != value)
				{
					instance._LiveTrackingHierarchy = value;

					Save();
				}
			}
		}

		public static bool dockingOpen
		{
			get
			{
				return instance._DockingOpen;
			}
			set
			{
				if (instance._DockingOpen != value)
				{
					instance._DockingOpen = value;

					Save();
				}
			}
		}

		public static bool nodeCommentAffectsZoom
		{
			get
			{
				return instance._NodeCommentAffectsZoom;
			}
			set
			{
				if (instance._NodeCommentAffectsZoom != value)
				{
					instance._NodeCommentAffectsZoom = value;

					Save();
				}
			}
		}

		public static NodeCommentViewMode nodeCommentViewMode
		{
			get
			{
				return instance._NodeCommentViewMode;
			}
			set
			{
				if (instance._NodeCommentViewMode != value)
				{
					instance._NodeCommentViewMode = value;

					Save();
				}
			}
		}

		public static MouseWheelMode mouseWheelMode
		{
			get
			{
				return instance._MouseWheelMode;
			}
			set
			{
				if (instance._MouseWheelMode != value)
				{
					instance._MouseWheelMode = value;

					Save();
				}
			}
		}

		public static bool showDataValue
		{
			get
			{
				return instance._ShowDataValue;
			}
			set
			{
				if (instance._ShowDataValue != value)
				{
					instance._ShowDataValue = value;

					Save();
				}
			}
		}

		public static SidePanelTab sidePanelTab
		{
			get
			{
				return instance._SidePanelTab;
			}
			set
			{
				if (instance._SidePanelTab != value)
				{
					instance._SidePanelTab = value;

					Save();
				}
			}
		}

		public static DataSlotShowMode dataSlotShowMode
		{
			get
			{
				return instance._DataSlotShowMode;
			}
			set
			{
				if (instance._DataSlotShowMode != value)
				{
					instance._DataSlotShowMode = value;

					Save();
				}
			}
		}

		public static StateLinkShowMode stateLinkShowMode
		{
			get
			{
				return instance._StateLinkShowMode;
			}
			set
			{
				if (instance._StateLinkShowMode != value)
				{
					instance._StateLinkShowMode = value;

					Save();
				}
			}
		}

		public static SystemLanguage currnentLanguage
		{
			get
			{
				switch (instance._LanguageMode)
				{
					case LanguageMode.Custom:
						return instance._Language;
					case LanguageMode.System:
						return Localization.GetSystemLanguage();
					case LanguageMode.UnityEditor:
						return Localization.GetEditorLanguage();
				}

				return instance._Language;
			}
		}

		public static void ResetGrid()
		{
			instance._ShowGrid = kDefaultShowGrid;
			instance._SnapGrid = kDefaultSnapGrid;
			instance._GridSize = kDefaultGridSize;
			instance._GridSplitNum = kDefaultGridSplitNum;

			Save();
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		static LanguageMode ValidLanguageMode(LanguageMode languageMode)
		{
#if !ENABLE_UNITY_EDITOR_LANGUAGE
			if (languageMode == LanguageMode.UnityEditor)
			{
				languageMode = LanguageMode.System;
			}
#endif
			return languageMode;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			_LanguageMode = ValidLanguageMode(_LanguageMode);
		}
	}
}
