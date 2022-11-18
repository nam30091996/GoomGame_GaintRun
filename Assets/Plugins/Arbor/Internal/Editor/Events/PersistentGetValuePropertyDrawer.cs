﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ArborEditor.Events
{
	using Arbor;
	using Arbor.Events;

	internal sealed class PersistentGetValueEditor : PropertyEditor
	{
		private const float kSpacing = 5;
		private const int kExtraSpacing = 6;

		private PersistentGetValueProperty _GetValueProperty;

		private LayoutArea _LayoutArea = new LayoutArea();

		protected override void OnInitialize()
		{
			base.OnInitialize();

			_GetValueProperty = new PersistentGetValueProperty(property);
		}

		void DoGUI()
		{
			System.Type targetType = _GetValueProperty.targetTypeProperty.type;
			string targetTypeName = _GetValueProperty.targetTypeProperty.assemblyTypeName.stringValue;
			bool isTargetTypeMissing = targetType == null && !string.IsNullOrEmpty(targetTypeName);

			_LayoutArea.BeginHorizontal();

			EditorGUI.BeginChangeCheck();
			_LayoutArea.PropertyField(_GetValueProperty.targetTypeProperty.property, GUIContent.none, false, LayoutArea.Width(EditorGUIUtility.labelWidth - kSpacing));
			if (EditorGUI.EndChangeCheck())
			{
				System.Type newTargetType = _GetValueProperty.targetTypeProperty.type;
				if (targetType != newTargetType || isTargetTypeMissing)
				{
					targetType = newTargetType;

					_GetValueProperty.ClearType();

					_GetValueProperty.targetMode = PersistentCallProperty.GetTargetMode(targetType);
				}
			}

			_LayoutArea.Space(kSpacing);

			EditorGUI.BeginDisabledGroup(targetType == null);

			MemberInfo memberInfo = _GetValueProperty.memberInfo;

			Rect methodNameRect = _LayoutArea.GetRect(0f, EditorGUIUtility.singleLineHeight);

			if (_LayoutArea.IsDraw(methodNameRect))
			{
				string memberName = _GetValueProperty.memberNameProperty.stringValue;
				bool isEmpty = string.IsNullOrEmpty(memberName);
				bool isMissing = memberInfo == null && !isEmpty;

				string displayMemberName = _GetValueProperty.GetMemberName();

				if (isMissing)
				{
					displayMemberName = string.Format("<Missing {0} >", displayMemberName);
				}
				else if (string.IsNullOrEmpty(displayMemberName))
				{
					displayMemberName = MemberPopupWindow.kNoFunctionText;
				}

				MemberFilterFlags memberFilterFlags = (MemberFilterFlags)(-1) & ~(MemberFilterFlags.SetProperty | MemberFilterFlags.Method); // ignore Method, SetProperty

				EditorGUI.BeginChangeCheck();
				MemberInfo newMemberInfo = MemberPopupWindow.PopupField(methodNameRect, memberInfo, targetType, true, memberFilterFlags, EditorGUITools.GetTextContent(displayMemberName));
				if (EditorGUI.EndChangeCheck())
				{
					if (newMemberInfo != memberInfo || isMissing)
					{
						System.Type instanceType = ArborEventUtility.IsStatic(newMemberInfo) ? null : targetType;
						TargetMode newTargetMode = PersistentCallProperty.GetTargetMode(instanceType);
						if (_GetValueProperty.targetMode != newTargetMode)
						{
							_GetValueProperty.ClearType();
							_GetValueProperty.targetMode = newTargetMode;
						}

						memberInfo = newMemberInfo;
						_GetValueProperty.memberInfo = newMemberInfo;

						_GetValueProperty.property.serializedObject.ApplyModifiedProperties();
						_GetValueProperty.property.serializedObject.Update();
					}
				}
			}

			EditorGUI.EndDisabledGroup();

			_LayoutArea.EndHorizontal();

			System.ObsoleteAttribute obsoleteAttribute = AttributeHelper.GetAttribute<System.ObsoleteAttribute>(memberInfo);
			if (obsoleteAttribute != null)
			{
				string message = string.Format("Obsolete : {0}", obsoleteAttribute.Message);
				MessageType messageType = obsoleteAttribute.IsError ? MessageType.Error : MessageType.Warning;

				_LayoutArea.HelpBox(message, messageType);
			}

			GUIContent targetContent = EditorGUITools.GetTextContent("<Target>");

			switch (_GetValueProperty.targetMode)
			{
				case TargetMode.Component:
					{
						FlexibleComponentProperty targetComponentProperty = _GetValueProperty.targetComponentProperty;
						targetComponentProperty.overrideConstraintType = targetType;

						_LayoutArea.PropertyField(targetComponentProperty.property, targetContent, true);
					}
					break;
				case TargetMode.GameObject:
					{
						FlexibleSceneObjectProperty targetGameObjectProperty = _GetValueProperty.targetGameObjectProperty;

						_LayoutArea.PropertyField(targetGameObjectProperty.property, targetContent, true);
					}
					break;
				case TargetMode.AssetObject:
					{
						FlexibleFieldProperty targetAssetObjectProperty = _GetValueProperty.targetAssetObjectProperty;
						targetAssetObjectProperty.property.SetStateData(targetType);

						_LayoutArea.PropertyField(targetAssetObjectProperty.property, targetContent, true);
					}
					break;
				case TargetMode.Slot:
					{
						InputSlotBaseProperty targetSlotProperty = _GetValueProperty.targetSlotProperty;

						DataSlotField slotField = targetSlotProperty.dataSlotField;
						if (slotField != null)
						{
							slotField.overrideConstraint = new ClassConstraintInfo() { baseType = targetType };
						}

						_LayoutArea.PropertyField(targetSlotProperty.property, targetContent, true);
					}
					break;
				case TargetMode.Static:
					{
						GUIContent staticContent = EditorGUITools.GetTextContent("Static");
						_LayoutArea.LabelField(targetContent, staticContent);
					}
					break;
			}

			if (memberInfo != null)
			{
				_LayoutArea.PropertyField(_GetValueProperty.outputValueProperty.property);
			}

			if (memberInfo != null)
			{
				List<InvalidRepair> invalidRepairs = new List<InvalidRepair>();

				switch (_GetValueProperty.memberType)
				{
					case MemberType.Method:
						{
						}
						break;
					case MemberType.Field:
						{
							FieldInfo fieldInfo = memberInfo as FieldInfo;
							if (fieldInfo == null)
							{
								break;
							}

							System.Type fieldType = fieldInfo.FieldType;

							if (_GetValueProperty.outputValueProperty.type != fieldType)
							{
								invalidRepairs.Add(new OutputValueTypeRepair(_GetValueProperty, fieldType));
							}
						}
						break;
					case MemberType.Property:
						{
							PropertyInfo propertyInfo = memberInfo as PropertyInfo;
							if (propertyInfo == null)
							{
								break;
							}

							System.Type propertyType = propertyInfo.PropertyType;

							if (_GetValueProperty.outputValueProperty.type != propertyType)
							{
								invalidRepairs.Add(new OutputValueTypeRepair(_GetValueProperty, propertyType));
							}
						}
						break;
				}

				if (invalidRepairs.Count > 0)
				{
					StringBuilder invalidMessageBuilder = new StringBuilder();

					foreach (var repair in invalidRepairs)
					{
						if (invalidMessageBuilder.Length > 0)
						{
							invalidMessageBuilder.AppendLine();
						}
						invalidMessageBuilder.Append(repair.GetMessage());
					}

					string invalidMessage = invalidMessageBuilder.ToString();

					_LayoutArea.HelpBox(invalidMessage, MessageType.Error);

					if (_LayoutArea.Button(EditorContents.repair))
					{
						foreach (var repair in invalidRepairs)
						{
							repair.OnRepair();
						}
					}
				}
			}
		}

		protected override void OnGUI(Rect position, GUIContent label)
		{
			bool isLabel = (label != null && label != GUIContent.none);

			int indentLevel = EditorGUI.indentLevel;
			if (isLabel)
			{
				EditorGUI.LabelField(position, label);

				EditorGUI.indentLevel++;

				position.yMin += EditorGUIUtility.singleLineHeight;
			}

			_LayoutArea.Begin(position, false, new RectOffset(0, 0, 0, 2));

			DoGUI();

			_LayoutArea.End();

			if (isLabel)
			{
				EditorGUI.indentLevel = indentLevel;
			}
		}

		protected override float GetHeight(GUIContent label)
		{
			bool isLabel = (label != null && label != GUIContent.none);

			float height = 0f;
			if (isLabel)
			{
				height += EditorGUIUtility.singleLineHeight;
			}

			_LayoutArea.Begin(new Rect(), true, new RectOffset(0, 0, 0, 2));

			DoGUI();

			_LayoutArea.End();

			height += _LayoutArea.rect.height;

			return height;
		}
	}

	[CustomPropertyDrawer(typeof(PersistentGetValue))]
	internal sealed class PersistentGetValuePropertyDrawer : PropertyEditorDrawer<PersistentGetValueEditor>
	{
	}
}