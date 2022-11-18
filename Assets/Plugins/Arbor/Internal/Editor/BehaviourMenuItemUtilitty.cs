//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using Arbor;

namespace ArborEditor
{
	[InitializeOnLoad]
	internal static class BehaviourMenuItemUtilitty
	{
		internal sealed class Element
		{
			public BehaviourMenuItem menuItem;
			public MethodInfo method;
			public int index;
		}

		static List<Element> _Elements = new List<Element>();

		public static Element[] elements
		{
			get
			{
				return _Elements.ToArray();
			}
		}

		public static bool IsReady()
		{
			return true;
		}

		static BehaviourMenuItemUtilitty()
		{
			Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
			int assemblyCount = assemblies.Length;
			for (int assemblyIndex = 0; assemblyIndex < assemblyCount; assemblyIndex++)
			{
				Assembly assembly = assemblies[assemblyIndex];

				foreach (System.Type type in TypeUtility.GetTypesFromAssembly(assembly))
				{
					MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					int methodCount = methods.Length;
					for (int methodIndex = 0; methodIndex < methodCount; ++methodIndex)
					{
						MethodInfo method = methods[methodIndex];
						foreach (System.Attribute attr in AttributeHelper.GetAttributes(method))
						{
							BehaviourMenuItem menuItem = attr as BehaviourMenuItem;
							if (menuItem != null)
							{
								Element element = new Element();
								element.menuItem = menuItem;
								element.method = method;
								element.index = methodIndex;
								_Elements.Add(element);
							}
						}
					}
				}
			}
		}
	}
}
