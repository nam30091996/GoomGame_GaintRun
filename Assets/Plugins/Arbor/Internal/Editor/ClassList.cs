//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEditor;
using System;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;

namespace ArborEditor
{
	using Arbor;

	[InitializeOnLoad]
	internal static class ClassList
	{
		private const bool kUseThread = true;

		static HashSet<Assembly> _LoadAssemblies = new HashSet<Assembly>();

		static HashSet<Assembly> _Assemblies = new HashSet<Assembly>();
		static Dictionary<string, NamespaceItem> _NamespaceDic = new Dictionary<string, NamespaceItem>();
		static Dictionary<AssemblyName, Assembly> _AssemblieDic = new Dictionary<AssemblyName, Assembly>();

		static List<NamespaceItem> _Namespaces = new List<NamespaceItem>();
		static List<TypeItem> _TypeItems = new List<TypeItem>();

		static Thread _CreateMethodThread = null;

		enum BuildStatus
		{
			None,
			DelayBuild,
			Building,
			BuildingForce,
			Ready,
			Canceling,
		}

		private static volatile BuildStatus _BuildStatus = BuildStatus.None;

		public static bool isReady
		{
			get
			{
				if (_BuildStatus == BuildStatus.DelayBuild)
				{
					_BuildStatus = BuildStatus.BuildingForce;
					Build();
				}
				return _BuildStatus == BuildStatus.Ready;
			}
		}

		public static int namespaceCount
		{
			get
			{
				return _Namespaces.Count;
			}
		}

		static ClassList()
		{
			_BuildStatus = BuildStatus.DelayBuild;
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			switch (_BuildStatus)
			{
				case BuildStatus.DelayBuild:
					if (!EditorApplication.isPlayingOrWillChangePlaymode)
					{
						_BuildStatus = BuildStatus.Building;
						Build();
					}
					break;
				case BuildStatus.Building:
					if (EditorApplication.isPlayingOrWillChangePlaymode)
					{
						_BuildStatus = BuildStatus.Canceling;
					}
					break;
				case BuildStatus.Ready:
					{
						EditorApplication.update -= OnUpdate;
					}
					break;
			}
		}

		#region Build

		static void Build()
		{
			AddAssembly("System", false);
			AddAssembly("System.Core", false);
			AddAssembly("System.XML", false);
			AddAssembly("System.XML.Linq", false);

			AddAssembly("UnityEngine", true);

			foreach (PluginImporter pluginImporter in PluginImporter.GetAllImporters())
			{
				if (pluginImporter.GetCompatibleWithAnyPlatform() && !pluginImporter.isNativePlugin)
				{
					AddAssembly(pluginImporter.assetPath, false);
				}
			}

#if UNITY_2017_3_OR_NEWER
			AddAssembly("Arbor", true);
			AddAssembly("Arbor.Core", true);
			AddAssembly("Arbor.BuiltInBehaviours", true);
			AddAssembly("Arbor.Examples", true);
#endif

			AddAssembly("Assembly-UnityScript-firstpass", true);
			AddAssembly("Assembly-UnityScript", true);
			AddAssembly("Assembly-Boo-firstpass", true);
			AddAssembly("Assembly-Boo", true);
			AddAssembly("Assembly-CSharp-firstpass", true);
			AddAssembly("Assembly-CSharp", true);

			BeginCreateList();
		}

		static void BeginCreateList()
		{
#pragma warning disable 0162
			if (kUseThread)
			{
				_CreateMethodThread = new Thread(new ThreadStart(CreateMethodList));
				_CreateMethodThread.Start();
			}
			else
			{
				CreateMethodList();
			}
#pragma warning restore 0162
		}

		static Assembly LoadAssembly(string assemblyName)
		{
			try
			{
				return Assembly.Load(assemblyName);
			}
			catch { }

			try
			{
				return Assembly.LoadFile(assemblyName);
			}
			catch { }

			return null;
		}

		static bool ListUpTypes(Assembly assembly)
		{
			if (assembly == null)
			{
				return false;
			}

			if (!_Assemblies.Contains(assembly))
			{
				_Assemblies.Add(assembly);

				foreach (Type type in TypeUtility.GetTypesFromAssembly(assembly))
				{
					if (type.IsVisible && !type.IsSubclassOf(typeof(Attribute)) && !type.IsGenericType && !type.IsNested)
					{
						string Namespace = type.Namespace;
						if (string.IsNullOrEmpty(Namespace))
						{
							Namespace = "<unnamed>";
						}

						NamespaceItem namespaceItem = null;
						if (!_NamespaceDic.TryGetValue(Namespace, out namespaceItem))
						{
							namespaceItem = new NamespaceItem();
							namespaceItem.name = Namespace;

							_NamespaceDic.Add(Namespace, namespaceItem);
							_Namespaces.Add(namespaceItem);
						}

						namespaceItem.typeIndices.Add(AddType(type));
					}

					if (_BuildStatus == BuildStatus.Canceling)
					{
						break;
					}
				}

				return true;
			}

			return false;
		}

		static void AddAssembly(string assemblyName, bool reference)
		{
			if (assemblyName == "UnityEditor")
			{
				return;
			}

			Assembly assembly = LoadAssembly(assemblyName);
			if (assembly != null)
			{
				if (_LoadAssemblies.Add(assembly))
				{
					if (reference)
					{
						foreach (AssemblyName referenceAssemblyName in assembly.GetReferencedAssemblies())
						{
							AddAssembly(referenceAssemblyName.Name, false);
						}
					}
				}
			}
		}

		static void ClearAssemblyHashes()
		{
			_Assemblies.Clear();
			_NamespaceDic.Clear();
			_AssemblieDic.Clear();
		}

		static int SortTypeIndices(int lhs, int rhs)
		{
			TypeItem lhsType = _TypeItems[lhs];
			TypeItem rhsType = _TypeItems[rhs];
			return lhsType.CompareTo(rhsType);
		}

		static void CreateMethodList()
		{
			try
			{
				_Assemblies.Clear();
				_Namespaces.Clear();
				_TypeItems.Clear();

				foreach (Assembly assembly in _LoadAssemblies)
				{
					ListUpTypes(assembly);

					if (_BuildStatus == BuildStatus.Canceling)
					{
						return;
					}
				}

				_Namespaces.Sort();

				foreach (NamespaceItem namespaceItem in _Namespaces)
				{
					namespaceItem.typeIndices.Sort(SortTypeIndices);

					if (_BuildStatus == BuildStatus.Canceling)
					{
						return;
					}
				}

				_BuildStatus = BuildStatus.Ready;
			}
			finally
			{
				ClearAssemblyHashes();

				GC.Collect();

				if (_BuildStatus == BuildStatus.Canceling)
				{
					_BuildStatus = BuildStatus.DelayBuild;
				}
			}
		}

		static int AddType(Type type)
		{
			_TypeItems.Add(new TypeItem(type));
			return _TypeItems.Count - 1;
		}

		public static TypeItem GetType(int index)
		{
			return _TypeItems[index];
		}

		#endregion

		public static NamespaceItem GetNamespaceItem(int index)
		{
			return _Namespaces[index];
		}

		public static TypeItem GetTypeItem(Type type)
		{
			if (type == null)
			{
				return null;
			}

			int namespaceCount = _Namespaces.Count;
			for (int namespaceIndex = 0; namespaceIndex < namespaceCount; namespaceIndex++)
			{
				NamespaceItem namespaceItem = _Namespaces[namespaceIndex];
				int typeCount = namespaceItem.typeIndices.Count;
				for (int typeIndex = 0; typeIndex < typeCount; typeIndex++)
				{
					TypeItem typeItem = GetType(namespaceItem.typeIndices[typeIndex]);
					TypeItem findTypeItem = typeItem.GetTypeItem(type);
					if (findTypeItem != null)
					{
						return findTypeItem;
					}
				}
			}

			return null;
		}

		#region Item classes

		public class Item : System.IComparable
		{
			public string name;

			public int CompareTo(object obj)
			{
				return name.CompareTo((obj as Item).name);
			}
		}

		public sealed class TypeItem : Item
		{
			private RuntimeTypeHandle _TypeHandle;
			public Type type
			{
				get
				{
					return Type.GetTypeFromHandle(_TypeHandle);
				}
			}

			private List<int> _NestedTypeIndices = new List<int>();
			public List<int> nestedTypeIndices
			{
				get
				{
					return _NestedTypeIndices;
				}
			}

			public TypeItem(Type type)
			{
				this.name = Arbor.TypeUtility.GetTypeName(type);
				_TypeHandle = type.TypeHandle;

				CreateNestedTypes(type);
			}

			void CreateNestedTypes(Type type)
			{
				_NestedTypeIndices.Clear();

				Type[] nestedTypes = type.GetNestedTypes(BindingFlags.Public);
				int nestedTypeCount = nestedTypes.Length;
				for (int nestedTypeIndex = 0; nestedTypeIndex < nestedTypeCount; nestedTypeIndex++)
				{
					Type nestedType = nestedTypes[nestedTypeIndex];

					_NestedTypeIndices.Add(AddType(nestedType));

					if (_BuildStatus == BuildStatus.Canceling)
					{
						return;
					}
				}

				_NestedTypeIndices.Sort(SortTypeIndices);
			}

			public TypeItem GetTypeItem(Type type)
			{
				if (type == null)
				{
					return null;
				}

				if (type == this.type)
				{
					return this;
				}

				int nestedTypeCount = _NestedTypeIndices.Count;
				for (int nestedTypeIndex = 0; nestedTypeIndex < nestedTypeCount; nestedTypeIndex++)
				{
					TypeItem nestedType = ClassList.GetType((_NestedTypeIndices[nestedTypeIndex]));
					TypeItem findTypeItem = nestedType.GetTypeItem(type);
					if (findTypeItem != null)
					{
						return findTypeItem;
					}
				}

				return null;
			}
		}

		public sealed class NamespaceItem : Item
		{
			public List<int> typeIndices = new List<int>();
		}

		#endregion
	}
}
