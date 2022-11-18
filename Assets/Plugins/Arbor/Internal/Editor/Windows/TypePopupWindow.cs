//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

using Arbor;
namespace ArborEditor
{
	using UnityObject = UnityEngine.Object;

	public sealed class TypePopupWindow : TreePopupWindow<Type>, ITreeFilter<Type>
	{
		private static readonly int s_TypePopupHash = "ArborEditor.TypePopupWindow.s_TypePopupHash".GetHashCode();

		public const string kNoneText = "None";

		protected override string searchWord
		{
			get
			{
				return ArborEditorCache.typeSearch;
			}
			set
			{
				ArborEditorCache.typeSearch = value;
			}
		}

		protected override bool isReady
		{
			get
			{
				return ClassList.isReady;
			}
		}

		static void Open(Rect buttonRect, int controlID, Type selected, IDefinableType definableType, bool hasFilter, TypeFilterFlags filterFlags)
		{
			TypePopupWindow window = s_Instance as TypePopupWindow;
			if (window == null)
			{
				window = ScriptableObject.CreateInstance<TypePopupWindow>();
				s_Instance = window;
			}
			window.Init(buttonRect, controlID, selected, definableType, hasFilter, filterFlags);
		}

		static int BitCount(int flags)
		{
			var x = flags - ((flags >> 1) & 0x55555555);
			x = ((x >> 2) & 0x33333333) + (x & 0x33333333);
			x = (x >> 4) + x & 0x0f0f0f0f;
			x += x >> 8;
			return (x >> 16) + x & 0xff;
		}

		void Init(Rect buttonRect, int controlID, Type selected, IDefinableType definableType, bool hasFilter, TypeFilterFlags filterFlags)
		{
			_DefinableType = definableType;
			_HasFilter = hasFilter;
			_UseFilter = hasFilter && BitCount((int)filterFlags) > 1;
			if (ArborEditorCache.typeFilterMask != filterFlags)
			{
				ArborEditorCache.typeFilterMask = filterFlags;
				ArborEditorCache.typeFilterFlags = filterFlags;
			}

			Init(buttonRect, controlID, selected);
		}

		public static Type PopupField(Rect position, Type selected)
		{
			int controlID = GUIUtility.GetControlID(s_TypePopupHash, FocusType.Keyboard, position);

			return PopupField(position, controlID, selected, null);
		}

		public static Type PopupField(Rect position, Type selected, GUIContent label, IDefinableType definableType)
		{
			return PopupField(position, selected, label, definableType, false, (TypeFilterFlags)0);
		}

		public static Type PopupField(Rect position, Type selected, GUIContent label, IDefinableType definableType, bool hasFilter, TypeFilterFlags filterFlags)
		{
			int controlID = GUIUtility.GetControlID(s_TypePopupHash, FocusType.Keyboard, position);

			return PopupField(position, controlID, selected, label, definableType, hasFilter, filterFlags);
		}

		public static Type PopupField(Rect position, Type selected, GUIContent label, IDefinableType definableType, bool hasFilter, TypeFilterFlags filterFlags, GUIContent selectedName)
		{
			int controlID = GUIUtility.GetControlID(s_TypePopupHash, FocusType.Keyboard, position);

			return PopupField(position, controlID, selected, label, definableType, hasFilter, filterFlags, selectedName);
		}

		public static Type PopupField(Rect position, int controlID, Type selected, GUIContent label, IDefinableType definableType)
		{
			return PopupField(position, controlID, selected, label, definableType, false, (TypeFilterFlags)0);
		}

		public static Type PopupField(Rect position, int controlID, Type selected, GUIContent label, IDefinableType definableType, bool hasFilter, TypeFilterFlags filterFlags)
		{
			position = EditorGUI.PrefixLabel(position, controlID, label);

			return PopupField(position, controlID, selected, definableType, hasFilter, filterFlags);
		}

		public static Type PopupField(Rect position, int controlID, Type selected, GUIContent label, IDefinableType definableType, bool hasFilter, TypeFilterFlags filterFlags, GUIContent selectedName)
		{
			position = EditorGUI.PrefixLabel(position, controlID, label);

			return PopupField(position, controlID, selected, definableType, hasFilter, filterFlags, selectedName);
		}

		public static Type PopupField(Rect position, int controlID, Type selected, IDefinableType definableType)
		{
			return PopupField(position, controlID, selected, definableType, false, (TypeFilterFlags)0);
		}

		public static Type PopupField(Rect position, int controlID, Type selected, IDefinableType definableType, bool hasFilter, TypeFilterFlags filterFlags)
		{
			return PopupField(position, controlID, selected, definableType, hasFilter, filterFlags, null);
		}

		public static Type PopupField(Rect position, int controlID, Type selected, IDefinableType definableType, bool hasFilter, TypeFilterFlags filterFlags, GUIContent selectedName)
		{
			using (new EditorGUI.DisabledGroupScope(EditorApplication.isCompiling))
			{
				selected = GetSelectedValueForControl(controlID, selected);

				Event current = Event.current;

				EventType eventType = current.GetTypeForControl(controlID);

				selectedName = selectedName ?? new GUIContent(selected != null ? TypeUtility.GetTypeName(selected) : kNoneText);
				GUIStyle style = EditorStyles.popup;

				switch (eventType)
				{
					case EventType.MouseDown:
						if (current.button == 0 && position.Contains(current.mousePosition))
						{
							Rect buttonRect = EditorGUITools.GUIToScreenRect(position);
							Open(buttonRect, controlID, selected, definableType, hasFilter, filterFlags);
							current.Use();
						}
						break;
					case EventType.KeyDown:
						if (current.MainActionKeyForControl(controlID))
						{
							Rect buttonRect = EditorGUITools.GUIToScreenRect(position);
							Open(buttonRect, controlID, selected, definableType, hasFilter, filterFlags);
							current.Use();
						}
						break;
					case EventType.Repaint:
						style.Draw(position, selectedName, controlID);
						break;
				}

				return selected;
			}
		}

		IDefinableType _DefinableType;

		static Dictionary<System.Type, bool> s_HideTypes = new Dictionary<System.Type, bool>();

		static bool IsHideType(System.Type type)
		{
			bool isHideType = false;
			if (s_HideTypes.TryGetValue(type, out isHideType))
			{
				return isHideType;
			}

			for (System.Type current = type; current != null; current = current.BaseType)
			{
				HideTypeAttribute hideType = AttributeHelper.GetAttribute<HideTypeAttribute>(current);
				if (hideType != null)
				{
					if (type == current || hideType.forChildren)
					{
						isHideType = true;
						break;
					}
				}
			}

			s_HideTypes.Add(type, isHideType);

			return isHideType;
		}

		ValueElement CreateTypeTree(int depth, ClassList.TypeItem typeItem, string name)
		{
			using (new ProfilerScope(name))
			{
				Type type = (typeItem != null) ? typeItem.type : null;
				ValueElement typeElement = new ValueElement(depth, name, type, Icons.GetTypeIcon(type));
				if (type != null)
				{
					if (_DefinableType != null)
					{
						typeElement.disable |= !_DefinableType.IsDefinableType(type);
					}

					if (_HasFilter)
					{
						typeElement.disable |= !IsValidType(type, ArborEditorCache.typeFilterMask);
					}
				}

				if (typeItem != null)
				{
					List<int> nestedTypes = typeItem.nestedTypeIndices;
					int nestedTypeCount = nestedTypes.Count;
					for (int nestedTypeIndex = 0; nestedTypeIndex < nestedTypeCount; nestedTypeIndex++)
					{
						ClassList.TypeItem nestedType = ClassList.GetType(nestedTypes[nestedTypeIndex]);

						if (IsHideType(nestedType.type))
						{
							continue;
						}

						ValueElement nestedTypeElement = CreateTypeTree(depth + 1, nestedType, nestedType.name);
						if (nestedTypeElement != null)
						{
							if (nestedTypeElement.foldout)
							{
								typeElement.foldout = true;
							}
							typeElement.AddElement(nestedTypeElement);
						}
					}
				}

				if (_Selected == type)
				{
					_SelectElement = typeElement;
					typeElement.foldout = true;
				}

				if (typeElement.disable && typeElement.elements.Count == 0)
				{
					return null;
				}

				return typeElement;
			}
		}

		protected override void OnCreateTree(Element root)
		{
			_NoneValueElement = CreateTypeTree(root.depth + 1, null, kNoneText);
			root.AddElement(_NoneValueElement);

			int namespaceCount = ClassList.namespaceCount;
			for (int namespaceIndex = 0; namespaceIndex < namespaceCount; namespaceIndex++)
			{
				ClassList.NamespaceItem namespaceItem = ClassList.GetNamespaceItem(namespaceIndex);

				using (new ProfilerScope(namespaceItem.name))
				{
					Element namespaceGroup = new Element(root.depth + 1, namespaceItem.name, Icons.namespaceIcon);

					using (new ProfilerScope("CreateTypeTrees"))
					{
						int typeCount = namespaceItem.typeIndices.Count;
						for (int typeIndex = 0; typeIndex < typeCount; typeIndex++)
						{
							ClassList.TypeItem typeItem = ClassList.GetType(namespaceItem.typeIndices[typeIndex]);

							if (IsHideType(typeItem.type))
							{
								continue;
							}

							ValueElement typeElemnt = CreateTypeTree(namespaceGroup.depth + 1, typeItem, typeItem.name);
							if (typeElemnt != null)
							{
								if (typeElemnt.foldout)
								{
									namespaceGroup.foldout = true;
								}
								namespaceGroup.AddElement(typeElemnt);
							}
						}
					}

					if (namespaceGroup.elements.Count > 0)
					{
						root.AddElement(namespaceGroup);
					}
				}
			}
		}

		private bool _HasFilter;
		private bool _UseFilter;

		bool ITreeFilter<Type>.useFilter
		{
			get
			{
				return _UseFilter;
			}
		}

		bool ITreeFilter<Type>.openFilter
		{
			get
			{
				return ArborEditorCache.typeFilterEnable;
			}
			set
			{
				ArborEditorCache.typeFilterEnable = value;
			}
		}

		private sealed class FilterMenu
		{
			public TypeFilterFlags flags;
			public GUIContent content;
		};

		private static readonly FilterMenu[] s_FilterMenu =
		{
			new FilterMenu()
			{
				flags = TypeFilterFlags.SceneObject,
				content = new GUIContent("Scene"),
			},
			new FilterMenu()
			{
				flags = TypeFilterFlags.AssetObject,
				content = new GUIContent("Asset"),
			},
			new FilterMenu()
			{
				flags = TypeFilterFlags.Class,
				content = new GUIContent("Class"),
			},
			new FilterMenu()
			{
				flags = TypeFilterFlags.Struct,
				content = new GUIContent("Struct"),
			},
			new FilterMenu()
			{
				flags = TypeFilterFlags.Interface,
				content = new GUIContent("Interface"),
			},
			new FilterMenu()
			{
				flags = TypeFilterFlags.Enum,
				content = new GUIContent("Enum"),
			},
			new FilterMenu()
			{
				flags = TypeFilterFlags.Primitive,
				content = new GUIContent("Primitive"),
			},
			new FilterMenu()
			{
				flags = TypeFilterFlags.Delegate,
				content = new GUIContent("Delegate"),
			},
			new FilterMenu()
			{
				flags = TypeFilterFlags.Static,
				content = new GUIContent("Static"),
			},
		};

		void ITreeFilter<Type>.OnFilterSettingsGUI()
		{
			bool changed = false;

			float width = 0;
			Rect lineRect = new Rect();

			int column = 0;

			foreach (var filterMenu in s_FilterMenu)
			{
				TypeFilterFlags flags = filterMenu.flags;
				if ((ArborEditorCache.typeFilterMask & flags) == 0)
				{
					continue;
				}

				bool isFlag = (ArborEditorCache.typeFilterFlags & flags) != 0;

				Vector2 size = EditorStyles.toggle.CalcSize(filterMenu.content);
				if (size.x > width)
				{
					width = position.width;
					lineRect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
					lineRect.xMin += 2f;
					lineRect.xMax -= 2f;

					column = 0;
				}

				Rect toggleRect = lineRect;
				toggleRect.width = size.x;

				size.x += 2f;
				lineRect.xMin += size.x;
				width -= size.x;

				EditorGUI.BeginChangeCheck();
				isFlag = EditorGUI.ToggleLeft(toggleRect, filterMenu.content, isFlag);
				if (EditorGUI.EndChangeCheck())
				{
					if (isFlag)
					{
						ArborEditorCache.typeFilterFlags |= flags;
					}
					else
					{
						ArborEditorCache.typeFilterFlags &= ~flags;
					}
					changed = true;
				}

				column++;
			}

			if (changed)
			{
				RebuildSearch();
			}
		}

		static bool IsValidType(Type value, TypeFilterFlags typeFilterFlags)
		{
			if (value.IsClass && value.IsAbstract && value.IsSealed) // static
			{
				return (typeFilterFlags & TypeFilterFlags.Static) != 0;
			}
			else if (typeof(Component).IsAssignableFrom(value) || typeof(GameObject).IsAssignableFrom(value))
			{
				return (typeFilterFlags & TypeFilterFlags.SceneObject) != 0;
			}
			else if (typeof(UnityObject).IsAssignableFrom(value)) // only Asset Object : IsUnityObject && !IsComponent && !IsGameObject
			{
				return (typeFilterFlags & TypeFilterFlags.AssetObject) != 0;
			}
			else if (value.IsClass) // only normal class : IsClass && !IsUnityObject
			{
				if (typeof(System.Delegate).IsAssignableFrom(value))
				{
					return (typeFilterFlags & TypeFilterFlags.Delegate) != 0;
				}
				return (typeFilterFlags & TypeFilterFlags.Class) != 0;
			}
			else if (value.IsInterface)
			{
				return (typeFilterFlags & TypeFilterFlags.Interface) != 0;
			}
			else if (value.IsEnum)
			{
				return (typeFilterFlags & TypeFilterFlags.Enum) != 0;
			}
			else if (value.IsPrimitive)
			{
				return (typeFilterFlags & TypeFilterFlags.Primitive) != 0;
			}
			else if (value.IsValueType) // only struct : IsValueType && !IsEnum && !IsPrimitite
			{
				return (typeFilterFlags & TypeFilterFlags.Struct) != 0;
			}

			return false;
		}

		bool ITreeFilter<Type>.IsValid(Type value)
		{
			return IsValidType(value, ArborEditorCache.typeFilterFlags);
		}
	}
}
