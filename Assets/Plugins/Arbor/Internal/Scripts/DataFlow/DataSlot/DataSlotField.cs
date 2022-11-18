//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
namespace Arbor
{
#if ARBOR_DOC_JA
	/// <summary>
	/// DataSlotのField情報
	/// </summary>
#else
	/// <summary>
	/// Field information of DataSlot
	/// </summary>
#endif
	public sealed class DataSlotField
	{
		/// <summary>
		/// DataSlot
		/// </summary>
		public DataSlot slot
		{
			get;
			private set;
		}

		/// <summary>
		/// FieldInfo
		/// </summary>
		public System.Reflection.FieldInfo fieldInfo
		{
			get;
			private set;
		}

		private ClassConstraintInfo _ConstraintInfo;

#if ARBOR_DOC_JA
		/// <summary>
		/// 上書きする型制約の情報
		/// </summary>
#else
		/// <summary>
		/// override ClassConstraintInfo
		/// </summary>
#endif
		public ClassConstraintInfo overrideConstraint = null;

#if ARBOR_DOC_JA
		/// <summary>
		/// 型制約の情報を返す。
		/// </summary>
		/// <returns>型制約の情報</returns>
#else
		/// <summary>
		/// Return information on type constraints.
		/// </summary>
		/// <returns>Type constraint information</returns>
#endif
		public ClassConstraintInfo GetConstraint()
		{
			if (overrideConstraint != null)
			{
				return overrideConstraint;
			}

			if (_ConstraintInfo != null)
			{
				return _ConstraintInfo;
			}

			return null;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 接続可能な型
		/// </summary>
#else
		/// <summary>
		/// Connectable type
		/// </summary>
#endif
		public System.Type connectableType
		{
			get
			{
				ClassConstraintInfo constraint = GetConstraint();
				if (constraint != null)
				{
					return constraint.GetConstraintBaseType();
				}
				return slot.dataType;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 接続可能な型名
		/// </summary>
#else
		/// <summary>
		/// Connectable type name
		/// </summary>
#endif
		public string connectableTypeName
		{
			get
			{
				string typeName = string.Empty;
				ClassConstraintInfo constraint = GetConstraint();
				if (constraint != null)
				{
					typeName = constraint.GetConstraintTypeName();
				}
				else
				{
					typeName = TypeUtility.GetSlotTypeName(slot.dataType);
				}
				return typeName;
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// スロットのGUIが有効であるかどうか。
		/// </summary>
#else
		/// <summary>
		/// Whether the GUI for the slot is valid.
		/// </summary>
#endif
		public bool enabled = true;

		private bool _IsVisible = true;
		private bool _IsVisibleNext = false;

#if ARBOR_DOC_JA
		/// <summary>
		/// Editor用。
		/// </summary>
#else
		/// <summary>
		/// For Editor.
		/// </summary>
#endif
		public bool isVisible
		{
			get
			{
				return _IsVisible;
			}
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
		public void SetVisible()
		{
			_IsVisibleNext = true;
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
		public void ClearVisible()
		{
			_IsVisible = _IsVisibleNext;
			_IsVisibleNext = false;
		}

		/// <summary>
		/// DataSlotField constructor
		/// </summary>
		/// <param name="slot">DataSlot</param>
		/// <param name="fieldInfo">FieldInfo</param>
		public DataSlotField(DataSlot slot, System.Reflection.FieldInfo fieldInfo)
		{
			this.slot = slot;
			this.fieldInfo = fieldInfo;

			bool isConstraintableInputSlot = slot is InputSlotComponent || slot is InputSlotUnityObject || slot is InputSlotAny;
			if (isConstraintableInputSlot)
			{
				ClassTypeConstraintAttribute constraint = AttributeHelper.GetAttribute<ClassTypeConstraintAttribute>(fieldInfo);
				if (constraint != null)
				{
					_ConstraintInfo = new ClassConstraintInfo() { constraintAttribute = constraint, constraintFieldInfo = fieldInfo };
				}
			}

			if (_ConstraintInfo == null)
			{
				bool isConstraintableOutputSlot = slot is OutputSlotAny;
				if (isConstraintableInputSlot || isConstraintableOutputSlot)
				{
					SlotTypeAttribute slotType = AttributeHelper.GetAttribute<SlotTypeAttribute>(fieldInfo);
					if (slotType != null)
					{
						_ConstraintInfo = new ClassConstraintInfo() { slotTypeAttribute = slotType };
					}
				}
			}
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// DataSlotField同士が接続可能か判定する。
		/// </summary>
		/// <param name="inputSlotField">入力スロットフィールド</param>
		/// <param name="outputSlotField">出力スロットフィールド</param>
		/// <returns>接続可能であればtrueを返す。</returns>
#else
		/// <summary>
		/// Determine if DataSlotFields can connect to each other.
		/// </summary>
		/// <param name="inputSlotField">Input slot field</param>
		/// <param name="outputSlotField">Output slot field</param>
		/// <returns>Returns true if the connection is possible.</returns>
#endif
		public static bool IsConnectable(DataSlotField inputSlotField, DataSlotField outputSlotField)
		{
			if (inputSlotField == null || outputSlotField == null)
			{
				return false;
			}

			System.Type inputSlotType = inputSlotField.slot.dataType;
			System.Type outputConnectableType = outputSlotField.connectableType;

			bool isAnyInput = inputSlotType == null || inputSlotType == typeof(object);

			if (!isAnyInput && !TypeUtility.IsAssignableFrom(inputSlotType, outputConnectableType))
			{
				return false;
			}

			ClassConstraintInfo constraint = inputSlotField.GetConstraint();
			if (constraint != null)
			{
				return constraint.IsConstraintSatisfied(outputConnectableType);
			}

			return true;
		}

#if ARBOR_DOC_JA
		/// <summary>
		/// 接続可能か判定する。
		/// </summary>
		/// <param name="slotField">判定するスロット</param>
		/// <returns>接続可能ならtrue、それ以外はfalseを返す。</returns>
#else
		/// <summary>
		/// It is judged whether connection is possible.
		/// </summary>
		/// <param name="slotField">Slot to determine</param>
		/// <returns>Returns true if connectable, false otherwise.</returns>
#endif
		public bool IsConnectable(DataSlotField slotField)
		{
			DataSlotField inputSlotField = null;
			DataSlotField outputSlotField = null;

			if (slotField == null)
			{
				//UnityEngine.Debug.Log("slotField == null");
				return false;
			}

			switch (slot.slotType)
			{
				case SlotType.Input:
					if (slotField.slot.slotType == SlotType.Input)
					{
						return false;
					}

					inputSlotField = this;
					outputSlotField = slotField;
					break;
				case SlotType.Output:
				case SlotType.Reroute:
					if (slotField.slot.slotType == SlotType.Output)
					{
						return false;
					}

					inputSlotField = slotField;
					outputSlotField = this;
					break;
			}

			return IsConnectable(inputSlotField, outputSlotField);
		}

		internal void ClearDataBranchSlotField()
		{
			DataSlot slot = this.slot;
			IInputSlot inputSlot = slot as IInputSlot;
			if (inputSlot != null)
			{
				DataBranch branch = inputSlot.GetBranch();
				if (branch != null)
				{
					branch._InputSlotField = null;
				}
			}

			IOutputSlot outputSlot = slot as IOutputSlot;
			if (outputSlot != null)
			{
				for (int i = outputSlot.branchCount - 1; i >= 0; i--)
				{
					DataBranch branch = outputSlot.GetBranch(i);
					if (branch != null)
					{
						branch._OutputSlotField = null;
					}
				}
			}
		}

		internal void SetupDataBranchSlotField()
		{
			DataSlot slot = this.slot;
			IInputSlot inputSlot = slot as IInputSlot;
			if (inputSlot != null)
			{
				DataBranch branch = inputSlot.GetBranch();
				if (branch != null)
				{
					if (branch._InputSlotField == null)
					{
						branch._InputSlotField = this;

						if (branch.outBehaviour != null && branch._OutputSlotField != null && !IsConnectable(this, branch._OutputSlotField))
						{
							slot.nodeGraph.Internal_DeleteDataBranch(branch);
						}
					}
					else if (!object.ReferenceEquals(branch._InputSlotField, this))
					{
						inputSlot.RemoveBranch(branch);
					}
				}
			}

			IOutputSlot outputSlot = slot as IOutputSlot;
			if (outputSlot != null)
			{
				for (int i = outputSlot.branchCount - 1; i >= 0; i--)
				{
					DataBranch branch = outputSlot.GetBranch(i);
					if (branch != null)
					{
						if (branch._OutputSlotField == null)
						{
							branch._OutputSlotField = this;

							if (branch.inBehaviour != null && branch._InputSlotField != null && !IsConnectable(branch._InputSlotField, this))
							{
								slot.nodeGraph.Internal_DeleteDataBranch(branch);
							}
						}
						else if (!object.ReferenceEquals(branch._OutputSlotField, this))
						{
							outputSlot.RemoveBranch(branch);
						}
					}
				}
			}
		}
	}
}