//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace ArborEditor.Events
{
	using Arbor;
	using Arbor.Events;

	internal sealed class MemberPopupWindow : TreePopupWindow<MemberInfo>, ITreeFilter<MemberInfo>
	{
		private static readonly int s_MethodPopupHash = "s_MethodPopupHash".GetHashCode();

		public const string kNoFunctionText = "No Function";

		private static Dictionary<System.Type, List<MemberInfo>> s_MembersCache = new Dictionary<System.Type, List<MemberInfo>>();

		private System.Type _Type;

		protected override string searchWord
		{
			get
			{
				return ArborEditorCache.methodSearch;
			}
			set
			{
				ArborEditorCache.methodSearch = value;
			}
		}

		public static MemberInfo PopupField(Rect position, int controlID, MemberInfo selected, System.Type type, bool hasFilter, MemberFilterFlags memberFlags, GUIContent selectedName)
		{
			using (new EditorGUI.DisabledGroupScope(EditorApplication.isCompiling))
			{
				selected = GetSelectedValueForControl(controlID, selected);

				Event current = Event.current;

				EventType eventType = current.GetTypeForControl(controlID);

				selectedName = selectedName ?? new GUIContent(selected != null ? ArborEventUtility.GetMemberName(selected) : kNoFunctionText);
				GUIStyle style = EditorStyles.popup;

				switch (eventType)
				{
					case EventType.MouseDown:
						if (current.button == 0 && position.Contains(current.mousePosition))
						{
							Rect buttonRect = EditorGUITools.GUIToScreenRect(position);
							Open(buttonRect, controlID, selected, type, hasFilter, memberFlags);
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

		public static MemberInfo PopupField(Rect position, int controlID, MemberInfo selected, System.Type type, bool hasFilter, MemberFilterFlags memberFlags, GUIContent selectedName, GUIContent label)
		{
			position = EditorGUI.PrefixLabel(position, controlID, label);

			return PopupField(position, controlID, selected, type, hasFilter, memberFlags, selectedName);
		}

		public static MemberInfo PopupField(Rect position, MemberInfo selected, System.Type type, bool hasFilter, MemberFilterFlags memberFlags, GUIContent selectedName)
		{
			int controlID = GUIUtility.GetControlID(s_MethodPopupHash, FocusType.Passive, position);

			return PopupField(position, controlID, selected, type, hasFilter, memberFlags, selectedName, GUIContent.none);
		}

		public static MemberInfo PopupField(Rect position, MemberInfo selected, System.Type type, bool hasFilter, MemberFilterFlags memberFlags, GUIContent label, GUIContent selectedName)
		{
			int controlID = GUIUtility.GetControlID(s_MethodPopupHash, FocusType.Passive, position);

			return PopupField(position, controlID, selected, type, hasFilter, memberFlags, selectedName, label);
		}

		static void Open(Rect buttonRect, int controlID, MemberInfo selected, System.Type type, bool hasFilter, MemberFilterFlags memberFlags)
		{
			MemberPopupWindow window = s_Instance as MemberPopupWindow;
			if (window == null)
			{
				window = ScriptableObject.CreateInstance<MemberPopupWindow>();
				s_Instance = window;
			}
			window.Init(buttonRect, controlID, selected, type, hasFilter, memberFlags);
		}

		static int BitCount(int flags)
		{
			var x = flags - ((flags >> 1) & 0x55555555);
			x = ((x >> 2) & 0x33333333) + (x & 0x33333333);
			x = (x >> 4) + x & 0x0f0f0f0f;
			x += x >> 8;
			return (x >> 16) + x & 0xff;
		}

		void Init(Rect buttonRect, int controlID, MemberInfo selected, System.Type type, bool hasFilter, MemberFilterFlags memberFlags)
		{
			_Type = type;
			_HasFilter = hasFilter;
			_UseFilter = hasFilter && BitCount((int)memberFlags) > 1;
			if (ArborEditorCache.memberFilterMask != memberFlags)
			{
				ArborEditorCache.memberFilterMask = memberFlags;
				ArborEditorCache.memberFilterFlags = memberFlags;
			}

			Init(buttonRect, controlID, selected);
		}

		private sealed class MemberComparer : IComparer<MemberInfo>
		{
			private System.Type _Type;

			private Dictionary<System.Type, int> _Depth = new Dictionary<System.Type, int>();

			public MemberComparer(System.Type type)
			{
				_Type = type;
			}

			int GetBaseDepth(System.Type searchType)
			{
				int depth = 0;

				if (_Depth.TryGetValue(searchType, out depth))
				{
					return depth;
				}

				System.Type baseType = _Type;

				while (baseType != null && searchType != baseType)
				{
					depth++;
					baseType = baseType.BaseType;
				}

				_Depth.Add(searchType, depth);

				return depth;
			}

			static int Order(MemberInfo memberInfo)
			{
				if (memberInfo is MethodInfo)
				{
					return 0;
				}
				else if (memberInfo is FieldInfo)
				{
					return 1;
				}
				else if (memberInfo is PropertyInfo)
				{
					return 2;
				}
				return -1;

			}

			public int Compare(MemberInfo m1, MemberInfo m2)
			{
				System.Type t1 = m1.DeclaringType;
				System.Type t2 = m2.DeclaringType;

				if (t1 == t2)
				{
					int order1 = Order(m1);
					int order2 = Order(m2);
					if (order1 != order2)
					{
						return order1.CompareTo(order2);
					}

					return m1.Name.CompareTo(m2.Name);
				}

				int depth1 = GetBaseDepth(t1);
				int depth2 = GetBaseDepth(t2);

				return depth1.CompareTo(depth2);
			}
		}

		ValueElement CreateMemberTree(int depth, MemberInfo memberInfo, string name)
		{
			using (new ProfilerScope(name))
			{
				if (memberInfo != null && _HasFilter && !IsValidMember(memberInfo, ArborEditorCache.memberFilterMask))
				{
					return null;
				}

				ValueElement valueElement = new ValueElement(depth, name, memberInfo, Icons.GetMemberIcon(memberInfo));

				if (_Selected == memberInfo)
				{
					_SelectElement = valueElement;
					valueElement.foldout = true;
				}

				if (valueElement.disable && valueElement.elements.Count == 0)
				{
					return null;
				}

				return valueElement;
			}
		}

		bool IsSelectableMember(MemberInfo memberInfo)
		{
			MethodInfo methodInfo = memberInfo as MethodInfo;
			if (methodInfo != null)
			{
				return ArborEventUtility.IsSelectableMethod(methodInfo);
			}

			FieldInfo fieldInfo = memberInfo as FieldInfo;
			if (fieldInfo != null)
			{
				return ArborEventUtility.IsSelectableField(fieldInfo);
			}

			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null)
			{
				return ArborEventUtility.IsSelectableProperty(propertyInfo);
			}

			return false;
		}

		protected override void OnCreateTree(Element root)
		{
			_NoneValueElement = CreateMemberTree(root.depth + 1, null, kNoFunctionText);
			root.AddElement(_NoneValueElement);

			List<MemberInfo> members;
			if (!s_MembersCache.TryGetValue(_Type, out members))
			{
				members = _Type.GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).Where(IsSelectableMember).ToList();

				MemberComparer comparer = new MemberComparer(_Type);
				members.Sort(comparer);

				s_MembersCache.Add(_Type, members);
			}

			System.Type currentDeclaringType = null;
			Element groupElement = null;
			bool addedGroup = false;

			bool isStatic = _Type.IsAbstract && _Type.IsSealed;

			int memberCount = members.Count;
			for (int memberIndex = 0; memberIndex < memberCount; memberIndex++)
			{
				MemberInfo memberInfo = members[memberIndex];

				System.Type declaringType = memberInfo.DeclaringType;

				if (isStatic && declaringType != _Type)
				{
					continue;
				}

				if (currentDeclaringType == null || groupElement == null || currentDeclaringType != declaringType)
				{
					currentDeclaringType = declaringType;
					groupElement = new Element(root.depth + 1, TypeUtility.GetTypeName(currentDeclaringType), Icons.GetTypeIcon(currentDeclaringType));
					addedGroup = false;
				}

				ValueElement methodElement = CreateMemberTree(groupElement.depth + 1, memberInfo, ArborEventUtility.GetMemberName(memberInfo));
				if (methodElement != null)
				{
					if (methodElement.foldout)
					{
						groupElement.foldout = true;
					}
					groupElement.AddElement(methodElement);
					if (!addedGroup)
					{
						root.AddElement(groupElement);
						addedGroup = true;
					}
				}
			}
		}

		private bool _HasFilter;
		private bool _UseFilter;

		bool ITreeFilter<MemberInfo>.useFilter
		{
			get
			{
				return _UseFilter;
			}
		}

		bool ITreeFilter<MemberInfo>.openFilter
		{
			get
			{
				return ArborEditorCache.memberFilterEnable;
			}
			set
			{
				ArborEditorCache.memberFilterEnable = value;
			}
		}

		private sealed class FilterMenu
		{
			public MemberFilterFlags flags;
			public GUIContent content;
		};

		private static readonly FilterMenu[] s_FilterMenu =
		{
			new FilterMenu()
			{
				flags = MemberFilterFlags.Method,
				content = new GUIContent("Method"),
			},
			new FilterMenu()
			{
				flags = MemberFilterFlags.Field,
				content = new GUIContent("Field"),
			},
			new FilterMenu()
			{
				flags = MemberFilterFlags.ReadOnlyField,
				content = new GUIContent("ReadOnlyField"),
			},
			new FilterMenu()
			{
				flags = MemberFilterFlags.GetProperty,
				content = new GUIContent("GetProperty"),
			},
			new FilterMenu()
			{
				flags = MemberFilterFlags.SetProperty,
				content = new GUIContent("SetProperty"),
			},
			new FilterMenu()
			{
				flags = MemberFilterFlags.Instance,
				content = new GUIContent("Instance"),
			},
			new FilterMenu()
			{
				flags = MemberFilterFlags.Static,
				content = new GUIContent("Static"),
			},
		};

		void ITreeFilter<MemberInfo>.OnFilterSettingsGUI()
		{
			bool changed = false;

			float width = 0;
			Rect lineRect = new Rect();

			int column = 0;

			foreach (var filterMenu in s_FilterMenu)
			{
				MemberFilterFlags flags = filterMenu.flags;
				if ((ArborEditorCache.memberFilterMask & flags) == 0)
				{
					continue;
				}

				bool isFlag = (ArborEditorCache.memberFilterFlags & flags) != 0;

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
						ArborEditorCache.memberFilterFlags |= flags;
					}
					else
					{
						ArborEditorCache.memberFilterFlags &= ~flags;
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

		static bool IsValidMember(MemberInfo value, MemberFilterFlags memberFilterFlags)
		{
			if (ArborEventUtility.IsStatic(value))
			{
				if ((memberFilterFlags & MemberFilterFlags.Static) == 0)
				{
					return false;
				}
			}
			else if ((memberFilterFlags & MemberFilterFlags.Instance) == 0)
			{
				return false;
			}

			if (value.MemberType == MemberTypes.Method)
			{
				return (memberFilterFlags & MemberFilterFlags.Method) != 0;
			}
			else if (value.MemberType == MemberTypes.Field)
			{
				FieldInfo fieldInfo = value as FieldInfo;
				if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral)
				{
					return (memberFilterFlags & MemberFilterFlags.ReadOnlyField) != 0;
				}

				return (memberFilterFlags & MemberFilterFlags.Field) != 0;
			}
			else if (value.MemberType == MemberTypes.Property)
			{
				PropertyInfo propertyInfo = value as PropertyInfo;
				if (ArborEventUtility.IsGetProperty(propertyInfo))
				{
					if ((memberFilterFlags & MemberFilterFlags.GetProperty) != 0)
					{
						return true;
					}
				}
				if (ArborEventUtility.IsSetProperty(propertyInfo))
				{
					if ((memberFilterFlags & MemberFilterFlags.SetProperty) != 0)
					{
						return true;
					}
				}
			}

			return false;
		}

		bool ITreeFilter<MemberInfo>.IsValid(MemberInfo value)
		{
			return IsValidMember(value, ArborEditorCache.memberFilterFlags);
		}
	}
}
