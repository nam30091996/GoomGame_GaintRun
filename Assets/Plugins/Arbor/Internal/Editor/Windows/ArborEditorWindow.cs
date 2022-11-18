//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
#if !ARBOR_DLL
#if UNITY_2017_3_OR_NEWER
#define ARBOR_EDITOR_USE_UIELEMENTS
#endif

#define ARBOR_EDITOR_EXTENSIBLE
#define ARBOR_EDITOR_CAPTURABLE
#endif

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_2017_1_OR_NEWER && (ARBOR_EDITOR_USE_UIELEMENTS || ARBOR_EDITOR_CAPTURABLE)
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#else
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.Experimental.UIElements.StyleSheets;
using UnityEditor.Experimental.UIElements;
#endif
#endif

namespace ArborEditor
{
	using Arbor;
	using Arbor.DynamicReflection;
	using ArborEditor.UpdateCheck;

#if ARBOR_EDITOR_USE_UIELEMENTS
	using ArborEditor.Internal;
#endif

	[System.Reflection.Obfuscation(Exclude = true)]
	public sealed class ArborEditorWindow : EditorWindow, IHasCustomMenu, IPropertyChanged, IUpdateCallback
	{
		#region static
		private static GUIContent s_DefaultTitleContent = null;
		private const float k_ShowLogoTime = 3.0f;
		private const float k_FadeLogoTime = 1.0f;

#if ARBOR_EDITOR_EXTENSIBLE
		public static event System.Action<NodeGraph> toolbarGUI;
		public static event System.Action<NodeGraph, Rect> underlayGUI;
		public static event System.Action<NodeGraph, Rect> overlayGUI;
		public static ISkin skin;
#endif

#if ARBOR_EDITOR_USE_UIELEMENTS
#if UNITY_2020_1_OR_NEWER
		private static readonly MethodInfo s_GetWorldBoundingBoxMethod;
		private static readonly MethodInfo s_SetLastWorldClipMethod;
#endif
#else
		private static readonly DynamicField s_ParentField;
		private static readonly DynamicMethod s_GetBorderSizeMethod;
#endif

		private static class Types
		{
			public static readonly System.Type ArborFSMType;
			public static readonly System.Type BehaviourTreeType;

			static Types()
			{
				ArborFSMType = AssemblyHelper.GetTypeByName("Arbor.ArborFSM");
				BehaviourTreeType = AssemblyHelper.GetTypeByName("Arbor.BehaviourTree.BehaviourTree");
			}
		}

		public static ArborEditorWindow activeWindow
		{
			get;
			private set;
		}

		private static GUIContent defaultTitleContent
		{
			get
			{
				if (s_DefaultTitleContent == null)
				{
					s_DefaultTitleContent = new GUIContent("Arbor Editor", EditorGUIUtility.isProSkin ? Icons.logoIcon_DarkSkin : Icons.logoIcon_LightSkin);
				}
				return s_DefaultTitleContent;
			}
		}

		public static bool zoomable
		{
			get
			{
#if ARBOR_EDITOR_USE_UIELEMENTS
				return true;
#else
				return false;
#endif
			}
		}

		public static bool nodeCommentAffectsZoom
		{
			get
			{
				return zoomable && ArborSettings.nodeCommentAffectsZoom;
			}
		}

		public static bool captuarable
		{
			get
			{
#if ARBOR_EDITOR_CAPTURABLE
				return true;
#else
				return false;
#endif
			}
		}

		public static bool isBuildDocuments
		{
			get;
			set;
		}

		public static bool isInNodeEditor
		{
			get
			{
				if (isBuildDocuments)
				{
					return true;
				}

				ArborEditorWindow window = activeWindow;
				if (window != null)
				{
					return window._IsInNodeEditor;
				}

				return false;
			}
		}

		public static bool isInParametersPanel
		{
			get
			{
				ArborEditorWindow window = activeWindow;
				if (window != null)
				{
					return window._IsInParametersPanel;
				}

				return false;
			}
		}

		internal static bool isDarkSkin
		{
			get
			{
#if ARBOR_EDITOR_EXTENSIBLE
				if (skin != null)
				{
					return skin.isDarkSkin;
				}
#endif
				return EditorGUIUtility.isProSkin;
			}
		}

		static ArborEditorWindow()
		{
#if ARBOR_EDITOR_USE_UIELEMENTS
#if UNITY_2020_1_OR_NEWER
			var worldBoundingBoxProperty = typeof(VisualElement).GetProperty("worldBoundingBox", BindingFlags.Instance | BindingFlags.NonPublic);
			s_GetWorldBoundingBoxMethod = worldBoundingBoxProperty.GetGetMethod(true);
			
			var lastWorldClipProperty = typeof(IMGUIContainer).GetProperty("lastWorldClip", BindingFlags.Instance | BindingFlags.NonPublic);
			s_SetLastWorldClipMethod = lastWorldClipProperty.GetSetMethod(true);
#endif
#else
			FieldInfo parentField = typeof(EditorWindow).GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic);
			if (parentField != null)
			{
				s_ParentField = DynamicField.GetField(parentField);
			}

			Assembly assemblyUnityEditor = Assembly.Load("UnityEditor.dll");
			System.Type hostViewType = assemblyUnityEditor.GetType("UnityEditor.HostView");

			PropertyInfo borderSizeProperty = hostViewType.GetProperty("borderSize", BindingFlags.Instance | BindingFlags.NonPublic);
			if (borderSizeProperty != null)
			{
				s_GetBorderSizeMethod = DynamicMethod.GetMethod(borderSizeProperty.GetGetMethod(true));
			}
#endif
		}

		static ArborEditorWindow Open()
		{
			ArborEditorWindow window = ArborSettings.dockingOpen ? EditorWindow.GetWindow<ArborEditorWindow>(typeof(SceneView)) : EditorWindow.GetWindow<ArborEditorWindow>();
			window.titleContent = defaultTitleContent;
			return window;
		}

		[MenuItem("Window/Arbor/Arbor Editor")]
		public static void OpenFromMenu()
		{
			Open();
		}

		public static void Open(NodeGraph nodeGraph)
		{
			ArborEditorWindow window = Open();
			window.Initialize(nodeGraph);
		}

		#endregion // static

		#region Serialize fields
		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private NodeGraph _NodeGraphRoot = null;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private int _NodeGraphRootInstanceID = 0;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private NodeGraph _NodeGraphCurrent = null;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private int _NodeGraphCurrentInstanceID = 0;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private NodeGraphEditor _GraphEditor = null;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private bool _IsLocked = false;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private bool _SidePanelGraphFoldout = true;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private bool _SidePanelNodeListFoldout = true;

		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private TransformCache _TransformCache = new TransformCache();

#if !ARBOR_EDITOR_USE_UIELEMENTS
		[System.Reflection.Obfuscation(Exclude = true)]
		[SerializeField]
		private GraphGUI _GraphGUI = new GraphGUI();
#endif

		#endregion // Serialize fields

		#region fields

		private bool _FadeLogo = false;
		private double _FadeLogoBeginTime;

		[System.NonSerialized]
		private bool _NodeGraphRootAttachedCallback = false;

		[System.NonSerialized]
		private bool _NodeGraphCurrentAttachedCallback = false;

		[System.NonSerialized]
		private List<NodeGraph> _ChildrenNodeGraphs = new List<NodeGraph>();

		private NodeGraph _NextNodeGraph = null;

		private FrameSelected _FrameSelected = new FrameSelected();
		private FrameSelected _FrameSelectedZoom = new FrameSelected();

		private bool _IsPlaying = false;

		private bool _Initialized = false;

		private bool _IsLayoutSetup = false;

		private bool _IsSelection = false;
		private Rect _SelectionRect = new Rect();

		internal bool _GUIIsExiting = false;

		private bool _IsRepaint = false;

		private bool _IsUpdateLiveTracking = false;

		private bool _IsWindowVisible = false;

		private Rect _GraphExtents = new Rect(0, 0, 100, 100);

#if ARBOR_EDITOR_CAPTURABLE
		private bool _IsCapture = false;
		private EditorCapture _Capture = null;
		private Rect _GraphCaptureExtents = new Rect(0, 0, 100, 100);
#endif

#if ARBOR_EDITOR_USE_UIELEMENTS
		private GraphMainLayout _MainLayout;
		private GraphLayout _LeftPanelLayout;
		private GraphLayout _SideToolbarLayout;
		private GraphLayout _SidePanelLayout;
		private GraphLayout _RightPanelLayout;
		private GraphLayout _GraphPanel;
		private GraphView _GraphView;
		private StretchableIMGUIContainer _ToolbarUI;
		private StretchableIMGUIContainer _SideToolbarUI;
		private StretchableIMGUIContainer _SidePanelUI;
		private StretchableIMGUIContainer _NoGraphUI;
		private StretchableIMGUIContainer _GraphUnderlayUI;
		private StretchableIMGUIContainer _GraphUI;
#if UNITY_2020_1_OR_NEWER
		private System.Func<Rect> _GetWorldBoundingBox;
		private System.Action<Rect> _SetLastWorldClip;
#endif
#if UNITY_2019_2_OR_NEWER
		private VisualElement _GraphExtentsBoundingBoxElement;
#endif
		private StretchableIMGUIContainer _GraphOverlayUI;
		private StretchableIMGUIContainer _GraphBottomUI;
		private ZoomManipulator _ZoomManipulator;
		private PanManipulator _PanManipulator;

		[System.NonSerialized]
		private bool _MainLayoutInitialized = false;

		private bool _HasGraphView = false;

#if !UNITY_2018_2_OR_NEWER
		[System.NonSerialized]
		private int _GraphUIDepth = 0;
		[System.NonSerialized]
		private bool _ChangedGUIDepth = false;
#endif

		private Rect _GraphViewExtents = new Rect(0,0,100,100);
		private bool _EndFrameSelected = true;
#else
		Rect _SideToolbarRect;
		Rect _SidePanelRect;
		Rect _ToolBarRect;
		Rect _BreadcrumbRect;
		Rect _GraphRect;
#endif

		private bool _IsInNodeEditor;

		private bool _IsInParametersPanel;

		private GraphSettingsWindow _GraphSettingsWindow = null;

		#endregion // fields

		#region properties

		public bool visibleLogo
		{
			get
			{
				switch (ArborSettings.showLogo)
				{
					case LogoShowMode.Hidden:
						return false;
					case LogoShowMode.FadeOut:
						return _FadeLogo && EditorApplication.timeSinceStartup - _FadeLogoBeginTime <= (k_ShowLogoTime + k_FadeLogoTime);
					case LogoShowMode.AlwaysShow:
						return true;
				}
				return false;
			}
		}

		public bool fadeLogo
		{
			get
			{
				if (!visibleLogo)
				{
					return false;
				}

				switch (ArborSettings.showLogo)
				{
					case LogoShowMode.Hidden:
						return false;
					case LogoShowMode.FadeOut:
						if (_FadeLogo)
						{
							float elapseTime = (float)(EditorApplication.timeSinceStartup - (_FadeLogoBeginTime + k_ShowLogoTime));
							return 0 <= elapseTime && elapseTime <= k_FadeLogoTime;
						}
						return false;
					case LogoShowMode.AlwaysShow:
						return false;
				}

				return false;
			}
		}

		public NodeGraphEditor graphEditor
		{
			get
			{
				return _GraphEditor;
			}
		}

		public Rect graphExtents
		{
			get
			{
#if ARBOR_EDITOR_CAPTURABLE
				if (isCapture)
				{
					return _GraphCaptureExtents;
				}
				else
#endif
				{
#if ARBOR_EDITOR_USE_UIELEMENTS
					return _GraphViewExtents;
#else
					return _GraphGUI.extents;
#endif
				}
			}
		}

		public Rect graphViewRect
		{
			get
			{
#if ARBOR_EDITOR_USE_UIELEMENTS
				return _GraphUI.layout;
#else
				return _GraphRect;
#endif
			}
		}

		public Rect graphViewportPosition
		{
			get
			{
#if ARBOR_EDITOR_USE_UIELEMENTS
				return _GraphUI.layout;
#else
				return _GraphGUI.viewportPosition;
#endif
			}
		}

		public Rect graphViewport
		{
			get
			{
#if ARBOR_EDITOR_USE_UIELEMENTS
				return WindowToGraphRect(_GraphUI.layout);
#else
				return _GraphGUI.viewArea;
#endif
			}
		}

		public Vector3 graphScale
		{
			get
			{
#if ARBOR_EDITOR_USE_UIELEMENTS
				return _GraphUI.transform.scale;
#else
				return Vector3.one;
#endif
			}
		}

		public Vector2 scrollPos
		{
			get
			{
#if ARBOR_EDITOR_USE_UIELEMENTS
				Vector3 scale = _GraphUI.transform.scale;
				return -Vector3.Scale(_GraphUI.transform.position, new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z));
#else
				return _GraphGUI.scrollPos;
#endif
			}
			set
			{
#if ARBOR_EDITOR_USE_UIELEMENTS
				Vector3 scale = _GraphUI.transform.scale;
				_GraphUI.transform.position = -Vector3.Scale(value, scale);
#else
				_GraphGUI.scrollPos = value;
#endif
			}
		}

		public Matrix4x4 graphMatrix
		{
			get
			{
				if (isCapture)
				{
					return Matrix4x4.identity;
				}

#if ARBOR_EDITOR_USE_UIELEMENTS
				return _GraphUI.transform.matrix;
#else
				return Matrix4x4.identity;
#endif
			}
		}

		public bool isCapture
		{
			get
			{
#if ARBOR_EDITOR_CAPTURABLE
				return captuarable && _IsCapture;
#else
				return false;
#endif
			}
		}

		#endregion // properties

		#region Unity methods

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnEnable()
		{
#if ARBOR_DEBUG
			ArborUpdateCheck updateCheck = ArborUpdateCheck.instance;
			updateCheck.CheckStart(OnUpdateCheckDone,true);
#else
			if (ArborVersion.isUpdateCheck)
			{
				ArborUpdateCheck updateCheck = ArborUpdateCheck.instance;
				updateCheck.CheckStart(OnUpdateCheckDone);
			}
#endif

			if (activeWindow == null)
			{
				activeWindow = this;
			}

#if !ARBOR_EDITOR_USE_UIELEMENTS
			_GraphGUI.hostWindow = this;
#endif

			if (_GraphEditor != null)
			{
				_GraphEditor.hostWindow = this;
				BeginFadeLogo();
			}

			wantsMouseMove = true;

			titleContent = defaultTitleContent;

			_Initialized = false;

			_FrameSelectedZoom.stoppingDistance = 0.001f;

#if ARBOR_EDITOR_USE_UIELEMENTS
			SetupGUI();
#endif
			DoRepaint();

			EditorCallbackUtility.RegisterUpdateCallback(this);
			EditorCallbackUtility.RegisterPropertyChanged(this);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnDisable()
		{
			if (activeWindow == this)
			{
				activeWindow = null;
			}

			if (!_IsWindowVisible)
			{
				if (_GraphEditor != null)
				{
					Object.DestroyImmediate(_GraphEditor);
					_GraphEditor = null;
				}
			}

			EditorCallbackUtility.UnregisterUpdateCallback(this);
			EditorCallbackUtility.UnregisterPropertyChanged(this);
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void OnSelectionChange()
		{
			if (_IsLocked)
			{
				return;
			}

			GameObject gameObject = Selection.activeGameObject;
			if (gameObject == null)
			{
				return;
			}

			NodeGraph[] nodeGraphs = gameObject.GetComponents<NodeGraph>();
			if (nodeGraphs != null)
			{
				int graphCount = nodeGraphs.Length;
				for (int graphIndex = 0; graphIndex < graphCount; graphIndex++)
				{
					NodeGraph graph = nodeGraphs[graphIndex];
					if ((graph.hideFlags & HideFlags.HideInInspector) == HideFlags.None)
					{
						Initialize(graph);
						break;
					}
				}
			}
		}

		void IPropertyChanged.OnPropertyChanged(PropertyChangedType propertyChangedType)
		{
			if (propertyChangedType != PropertyChangedType.UndoRedoPerformed)
			{
				return;
			}

			if (_GraphEditor != null)
			{
				_GraphEditor.OnUndoRedoPerformed();
			}
			DoRepaint();
		}

		void IUpdateCallback.OnUpdate()
		{
			if (_NextNodeGraph != null)
			{
				SetCurrentNodeGraph(_NextNodeGraph);
				_NextNodeGraph = null;
			}

			ReatachIfNecessary();

			if (_IsWindowVisible)
			{
				if (_GraphEditor != null)
				{
					if (_IsUpdateLiveTracking)
					{
						_GraphEditor.LiveTracking();
						_IsUpdateLiveTracking = false;
					}

					if (IsDragScroll() || _IsRepaint || fadeLogo)
					{
						DoRepaint();
					}
				}

#if ARBOR_EDITOR_USE_UIELEMENTS
				UpdateDocked();
#endif
			}
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnBecameVisible()
		{
			_IsWindowVisible = true;
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnBecameInvisible()
		{
			_IsWindowVisible = false;
		}

#if ARBOR_EDITOR_EXTENSIBLE
		private bool _IsSkinChanged = false;

		void BeginSkin()
		{
			if (skin == null || _IsSkinChanged)
			{
				return;
			}

			skin.Begin();

			_IsSkinChanged = true;
		}

		void BeginSkin(Rect rect, bool isHostView)
		{
			BeginSkin();

			if (isHostView && Event.current.type == EventType.Repaint)
			{
				Styles.hostview.Draw(rect, GUIContent.none, false, false, false, false);
			}
		}

		void EndSkin()
		{
			if (skin == null || !_IsSkinChanged)
			{
				return;
			}

			skin.End();

			_IsSkinChanged = false;
		}
#endif

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnGUI()
		{
#if !ARBOR_EDITOR_USE_UIELEMENTS && ARBOR_EDITOR_EXTENSIBLE
			Rect hostPosition = position;
			hostPosition.position = Vector2.zero;
			BeginSkin(hostPosition, true);
#endif

			using (new ProfilerScope("ArborEditorWindow.OnGUI"))
			{
				bool isPlaying = EditorApplication.isPlaying;
				if (_IsPlaying != isPlaying)
				{
					_Initialized = false;
					_IsPlaying = isPlaying;
				}

				if (!_Initialized)
				{
					ReatachIfNecessary();

					if (_GraphEditor != null)
					{
						_GraphEditor.InitializeGraph();
						DoRepaint();

						if (isPlaying)
						{
							_IsUpdateLiveTracking = true;
						}
					}

					_Initialized = true;
				}
				else
				{
					if (_GraphEditor != null)
					{
						_GraphEditor.RebuildIfNecessary();
					}
				}

#if ARBOR_EDITOR_USE_UIELEMENTS
#if ARBOR_EDITOR_CAPTURABLE
				if (isCapture)
				{
					CaptureGUI();
				}
#endif
#else
				ResizeHandling(this.position.width, this.position.height - EditorStyles.toolbar.fixedHeight);
				CalculateRect();

				ToolbarGUI(_ToolBarRect);

				EditorGUILayout.BeginHorizontal();

				if (ArborSettings.openSidePanel)
				{
					SideToolbarGUI(_SideToolbarRect);
					SidePanelGUI(_SidePanelRect);
				}

				BreadcrumbGUI(_BreadcrumbRect);

#if ARBOR_EDITOR_CAPTURABLE
				if (isCapture)
				{
					CaptureGUI();
				}
				else
#endif
				{
					if (_GraphEditor == null)
					{
						NoGraphSelectedGUI(graphViewRect);
					}
					else
					{
						UnderlayGUI(graphViewportPosition);
						GraphViewGUI();
						OverlayGUI(graphViewportPosition);
					}
				}

				EditorGUILayout.EndHorizontal();

				CheckDragBehaviour();
#endif
			}

#if !ARBOR_EDITOR_USE_UIELEMENTS && ARBOR_EDITOR_EXTENSIBLE
			EndSkin();
#endif
		}

		void CheckDragBehaviour()
		{
			Event current = Event.current;
			switch (current.type)
			{
				case EventType.DragUpdated:
					{
						BehaviourDragInfo behaviourDragInfo = BehaviourDragInfo.GetBehaviourDragInfo();
						if (behaviourDragInfo != null)
						{
							behaviourDragInfo.dragging = true;
						}
					}
					break;
				case EventType.DragExited:
					{
						BehaviourDragInfo behaviourDragInfo = BehaviourDragInfo.GetBehaviourDragInfo();
						if (behaviourDragInfo != null)
						{
							behaviourDragInfo.dragging = false;
						}
					}
					break;
			}
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		private void ShowButton(Rect r)
		{
			bool flag = GUI.Toggle(r, _IsLocked, GUIContent.none, Styles.lockButton);
			if (flag == _IsLocked)
			{
				return;
			}
			_IsLocked = flag;
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnDestroy()
		{
			SetNodeGraphRoot(null);
			SetNodeGraphCurrent(null);

			if (_GraphEditor != null)
			{
				Object.DestroyImmediate(_GraphEditor);
				_GraphEditor = null;
			}
		}

		#endregion // Unity methods

		void OnUpdateCheckDone()
		{
			ArborUpdateCheck updateCheck = ArborUpdateCheck.instance;
			UpdateInfo updateInfo = updateCheck.updateInfo;
			if (updateInfo == null)
			{
				return;
			}

			bool isUpdated = updateCheck.isUpdated;
			if (isUpdated)
			{
				DoRepaint();
			}
		}

#if ARBOR_EDITOR_CAPTURABLE
		void CaptureGUI()
		{
			if (_Capture.BeginCaptureGUI())
			{
				UnderlayGUI(graphExtents);

				try
				{
					GraphGUI();

					OverlayGUI(graphExtents);
				}
				finally
				{
					if (_Capture.EndCaptureGUI())
					{
						string path = EditorUtility.SaveFilePanel("Save", ArborEditorCache.captureDirectory, _NodeGraphCurrent.graphName, "png");
						if (!string.IsNullOrEmpty(path))
						{
							ArborEditorCache.captureDirectory = System.IO.Path.GetDirectoryName(path);
						}
						_Capture.SaveImage(path, true);
						_Capture.Destroy();
						_Capture = null;

						_IsCapture = false;
#if UNITY_2019_1_OR_NEWER
						_IsLayoutSetup = false;
#endif
						DoRepaint();
					}
				}
			}
			else
			{
				_IsCapture = false;
#if UNITY_2019_1_OR_NEWER
				_IsLayoutSetup = false;
#endif
				DoRepaint();
			}
		}
#endif

		void DrawLogo(Rect rect, bool forceDraw = false)
		{
			if (!(visibleLogo || forceDraw) || Event.current.type != EventType.Repaint)
			{
				return;
			}

			float alpha = 0.5f;
			if (!forceDraw && ArborSettings.showLogo == LogoShowMode.FadeOut)
			{
				float t = Mathf.Clamp01((float)(EditorApplication.timeSinceStartup - (_FadeLogoBeginTime + k_ShowLogoTime)) / k_FadeLogoTime);
				alpha = Mathf.Lerp(0.5f, 0f, t);
			}

			Color tempColor = GUI.color;
			GUI.color = new Color(1f, 1f, 1f, alpha);
			Texture2D logoTex = Icons.logo;

			float width = 256f;
			float scale = width / logoTex.width;
			float height = logoTex.height * scale;

			Rect logoPosition = rect;
			logoPosition.xMax = logoPosition.xMin + width;
			logoPosition.yMax = logoPosition.yMin + height;
			GUI.DrawTexture(logoPosition, logoTex, ScaleMode.ScaleToFit);
			GUI.color = tempColor;
		}

		void UnderlayGUI(Rect rect)
		{
			if (_GraphEditor == null || _GraphEditor.nodeGraph == null)
			{
				return;
			}

			_GraphEditor.UpdateLayer(isCapture || !zoomable);

			EditorGUITools.DrawGridBackground(rect);

#if ARBOR_EDITOR_EXTENSIBLE
			if (underlayGUI != null)
			{
				GUI.BeginGroup(rect);

				Rect groupRect = rect;
				groupRect.position = Vector2.zero;

				GUILayout.BeginArea(groupRect);
				underlayGUI(_GraphEditor.nodeGraph, groupRect);
				GUILayout.EndArea();

				GUI.EndGroup();
			}
#endif

			GUIContent label = _GraphEditor.GetGraphLabel();
			if (label != null)
			{
				Vector2 size = Styles.graphLabel.CalcSize(label);
				Rect labelPosition = rect;
				labelPosition.xMin = labelPosition.xMax - size.x;
				labelPosition.yMin = labelPosition.yMax - size.y;
				GUI.Label(labelPosition, label, Styles.graphLabel);
			}

			if (Application.isPlaying && _GraphEditor.HasPlayState())
			{
				PlayState playState = _GraphEditor.GetPlayState();

				GUIContent playStateLabel = null;

				switch (playState)
				{
					case PlayState.Stopping:
						playStateLabel = Localization.GetTextContent("PlayState.Stopping");
						break;
					case PlayState.Playing:
						playStateLabel = Localization.GetTextContent("PlayState.Playing");
						break;
					case PlayState.Pausing:
						playStateLabel = Localization.GetTextContent("PlayState.Pausing");
						break;
					case PlayState.InactivePausing:
						playStateLabel = Localization.GetTextContent("PlayState.InactivePausing");
						break;
				}

				if (playStateLabel != null)
				{
					GUIStyle style = Styles.playStateLabel;

					Vector2 size = style.CalcSize(playStateLabel);
					Rect labelPosition = rect;
					labelPosition.xMin = labelPosition.xMax - size.x;
					labelPosition.yMin = labelPosition.yMax - size.y;
					labelPosition.width = size.x;
					labelPosition.height = size.y;
					GUI.Label(labelPosition, playStateLabel, style);
				}
			}
		}

		void OverlayGUI(Rect rect)
		{
			if (_GraphEditor == null || _GraphEditor.nodeGraph == null)
			{
				return;
			}

#if ARBOR_EDITOR_EXTENSIBLE
			if (overlayGUI != null)
			{
				GUI.BeginGroup(rect);

				Rect groupRect = rect;
				groupRect.position = Vector2.zero;

				GUILayout.BeginArea(groupRect);
				overlayGUI(_GraphEditor.nodeGraph, groupRect);
				GUILayout.EndArea();


				GUI.EndGroup();
			}
#endif

			if (!_GraphEditor.editable)
			{
				Color guiColor = GUI.color;
				GUI.color = Color.red;
				if (Event.current.type == EventType.Repaint)
				{
					Styles.graphHighlight.Draw(rect, false, false, false, false);
				}

				GUIStyle style = Styles.graphLabel;

				Vector2 size = style.CalcSize(EditorContents.notEditable);
				Rect labelPosition = rect;
				labelPosition.xMin = labelPosition.xMax - size.x - Styles.graphLabel.padding.right;
				labelPosition.yMax = labelPosition.yMin + size.y;
				GUI.Label(labelPosition, EditorContents.notEditable, style);

				GUI.color = guiColor;
			}

#if ARBOR_TRIAL
			DrawLogo(rect);
#else
			if (isCapture || ArborSettings.showLogo == LogoShowMode.FadeOut)
			{
				DrawLogo(rect, isCapture);
			}
#endif
		}

		internal void OnPostLayout()
		{
			if (!_IsLayoutSetup)
			{
				_IsLayoutSetup = true;
				CenterOnStoredPosition(_NodeGraphCurrent);

#if !ARBOR_EDITOR_USE_UIELEMENTS
				DoRepaint();
#endif
			}
		}

		void Initialize()
		{
			if (_GraphEditor != null)
			{
				Object.DestroyImmediate(_GraphEditor);
				_GraphEditor = null;
			}

			Undo.RecordObject(this, "Select NodeGraph");

			if (_NodeGraphRoot != null)
			{
				SetNodeGraphCurrent(_NodeGraphRoot);
			}
			else
			{
				SetNodeGraphRoot(null);
				SetNodeGraphCurrent(null);
			}

			_Initialized = false;

			EditorUtility.SetDirty(this);

			DoRepaint();
		}

		void Initialize(NodeGraph nodeGraph)
		{
			if (_NodeGraphRoot == nodeGraph && _NodeGraphCurrent == nodeGraph)
			{
				return;
			}

			int undoGroup = Undo.GetCurrentGroup();

			Undo.RecordObject(this, "Select NodeGraph");

			SetNodeGraphRoot(nodeGraph);

			SetCurrentNodeGraph(_NodeGraphRoot);

			Undo.CollapseUndoOperations(undoGroup);

			EditorUtility.SetDirty(this);
		}

		internal void BeginFadeLogo(bool forceFade = false)
		{
			if ((!_FadeLogo || forceFade) && ArborSettings.showLogo == LogoShowMode.FadeOut)
			{
				_FadeLogo = true;
				_FadeLogoBeginTime = EditorApplication.timeSinceStartup;
			}
		}

		void RebuildGraphEditor()
		{
			if (_GraphEditor != null)
			{
				if (_GraphEditor.nodeGraph != null && _GraphEditor.nodeGraph == _NodeGraphCurrent)
				{
					return;
				}
			}

			if (_GraphEditor != null)
			{
				Object.DestroyImmediate(_GraphEditor);
				_GraphEditor = null;
			}

			bool nextHasGraphEditor = _NodeGraphCurrent != null;

			if (!nextHasGraphEditor)
			{

#if ARBOR_EDITOR_USE_UIELEMENTS
				if (_HasGraphView)
				{
					_GraphPanel.Remove(_GraphView);
					_GraphPanel.Add(_NoGraphUI);
					_HasGraphView = false;
				}
#endif
				return;
			}

			_GraphEditor = NodeGraphEditor.CreateEditor(this, _NodeGraphCurrent);

#if ARBOR_EDITOR_USE_UIELEMENTS
			if (!_HasGraphView)
			{
				_GraphPanel.Remove(_NoGraphUI);
				_GraphPanel.Add(_GraphView);
				_HasGraphView = true;
			}
#endif

			_IsRepaint = true;
			_Initialized = false;

			DirtyGraphExtents();
		}

		public void ChangeCurrentNodeGraph(NodeGraph nodeGraph, bool liveTracking = false)
		{
			if (_NodeGraphCurrent == nodeGraph)
			{
				return;
			}

			if (!liveTracking && Application.isPlaying &&
				ArborSettings.liveTracking && ArborSettings.liveTrackingHierarchy &&
				_GraphEditor != null && _GraphEditor.GetPlayState() != PlayState.Stopping)
			{
				ArborSettings.liveTracking = false;
			}

			GUIUtility.keyboardControl = 0;

			_NextNodeGraph = nodeGraph;
			DoRepaint();
		}

		void SetCurrentNodeGraph(NodeGraph nodeGraph)
		{
			if (_NodeGraphCurrent == nodeGraph)
			{
				return;
			}

			int undoGroup = Undo.GetCurrentGroup();

			Undo.RecordObject(this, "Select NodeGraph");

			SetNodeGraphCurrent(nodeGraph);

			Undo.CollapseUndoOperations(undoGroup);

			EditorUtility.SetDirty(this);

			RebuildGraphEditor();

			BeginFadeLogo(true);

			DoRepaint();
		}

		internal void DoRepaint()
		{
#if ARBOR_EDITOR_USE_UIELEMENTS
#if UNITY_2018_3_OR_NEWER
			_GraphUI.MarkDirtyRepaint();
#else
			_GraphUI.Dirty(ChangeType.Repaint);
#endif
#endif

			Repaint();
			_IsRepaint = false;
		}

		internal void BeginSelection()
		{
			_IsSelection = true;
			_SelectionRect = new Rect();
		}

		internal void SetSelectionRect(Rect rect)
		{
			_SelectionRect = GraphToWindowRect(rect);
		}

		internal void EndSelection()
		{
			_IsSelection = false;
		}

		void DrawSelection()
		{
			if (Event.current.type != EventType.Repaint || !_IsSelection || _SelectionRect == new Rect())
			{
				return;
			}

			Styles.selectionRect.Draw(_SelectionRect, false, false, false, false);
		}

		void CreateFSM()
		{
			int undoGroup = Undo.GetCurrentGroup();

			NodeGraph nodeGraph = NodeGraphUtility.CreateGraphObject(Types.ArborFSMType, "ArborFSM", null);

			Initialize(nodeGraph);

			Undo.CollapseUndoOperations(undoGroup);
		}

		void CreateBT()
		{
			int undoGroup = Undo.GetCurrentGroup();

			NodeGraph nodeGraph = NodeGraphUtility.CreateGraphObject(Types.BehaviourTreeType, "BehaviourTree", null);

			Initialize(nodeGraph);

			Undo.CollapseUndoOperations(undoGroup);
		}

		void SidePanelToggle()
		{
			EditorGUI.BeginChangeCheck();
			ArborSettings.openSidePanel = GUILayout.Toggle(ArborSettings.openSidePanel, ArborSettings.openSidePanel ? EditorContents.sidePanelOn : EditorContents.sidePanelOff, Styles.invisibleButton);
			if (EditorGUI.EndChangeCheck())
			{
#if ARBOR_EDITOR_USE_UIELEMENTS
				if (ArborSettings.openSidePanel)
				{
					_MainLayout.Insert(0,_LeftPanelLayout);
				}
				else
				{
					_MainLayout.Remove(_LeftPanelLayout);
				}
#endif
			}
		}

		void ToolbarGUI(Rect rect)
		{
			using (new ProfilerScope("ToolbarGUI"))
			{
				GUILayout.BeginArea(rect);

				EditorGUILayout.BeginHorizontal(Styles.toolbar, GUILayout.Height(EditorGUITools.toolbarHeight));

				if (!ArborSettings.openSidePanel)
				{
					using (new ProfilerScope("SidePanel Toggle"))
					{
						SidePanelToggle();

						EditorGUILayout.Space();
					}
				}

				using (new ProfilerScope("Create Field"))
				{
					GUIContent content = EditorContents.create;
					GUIStyle style = EditorStyles.toolbarDropDown;
					Rect buttonRect = GUILayoutUtility.GetRect(content, style);
					if (GUI.Button(buttonRect, content, style))
					{
						GUIUtility.keyboardControl = 0;

						GenericMenu menu = new GenericMenu();

						menu.AddItem(EditorGUITools.GetTextContent("ArborFSM"), false, () =>
						{
							CreateFSM();
						});

						menu.AddItem(EditorGUITools.GetTextContent("BehaviourTree"), false, () =>
						{
							CreateBT();
						});

						menu.DropDown(buttonRect);
					}
				}

				using (new ProfilerScope("NodeGraph Field"))
				{
					EditorGUI.BeginChangeCheck();
					NodeGraph nodeGraph = EditorGUILayout.ObjectField(_NodeGraphRoot, typeof(NodeGraph), true, GUILayout.Width(200)) as NodeGraph;
					if (EditorGUI.EndChangeCheck())
					{
						Initialize(nodeGraph);
					}
				}

				GUILayout.FlexibleSpace();

				if (_GraphEditor != null)
				{
#if ARBOR_EDITOR_EXTENSIBLE
					if (toolbarGUI != null)
					{
						toolbarGUI(_GraphEditor.nodeGraph);
					}
#endif

					using (new ProfilerScope("LiveTrace Button"))
					{
						EditorGUI.BeginChangeCheck();
						ArborSettings.liveTracking = GUILayout.Toggle(ArborSettings.liveTracking, EditorContents.liveTracking, EditorStyles.toolbarButton);
						if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying)
						{
							_IsUpdateLiveTracking = true;
						}
					}

					using (new ProfilerScope("View Button"))
					{
						GUIContent content = EditorContents.view;
						Rect buttonPosition = GUILayoutUtility.GetRect(content, EditorStyles.toolbarDropDown);
						if (GUI.Button(buttonPosition, content, EditorStyles.toolbarDropDown))
						{
							GenericMenu menu = new GenericMenu();

							_GraphEditor.SetViewMenu(menu);

							menu.DropDown(buttonPosition);
						}
					}

					using (new ProfilerScope("Debug Button"))
					{
						GUIContent content = EditorContents.debug;
						Rect buttonPosition = GUILayoutUtility.GetRect(content, EditorStyles.toolbarDropDown);
						if (GUI.Button(buttonPosition, content, EditorStyles.toolbarDropDown))
						{
							GenericMenu menu = new GenericMenu();

							_GraphEditor.SetDenugMenu(menu);

							menu.DropDown(buttonPosition);
						}
					}
#if ARBOR_EDITOR_CAPTURABLE
					using (new ProfilerScope("Capture Button"))
					{
						Color contentColor = GUI.contentColor;
						GUI.contentColor = isDarkSkin ? Color.white : Color.black;
						if (EditorGUITools.IconButton(EditorContents.captureIcon))
						{
							_GraphCaptureExtents = new RectOffset(100, 100, 100, 100).Add(_GraphExtents);

							if (_GraphCaptureExtents.width < 500)
							{
								float center = _GraphCaptureExtents.center.x;
								_GraphCaptureExtents.xMin = center - 250;
								_GraphCaptureExtents.xMax = center + 250;
							}
							if (_GraphCaptureExtents.height < 500)
							{
								float center = _GraphCaptureExtents.center.y;
								_GraphCaptureExtents.yMin = center - 250;
								_GraphCaptureExtents.yMax = center + 250;
							}

							_GraphCaptureExtents.x = Mathf.Floor(_GraphCaptureExtents.x);
							_GraphCaptureExtents.width = Mathf.Floor(_GraphCaptureExtents.width);
							_GraphCaptureExtents.y = Mathf.Floor(_GraphCaptureExtents.y);
							_GraphCaptureExtents.height = Mathf.Floor(_GraphCaptureExtents.height);

							int maxTextureSize = SystemInfo.maxTextureSize;
							if (_GraphCaptureExtents.width <= maxTextureSize && _GraphCaptureExtents.height <= maxTextureSize)
							{
								_Capture = new EditorCapture(this);
								if (_Capture.Initialize(_GraphCaptureExtents))
								{
									_IsCapture = true;
									DoRepaint();
								}
							}
							else
							{
								Debug.LogError("Screenshot failed : Graph size is too large.");
							}
						}
						GUI.contentColor = contentColor;
					}
#endif
				}

				ArborUpdateCheck updateCheck = ArborUpdateCheck.instance;
				if (updateCheck.isUpdated || updateCheck.isUpgrade)
				{
					using (new ProfilerScope("Update Button"))
					{
						Color contentColor = GUI.contentColor;
						GUI.contentColor = isDarkSkin ? Color.white : Color.black;
						if (EditorGUITools.IconButton(EditorContents.notificationIcon))
						{
							UpdateNotificationWindow.Open();
						}
						GUI.contentColor = contentColor;
					}
				}

				using (new ProfilerScope("Help Button"))
				{
					Rect helpButtonPosition;
					if (EditorGUITools.IconButton(EditorContents.helpIcon, out helpButtonPosition))
					{
						GenericMenu menu = new GenericMenu();
						menu.AddItem(EditorContents.assetStore, false, () =>
						{
							ArborVersion.OpenAssetStore();
						});
						menu.AddSeparator("");
						menu.AddItem(EditorContents.officialSite, false, () =>
						{
							Help.BrowseURL(Localization.GetWord("SiteURL"));
						});
						menu.AddItem(EditorContents.manual, false, () =>
						{
							Help.BrowseURL(Localization.GetWord("ManualURL"));
						});
						menu.AddItem(EditorContents.arborReference, false, () =>
						{
							Help.BrowseURL(Localization.GetWord("ArborReferenceURL"));
						});
						menu.AddItem(EditorContents.scriptReference, false, () =>
						{
							Help.BrowseURL(Localization.GetWord("ScriptReferenceURL"));
						});
						menu.AddSeparator("");
						menu.AddItem(EditorContents.releaseNotes, false, () =>
						{
							Help.BrowseURL(Localization.GetWord("ReleaseNotesURL"));
						});
						menu.AddItem(EditorContents.forum, false, () =>
						{
							Help.BrowseURL(Localization.GetWord("ForumURL"));
						});
						menu.DropDown(helpButtonPosition);
					}
				}

				using (new ProfilerScope("Settings Button"))
				{
					GUIContent settingContent = EditorContents.popupIcon;
					Rect settingButtonPosition;
					if (EditorGUITools.IconButton(settingContent, out settingButtonPosition))
					{
						if (_GraphSettingsWindow == null)
						{
							_GraphSettingsWindow = new GraphSettingsWindow(this);
						}
						PopupWindowUtility.Show(settingButtonPosition, _GraphSettingsWindow, true);
					}
				}

				EditorGUILayout.EndHorizontal();

				GUILayout.EndArea();
			}
		}

		void SideToolbarGUI()
		{
			EditorGUILayout.BeginHorizontal(Styles.toolbar, GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUITools.toolbarHeight));

			EditorGUI.BeginChangeCheck();
			bool isGraphTab = GUILayout.Toggle(ArborSettings.sidePanelTab == SidePanelTab.Graph, EditorContents.graph, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
			if (EditorGUI.EndChangeCheck() && isGraphTab)
			{
				ArborSettings.sidePanelTab = SidePanelTab.Graph;
			}

			EditorGUI.BeginChangeCheck();
			bool isParameterTab = GUILayout.Toggle(ArborSettings.sidePanelTab == SidePanelTab.Parameters, EditorContents.parameters, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
			if (EditorGUI.EndChangeCheck() && isParameterTab)
			{
				ArborSettings.sidePanelTab = SidePanelTab.Parameters;
			}

			GUILayout.FlexibleSpace();

			SidePanelToggle();

			EditorGUILayout.EndHorizontal();
		}

		void SideGraphPanel()
		{
			EditorGUIUtility.labelWidth = 100;

			if (_GraphEditor != null && _GraphEditor.nodeGraph != null)
			{
				bool editable = _GraphEditor.editable;

				using (new ProfilerScope("GraphPanel"))
				{
					_SidePanelGraphFoldout = GUILayout.Toggle(_SidePanelGraphFoldout, EditorContents.graph, Styles.sidePanelTitlebar);
					if (_SidePanelGraphFoldout)
					{
						EditorGUI.BeginDisabledGroup(!editable);

						NodeGraph nodeGraph = _GraphEditor.nodeGraph;
						string graphName = nodeGraph.graphName;
						EditorGUI.BeginChangeCheck();
						graphName = EditorGUILayout.DelayedTextField("Name", graphName);
						if (EditorGUI.EndChangeCheck())
						{
							Undo.RecordObject(nodeGraph, "Change Graph Name");

							nodeGraph.graphName = graphName;

							EditorUtility.SetDirty(nodeGraph);
						}

						EditorGUI.EndDisabledGroup();

						EditorGUILayout.LabelField(EditorContents.parentGraph, EditorStyles.boldLabel);

						NodeGraph parentGraph = nodeGraph.parentGraph;
						string parentGraphName = "None";

						if (parentGraph != null)
						{
							parentGraphName = parentGraph.displayGraphName;
						}

						EditorGUI.BeginDisabledGroup(parentGraph == null);

						if (GUILayout.Button(parentGraphName))
						{
							ChangeCurrentNodeGraph(parentGraph);
						}

						EditorGUI.EndDisabledGroup();

						_ChildrenNodeGraphs.Clear();

						int nodeCount = nodeGraph.nodeCount;
						for (int nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
						{
							INodeBehaviourContainer behaviours = nodeGraph.GetNodeFromIndex(nodeIndex) as INodeBehaviourContainer;
							if (behaviours != null)
							{
								int behaviourCount = behaviours.GetNodeBehaviourCount();
								for (int behaviourIndex = 0; behaviourIndex < behaviourCount; behaviourIndex++)
								{
									INodeGraphContainer graphContainer = behaviours.GetNodeBehaviour<NodeBehaviour>(behaviourIndex) as INodeGraphContainer;
									if (graphContainer != null)
									{
										int graphCount = graphContainer.GetNodeGraphCount();
										for (int graphIndex = 0; graphIndex < graphCount; graphIndex++)
										{
											_ChildrenNodeGraphs.Add(graphContainer.GetNodeGraph<NodeGraph>(graphIndex));
										}
									}
								}
							}
						}

						int childCount = _ChildrenNodeGraphs.Count;
						if (childCount > 0)
						{
							EditorGUILayout.LabelField(EditorContents.children, EditorStyles.boldLabel);

							for (int childIndex = 0; childIndex < childCount; childIndex++)
							{
								NodeGraph graph = _ChildrenNodeGraphs[childIndex];

								if (graph != null)
								{
									string childGraphName = graph.displayGraphName;
									if (GUILayout.Button(childGraphName))
									{
										ChangeCurrentNodeGraph(graph);
									}
								}
							}
						}

						EditorGUILayout.Space();
					}
				}

				using (new ProfilerScope("NodeListPanel"))
				{
					_SidePanelNodeListFoldout = GUILayout.Toggle(_SidePanelNodeListFoldout, EditorContents.nodeList, Styles.sidePanelTitlebar);
					if (_SidePanelNodeListFoldout)
					{
						_GraphEditor.NodeListPanelGUI();
					}
				}
			}

			if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				GUIUtility.keyboardControl = 0;
				Event.current.Use();
			}

			EditorGUIUtility.labelWidth = 0f;
		}

		void SideParametersPanel()
		{
			if (_GraphEditor != null && _GraphEditor.nodeGraph != null)
			{
				EditorGUIUtility.labelWidth = 0f;
				EditorGUIUtility.fieldWidth = 0f;

				_IsInParametersPanel = true;
				try
				{
					_GraphEditor.ParametersPanelGUI();
				}
				finally
				{
					_IsInParametersPanel = false;
				}
			}
		}

		void SideToolbarGUI(Rect rect)
		{
			GUILayout.BeginArea(rect);

			rect.position = Vector2.zero;

			// When EventType.Layout, GUIClip.GetTopRect() is an invalid value, so overwrite it with BeginGroup.
			GUI.BeginGroup(rect);

			SideToolbarGUI();

			if (Event.current.type == EventType.Repaint)
			{
				Rect borderRect = rect;
				borderRect.xMin = borderRect.xMax - 1f;
				EditorGUI.DrawRect(borderRect, EditorGUITools.GetSplitColor(isDarkSkin));
			}

			GUI.EndGroup();

			GUILayout.EndArea();
		}

		void SidePanelGUI(Rect rect)
		{
			using (new ProfilerScope("SidePanelGUI"))
			{
				GUILayout.BeginArea(rect);

				rect.position = Vector2.zero;

				// When EventType.Layout, GUIClip.GetTopRect() is an invalid value, so overwrite it with BeginGroup.
				GUI.BeginGroup(rect);

				bool hierarchyMode = EditorGUIUtility.hierarchyMode;
				EditorGUIUtility.hierarchyMode = true;

				bool wideMode = EditorGUIUtility.wideMode;
				EditorGUIUtility.wideMode = rect.width > EditorGUITools.kWideModeMinWidth;

				try
				{
					switch (ArborSettings.sidePanelTab)
					{
						case SidePanelTab.Graph:
							SideGraphPanel();
							break;
						case SidePanelTab.Parameters:
							SideParametersPanel();
							break;
					}

					if (Event.current.type == EventType.Repaint)
					{
						Rect borderRect = rect;
						borderRect.xMin = borderRect.xMax - 1f;
						EditorGUI.DrawRect(borderRect, EditorGUITools.GetSplitColor(isDarkSkin));
					}
				}
				finally
				{
					EditorGUIUtility.wideMode = wideMode;
					EditorGUIUtility.hierarchyMode = hierarchyMode;
				}

				GUI.EndGroup();

				GUILayout.EndArea();
			}
		}

		private List<NodeGraph> _BreadCrumbGraphs = new List<NodeGraph>();

		void BreadcrumbGUI(Rect rect)
		{
			using (new ProfilerScope("BreadcrumbGUI"))
			{
				if (Event.current.type == EventType.Repaint)
				{
					Styles.toolbar.Draw(rect, false, false, false, false);
				}

				rect = Styles.toolbar.padding.Remove(rect);

				rect.width = 0f;

				for (int i = 0; i < _BreadCrumbGraphs.Count; i++)
				{
					NodeGraph graph = _BreadCrumbGraphs[i];

					GUIContent graphName = EditorGUITools.GetTextContent(graph.displayGraphName);

					GUIStyle style = i != 0 ? Styles.breadcrumbMid : Styles.breadcrumbLeft;
					GUIStyle guiStyle = i != 0 ? Styles.breadcrumbMidBg : Styles.breadcrumbLeftBg;

					Vector2 size = style.CalcSize(graphName);

					rect.width = size.x;

					bool on = i == _BreadCrumbGraphs.Count - 1;

					if (guiStyle != null && Event.current.type == EventType.Repaint)
					{
						guiStyle.Draw(rect, GUIContent.none, 0, on);
					}

					EditorGUI.BeginChangeCheck();
					GUI.Toggle(rect, on, graphName, style);
					if (EditorGUI.EndChangeCheck())
					{
						ChangeCurrentNodeGraph(graph);
					}

					rect.x += rect.width;
				}
			}
		}

		void OnDestroyNodeGraph(NodeGraph nodeGraph)
		{
			if (EditorApplication.isPlaying != EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return;
			}

			Undo.RecordObject(this, "Destroy NodeGraph");

			if ((object)_NodeGraphRoot == (object)nodeGraph)
			{
				SetNodeGraphRoot(null);
				SetNodeGraphCurrent(null);
			}
			else if ((object)_NodeGraphCurrent == (object)nodeGraph)
			{
				SetNodeGraphCurrent(_NodeGraphRoot);
			}

			RebuildGraphEditor();

			EditorUtility.SetDirty(this);

			DoRepaint();
		}

		void SetNodeGraphRoot(NodeGraph nodeGraph)
		{
			if (_NodeGraphRoot != null)
			{
				_NodeGraphRoot.destroyCallback -= OnDestroyNodeGraph;
				_NodeGraphRoot.stateChangedCallback -= OnStateChanged;
				_NodeGraphRootAttachedCallback = false;
			}

			_NodeGraphRoot = nodeGraph;

			if (_NodeGraphRoot != null)
			{
				_NodeGraphRoot.destroyCallback += OnDestroyNodeGraph;
				_NodeGraphRoot.stateChangedCallback += OnStateChanged;
				_NodeGraphRootAttachedCallback = true;
				_NodeGraphRootInstanceID = _NodeGraphRoot.GetInstanceID();
			}
			else
			{
				_NodeGraphRootInstanceID = 0;
			}
		}

		void SetNodeGraphCurrent(NodeGraph nodeGraph)
		{
			StoreCurrentTransform();

			if (_NodeGraphCurrent != null)
			{
				_NodeGraphCurrent.destroyCallback -= OnDestroyNodeGraph;
				_NodeGraphCurrent.stateChangedCallback -= OnStateChanged;
				_NodeGraphCurrentAttachedCallback = false;
			}

			_NodeGraphCurrent = nodeGraph;

			if (_NodeGraphCurrent != null)
			{
				_NodeGraphCurrent.destroyCallback += OnDestroyNodeGraph;
				_NodeGraphCurrent.stateChangedCallback += OnStateChanged;
				_NodeGraphCurrentAttachedCallback = true;
				_NodeGraphCurrentInstanceID = _NodeGraphCurrent.GetInstanceID();
			}
			else
			{
				_NodeGraphCurrentInstanceID = 0;
			}

			_BreadCrumbGraphs.Clear();

			NodeGraph current = _NodeGraphCurrent;
			while (current != null)
			{
				_BreadCrumbGraphs.Insert(0, current);
				if (current.ownerBehaviour != null)
				{
					current = current.ownerBehaviour.nodeGraph;
				}
				else
				{
					current = null;
				}
			}
		}

		private void ReatachIfNecessary()
		{
			bool reatached = false;

			if (_GraphEditor != null)
			{
				_GraphEditor.ReatachIfNecessary();
			}

			if (_NodeGraphRoot == null && _NodeGraphRootInstanceID != 0)
			{
				SetNodeGraphRoot(EditorUtility.InstanceIDToObject(_NodeGraphRootInstanceID) as NodeGraph);
				reatached = true;
			}
			if (_NodeGraphCurrent == null && _NodeGraphCurrentInstanceID != 0)
			{
				SetNodeGraphCurrent(EditorUtility.InstanceIDToObject(_NodeGraphCurrentInstanceID) as NodeGraph);
				RebuildGraphEditor();
				reatached = true;
			}

			if (!reatached)
			{
				if (_NodeGraphRoot != null)
				{
					if (!_NodeGraphRootAttachedCallback)
					{
						_NodeGraphRoot.destroyCallback += OnDestroyNodeGraph;
						_NodeGraphRoot.stateChangedCallback += OnStateChanged;
						_NodeGraphRootAttachedCallback = true;
					}
				}
				else
				{
					_NodeGraphRootAttachedCallback = false;
				}

				if (_NodeGraphCurrent != null)
				{
					if (!_NodeGraphCurrentAttachedCallback)
					{
						_NodeGraphCurrent.destroyCallback += OnDestroyNodeGraph;
						_NodeGraphCurrent.stateChangedCallback += OnStateChanged;
						_NodeGraphCurrentAttachedCallback = true;
					}
				}
				else
				{
					_NodeGraphCurrentAttachedCallback = false;
				}
			}

			if (reatached)
			{
				if (_NodeGraphRoot == null || _NodeGraphCurrent == null)
				{
					Initialize();
				}
			}
			else
			{
				if ((_GraphEditor == null && _NodeGraphCurrent != null) || (_GraphEditor != null && _GraphEditor.nodeGraph != _NodeGraphCurrent))
				{
					RebuildGraphEditor();
				}
				else if (_GraphEditor != null && _GraphEditor.ReatachIfNecessary())
				{
					if (_GraphEditor.nodeGraph == null)
					{
						Initialize();
					}
				}
			}
		}

		internal void UpdateGraphExtents()
		{
			if (_GraphEditor == null)
			{
				return;
			}

			Rect extents = _GraphEditor.UpdateGraphExtents();
			_GraphExtents = extents;

			Rect graphPosition = graphViewport;

			extents.xMin -= graphPosition.width * 0.6f;
			extents.xMax += graphPosition.width * 0.6f;
			extents.yMin -= graphPosition.height * 0.6f;
			extents.yMax += graphPosition.height * 0.6f;

			extents.xMin = (int)extents.xMin;
			extents.xMax = (int)extents.xMax;
			extents.yMin = (int)extents.yMin;
			extents.yMax = (int)extents.yMax;

			if (_GraphEditor.isDragNodes)
			{
				if (graphPosition.xMin < extents.xMin)
				{
					extents.xMin = graphPosition.xMin;
				}
				if (extents.xMax < graphPosition.xMax)
				{
					extents.xMax = graphPosition.xMax;
				}

				if (graphPosition.yMin < extents.yMin)
				{
					extents.yMin = graphPosition.yMin;
				}
				if (extents.yMax < graphPosition.yMax)
				{
					extents.yMax = graphPosition.yMax;
				}
			}

			if (graphExtents != extents)
			{
#if ARBOR_EDITOR_USE_UIELEMENTS
#if UNITY_2019_2_OR_NEWER
				_GraphExtentsBoundingBoxElement.style.left = extents.x;
				_GraphExtentsBoundingBoxElement.style.top = extents.y;
				_GraphExtentsBoundingBoxElement.style.width = extents.width;
				_GraphExtentsBoundingBoxElement.style.height = extents.height;
#endif
				_GraphViewExtents = extents;
#else
				_GraphGUI.extents = extents;
				DoRepaint();
#endif
			}
		}

		public void FrameSelected(Vector2 frameSelectTarget)
		{
			_FrameSelected.Begin(frameSelectTarget);
			_FrameSelectedZoom.Begin(Vector2.one);

			DoRepaint();
		}

		public bool OverlapsVewArea(Rect position)
		{
			return graphViewport.Overlaps(position);
		}

		void UpdateScrollbar()
		{
#if ARBOR_EDITOR_USE_UIELEMENTS
			bool endFrameSelected = _EndFrameSelected;
			_EndFrameSelected = false;

			bool repaint = false;

			if (_FrameSelectedZoom.isPlaying)
			{
				Vector2 zoomScale = _FrameSelectedZoom.Update(this.graphScale,Vector2.zero);

				SetZoom(graphViewport.center, new Vector3(zoomScale.x, zoomScale.y, 1), false, !_FrameSelected.isPlaying);

				repaint = true;
			}

			if (_FrameSelected.isPlaying)
			{
				Vector2 scrollPos = _FrameSelected.Update(this.scrollPos, -graphViewport.size * 0.5f);

				SetScroll(scrollPos, true, false);

				repaint = true;
			}

			if (repaint)
			{
				DoRepaint();
			}

			_EndFrameSelected = endFrameSelected;
#else
			if (_FrameSelected.isPlaying)
			{
				switch (Event.current.type)
				{
					case EventType.MouseDown:
						if (graphViewportPosition.Contains(Event.current.mousePosition))
						{
							Vector2 scrollPos = this.scrollPos;
							scrollPos.x = (int)scrollPos.x;
							scrollPos.y = (int)scrollPos.y;
							SetScroll(scrollPos, true, true);
						}
						break;
					case EventType.Repaint:
						{
							Vector2 offset = -graphViewport.size * 0.5f;
							offset.x = Mathf.Floor(offset.x);
							offset.y = Mathf.Floor(offset.y);
							Vector2 scrollPos = _FrameSelected.Update(this.scrollPos, offset);

							SetScroll(scrollPos, true, false);

							DoRepaint();
						}
						break;
				}
			}

			if (_IsLayoutSetup)
			{
				EditorGUI.BeginChangeCheck();
				_GraphGUI.HandleScrollbar();
				if (EditorGUI.EndChangeCheck())
				{
					SetScroll(_GraphGUI.scrollPos, false, true);
				}
			}
#endif
		}

		bool IsDragNodeBehaviour()
		{
			BehaviourDragInfo behaviourDragInfo = BehaviourDragInfo.GetBehaviourDragInfo();
			return behaviourDragInfo != null && behaviourDragInfo.dragging;
		}

		private bool _IsDragObject = false;

		static bool Internal_IsDragObject()
		{
			foreach (var obj in DragAndDrop.objectReferences)
			{
				if (obj is GameObject || obj is Component)
				{
					return true;
				}
			}

			return false;
		}

		void HandleDragObject()
		{
			Event current = Event.current;

			switch (current.type)
			{
				case EventType.DragUpdated:
					_IsDragObject = Internal_IsDragObject();
					break;
				case EventType.DragExited:
					_IsDragObject = false;
					DoRepaint();
					break;
				default:
					if(_IsDragObject)
					{
						_IsDragObject = Internal_IsDragObject();
					}
					break;
			}
		}

		bool IsDragObject()
		{
			return _GraphEditor.IsDragObject() || _IsDragObject;
		}

		bool IsDragScroll()
		{
			return _GraphEditor.IsDragScroll() || IsDragNodeBehaviour() || IsDragObject();
		}

		private static class AutoScrollDefaults
		{
			public static readonly Color color;
			public static readonly RectOffset offset;

			static AutoScrollDefaults()
			{
				color = new Color(0.0f, 0.5f, 1.0f, 0.1f);
				offset = new RectOffset(30, 30, 30, 30);
			}
		}

		private bool _IsAutoScrolling = false;

		bool DoAutoScroll()
		{
			if (Event.current.type != EventType.Repaint)
			{
				return false;
			}

			if (!IsDragScroll())
			{
				_IsAutoScrolling = false;
				return false;
			}

			bool isDragObject = IsDragNodeBehaviour() || IsDragObject();

			Vector2 offset = Vector2.zero;

			Vector2 mousePosition = Event.current.mousePosition;

			RectOffset scrollAreaOffset = AutoScrollDefaults.offset;

			Rect viewport = graphViewport;
			Rect noScrollArea = scrollAreaOffset.Remove(viewport);

			if (isDragObject)
			{
				if (!_IsAutoScrolling)
				{
					_IsAutoScrolling = noScrollArea.Contains(mousePosition);
				}
			}
			else
			{
				_IsAutoScrolling = true;
			}

			if (!_IsAutoScrolling)
			{
				return false;
			}

			EditorGUI.DrawRect(Rect.MinMaxRect(viewport.xMin, viewport.yMin, noScrollArea.xMin, viewport.yMax), AutoScrollDefaults.color);
			EditorGUI.DrawRect(Rect.MinMaxRect(noScrollArea.xMax, viewport.yMin, viewport.xMax, viewport.yMax), AutoScrollDefaults.color);

			EditorGUI.DrawRect(Rect.MinMaxRect(noScrollArea.xMin, viewport.yMin, noScrollArea.xMax, noScrollArea.yMin), AutoScrollDefaults.color);
			EditorGUI.DrawRect(Rect.MinMaxRect(noScrollArea.xMin, noScrollArea.yMax, noScrollArea.xMax, viewport.yMax), AutoScrollDefaults.color);

			if (isDragObject && !viewport.Contains(mousePosition))
			{
				return false;
			}

			if (mousePosition.x < noScrollArea.xMin)
			{
				offset.x = mousePosition.x - noScrollArea.xMin;
			}
			else if (noScrollArea.xMax < mousePosition.x)
			{
				offset.x = mousePosition.x - noScrollArea.xMax;
			}

			if (mousePosition.y < noScrollArea.yMin)
			{
				offset.y = mousePosition.y - noScrollArea.yMin;
			}
			else if (noScrollArea.yMax < mousePosition.y)
			{
				offset.y = mousePosition.y - noScrollArea.yMax;
			}

			offset.x = Mathf.Clamp(offset.x, -10.0f, 10.0f);
			offset.y = Mathf.Clamp(offset.y, -10.0f, 10.0f);

			if (offset.sqrMagnitude > 0.0f)
			{
				Vector2 scrollPos = this.scrollPos;

				scrollPos += offset;
				scrollPos.x = (int)scrollPos.x;
				scrollPos.y = (int)scrollPos.y;

				if (graphExtents.Contains(scrollPos))
				{
					SetScroll(scrollPos, true, false);

					return true;
				}
			}

			return false;
		}

		internal void DirtyGraphExtents()
		{
			_IsLayoutSetup = false;
#if ARBOR_EDITOR_USE_UIELEMENTS
			_GraphView.UpdateLayout();
#else
			DoRepaint();
#endif
		}
		private Vector2 _NoGraphScrollPos = Vector2.zero;

		void NoGraphSelectedGUI(Rect rect)
		{
			using (new GUILayout.AreaScope(rect))
			{
				rect.position = Vector2.zero;

				using (GUILayout.ScrollViewScope scope = new GUILayout.ScrollViewScope(_NoGraphScrollPos))
				{
					_NoGraphScrollPos = scope.scrollPosition;

					GUILayout.FlexibleSpace();

					using (new GUILayout.HorizontalScope())
					{
						GUILayout.FlexibleSpace();

						GUILayout.Label(EditorContents.noGraphSelectedMessage);

						GUILayout.FlexibleSpace();
					}

					EditorGUILayout.Space();

					using (new GUILayout.HorizontalScope())
					{
						GUILayout.FlexibleSpace();

						using (new GUILayout.HorizontalScope())
						{
							if (GUILayout.Button(EditorContents.createArborFSM, Styles.largeButtonLeft, GUILayout.Width(200)))
							{
								CreateFSM();
							}

							if (GUILayout.Button(EditorContents.createBehaviourTree, Styles.largeButtonRight, GUILayout.Width(200)))
							{
								CreateBT();
							}
						}

						GUILayout.FlexibleSpace();
					}

					EditorGUILayout.Space();

					float buttonWidth = 130;

					using (new GUILayout.HorizontalScope())
					{
						GUILayout.FlexibleSpace();

						if (GUILayout.Button(EditorContents.assetStore, EditorStyles.miniButtonLeft, GUILayout.Width(buttonWidth)))
						{
							ArborVersion.OpenAssetStore();
						}

						if (GUILayout.Button(EditorContents.officialSite, EditorStyles.miniButtonMid, GUILayout.Width(buttonWidth)))
						{
							Help.BrowseURL(Localization.GetWord("SiteURL"));
						}

						if (GUILayout.Button(EditorContents.releaseNotes, EditorStyles.miniButtonRight, GUILayout.Width(buttonWidth)))
						{
							Help.BrowseURL(Localization.GetWord("ReleaseNotesURL"));
						}

						GUILayout.FlexibleSpace();
					}

					using (new GUILayout.HorizontalScope())
					{
						GUILayout.FlexibleSpace();

						if (GUILayout.Button(EditorContents.manual, EditorStyles.miniButtonLeft, GUILayout.Width(buttonWidth)))
						{
							Help.BrowseURL(Localization.GetWord("ManualURL"));
						}

						if (GUILayout.Button(EditorContents.arborReference, EditorStyles.miniButtonMid, GUILayout.Width(buttonWidth)))
						{
							Help.BrowseURL(Localization.GetWord("ArborReferenceURL"));
						}

						if (GUILayout.Button(EditorContents.scriptReference, EditorStyles.miniButtonRight, GUILayout.Width(buttonWidth)))
						{
							Help.BrowseURL(Localization.GetWord("ScriptReferenceURL"));
						}

						GUILayout.FlexibleSpace();
					}

					GUILayout.FlexibleSpace();
				}
			}
		}

		void GraphViewGUI()
		{
			if (_GraphEditor == null || _GraphEditor.nodeGraph == null)
			{
				return;
			}

#if !ARBOR_EDITOR_USE_UIELEMENTS
			_GraphGUI.position = _GraphRect;
#endif

			_GraphEditor.OnRenameEvent();

#if ARBOR_TRIAL
			GUIContent openContent = EditorGUITools.GetTextContent( "Open Asset Store" );
			Vector2 openButtonSize = Styles.largeButton.CalcSize( openContent );
			Rect openRect = new Rect(_GraphRect.xMin + 16.0f, _GraphRect.yMax - openButtonSize.y - 16.0f, openButtonSize.x, openButtonSize.y );

			if( Event.current.type != EventType.Repaint )
			{
				if( GUI.Button( openRect, openContent, Styles.largeButton ) )
				{
					ArborVersion.OpenAssetStore();
				}
			}
#endif

			UpdateScrollbar();

#if !ARBOR_EDITOR_USE_UIELEMENTS
			_GraphGUI.BeginGraphGUI();
#endif

			GraphGUI();

#if !ARBOR_EDITOR_USE_UIELEMENTS
			_GraphGUI.EndGraphGUI();
#endif

#if !ARBOR_EDITOR_USE_UIELEMENTS
			UpdateGraphExtents();
			OnPostLayout();
#endif

			_GraphEditor.UpdateVisibleNodes();

#if ARBOR_TRIAL
			if( Event.current.type == EventType.Repaint )
			{
				if( GUI.Button( openRect, openContent, Styles.largeButton ) )
				{
					ArborVersion.OpenAssetStore();
				}
			}
#endif
		}

		void GraphGUI()
		{
			if (_GraphEditor == null || _GraphEditor.nodeGraph == null)
			{
				return;
			}

			using (new ProfilerScope("GraphGUI"))
			{
				using (new ProfilerScope("BeginGraphGUI"))
				{
					if (ArborSettings.showGrid)
					{
						float zoomLevel = isCapture ? 1f : graphScale.x;
						EditorGUITools.DrawGrid(graphExtents, zoomLevel);
					}

#if !ARBOR_TRIAL
					if (!isCapture && ArborSettings.showLogo == LogoShowMode.AlwaysShow)
					{
						DrawLogo(graphViewport);
					}
#endif

					_GraphEditor.BeginGraphGUI(isCapture || !zoomable);
				}

				using (new ProfilerScope("BeginWindows"))
				{
					_GUIIsExiting = false;

					BeginWindows();

					if (_GUIIsExiting)
					{
						GUIUtility.ExitGUI();
					}
				}

				using (new ProfilerScope("OnGraphGUI"))
				{
					_GraphEditor.OnGraphGUI();
				}

				using (new ProfilerScope("EndWindows"))
				{
					_GUIIsExiting = false;

					EndWindows();

					if (_GUIIsExiting)
					{
						GUIUtility.ExitGUI();
					}
				}

				using (new ProfilerScope("EndGraphGUI"))
				{
					HandleDragObject();

					_GraphEditor.EndGraphGUI(isCapture || !zoomable);

#if !ARBOR_EDITOR_USE_UIELEMENTS
					DrawSelection();
#endif

					bool scrolled = false;
					if (DoAutoScroll())
					{
						scrolled = true;
					}

#if ARBOR_EDITOR_USE_UIELEMENTS
					if (Event.current.type == EventType.Repaint)
					{
						if (_PanManipulator.isActive)
						{
							EditorGUIUtility.AddCursorRect(graphViewport, MouseCursor.Pan);
						}
						else if( _ZoomManipulator.isActive)
						{
							EditorGUIUtility.AddCursorRect(graphViewport, MouseCursor.Zoom);
						}
					}
#else
					if (_GraphGUI.DragGrid())
					{
						StoreCurrentTransform();
						scrolled = true;
					}
#endif

					if (scrolled)
					{
						_FrameSelected.End();
						_FrameSelectedZoom.End();
					}
				}
			}
		}

		internal void BeginNode()
		{
#if ARBOR_EDITOR_USE_UIELEMENTS && !UNITY_2018_2_OR_NEWER
			_GraphUIDepth = _GraphUI.GetGUIDepth();
			int currentGUIDepth = EditorGUITools.currentGUIDepth;
			if (_GraphUIDepth != currentGUIDepth)
			{
				if (_ChangedGUIDepth)
				{
					Debug.LogWarning("Already changed GUIDepth");
				}
				_ChangedGUIDepth = true;
				_GraphUI.SetGUIDepth(currentGUIDepth);
			}
			else
			{
				_ChangedGUIDepth = false;
			}
#endif

			_IsInNodeEditor = true;

#if ARBOR_EDITOR_EXTENSIBLE
			EndSkin();
			BeginSkin();
#endif
		}

		internal void EndNode()
		{
#if ARBOR_EDITOR_EXTENSIBLE
			EndSkin();
			BeginSkin();
#endif

#if ARBOR_EDITOR_USE_UIELEMENTS && !UNITY_2018_2_OR_NEWER
			if (_ChangedGUIDepth)
			{
				_GraphUI.SetGUIDepth(_GraphUIDepth);
				_ChangedGUIDepth = false;
			}
#endif

			_IsInNodeEditor = false;
		}

		void OnStateChanged(NodeGraph nodeGraph)
		{
			_IsRepaint = true;

			if (_GraphEditor != null && _GraphEditor.nodeGraph == nodeGraph)
			{
				_IsUpdateLiveTracking = true;
			}
		}

		private void FlipLocked()
		{
			_IsLocked = !_IsLocked;
		}

		public void AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(EditorGUITools.GetTextContent("Lock"), _IsLocked, FlipLocked);
		}

		internal void SetScroll(Vector2 position, bool updateView, bool endFrameSelected)
		{
			if (!_IsLayoutSetup || isCapture)
			{
				return;
			}

			Rect extents = graphExtents;
			Rect viewport = graphViewport;
			position.x = Mathf.Clamp(position.x, extents.xMin, extents.xMax - viewport.width);
			position.y = Mathf.Clamp(position.y, extents.yMin, extents.yMax - viewport.height);

			this.scrollPos = position;

#if ARBOR_EDITOR_USE_UIELEMENTS
			if (updateView)
			{
				if (_GraphUI != null)
				{
					_GraphView.SetScrollOffset(position, false, false);
				}
			}

			endFrameSelected = endFrameSelected && _EndFrameSelected;
#endif

			if (endFrameSelected)
			{
				_FrameSelected.End();
				_FrameSelectedZoom.End();
			}

			StoreCurrentTransform();
		}

		internal void OnScroll(Vector2 delta)
		{
			SetScroll(scrollPos + delta, true, true);
		}

		internal void SetZoom(Vector2 zoomCenter, Vector3 zoomScale, bool endFrameSelected, bool updateScroll = true)
		{
#if ARBOR_EDITOR_USE_UIELEMENTS
			Vector3 position = _GraphUI.transform.position;
			Vector3 scale = _GraphUI.transform.scale;

			position += Vector3.Scale(zoomCenter, scale);
			zoomScale.x = Mathf.Clamp(zoomScale.x, 0.1f, 1f);
			zoomScale.y = Mathf.Clamp(zoomScale.y, 0.1f, 1f);

			_GraphUI.transform.position = position - Vector3.Scale(zoomCenter, zoomScale);
			_GraphUI.transform.scale = zoomScale;

			Vector2 scrollPos = this.scrollPos;

			_GraphView.UpdateLayout();

			if (updateScroll)
			{
				SetScroll(scrollPos, true, endFrameSelected);
			}
#endif
		}

		internal void OnZoom(Vector2 zoomCenter, float zoomScale)
		{
#if ARBOR_EDITOR_USE_UIELEMENTS
			Vector3 scale = _GraphUI.transform.scale;
			SetZoom(zoomCenter, Vector3.Scale(scale, new Vector3(zoomScale, zoomScale, 1f)), true);
#endif
		}

		private void CenterOnStoredPosition(NodeGraph graph)
		{
			if (!_IsLayoutSetup)
			{
				return;
			}

			if (_TransformCache.HasTransform(graph))
			{
				Vector2 scrollPos = _TransformCache.GetPosition(graph);
				Vector3 scale = _TransformCache.GetScale(graph);

				SetZoom(Vector2.zero, scale, false);
				SetScroll(scrollPos, true, false);
			}
			else
			{
				SetZoom(Vector2.zero, Vector3.one, false);

				Vector2 center = graphExtents.center - graphViewRect.size * 0.5f;
				center.x = Mathf.Floor(center.x);
				center.y = Mathf.Floor(center.y);
				SetScroll(center, true, false);
			}
		}

		private void StoreCurrentTransform()
		{
			if (_NodeGraphCurrent == null)
			{
				return;
			}

			if (!_IsLayoutSetup)
			{
				return;
			}

			_TransformCache.SetPosition(_NodeGraphCurrent, scrollPos);
			_TransformCache.SetScale(_NodeGraphCurrent, graphScale);
		}

		public Vector2 GraphToWindowPoint(Vector2 point)
		{
			if (isCapture)
			{
				return point;
			}
			return graphMatrix.MultiplyPoint(point);
		}

		public Rect GraphToWindowRect(Rect rect)
		{
			Matrix4x4 graphMatrix = this.graphMatrix;
			Vector2 min = graphMatrix.MultiplyPoint(rect.min);
			Vector2 max = graphMatrix.MultiplyPoint(rect.max);
			return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
		}

		public Vector2 WindowToGraphPoint(Vector2 point)
		{
			Matrix4x4 graphMatrix = this.graphMatrix.inverse;
			return graphMatrix.MultiplyPoint(point);
		}

		public Rect WindowToGraphRect(Rect rect)
		{
			Matrix4x4 graphMatrix = this.graphMatrix.inverse;
			Vector2 min = graphMatrix.MultiplyPoint(rect.min);
			Vector2 max = graphMatrix.MultiplyPoint(rect.max);
			return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
		}

		public Vector2 ClipToGraph(Vector2 absolutePos)
		{
#if ARBOR_EDITOR_USE_UIELEMENTS
			return GUIClip.Clip(absolutePos);
#else
			if (s_ParentField != null && s_GetBorderSizeMethod != null)
			{
				var hostView = s_ParentField.GetValue(this);
				RectOffset borderSize = (RectOffset)s_GetBorderSizeMethod.Invoke(hostView, null);

				absolutePos -= graphViewport.position - graphViewportPosition.position - new Vector2(borderSize.left, borderSize.top);
				Vector2 pos = GUIClip.Clip(absolutePos);
				return pos;
			}
			else
			{
				absolutePos -= graphViewport.position - graphViewportPosition.position - position.position;
				Vector2 pos = GUIUtility.ScreenToGUIPoint(absolutePos);
				return pos;
			}
#endif
		}

		public Vector2 UnclipToGraph(Vector2 pos)
		{
#if ARBOR_EDITOR_USE_UIELEMENTS
			return GUIClip.Unclip(pos);
#else
			if (s_ParentField != null && s_GetBorderSizeMethod != null)
			{
				var hostView = s_ParentField.GetValue(this);
				RectOffset borderSize = (RectOffset)s_GetBorderSizeMethod.Invoke(hostView, null);

				Vector2 absolutePos = GUIClip.Unclip(pos);
				absolutePos += graphViewport.position - graphViewportPosition.position - new Vector2(borderSize.left, borderSize.top);
				return absolutePos;
			}
			else
			{
				Vector2 absolutePos = GUIUtility.GUIToScreenPoint(pos);
				absolutePos += graphViewport.position - graphViewportPosition.position - position.position;
				return absolutePos;
			}
#endif
		}

#if ARBOR_EDITOR_USE_UIELEMENTS
		private static readonly string s_UnityVersion_2019_1_0_a5 = "2019.1.0a5";

		[System.NonSerialized]
		VisualElement _RootElement;

#if UNITY_2018_3_OR_NEWER
		[System.NonSerialized]
		bool _IsInitializedUpdateDocked = false;

		[System.NonSerialized]
		bool _IsUpdateDocked = false;

		[System.NonSerialized]
		bool _IsDocked = false;
#endif

		void UpdateDocked()
		{
#if UNITY_2018_3_OR_NEWER
			if (_IsInitializedUpdateDocked && !_IsUpdateDocked || _RootElement == null)
			{
				return;
			}

#if UNITY_2019_1_OR_NEWER
			float positionTop = (float)_RootElement.resolvedStyle.top;
#else
			float positionTop = (float)_RootElement.style.positionTop;
#endif

			if (!_IsInitializedUpdateDocked)
			{
				if (positionTop == 0f)
				{
					_IsUpdateDocked = true;
					return;
				}
			}

#if UNITY_2019_1_OR_NEWER
			float positionLeft = (float)_RootElement.resolvedStyle.left;
#else
			float positionLeft = (float)_RootElement.style.positionLeft;
#endif

			// Whether it is docked to the MainView.
			bool docked = positionLeft == 2f;

			if (!_IsInitializedUpdateDocked || _IsDocked != docked)
			{
				if (docked)
				{
#if UNITY_2019_1_OR_NEWER
					_MainLayout.style.top = 19f - positionTop;
#else
					_MainLayout.style.positionTop = 19f - positionTop;
#endif
				}
				else
				{
#if UNITY_2019_1_OR_NEWER
					_MainLayout.style.top = 20f - positionTop;
#else
					_MainLayout.style.positionTop = 23f - positionTop;
#endif
				}

				_IsDocked = docked;
				_IsInitializedUpdateDocked = true;
			}
#endif
		}

		void SetupGUI()
		{
#if UNITY_2019_1_OR_NEWER
			_RootElement = this.rootVisualElement;
#else
			_RootElement = this.GetRootVisualContainer();
#endif

			_MainLayout = new GraphMainLayout() {
				name = "MainLayout",
				style =
				{
					flexDirection = FlexDirection.Row,
#if UNITY_2019_1_OR_NEWER
					position = Position.Absolute,
					top = 0,
					bottom = 0,
					right = 0,
					left = 0,
#else
					positionType = PositionType.Absolute,
					positionTop = 0,
					positionBottom = 0,
					positionRight = 0,
					positionLeft = 0,
#endif
				}
			};

			_MainLayout.handleEventDelegate += (evt) => 
			{
				if (!UIElementsUtility.IsLayoutEvent(evt) || _MainLayoutInitialized)
				{
					return;
				}

				float leftPanelFlex = 1f;
				float rightPanelFlex = 3f;
				float totalFlex = leftPanelFlex + rightPanelFlex;

				Vector2 leftPanelMinSize = new Vector2(150f, 0f);
				Vector2 rightPanelMinSize = new Vector2(150f, 0f);

				float sidePanelWidth = ArborSettings.sidePanelWidth;
				leftPanelFlex = VisualSplitter.CalcFlex(new Vector2(sidePanelWidth, 0f), Vector2.zero, leftPanelMinSize, rightPanelMinSize, _MainLayout.layout.size, FlexDirection.Row, totalFlex);
				rightPanelFlex = totalFlex - leftPanelFlex;

#if UNITY_2019_1_OR_NEWER
				_LeftPanelLayout.style.flexGrow = leftPanelFlex;
#elif UNITY_2018_3_OR_NEWER
				_LeftPanelLayout.style.flex = new Flex(leftPanelFlex);
#else
				_LeftPanelLayout.style.flex = leftPanelFlex;
#endif
				_LeftPanelLayout.style.minWidth = leftPanelMinSize.x;

#if UNITY_2019_1_OR_NEWER
				_RightPanelLayout.style.flexGrow = rightPanelFlex;
#elif UNITY_2018_3_OR_NEWER
				_RightPanelLayout.style.flex = new Flex(rightPanelFlex);
#else
				_RightPanelLayout.style.flex = rightPanelFlex;
#endif
				_RightPanelLayout.style.minWidth = rightPanelMinSize.x;

				_MainLayoutInitialized = true;
			};

			_LeftPanelLayout = new GraphLayout() {
				name = "LeftPanel",
			};

			_LeftPanelLayout.handleEventDelegate += (evt) =>
			{
				if (UIElementsUtility.IsLayoutEvent(evt) && _MainLayoutInitialized)
				{
					ArborSettings.sidePanelWidth = _LeftPanelLayout.layout.width;
				}
			};

			float toolbarHeight = EditorGUITools.toolbarHeight;
			if (Application.unityVersion == s_UnityVersion_2019_1_0_a5)
			{
				toolbarHeight = 20f;
			}

			_SideToolbarLayout = new GraphLayout()
			{
				name = "SideToolbar",
				style =
				{
					height = toolbarHeight,
				}
			};

			_SideToolbarUI = new StretchableIMGUIContainer(
				() => {
					Rect rect = _SideToolbarUI.layout;

#if UNITY_2018_2_OR_NEWER
					// _SideToolbarUI.layout is not updated correctly when the window size has been changed.
					rect.size = _SideToolbarLayout.layout.size;
#endif

					SideToolbarGUI(rect);
				}, StretchableIMGUIContainer.StretchMode.Flex)
			{
				name = "SideToolbarUI",
			};

			_SideToolbarLayout.Add(_SideToolbarUI);
			_LeftPanelLayout.Add(_SideToolbarLayout);

			_SidePanelLayout = new GraphLayout()
			{
				name = "SidePanelLayout",
				style =
				{
#if UNITY_2019_1_OR_NEWER
					flexGrow = 1f,
#elif UNITY_2018_3_OR_NEWER
					flex = new Flex(1f),
#else
					flex = 1f,
#endif
				}
			};

			_SidePanelUI = new StretchableIMGUIContainer(
				() => {
					Rect rect = _SidePanelUI.layout;

#if UNITY_2018_2_OR_NEWER
					// _SidePanelUI.layout is not updated correctly when the window size has been changed.
					rect.size = _SidePanelLayout.layout.size;
#endif

					SidePanelGUI(rect);
				}, StretchableIMGUIContainer.StretchMode.Flex)
			{
				name = "SidePanelUI",
			};

			_SidePanelLayout.Add(_SidePanelUI);

			_LeftPanelLayout.Add(_SidePanelLayout);

			if (ArborSettings.openSidePanel)
			{
				_MainLayout.Add(_LeftPanelLayout);
			}

			_RightPanelLayout = new GraphLayout() {
				name = "RightPanel",
			};

			GraphLayout toolbarLayout = new GraphLayout()
			{
				name = "Toolbar",
				style =
				{
					height = toolbarHeight,
				}
			};

			_ToolbarUI = new StretchableIMGUIContainer(
				() => {
					ToolbarGUI(_ToolbarUI.layout);
				}, StretchableIMGUIContainer.StretchMode.Flex)
			{
				name = "ToolbarUI",
			};

			toolbarLayout.Add(_ToolbarUI);
			_RightPanelLayout.Add(toolbarLayout);

			_GraphPanel = new GraphLayout() {
				name = "GraphPanel",
				style =
				{
#if UNITY_2019_1_OR_NEWER
					flexGrow = 1f,
#elif UNITY_2018_3_OR_NEWER
					flex = new Flex(1f),
#else
					flex = 1f,
#endif
				}
			};

			_GraphView = new GraphView(this) {
				name = "GraphView",
			};

			_GraphUnderlayUI = new StretchableIMGUIContainer(
				() => {
					UnderlayGUI(_GraphUnderlayUI.layout);
				}, StretchableIMGUIContainer.StretchMode.Absolute)
			{
				name = "GraphUnderlayUI",
			};

			_GraphView.contentContainer.Add(_GraphUnderlayUI);

			_GraphUI = new StretchableIMGUIContainer(
				() => {
					if (!isCapture)
					{
						if (Event.current.type == EventType.MouseMove)
						{
							if (_GraphEditor != null)
							{
								_GraphEditor.CloseAllPopupButtonControl();
							}
						}
						GraphViewGUI();
					}
					CheckDragBehaviour();
				}, StretchableIMGUIContainer.StretchMode.Flex)
			{
				name = "GraphUI",
#if UNITY_2019_1_OR_NEWER
				//clippingOptions = VisualElement.ClippingOptions.NoClipping,
#else
				clippingOptions = VisualElement.ClippingOptions.NoClipping,
#endif
			};
#if UNITY_2018_3_OR_NEWER
			_GraphUI.style.overflow = Overflow.Visible;
#endif
			_GraphView.contentContainer.Add(_GraphUI);

#if UNITY_2020_1_OR_NEWER
			_GetWorldBoundingBox = (System.Func<Rect>)System.Delegate.CreateDelegate(typeof(System.Func<Rect>), _GraphUI, s_GetWorldBoundingBoxMethod);
			_SetLastWorldClip = (System.Action<Rect>)System.Delegate.CreateDelegate(typeof(System.Action<Rect>), _GraphUI, s_SetLastWorldClipMethod);
			_GraphUI.generateVisualContent += (mgc) =>
			{
				_SetLastWorldClip(_GetWorldBoundingBox());
			};
#endif

#if UNITY_2019_2_OR_NEWER
			_GraphExtentsBoundingBoxElement = new VisualElement()
			{
				name = "GraphExtents",
				pickingMode = PickingMode.Ignore,
				style =
				{
					position = Position.Absolute,
					overflow = Overflow.Hidden,
					visibility = Visibility.Hidden,
				}
			};

			_GraphUI.hierarchy.Add(_GraphExtentsBoundingBoxElement);
#endif

			_GraphOverlayUI = new StretchableIMGUIContainer(
				() => {
					if (_GraphEditor != null)
					{
						_GraphEditor.UpdateOverlayLayer();
						_GraphEditor.BeginOverlayLayer();
						_GraphEditor.EndOverlayLayer();
					}
					DrawSelection();
					OverlayGUI(_GraphOverlayUI.layout);
				}, StretchableIMGUIContainer.StretchMode.Absolute)
			{
				name = "GraphOverlayUI",
			};

			_GraphOverlayUI.containsPointCallback += (localPoint) =>
			{
				if (_GraphEditor != null)
				{
					return _GraphEditor.ContainsOverlayLayer(localPoint);
				}

				return true;
			};

			_GraphView.contentContainer.Add(_GraphOverlayUI);

			_ZoomManipulator = new ZoomManipulator(_GraphUI, this);
			_GraphView.contentContainer.AddManipulator(_ZoomManipulator);

			_PanManipulator = new PanManipulator(_GraphUI, this);
			_GraphView.contentContainer.AddManipulator(_PanManipulator);

			_NoGraphUI = new StretchableIMGUIContainer(
				() =>
				{
					NoGraphSelectedGUI(_NoGraphUI.layout);
				}, StretchableIMGUIContainer.StretchMode.Flex);

			if (_GraphEditor != null)
			{
				_GraphPanel.Add(_GraphView);
				_HasGraphView = true;
			}
			else
			{
				_GraphPanel.Add(_NoGraphUI);
				_HasGraphView = false;
			}

			_RightPanelLayout.Add(_GraphPanel);

			float graphBottomHeight = EditorGUITools.toolbarHeight;
			if (Application.unityVersion == s_UnityVersion_2019_1_0_a5)
			{
				graphBottomHeight = 17f;
			}

			GraphLayout graphBottomLayout = new GraphLayout() {
				name = "GraphBottomBar",
				style =
				{
					height = graphBottomHeight,
				}
			};

			_GraphBottomUI = new StretchableIMGUIContainer(
				() => {
					BreadcrumbGUI(_GraphBottomUI.layout);
				}, StretchableIMGUIContainer.StretchMode.Flex)
			{
				name = "GraphBottomUI",
			};

			graphBottomLayout.Add(_GraphBottomUI);

			_RightPanelLayout.Add(graphBottomLayout);

			_MainLayout.Add(_RightPanelLayout);

			_RootElement.Add(_MainLayout);

			DirtyGraphExtents();

			UpdateDocked();
		}
#else
		void CalculateRect()
		{
			_ToolBarRect = new Rect(0.0f, 0.0f, this.position.width, EditorStyles.toolbar.fixedHeight);

			if (ArborSettings.openSidePanel)
			{
				_SideToolbarRect = new Rect(0.0f, 0.0f, ArborSettings.sidePanelWidth, EditorStyles.toolbar.fixedHeight);
				_SidePanelRect = new Rect(0.0f, _SideToolbarRect.yMax, ArborSettings.sidePanelWidth, this.position.height - _SideToolbarRect.height);

				_ToolBarRect.xMin = _SidePanelRect.xMax;

				float graphWidth = this.position.width - _SidePanelRect.width;
				if (graphWidth < k_MinRightSideWidth)
				{
					graphWidth = k_MinRightSideWidth;
				}

				_BreadcrumbRect = new Rect(_SidePanelRect.xMax, this.position.height - EditorStyles.toolbar.fixedHeight, graphWidth, EditorStyles.toolbar.fixedHeight);

				float graphHeight = this.position.height - (_ToolBarRect.height + _BreadcrumbRect.height);
				_GraphRect = new Rect(_SidePanelRect.xMax, _ToolBarRect.yMax, graphWidth, graphHeight);
			}
			else
			{
				float graphWidth = this.position.width;

				_BreadcrumbRect = new Rect(0.0f, this.position.height - EditorStyles.toolbar.fixedHeight, graphWidth, EditorStyles.toolbar.fixedHeight);

				float graphHeight = this.position.height - (_ToolBarRect.height + _BreadcrumbRect.height);
				_GraphRect = new Rect(0.0f, _ToolBarRect.yMax, graphWidth, graphHeight);
			}
		}

		private static readonly int s_MouseDeltaReaderHash = "s_MouseDeltaReaderHash".GetHashCode();
		static Vector2 s_MouseDeltaReaderLastPos;

		static readonly float k_MinLeftSideWidth = 150f;
		static readonly float k_MinRightSideWidth = 110f;

		static Vector2 MouseDeltaReader(Rect position, bool activated)
		{
			int controlId = GUIUtility.GetControlID(s_MouseDeltaReaderHash, FocusType.Passive, position);
			Event current = Event.current;
			switch (current.GetTypeForControl(controlId))
			{
				case EventType.MouseDown:
					if (activated && GUIUtility.hotControl == 0 && (position.Contains(current.mousePosition) && current.button == 0))
					{
						GUIUtility.hotControl = controlId;
						GUIUtility.keyboardControl = 0;

						s_MouseDeltaReaderLastPos = GUIUtility.GUIToScreenPoint(current.mousePosition);

						current.Use();
						break;
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == controlId && current.button == 0)
					{
						GUIUtility.hotControl = 0;
						current.Use();
						break;
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controlId)
					{
						Vector2 vector2_1 = GUIUtility.GUIToScreenPoint(current.mousePosition);
						Vector2 vector2_2 = vector2_1 - s_MouseDeltaReaderLastPos;
						s_MouseDeltaReaderLastPos = vector2_1;
						current.Use();
						return vector2_2;
					}
					break;
			}
			return Vector2.zero;
		}

		void ResizeHandling(float width, float height)
		{
			if (!ArborSettings.openSidePanel)
			{
				return;
			}

			Rect dragRect = new Rect(ArborSettings.sidePanelWidth, 0.0f, 5.0f, height);
			float minLeftSide = k_MinLeftSideWidth;
			float minRightSide = k_MinRightSideWidth;

			if (Event.current.type == EventType.Repaint)
			{
				EditorGUIUtility.AddCursorRect(dragRect, MouseCursor.SplitResizeLeftRight);
			}
			float num = 0.0f;
			float x = MouseDeltaReader(dragRect, true).x;
			if (x != 0.0f)
			{
				dragRect.x += x;
			}

			if (dragRect.x < minLeftSide || minLeftSide > width - minRightSide)
			{
				num = minLeftSide;
			}
			else if (dragRect.x > width - minRightSide)
			{
				num = width - minRightSide;
			}
			else
			{
				num = dragRect.x;
			}

			if (num > 0.0)
			{
				ArborSettings.sidePanelWidth = num;
			}
		}
#endif
	}
}