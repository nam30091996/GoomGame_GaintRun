//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ArborEditor
{
	using Arbor;

	public abstract class TreePopupWindow<T> : EditorWindow
	{
		protected static TreePopupWindow<T> s_Instance;

		protected int _ControlID;

		const string kTreePopupWindowChangedMessage = "TreePopupWindowChanged";
		const string kTreePopupSearchControlName = "TreePopupSearchControlName";

		Vector2 _ScrollPos;

		Element _Root;
		Element _FilterRoot;
		Element _SearchRoot;

		private Element currentRoot
		{
			get
			{
				return _SearchRoot ?? _FilterRoot ?? _Root;
			}
		}

		private bool _FocusToSearchBar = false;
		private bool _ScrollToSelected = false;

		protected ValueElement _NoneValueElement = null;

		protected T _Selected;
		protected Element _SelectElement;
		private Element _LastSelectElement;

		protected EditorWindow _SourceView;

		protected abstract string searchWord
		{
			get;
			set;
		}

		private bool hasSearch
		{
			get
			{
				return !string.IsNullOrEmpty(searchWord);
			}
		}

		protected abstract void OnCreateTree(Element root);

		protected void CreateTree()
		{
			_Root = new Element(0, "Root", null);

			OnCreateTree(_Root);

			_IsCreatedTree = true;

			RebuildSearch();

			_FocusToSearchBar = true;
			_ScrollToSelected = true;
		}

		void DrawBackground()
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}

			Rect rect = position;
			rect.position = Vector2.zero;

			Styles.background.Draw(rect, false, false, false, false);
		}

		protected static T GetSelectedValueForControl(int controlID, T selected)
		{
			Event current = Event.current;
			if (current.type == EventType.ExecuteCommand && current.commandName == kTreePopupWindowChangedMessage)
			{
				if (s_Instance != null && s_Instance._ControlID == controlID)
				{
					selected = s_Instance._Selected;
					GUI.changed = true;

					current.Use();
				}
			}
			return selected;
		}

		const float k_BaseIndent = 2f;
		const float k_IndentWidth = 14f;
		const float k_FoldoutWidth = 12f;
		const float k_IconWidth = 16f;

		float indentWidth
		{
			get
			{
				return k_IndentWidth;
			}
		}

		float GetFoldoutIndent(Element element)
		{
			return k_BaseIndent + (element.depth - 1) * indentWidth;
		}

		float GetContentIndent(Element element)
		{
			return GetFoldoutIndent(element) + k_FoldoutWidth;
		}

		bool SubmitElement(Element element)
		{
			SearchItemElement searchItemElement = element as SearchItemElement;
			if (searchItemElement != null)
			{
				if (searchItemElement.disable)
				{
					return false;
				}

				element = searchItemElement.original;
			}

			ValueElement valueElement = element as ValueElement;
			if (valueElement != null && !valueElement.disable)
			{
				_Selected = valueElement.value;
				_SourceView.SendEvent(EditorGUIUtility.CommandEvent(kTreePopupWindowChangedMessage));

				return true;
			}

			return false;
		}

		void DoElementGUI(Rect position, Element element)
		{
			Vector2 iconSize = EditorGUIUtility.GetIconSize();
			EditorGUIUtility.SetIconSize(new Vector2(k_IconWidth, k_IconWidth));

			float foldoutIndent = GetFoldoutIndent(element);

			Event current = Event.current;

			Rect foldoutPosition = position;
			foldoutPosition.x = foldoutIndent;
			foldoutPosition.width = k_FoldoutWidth;

			Rect labelPosition = position;
			labelPosition.x = foldoutPosition.xMax;

			if (current.type != EventType.Layout)
			{
				element.position = position;
			}

			switch (current.type)
			{
				case EventType.MouseDown:
					if (labelPosition.Contains(current.mousePosition))
					{
						_SelectElement = element;
						SubmitElement(element);

						current.Use();
						EditorGUIUtility.ExitGUI();
					}
					break;
				case EventType.Repaint:
					{
						EditorGUI.BeginDisabledGroup(element.disable);

						Rect rect = position;

						bool selected = _SelectElement == element;
						bool focus = true;
						if (!selected)
						{
							Element currentElement = element;
							SearchItemElement searchItemElement = currentElement as SearchItemElement;
							if (searchItemElement != null)
							{
								currentElement = searchItemElement.original;
							}

							ValueElement valueElement = currentElement as ValueElement;
							if (valueElement != null && EqualityComparer<T>.Default.Equals(valueElement.value, _Selected))
							{
								selected = true;
								focus = false;
							}
						}

						if (selected)
						{
							Styles.treeStyle.Draw(rect, GUIContent.none, false, false, selected, focus);
						}

						float contentIndent = GetContentIndent(element);
						rect.x += contentIndent;
						rect.width -= contentIndent;

						Styles.treeStyle.Draw(rect, element.content, false, false, selected, focus);

						EditorGUI.EndDisabledGroup();
					}
					break;
			}

			if (element.elements.Count > 0)
			{
				EditorGUI.BeginChangeCheck();
				element.foldout = GUI.Toggle(foldoutPosition, element.foldout, GUIContent.none, Styles.groupFoldout);
				if (EditorGUI.EndChangeCheck())
				{
					_IsDirtyTreeView = true;
				}
			}

			EditorGUIUtility.SetIconSize(iconSize);
		}

		string[] _SearchWords;

		bool SearchElement(Element searchGroup, Element group, int searchIndex)
		{
			using (new ProfilerScope(group.name))
			{
				int elementCount = group.elements.Count;
				for (int elementIndex = 0; elementIndex < elementCount; elementIndex++)
				{
					Element element = group.elements[elementIndex];

					Element originalElement = element;
					SearchItemElement searchElement = element as SearchItemElement;
					if (searchElement != null)
					{
						originalElement = searchElement.original;
					}

					string searchName = element.searchName;

					if (element.elements.Count > 0)
					{
						int localSearchIndex = searchIndex;
						if (_SearchWords.Length > searchIndex)
						{
							string word = _SearchWords[searchIndex];
							if (searchName.Contains(word))
							{
								localSearchIndex++;
							}
						}

						SearchItemElement newElement = new SearchItemElement(originalElement);
						if (SearchElement(newElement, element, localSearchIndex))
						{
							newElement.foldout = true;
							searchGroup.AddElement(newElement);

							if (originalElement == _LastSelectElement)
							{
								_SelectElement = newElement;
							}
						}
					}
					else
					{
						bool flag = true;

						if (element != _NoneValueElement)
						{
							int wordCount = _SearchWords.Length;
							for (int wordIndex = searchIndex; wordIndex < wordCount; wordIndex++)
							{
								if (!searchName.Contains(_SearchWords[wordIndex]))
								{
									flag = false;
									break;
								}
							}
						}

						if (flag)
						{
							SearchItemElement newElement = new SearchItemElement(originalElement);
							searchGroup.AddElement(newElement);

							if (originalElement == _LastSelectElement)
							{
								_SelectElement = newElement;
							}
						}
					}
				}

				return searchGroup.elements.Count > 0;
			}
		}

		bool IsFilterValid(ValueElement valueElement)
		{
			if (valueElement == null || valueElement == _NoneValueElement)
			{
				return true;
			}

			ITreeFilter<T> filter = this as ITreeFilter<T>;
			return filter.IsValid(valueElement.value);
		}

		bool FilterElement(Element filterGroup, Element group)
		{
			using (new ProfilerScope(group.name))
			{
				int elementCount = group.elements.Count;
				for (int elementIndex = 0; elementIndex < elementCount; elementIndex++)
				{
					Element element = group.elements[elementIndex];

					Element originalElement = element;
					SearchItemElement searchElement = element as SearchItemElement;
					if (searchElement != null)
					{
						originalElement = searchElement.original;
					}

					if (element.elements.Count > 0)
					{
						SearchItemElement newElement = new SearchItemElement(originalElement);
						if (FilterElement(newElement, element))
						{
							newElement.foldout = true;
							newElement.disable = newElement.disable || !IsFilterValid(originalElement as ValueElement);
							filterGroup.AddElement(newElement);

							if (originalElement == _LastSelectElement)
							{
								_SelectElement = newElement;
							}
						}
					}
					else
					{
						if (IsFilterValid(originalElement as ValueElement))
						{
							SearchItemElement newElement = new SearchItemElement(originalElement);
							filterGroup.AddElement(newElement);

							if (originalElement == _LastSelectElement)
							{
								_SelectElement = newElement;
							}
						}
					}
				}

				return filterGroup.elements.Count > 0;
			}
		}

		protected virtual bool isReady
		{
			get
			{
				return true;
			}
		}

		private bool _IsCreatedTree;

		protected void Init(Rect buttonRect, int controlID, T selected)
		{
			_ControlID = controlID;
			_Selected = selected;
			_SourceView = EditorWindow.focusedWindow;

			if (isReady)
			{
				CreateTree();
			}
			else
			{
				_IsCreatedTree = false;
			}

			Vector2 center = buttonRect.center;
			buttonRect.width = 300f;
			buttonRect.center = center;
			ShowAsDropDown(buttonRect, new Vector2(300f, 320f));
		}

		private bool _IsDirtyTreeView = false;

		protected void RebuildSearch()
		{
			if (!_IsCreatedTree)
			{
				return;
			}

			ITreeFilter<T> filter = this as ITreeFilter<T>;
			if (filter != null && filter.useFilter && filter.openFilter)
			{
				SearchItemElement searchItemElement = _SelectElement as SearchItemElement;
				if (searchItemElement != null)
				{
					_LastSelectElement = searchItemElement.original;
				}
				else if (_SelectElement != null)
				{
					_LastSelectElement = _SelectElement;
				}

				_SelectElement = null;

				_FilterRoot = new Element(0, "Search", null);
				FilterElement(_FilterRoot, _Root);

				if (_SelectElement != null)
				{
					_ScrollToSelected = true;
				}
			}
			else
			{
				if (_FilterRoot != null)
				{
					_FilterRoot = null;
					SearchItemElement searchItemElement = _SelectElement as SearchItemElement;
					if (searchItemElement != null)
					{
						_SelectElement = searchItemElement.original;
					}
					else
					{
						_SelectElement = _LastSelectElement;
					}
				}
			}

			if (!hasSearch)
			{
				if (_SearchRoot != null)
				{
					_SearchRoot = null;

					SearchItemElement searchItemElement = _SelectElement as SearchItemElement;
					if (searchItemElement != null)
					{
						_SelectElement = searchItemElement.original;
					}
					else
					{
						_SelectElement = _LastSelectElement;
					}
				}
			}
			else
			{
				using (new ProfilerScope("RebuildSearch"))
				{
					string str1 = searchWord.ToLower();
					_SearchWords = str1.Split(new char[] { ' ' });

					SearchItemElement searchItemElement = _SelectElement as SearchItemElement;
					if (searchItemElement != null)
					{
						_LastSelectElement = searchItemElement.original;
					}
					else if (_SelectElement != null)
					{
						_LastSelectElement = _SelectElement;
					}

					_SelectElement = null;

					_SearchRoot = new Element(0, "Search", null);
					SearchElement(_SearchRoot, _FilterRoot ?? _Root, 0);

					if (_SelectElement != null)
					{
						_ScrollToSelected = true;
					}
				}
			}

			_IsDirtyTreeView = true;
		}

		void SearchGUI()
		{
			ITreeFilter<T> filterSettings = this as ITreeFilter<T>;

			EditorGUILayout.BeginHorizontal(Styles.popupWindowToolbar);

			GUI.SetNextControlName(kTreePopupSearchControlName);
			EditorGUI.BeginChangeCheck();
			int i = 0;
			string str = EditorGUITools.ToolbarSearchField(searchWord, null, ref i);
			if (EditorGUI.EndChangeCheck())
			{
				if (str != searchWord)
				{
					searchWord = str;
					RebuildSearch();
				}
			}

			if (filterSettings != null && filterSettings.useFilter)
			{
				GUILayout.Space(2f);

				GUIStyle style = EditorStyles.toolbarButton;
				GUIContent content = EditorContents.filterIcon;

				Color contentColor = GUI.contentColor;
				GUI.contentColor = ArborEditorWindow.isDarkSkin ? Color.white : Color.black;

				EditorGUI.BeginChangeCheck();
				filterSettings.openFilter = GUILayout.Toggle(filterSettings.openFilter, content, style);
				if (EditorGUI.EndChangeCheck())
				{
					RebuildSearch();
				}

				GUI.contentColor = contentColor;
			}

			EditorGUILayout.EndHorizontal();

			if (filterSettings != null && filterSettings.useFilter && filterSettings.openFilter)
			{
				GUILayout.Space(3f);

				filterSettings.OnFilterSettingsGUI();

				GUILayout.Space(3f);

				EditorGUITools.DrawSeparator(ArborEditorWindow.isDarkSkin);
			}
		}

		private List<Element> _ViewElements = new List<Element>();

		void ListupTreeView(Element group)
		{
			int elementCount = group.elements.Count;
			for (int elementIndex = 0; elementIndex < elementCount; elementIndex++)
			{
				Element element = group.elements[elementIndex];

				_ViewElements.Add(element);

				if (element.elements.Count > 0 && element.foldout)
				{
					ListupTreeView(element);
				}
			}
		}

		int _ViewCount;

		void DoTreeViewGUI()
		{
			Event currentEvent = Event.current;
			if (currentEvent.type == EventType.Layout && _IsDirtyTreeView)
			{
				_ViewElements.Clear();

				ListupTreeView(currentRoot);

				_IsDirtyTreeView = false;

				_ViewCount = _ViewElements.Count;
			}

			_ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos);

			float lineHeight = EditorGUIUtility.singleLineHeight;

			int listCount = _ViewElements.Count;

			Rect totalPosition = GUILayoutUtility.GetRect(0.0f, listCount * lineHeight);

			int startIndex = Mathf.FloorToInt(_ScrollPos.y / lineHeight);
			int endIndex = Mathf.Min(startIndex + _ViewCount, listCount);

			Rect elementPosition = totalPosition;
			elementPosition.height = lineHeight;
			elementPosition.y += startIndex * lineHeight;

			for (int i = startIndex; i < endIndex; i++)
			{
				Element element = _ViewElements[i];

				DoElementGUI(elementPosition, element);

				elementPosition.y += elementPosition.height;
			}

			EditorGUILayout.EndScrollView();

			EventType eventType = currentEvent.type;
			if (eventType != EventType.Layout && eventType != EventType.Used)
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();

				_ViewCount = Mathf.FloorToInt(lastRect.height / lineHeight) + 1;
			}
		}

		protected void DoTreeGUI()
		{
			SearchGUI();

			DoTreeViewGUI();

			if (_FocusToSearchBar)
			{
				_FocusToSearchBar = false;
				EditorGUI.FocusTextInControl(kTreePopupSearchControlName);
			}

			if (!_ScrollToSelected || _SelectElement == null || Event.current.type != EventType.Repaint)
			{
				return;
			}

			_ScrollToSelected = false;

			Rect lastRect = GUILayoutUtility.GetLastRect();
			Rect selectRect = _SelectElement.position;
			if (selectRect.yMax - lastRect.height > _ScrollPos.y)
			{
				_ScrollPos.y = selectRect.yMax - lastRect.height;
				Repaint();
			}
			if (selectRect.y >= _ScrollPos.y)
			{
				return;
			}
			_ScrollPos.y = selectRect.y;
			Repaint();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnEnable()
		{
			wantsMouseMove = true;
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnDisable()
		{
			_ControlID = 0;
			s_Instance = null;
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnInspectorUpdate()
		{
			if (EditorApplication.isCompiling)
			{
				Close();
			}
			else if (!_IsCreatedTree)
			{
				if (ClassList.isReady)
				{
					CreateTree();
				}

				Repaint();
			}
		}

		void SelectElement(Element element)
		{
			_SelectElement = element;
			_ScrollToSelected = true;
		}

		void HandleKeyboard()
		{
			Event current = Event.current;

			if (current.type == EventType.MouseDown)
			{
				GUIUtility.keyboardControl = 0;
			}

			if (current.type != EventType.KeyDown)
			{
				return;
			}

			if (current.keyCode == KeyCode.DownArrow)
			{
				if (_SelectElement != null)
				{
					if (_SelectElement.elements.Count > 0 && _SelectElement.foldout)
					{
						SelectElement(_SelectElement.elements[0]);
					}
					else
					{
						Element currentElement = _SelectElement;
						Element parent = _SelectElement.parent;
						while (parent != null)
						{
							int index = parent.elements.IndexOf(currentElement) + 1;

							if (index < parent.elements.Count)
							{
								SelectElement(parent.elements[index]);
								break;
							}
							else
							{
								currentElement = parent;
								parent = parent.parent;
							}
						}
					}
				}
				else
				{
					Element root = currentRoot;
					if (root != null && root.elements.Count > 0)
					{
						SelectElement(root.elements[0]);
					}
				}
				current.Use();
			}
			if (current.keyCode == KeyCode.UpArrow)
			{
				if (_SelectElement != null)
				{
					Element parent = _SelectElement.parent;
					if (parent != null)
					{
						int index = parent.elements.IndexOf(_SelectElement) - 1;

						if (index >= 0)
						{
							Element element = parent.elements[index];
							if (element.elements.Count > 0 && element.foldout)
							{
								SelectElement(element.elements[element.elements.Count - 1]);
							}
							else
							{
								SelectElement(parent.elements[index]);
							}
						}
						else
						{
							if (parent.parent != null)
							{
								SelectElement(parent);
							}
						}
					}
				}
				else
				{
					Element root = currentRoot;
					if (root != null && root.elements.Count > 0)
					{
						SelectElement(root.elements[0]);
					}
				}
				current.Use();
			}
			if (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter)
			{
				if (_SelectElement != null)
				{
					SubmitElement(_SelectElement);
				}
				current.Use();
			}

			if (hasSearch && GUI.GetNameOfFocusedControl() == kTreePopupSearchControlName && EditorGUIUtility.editingTextField)
			{
				return;
			}

			if (current.keyCode == KeyCode.LeftArrow)
			{
				if (_SelectElement != null)
				{
					if (_SelectElement.elements.Count > 0 && _SelectElement.foldout)
					{
						_SelectElement.foldout = false;
					}
					else
					{
						Element parent = _SelectElement.parent;
						if (parent != null)
						{
							if (parent.parent != null)
							{
								SelectElement(parent);
							}
							else
							{
								int index = parent.elements.IndexOf(_SelectElement) - 1;
								if (index >= 0)
								{
									SelectElement(parent.elements[index]);
								}
							}
						}
					}
				}
				current.Use();
			}

			if (current.keyCode == KeyCode.RightArrow)
			{
				if (_SelectElement != null)
				{
					if (_SelectElement.elements.Count > 0 && !_SelectElement.foldout)
					{
						_SelectElement.foldout = true;
					}
					else
					{
						bool find = false;

						for (int i = 0, count = _SelectElement.elements.Count; i < count; ++i)
						{
							Element element = _SelectElement.elements[i];
							if (element.elements.Count > 0)
							{
								SelectElement(element);
								find = true;
								break;
							}
						}

						if (!find)
						{
							Element currentElement = _SelectElement;
							while (currentElement != null && currentElement.parent != null)
							{
								Element parent = currentElement.parent;
								int index = parent.elements.IndexOf(currentElement) + 1;
								for (int i = index, count = parent.elements.Count; i < count; ++i)
								{
									Element element = parent.elements[i];
									if (element.elements.Count > 0)
									{
										SelectElement(element);
										_ScrollToSelected = true;
										find = true;
										break;
									}
								}

								if (find)
								{
									break;
								}

								currentElement = parent;
							}
						}
					}
				}
				current.Use();
			}

			if (current.keyCode != KeyCode.Escape)
			{
				return;
			}

			Close();
			current.Use();
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		void OnGUI()
		{
			DrawBackground();

			if (isReady && _IsCreatedTree)
			{
				HandleKeyboard();
				DoTreeGUI();
			}
			else
			{
				Rect rect = position;
				rect.x = rect.y = 0;
				EditorGUITools.DrawIndicator(rect, Localization.GetWord("Loading"));
				//EditorGUILayout.HelpBox(Localization.GetWord("Loading"), MessageType.Info, true);
			}
		}

		protected class Element : System.IComparable
		{
			public Element parent = null;

			public int depth;
			public GUIContent content;
			public bool disable = false;

			public bool foldout = false;
			public List<Element> elements = new List<Element>();

			public Rect position;

			public string name
			{
				get
				{
					return content.text;
				}
			}

			public string searchName;

			public Element(int depth, string name, Texture icon)
			{
				this.depth = depth;
				this.content = new GUIContent(name, icon);
				this.searchName = name.ToLower().Replace(" ", string.Empty);
			}

			public Element(int depth, GUIContent content)
			{
				this.depth = depth;
				this.content = content;
				this.searchName = name.ToLower().Replace(" ", string.Empty);
			}

			public int CompareTo(object obj)
			{
				return name.CompareTo((obj as Element).name);
			}

			public void AddElement(Element element)
			{
				element.parent = this;
				elements.Add(element);
			}
		}

		protected class ValueElement : Element
		{
			public T value;

			public ValueElement(int depth, string name, T value, Texture icon) : base(depth, name, icon)
			{
				this.value = value;
			}

			public ValueElement(int depth, GUIContent content, T value) : base(depth, content)
			{
				this.value = value;
			}
		}

		protected class SearchItemElement : Element
		{
			public Element original;

			public SearchItemElement(Element original) : base(original.depth, original.content)
			{
				this.original = original;
				this.disable = original.disable;
			}

			public SearchItemElement(int depth, string name, T value, Texture icon) : base(depth, name, icon)
			{
			}

			public SearchItemElement(int depth, GUIContent content) : base(depth, content)
			{
			}
		}
	}
}