﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

using Arbor.ObjectPooling;

namespace Arbor.StateMachine.StateBehaviours
{
#if ARBOR_DOC_JA
	/// <summary>
	/// OnTriggerEnter2Dが呼ばれた際に、相手のGameObjectを破棄する。
	/// </summary>
#else
	/// <summary>
	/// When OnTriggerEnter2D is called, it will destroy an opponent GameObject.
	/// </summary>
#endif
	[AddComponentMenu("")]
	[AddBehaviourMenu("Collision2D/OnTriggerEnter2DDestroy")]
	[BuiltInBehaviour]
	public sealed class OnTriggerEnter2DDestroy : CheckTagBehaviourBase
	{
		void OnTriggerEnter2D(Collider2D collider)
		{
			if (!enabled)
			{
				return;
			}

			GameObject target = collider.gameObject;
			if (CheckTag(target))
			{
				ObjectPool.Destroy(target);
			}
		}
	}
}
