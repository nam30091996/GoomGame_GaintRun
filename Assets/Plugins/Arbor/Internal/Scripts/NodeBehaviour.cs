//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// ArborFSMの各種ノードに割り当てるスクリプトの基本クラス。
	/// </summary>
#else
	/// <summary>
	/// Base class for scripts to be assigned to various nodes of ArborFSM.
	/// </summary>
#endif
	[AddComponentMenu("")]
	[HideType(true)]
	public class NodeBehaviour : MonoBehaviour, ISerializationCallbackReceiver
	{
#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("_StateMachine")]
		private NodeGraph _NodeGraph;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
		[FormerlySerializedAs("_StateID")]
		[FormerlySerializedAs("_CalculatorID")]
		[HideInInspector]
		private int _NodeID;

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[SerializeField]
#if !ARBOR_DEBUG
		[HideInInspector]
#endif
		private List<DataLinkFieldInfo> _DataSlotFieldLinks = new List<DataLinkFieldInfo>();

		private List<DataSlotField> _DataSlotFields = new List<DataSlotField>();

		private static Node s_CreatingNode;

#if ARBOR_DOC_JA
		/// <summary>
		/// NodeGraphを取得。
		/// </summary>
#else
		/// <summary>
		/// Gets the NodeGraph.
		/// </summary>
#endif
		public NodeGraph nodeGraph
		{
			get
			{
				if (_NodeGraph == null)
				{
					if (gameObject != null)
					{
						NodeGraph[] nodeGraphs = gameObject.GetComponents<NodeGraph>();
						foreach (NodeGraph nodeGraph in nodeGraphs)
						{
							Node c = nodeGraph.FindNodeContainsBehaviour(this);
							if (c != null)
							{
								_NodeGraph = nodeGraph;
								_NodeID = c.nodeID;
								break;
							}
						}
					}
				}
				if (_NodeGraph == null && s_CreatingNode != null)
				{
					_NodeGraph = s_CreatingNode.nodeGraph;
					_NodeID = s_CreatingNode.nodeID;
				}
				return _NodeGraph;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// ノードIDを取得。
		/// </summary>
#else
		/// <summary>
		/// Gets the node identifier.
		/// </summary>
#endif
		public int nodeID
		{
			get
			{
				if (_NodeID == 0 && s_CreatingNode != null)
				{
					_NodeID = s_CreatingNode.nodeID;
				}
				return _NodeID;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Nodeを取得。
		/// </summary>
#else
		/// <summary>
		/// Get the Node.
		/// </summary>
#endif
		public Node node
		{
			get
			{
				Node node = null;

				NodeGraph graph = nodeGraph;
				if (graph != null)
				{
					node = graph.GetNodeFromID(nodeID);
				}

				if (node == null && s_CreatingNode != null)
				{
					node = s_CreatingNode;
				}

				return node;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// MonoBehaviour.OnValidate を参照してください
		/// </summary>
#else
		/// <summary>
		/// See MonoBehaviour.OnValidate.
		/// </summary>
#endif
		protected virtual void OnValidate()
		{
			if (nodeGraph != null)
			{
				nodeGraph.DelayRefresh();
			}
		}

		void Initialize(Node node, bool duplicate)
		{
#if !ARBOR_DEBUG
			hideFlags |= HideFlags.HideInInspector | HideFlags.HideInHierarchy;
#endif

			Initialize(node.nodeGraph, node.nodeID);

			RebuildFields();

			if (!duplicate)
			{
				OnCreated();
			}

			OnInitializeEnabled();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 内部用。
		/// </summary>
#else
		/// <summary>
		/// For internal.
		/// </summary>
#endif
		public void RebuildFields()
		{
			RebuildFlexibleSceneObjects();

			RebuildDataSlotFields();

			OnRebuildFields();
		}

		internal virtual void OnRebuildFields()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Editor用。
		/// </summary>
#else
		/// <summary>
		/// For Editor.
		/// </summary>
#endif
		public static NodeBehaviour CreateNodeBehaviour(Node node, System.Type type, bool duplicate = false)
		{
			System.Type classType = typeof(NodeBehaviour);
			if (type != classType && !TypeUtility.IsSubclassOf(type, classType))
			{
				throw new System.ArgumentException("The type `" + type.Name + "' must be convertible to `NodeBehaviour' in order to use it as parameter `type'", "type");
			}

			s_CreatingNode = node;

			NodeBehaviour nodeBehaviour = ComponentUtility.AddComponent(node.nodeGraph.gameObject, type) as NodeBehaviour;

			ComponentUtility.RecordObject(nodeBehaviour, "Add " + type.Name);

			nodeBehaviour.Initialize(node, duplicate);

			ComponentUtility.SetDirty(nodeBehaviour);

			s_CreatingNode = null;

			return nodeBehaviour;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Editor用。
		/// </summary>
#else
		/// <summary>
		/// For Editor.
		/// </summary>
#endif
		public static Type CreateNodeBehaviour<Type>(Node node, bool duplicate = false) where Type : NodeBehaviour
		{
			s_CreatingNode = node;

			Type nodeBehaviour = ComponentUtility.AddComponent<Type>(node.nodeGraph.gameObject);

			ComponentUtility.RecordObject(nodeBehaviour, "Add " + nodeBehaviour.GetType().Name);

			nodeBehaviour.Initialize(node, duplicate);

			ComponentUtility.SetDirty(nodeBehaviour);

			s_CreatingNode = null;

			return nodeBehaviour;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// NodeBehaviourを破棄する。
		/// </summary>
		/// <param name="behaviour">NodeBehaviour</param>
#else
		/// <summary>
		/// Destroy NodeBehaviour.
		/// </summary>
		/// <param name="behaviour">NodeBehaviour</param>
#endif
		public static void Destroy(NodeBehaviour behaviour)
		{
			if (behaviour != null)
			{
				behaviour.OnPreDestroy();
			}
			ComponentUtility.Destroy(behaviour);
		}

		internal virtual void OnBeforeSerializeInternal()
		{
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			OnBeforeSerializeInternal();

			INodeBehaviourSerializationCallbackReceiver receiver = this as INodeBehaviourSerializationCallbackReceiver;
			if (receiver != null)
			{
				receiver.OnBeforeSerialize();
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			INodeBehaviourSerializationCallbackReceiver receiver = this as INodeBehaviourSerializationCallbackReceiver;
			if (receiver != null)
			{
				receiver.OnAfterDeserialize();
			}

			RebuildFields();
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataSlotFieldの個数
		/// </summary>
#else
		/// <summary>
		/// Number of DataSlotField
		/// </summary>
#endif
		public int dataSlotFieldCount
		{
			get
			{
				return _DataSlotFields.Count;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataSlotFieldの個数
		/// </summary>
#else
		/// <summary>
		/// Number of DataSlotField
		/// </summary>
#endif
		[System.Obsolete("use dataSlotFieldCount")]
		public int calculatorSlotFieldCount
		{
			get
			{
				return dataSlotFieldCount;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataSlotFieldを取得する。
		/// </summary>
		/// <param name="index">インデックス</param>
		/// <returns>DataSlotField</returns>
#else
		/// <summary>
		/// Get DataSlotField.
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>DataSlotField</returns>
#endif
		public DataSlotField GetDataSlotField(int index)
		{
			DataSlotField slotField = _DataSlotFields[index];
			if (slotField == null)
			{
				Debug.LogError(this.GetType() + " : GetDataSlotField(" + index + ") == null");
			}
			return slotField;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataSlotFieldを取得する。
		/// </summary>
		/// <param name="index">インデックス</param>
		/// <returns>DataSlotField</returns>
#else
		/// <summary>
		/// Get DataSlotField.
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>DataSlotField</returns>
#endif
		[System.Obsolete("use GetDataSlotField")]
		public DataSlotField GetCalculatorSlotField(int index)
		{
			return GetDataSlotField(index);
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataSlotFieldを取得する。
		/// </summary>
		/// <param name="slot">DataSlot</param>
		/// <param name="rebuild">見つからない場合に再構築する。</param>
		/// <returns>DataSlotField</returns>
#else
		/// <summary>
		/// Get DataSlotField.
		/// </summary>
		/// <param name="slot">DataSlot</param>
		/// <param name="rebuild">Rebuild if not found.</param>
		/// <returns>DataSlotField</returns>
#endif
		public DataSlotField GetDataSlotField(DataSlot slot, bool rebuild = false)
		{
			int count = _DataSlotFields.Count;
			for (int i = 0; i < count; i++)
			{
				DataSlotField slotField = _DataSlotFields[i];
				if (slotField != null && object.ReferenceEquals(slotField.slot, slot))
				{
					return slotField;
				}
			}

			if (rebuild)
			{
				Debug.LogWarning("GetDataSlotField: DataSlotField was not found.\nIf you change the number of DataSlot fields dynamically from the script, it is recommended to invoke NodeBehaviour.RebuildDataSlotFields() immediately after the change.");
				RebuildDataSlotFields();
				return GetDataSlotField(slot, false);
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataSlotFieldを取得する。
		/// </summary>
		/// <param name="slot">DataSlot</param>
		/// <returns>DataSlotField</returns>
#else
		/// <summary>
		/// Get DataSlotField.
		/// </summary>
		/// <param name="slot">DataSlot</param>
		/// <returns>DataSlotField</returns>
#endif
		[System.Obsolete("use GetDataSlotField(DataSlot)")]
		public DataSlotField GetCalculatorSlotField(DataSlot slot)
		{
			return GetDataSlotField(slot);
		}

		private sealed class FieldCache
		{
			public DynamicReflection.DynamicField field;
			public DataLinkAttribute attribute;
			public FormerlySerializedAsAttribute[] formerlySerializedAsList;
		}

		private sealed class TypeCache
		{
			private static Dictionary<System.Type, TypeCache> s_Types = new Dictionary<System.Type, TypeCache>();

			public static TypeCache GetTypeCache(System.Type type)
			{
				TypeCache typeCache = null;

				if (!s_Types.TryGetValue(type, out typeCache))
				{
					typeCache = new TypeCache(type);
					s_Types.Add(type, typeCache);
				}

				return typeCache;
			}

			public static bool IsValidDataSlotField(System.Type fieldType)
			{
				return Serialization.SerializationUtility.IsSerializableFieldType(fieldType) &&
						!Serialization.SerializationUtility.IsSupportedArray(fieldType);
			}

			public List<FieldCache> dataLinkFields
			{
				get;
				private set;
			}

			public TypeCache baseType
			{
				get;
				private set;
			}

			public TypeCache(System.Type type)
			{
				dataLinkFields = new List<FieldCache>();

				var fields = Serialization.FieldCache.GetFields(type);
				for (int i = 0, count = fields.Length; i < count; i++)
				{
					var fieldInfo = fields[i];
					System.Type fieldType = fieldInfo.FieldType;
					if (IsValidDataSlotField(fieldType))
					{
						DataLinkAttribute attribute = AttributeHelper.GetAttribute<DataLinkAttribute>(fieldInfo);
						if (attribute != null)
						{
							FieldCache fieldCache = new FieldCache();
							fieldCache.field = DynamicReflection.DynamicField.GetField(fieldInfo);
							fieldCache.attribute = attribute;
							fieldCache.formerlySerializedAsList = AttributeHelper.GetAttributes<FormerlySerializedAsAttribute>(fieldInfo);
							dataLinkFields.Add(fieldCache);
						}
					}
				}

				System.Type baseType = TypeUtility.GetBaseType(type);
				if (baseType != null && !EachFieldUtility.IsIgnoreDeclaringType(baseType))
				{
					this.baseType = GetTypeCache(baseType);
				}
			}
		}

		void RebuildDataSlotFieldLinks()
		{
			System.Type type = GetType();

			Dictionary<string, DataLinkFieldInfo> oldDataLinks = new Dictionary<string, DataLinkFieldInfo>();

			foreach (DataLinkFieldInfo link in _DataSlotFieldLinks)
			{
				oldDataLinks.Add(link.fieldName, link);
			}

			_DataSlotFieldLinks.Clear();

			for (TypeCache typeCache = TypeCache.GetTypeCache(type); typeCache != null; typeCache = typeCache.baseType)
			{
				foreach (FieldCache fieldCache in typeCache.dataLinkFields)
				{
					DynamicReflection.DynamicField field = fieldCache.field;

					System.Reflection.FieldInfo fieldInfo = field.fieldInfo;
					string fieldName = fieldInfo.Name;

					System.Type fieldType = fieldInfo.FieldType;

					DataLinkFieldInfo link = null;
					if (oldDataLinks.TryGetValue(fieldName, out link))
					{
						oldDataLinks.Remove(fieldName);
					}
					else
					{
						foreach (FormerlySerializedAsAttribute formerlySerializedAs in fieldCache.formerlySerializedAsList)
						{
							string oldFieldName = formerlySerializedAs.oldName;
							if (oldDataLinks.TryGetValue(formerlySerializedAs.oldName, out link) && link.slot.dataType == fieldType)
							{
								oldDataLinks.Remove(oldFieldName);
								link.fieldName = fieldName;
								break;
							}

							link = null;
						}
					}

					if (link == null)
					{
						link = new DataLinkFieldInfo();
						link.fieldName = fieldName;
					}

					System.Type slotType = link.slot.dataType;

					if (slotType != fieldType)
					{
						if (link.slot.branchID != 0 && (slotType == null || !TypeUtility.IsAssignableFrom(slotType, fieldType)))
						{
							Debug.LogWarningFormat("{0}#{1} is disconnected because its type has been changed : {2} -> {3}", TypeUtility.GetTypeName(type), fieldName, TypeUtility.GetTypeName(slotType), TypeUtility.GetTypeName(fieldType));
							link.slot.Disconnect();
						}
						link.slot.SetType(fieldType);
					}

					link.field = field;
					link.attribute = fieldCache.attribute;

					_DataSlotFieldLinks.Add(link);
				}
			}

			foreach (DataLinkFieldInfo link in oldDataLinks.Values)
			{
				link.slot.Disconnect();
			}
		}

#if !NETFX_CORE
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		[System.NonSerialized]
		private bool _Internal_IsClipboard = false;

#if ARBOR_DOC_JA
		/// <summary>
		/// DataSlotFieldを再構築する。
		/// </summary>
		/// <remarks>ランタイム中にDataSlotのフィールドの数を変更した場合に呼ぶ必要があります。</remarks>
#else
		/// <summary>
		/// Rebuild the DataSlotField.
		/// </summary>
		/// <remarks>It is necessary to call it when changing the number of fields of DataSlot at runtime.</remarks>
#endif
		public void RebuildDataSlotFields()
		{
			RebuildDataSlotFieldLinks();

			List<DataSlotField> newDataSlotFields = new List<DataSlotField>();

			EachField<IDataSlot>.Find(this, this.GetType(), (s, f) =>
			{
				DataSlot dataSlot = s as DataSlot;
				if (dataSlot == null)
				{
					return;
				}

				DataSlotField slotField = GetDataSlotField(dataSlot);
				if (slotField == null)
				{
					slotField = new DataSlotField(dataSlot, f);
				}

				newDataSlotFields.Add(slotField);
			});

			_DataSlotFields.Clear();
			_DataSlotFields = newDataSlotFields;

			if (!_Internal_IsClipboard)
			{
				if (_NodeGraph != null)
				{
					if (_NodeGraph.isDeserialized)
					{
						ClearDataBranchSlotField();
						SetupDataBranchSlotField();
					}
					else
					{
						_NodeGraph.onAfterDeserialize += SetupDataBranchSlotField;
					}
				}
			}
		}

		void ClearDataBranchSlotField()
		{
			for (int i = 0, count = _DataSlotFields.Count; i < count; i++)
			{
				DataSlotField slotField = _DataSlotFields[i];
				if (slotField == null)
				{
					continue;
				}

				slotField.ClearDataBranchSlotField();
			}
		}

		void SetupDataBranchSlotField()
		{
			for (int i = 0, count = _DataSlotFields.Count; i < count; i++)
			{
				DataSlotField slotField = _DataSlotFields[i];
				if (slotField == null)
				{
					continue;
				}

				slotField.SetupDataBranchSlotField();
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataSlotFieldを再構築する。
		/// </summary>
		/// <remarks>ランタイム中にDataSlotのフィールドの数を変更した場合に呼ぶ必要があります。</remarks>
#else
		/// <summary>
		/// Rebuild the DataSlotField.
		/// </summary>
		/// <remarks>It is necessary to call it when changing the number of fields of DataSlot at runtime.</remarks>
#endif
		[System.Obsolete("use RebuildDataSlotFields")]
		public void RebuildCalculatorSlotFields()
		{
			RebuildDataSlotFields();
		}

		void RebuildFlexibleSceneObjects()
		{
			EachField<IFlexibleField>.Find(this, this.GetType(), (IFlexibleField field, System.Reflection.FieldInfo fi) =>
			{
				FlexibleSceneObjectBase flexibleObject = field as FlexibleSceneObjectBase;
				if (flexibleObject == null)
				{
					return;
				}

				flexibleObject.ownerObject = this;
				flexibleObject.fieldInfo = fi;
			});
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// Editor用。
		/// </summary>
		/// <param name="nodeGraph">NodeGraph</param>
		/// <param name="nodeID">ノードID</param>
#else
		/// <summary>
		/// For Editor.
		/// </summary>
		/// <param name="nodeGraph">NodeGraph</param>
		/// <param name="nodeID">Node ID</param>
#endif
		public void Initialize(NodeGraph nodeGraph, int nodeID)
		{
			_NodeGraph = nodeGraph;
			_NodeID = nodeID;
		}

		internal static void RefreshBehaviour(Object behaviourObj, bool isPlaying)
		{
			if (!ComponentUtility.IsValidObject(behaviourObj))
			{
				return;
			}

#if !ARBOR_DEBUG
			behaviourObj.hideFlags |= HideFlags.HideInHierarchy | HideFlags.HideInInspector;
#endif

			NodeBehaviour behaviour = behaviourObj as NodeBehaviour;
			if (behaviour != null)
			{
				if (!isPlaying && behaviour.enabled)
				{
					behaviour.enabled = false;
				}

				foreach (var slotField in behaviour._DataSlotFields)
				{
					slotField.slot.DirtyBranchCache();
				}
			}

			INodeGraphContainer graphContainer = behaviourObj as INodeGraphContainer;
			if (graphContainer != null)
			{
				int graphCount = graphContainer.GetNodeGraphCount();
				for (int graphIndex = 0; graphIndex < graphCount; graphIndex++)
				{
					NodeGraph nodeGraph = graphContainer.GetNodeGraph<NodeGraph>(graphIndex);
					if (nodeGraph != null && !nodeGraph.external)
					{
#if !ARBOR_DEBUG
						nodeGraph.hideFlags |= HideFlags.HideInHierarchy | HideFlags.HideInInspector;
#endif

						if (!isPlaying)
						{
							nodeGraph.enabled = false;
						}
					}
				}
			}
		}


#if ARBOR_DOC_JA
		/// <summary>
		/// 生成時に呼ばれるメソッド.
		/// </summary>
#else
		/// <summary>
		/// Raises the created event.
		/// </summary>
#endif
		protected virtual void OnCreated()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// enabledの初期化を行うために呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// Called to perform enabled initialization.
		/// </summary>
#endif
		protected virtual void OnInitializeEnabled()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 破棄前に呼ばれるメソッド。
		/// </summary>
#else
		/// <summary>
		/// Raises the pre destroy event.
		/// </summary>
#endif
		protected virtual void OnPreDestroy()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// この関数はグラフが一時停止したときに呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// This function is called when the graph is paused.
		/// </summary>
#endif
		protected virtual void OnGraphPause()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// この関数はグラフが再開したときに呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// This function is called when the graph resumes.
		/// </summary>
#endif
		protected virtual void OnGraphResume()
		{
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// この関数はグラフが停止したときに呼ばれる。
		/// </summary>
#else
		/// <summary>
		/// This function is called when the graph stops.
		/// </summary>
#endif
		protected virtual void OnGraphStop()
		{
		}

		internal void CallPauseEvent()
		{
			UpdateDataLink(DataLinkUpdateTiming.Execute);

			try
			{
				OnGraphPause();
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex, this);
			}
		}

		internal void CallResumeEvent()
		{
			UpdateDataLink(DataLinkUpdateTiming.Execute);

			try
			{
				OnGraphResume();
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex, this);
			}
		}

		internal void CallStopEvent()
		{
			UpdateDataLink(DataLinkUpdateTiming.Execute);

			try
			{
				OnGraphStop();
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex, this);
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 手動によるDataLinkの値更新。
		/// <see cref="DataLinkUpdateTiming.Manual"/>のDataLinkフィールドの値を更新する。
		/// </summary>
#else
		/// <summary>
		/// Manually update DataLink values.
		/// Update the value of DataLink field of <see cref="DataLinkUpdateTiming.Manual"/>.
		/// </summary>
#endif
		public void UpdateDataLink()
		{
			UpdateDataLink(DataLinkUpdateTiming.Manual);
		}

		internal void UpdateDataLink(DataLinkUpdateTiming updateTiming)
		{
			foreach (DataLinkFieldInfo link in _DataSlotFieldLinks)
			{
				if ((link.currentUpdateTiming & updateTiming) != 0)
				{
					object value = null;
					if (link.slot.GetValue(ref value))
					{
						link.field.SetValue(this, value);
					}
				}
			}
		}
	}
}