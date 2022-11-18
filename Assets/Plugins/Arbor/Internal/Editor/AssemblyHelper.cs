//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ArborEditor
{
	using Arbor;

	public static class AssemblyHelper
	{
		private static Dictionary<string, Type> s_Types = null;

		public static Type GetTypeByName(string name)
		{
			if (s_Types == null)
			{
				s_Types = new Dictionary<string, Type>();

				foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					foreach (Type t in TypeUtility.GetTypesFromAssembly(assembly))
					{
						s_Types[t.FullName] = t;
					}
				}
			}

			Type type = null;
			if (s_Types.TryGetValue(name, out type))
			{
				return type;
			}

			return null;
		}

		private static Assembly TryToLoad(AssemblyName assemblyName)
		{
			try
			{
				return Assembly.Load(assemblyName);
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}