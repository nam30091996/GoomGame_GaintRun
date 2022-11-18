//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
#if !ARBOR_DLL

using UnityEngine;
using UnityEditor;

#if UNITY_2017_1_OR_NEWER
using System.Collections.Generic;
#endif

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#elif UNITY_2017_1_OR_NEWER
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEditor.Experimental.UIElements;
#endif

namespace ArborEditor
{
	public sealed class EditorCapture
	{
#pragma warning disable 414
		private EditorWindow _Window = null;
#pragma warning restore 414

		private Rect _Position = new Rect();

		private Rect _SavedTopmostRect = new Rect();
		private bool _IsTopmostGroup = false;
		private Matrix4x4 _SavedGUIMatrix = Matrix4x4.identity;
		private RenderTexture _SavedRenderTexture = null;
		private RenderTexture _RenderTexture = null;
		private Rect _CaptureRect;

#if UNITY_2017_1_OR_NEWER
		private sealed class VisualElementCache
		{
			private VisualElement _Element;
#if UNITY_2019_1_OR_NEWER
			//private VisualElement.ClippingOptions _ClippingOptions;
			private bool _IsLayoutManual;
#elif UNITY_2017_3_OR_NEWER
			private VisualElement.ClippingOptions _ClippingOptions;
#else
			private PositionType _PositionType;
			private Rect _Position;
#endif

#if UNITY_2018_3_OR_NEWER
			private Rect _Layout;
			private Overflow _Overflow;
#endif

			public VisualElementCache(VisualElement element)
			{
				_Element = element;

#if UNITY_2019_1_OR_NEWER
				// _ClippingOptions = element.clippingOptions;
#elif UNITY_2017_3_OR_NEWER
				_ClippingOptions = element.clippingOptions;
#elif UNITY_2017_2_OR_NEWER
				_PositionType = element.style.positionType;
				_Position = element.layout;
#else
				_PositionType = element.positionType;
				_Position = element.position;
#endif

#if UNITY_2019_1_OR_NEWER
				_Overflow = element.style.overflow.value;

				_IsLayoutManual = UIElementsUtility.IsLayoutManual(element);
				if (_IsLayoutManual)
				{
					_Layout = element.layout;
				}
#elif UNITY_2018_3_OR_NEWER
				_Overflow = element.style.overflow;

				if (element.style.positionType == PositionType.Manual)
				{
					_Layout = element.layout;
				}
#endif
			}

			public void Revert()
			{
#if UNITY_2019_1_OR_NEWER
				// _Element.clippingOptions = _ClippingOptions;
#elif UNITY_2017_3_OR_NEWER
				_Element.clippingOptions = _ClippingOptions;
#elif UNITY_2017_2_OR_NEWER
				_Element.style.positionType = _PositionType;
				_Element.layout = _Position;
#else
				_Element.positionType = _PositionType;
				_Element.position = _Position;
#endif

#if UNITY_2018_3_OR_NEWER
				_Element.style.overflow = _Overflow;
#endif

#if UNITY_2019_1_OR_NEWER
				if (_IsLayoutManual)
				{
					UIElementsUtility.SetLayout(_Element, _Layout);
				}
#elif UNITY_2018_3_OR_NEWER
				if (_Element.style.positionType == PositionType.Manual)
				{
					_Element.layout = _Layout;
				}
#endif
			}
		}
		Stack<VisualElementCache> _ElementCache = new Stack<VisualElementCache>();
#endif

#if UNITY_2017_1_OR_NEWER
		private IMGUIContainer _IMGUIContainer = null;
		private IMGUIContainer imguiContainer
		{
			get
			{
				if( _IMGUIContainer == null )
				{
#if UNITY_2019_1_OR_NEWER
					VisualElement rootVisualContainer = _Window.rootVisualElement;
#else
					VisualElement rootVisualContainer = _Window.GetRootVisualContainer();
#endif
					foreach (var child in rootVisualContainer.parent.Children())
					{
						var container = child as IMGUIContainer;
						if (container != null && container.name.Contains("Dockarea"))
						{
							_IMGUIContainer = container;
							break;
						}
					}
				}
				return _IMGUIContainer;
			}
		}
#endif

		public Texture2D captureImage
		{
			get;
			private set;
		}

		public bool isCaptured
		{
			get
			{
				return captureImage != null;
			}
		}

		public Rect position
		{
			get
			{
				return _Position;
			}
		}

		public EditorCapture(EditorWindow window)
		{
			_Window = window;
		}

		public bool Initialize()
		{
			int maxTextureSize = SystemInfo.maxTextureSize;
			Rect position = new Rect(0, 0, maxTextureSize, maxTextureSize);
			return Initialize(position);
		}

		void SaveVisualElementsState(Rect position)
		{
#if UNITY_2017_1_OR_NEWER
			VisualElement current = imguiContainer;
			while (current != null)
			{
				_ElementCache.Push(new VisualElementCache(current));

#if UNITY_2019_1_OR_NEWER
				//current.clippingOptions = VisualElement.ClippingOptions.NoClipping;
#elif UNITY_2017_3_OR_NEWER
				current.clippingOptions = VisualElement.ClippingOptions.NoClipping;
#elif UNITY_2017_2_OR_NEWER
				current.style.positionType = PositionType.Manual;
				current.layout = new Rect(0, 0, Mathf.Max(position.width, current.layout.width), Mathf.Max(position.height, current.layout.height));
#else
				current.positionType = PositionType.Manual;
				current.position = new Rect(0,0,Mathf.Max(position.width, current.position.width), Mathf.Max(position.height, current.position.height) );
#endif

#if UNITY_2018_3_OR_NEWER
				current.style.overflow = Overflow.Visible;
#endif

#if UNITY_2019_1_OR_NEWER
				if (UIElementsUtility.IsLayoutManual(current))
				{
					UIElementsUtility.SetLayout(current,new Rect(Vector2.zero, Vector2.Max(position.size, current.layout.size)));
				}
#elif UNITY_2018_3_OR_NEWER
				if (current.style.positionType == PositionType.Manual)
				{
					current.layout = new Rect(Vector2.zero, Vector2.Max(position.size, current.layout.size));
				}
#endif
				current = current.parent;
			}
#endif
		}

		void RevertVisualElementsState()
		{
#if UNITY_2017_1_OR_NEWER
			while (_ElementCache.Count > 0)
			{
				VisualElementCache cache = _ElementCache.Pop();
				cache.Revert();
			}
#endif
		}

		public bool Initialize(Rect position)
		{
			int maxTextureSize = SystemInfo.maxTextureSize;
			if ((int)position.width <= 0 || position.width > maxTextureSize || (int)position.height <= 0 || position.height > maxTextureSize)
			{
				Debug.LogError("Capture failed : Capture size is too large.");
				return false;
			}

			_Position = position;

			SaveVisualElementsState(position);

			return true;
		}

		public bool BeginCaptureGUI(Rect captureRect)
		{
			if (Event.current.type == EventType.Repaint)
			{
				captureRect.min = Vector2.Max(captureRect.min, _Position.min);
				captureRect.max = Vector2.Min(captureRect.max, _Position.max);

				_CaptureRect = captureRect;
				_CaptureRect.position = Vector2.zero;

				_RenderTexture = RenderTexture.GetTemporary((int)captureRect.width, (int)captureRect.height, 0, RenderTextureFormat.ARGB32);

				if (_RenderTexture != null && _RenderTexture.Create() && _RenderTexture.GetNativeTexturePtr() != System.IntPtr.Zero)
				{
					_SavedRenderTexture = RenderTexture.active;
					RenderTexture.active = _RenderTexture;

					GL.PushMatrix();
					GL.LoadOrtho();
					GL.LoadPixelMatrix(captureRect.xMin, captureRect.xMax, captureRect.yMax, captureRect.yMin);
					GL.Viewport(new Rect(Vector2.zero, captureRect.size));

					GL.Clear(true, true, Color.black);
				}
				else
				{
					Debug.LogError("Screenshot failed.");

					if (_RenderTexture != null)
					{
						RenderTexture.ReleaseTemporary(_RenderTexture);
						_RenderTexture = null;
					}

					return false;
				}
			}

			Rect topmostRect = GUIClip.topmostRect;
			_IsTopmostGroup = (topmostRect != GUIClip.visibleRect);

			if (_IsTopmostGroup)
			{
				_SavedTopmostRect = topmostRect;

				GUI.EndGroup();
			}

			_SavedGUIMatrix = GUI.matrix;

			float scaling = 1f / EditorGUIUtility.pixelsPerPoint;

			GUI.matrix = Matrix4x4.TRS(-captureRect.min * scaling, Quaternion.identity, Vector3.one * scaling);

			return true;
		}

		public bool BeginCaptureGUI()
		{
			return BeginCaptureGUI(_Position);
		}

		public bool EndCaptureGUI()
		{
			bool isCaptured = false;

			GUI.matrix = _SavedGUIMatrix;

			if (Event.current.type == EventType.Repaint)
			{
				GL.PopMatrix();

				int renderWidth = (int)_CaptureRect.width;
				int renderHeight = (int)_CaptureRect.height;

				if (captureImage != null && (captureImage.width != renderWidth || captureImage.height != renderHeight))
				{
					DestroyImage();
				}

				if (captureImage == null)
				{
					captureImage = new Texture2D(renderWidth, renderHeight, TextureFormat.RGB24, false);
					captureImage.hideFlags = HideFlags.HideAndDontSave;
				}

				captureImage.ReadPixels(_CaptureRect, 0, 0, false);
				captureImage.Apply();

				RenderTexture.active = _SavedRenderTexture;
				_SavedRenderTexture = null;

				RenderTexture.ReleaseTemporary(_RenderTexture);
				_RenderTexture = null;

				RevertVisualElementsState();

				isCaptured = true;
			}

			if (_IsTopmostGroup)
			{
				GUI.BeginGroup(_SavedTopmostRect);
			}
			return isCaptured;
		}

		public byte[] EncodeToPNG()
		{
			if (captureImage == null)
			{
				return null;
			}

			return captureImage.EncodeToPNG();
		}

		public bool SaveImage(string path, bool openFolder)
		{
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}

			byte[] bytes = EncodeToPNG();
			if (bytes == null)
			{
				return false;
			}

			System.IO.File.WriteAllBytes(path, bytes);

			if (openFolder)
			{
				EditorUtility.RevealInFinder(path);
			}

			return true;
		}

		public void DestroyImage()
		{
			if (captureImage != null)
			{
				Object.DestroyImmediate(captureImage);
				captureImage = null;
			}
		}

		public void Destroy()
		{
			DestroyImage();
		}
	}
}

#endif