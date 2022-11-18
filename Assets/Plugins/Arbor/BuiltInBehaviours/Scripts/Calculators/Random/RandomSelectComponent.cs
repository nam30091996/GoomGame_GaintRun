//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.Calculators
{
	[System.Serializable]
	[Arbor.Internal.Documentable]
	public sealed class WeightComponentList : WeightList<FlexibleComponent>
	{
	}

#if ARBOR_DOC_JA
	/// <summary>
	/// リストからランダムに選択したComponentを出力する。
	/// </summary>
#else
	/// <summary>
	/// Output component selected randomly from the list.
	/// </summary>
#endif
	[AddComponentMenu("")]
	[AddBehaviourMenu("Random/Random.SelectComponent")]
	[BehaviourTitle("Random.SelectComponent")]
	[BuiltInBehaviour]
	public sealed class RandomSelectComponent : Calculator
	{
#if ARBOR_DOC_JA
		/// <summary>
		/// コンポーネントの重みリスト
		/// </summary>
#else
		/// <summary>
		/// Component weight list
		/// </summary>
#endif
		[SerializeField]
		private WeightComponentList _Weights = new WeightComponentList();

#if ARBOR_DOC_JA
		/// <summary>
		/// 出力スロット
		/// </summary>
#else
		/// <summary>
		/// Output slot
		/// </summary>
#endif
		[SerializeField]
		[HideSlotFields]
		private OutputSlotComponent _Output = new OutputSlotComponent();

		public System.Type type
		{
			get
			{
				return _Output.dataType;
			}
		}

		public override bool OnCheckDirty()
		{
			return true;
		}

		public override void OnCalculate()
		{
			FlexibleComponent item = _Weights.GetRandomItem();

			Component value = (item != null) ? item.value : null;
			_Output.SetValue(value);
		}
	}
}