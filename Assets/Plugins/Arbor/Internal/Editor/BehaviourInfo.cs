//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEditor;

using Arbor;

namespace ArborEditor
{
	public sealed class BehaviourInfo
	{
		public string helpUrl;
		public string helpTooltip;
		public GUIContent titleContent;
		public System.ObsoleteAttribute obsolete;

		public void Initialize(System.Type type)
		{
			if (type == null)
			{
				titleContent = EditorGUITools.GetTextContent("Missing");
				return;
			}

			obsolete = AttributeHelper.GetAttribute<System.ObsoleteAttribute>(type);

			SetTitle(type);
			SetHelp(type);
		}

		static string GetObjectTypeName(System.Type type)
		{
			string titleName = type.Name;

			bool useNicifyName = true;

			BehaviourTitle behaviourTitle = AttributeHelper.GetAttribute<BehaviourTitle>(type);
			if (behaviourTitle != null)
			{
				if (behaviourTitle.localization)
				{
					titleName = Localization.GetWord(behaviourTitle.titleName);
				}
				else
				{
					titleName = behaviourTitle.titleName;
				}

				useNicifyName = behaviourTitle.useNicifyName;
			}
			else
			{
				if (type.IsSubclassOf(typeof(Calculator)))
				{
#pragma warning disable 0618
					CalculatorTitle calculatorTitle = AttributeHelper.GetAttribute<CalculatorTitle>(type);
					if (calculatorTitle != null)
					{
						if (behaviourTitle.localization)
						{
							return Localization.GetWord(calculatorTitle.titleName);
						}

						titleName = calculatorTitle.titleName;
					}
#pragma warning restore 0618
				}
				else if (type.IsSubclassOf(typeof(VariableBase)))
				{
					titleName = VariableEditorUtility.GetVariableName(type);
				}
			}

			return useNicifyName ? ObjectNames.NicifyVariableName(titleName) : titleName;
		}

		void SetTitle(System.Type type)
		{
			string title = GetObjectTypeName(type);

			if (obsolete != null)
			{
				title += " (Deprecated)";
			}

			titleContent = EditorGUITools.GetTextContent(title);
		}

		void SetHelp(System.Type type)
		{
			string classTypeName = type.Name;

			string docURL = ArborReferenceUtility.docUrl;

			BehaviourHelp behaviourHelp = AttributeHelper.GetAttribute<BehaviourHelp>(type);
			if (behaviourHelp != null)
			{
				helpUrl = behaviourHelp.url;
				helpTooltip = string.Format("Open Reference for {0}.", classTypeName);
				return;
			}

			if (AttributeHelper.HasAttribute<BuiltInBehaviour>(type))
			{
				helpUrl = PathUtility.Combine(docURL, BuiltinPathUtility.GetBuiltinPath(type), classTypeName.ToLower() + ".html");
				helpTooltip = string.Format("Open Reference for {0}.", classTypeName);
				return;
			}

			if (type.IsSubclassOf(typeof(Calculator)))
			{
#pragma warning disable 0618
				CalculatorHelp calculatorHelp = AttributeHelper.GetAttribute<CalculatorHelp>(type);
				if (calculatorHelp != null)
				{
					helpUrl = calculatorHelp.url;
					helpTooltip = string.Format("Open Reference for {0}.", classTypeName);
					return;
				}

				if (AttributeHelper.HasAttribute<BuiltInCalculator>(type))
				{
					helpUrl = PathUtility.Combine(docURL, BuiltinPathUtility.GetBuiltinPath(type), classTypeName.ToLower() + ".html");
					helpTooltip = string.Format("Open Reference for {0}.", classTypeName);
					return;
				}
#pragma warning restore 0618
			}

			helpUrl = string.Empty;
			helpTooltip = "Open Arbor Document";
		}
	}
}