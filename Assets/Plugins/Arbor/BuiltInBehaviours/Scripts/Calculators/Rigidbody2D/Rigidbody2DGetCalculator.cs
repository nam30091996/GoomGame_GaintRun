//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.Calculators
{
#if ARBOR_DOC_JA
	/// <summary>
	/// GameObjectにアタッチされているRigidbody2Dを取得する。
	/// </summary>
#else
	/// <summary>
	/// Gets the Rigidbody2D attached to GameObject.
	/// </summary>
#endif
	[AddComponentMenu("")]
	[AddBehaviourMenu("Rigidbody2D/Rigidbody2D.Get")]
	[BehaviourTitle("Rigidbody2D.Get")]
	[BuiltInBehaviour]
	public sealed class Rigidbody2DGetCalculator : Calculator
	{
		#region Serialize fields

		/// <summary>
		/// GameObject
		/// </summary>
		[SerializeField] private FlexibleGameObject _GameObject = new FlexibleGameObject(FlexibleHierarchyType.Self);

#if ARBOR_DOC_JA
		/// <summary>
		/// 取得したRigidbody2D
		/// </summary>
#else
		/// <summary>
		/// Get the Rigidbody2D
		/// </summary>
#endif
		[SerializeField] private OutputSlotRigidbody2D _Rigidbody2D = new OutputSlotRigidbody2D();

		#endregion // Serialize fields

		// Use this for calculate
		public override void OnCalculate()
		{
			GameObject gameObject = _GameObject.value;
			if (gameObject != null)
			{
				_Rigidbody2D.SetValue(gameObject.GetComponent<Rigidbody2D>());
			}
		}
	}
}
