//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;

namespace Arbor.ObjectPooling
{
	internal sealed class PoolObject : MonoBehaviour
	{
		public Object original
		{
			get;
			set;
		}
		public Object instance
		{
			get;
			set;
		}

		private const HideFlags kPoolHideFlags = HideFlags.NotEditable;

		public bool isUsing
		{
			get;
			private set;
		}

		internal bool isValid
		{
			get
			{
				return gameObject != null && instance != null;
			}
		}

		internal void OnPoolResume()
		{
			Transform transform = gameObject.GetComponent<Transform>();
			transform.SetParent(null, true);
			gameObject.SetActive(true);
			gameObject.hideFlags &= ~kPoolHideFlags;

			hideFlags |= HideFlags.HideAndDontSave | HideFlags.HideInInspector;

			if (!isUsing)
			{
				foreach (IPoolCallbackReceiver receiver in gameObject.GetComponentsInChildren<IPoolCallbackReceiver>())
				{
					if (receiver != null)
					{
						receiver.OnPoolResume();
					}
				}
			}

			isUsing = true;
		}

		internal void OnPoolSleep()
		{
			if (isUsing)
			{
				foreach (IPoolCallbackReceiver receiver in gameObject.GetComponentsInChildren<IPoolCallbackReceiver>())
				{
					if (receiver != null)
					{
						receiver.OnPoolSleep();
					}
				}
			}

			Transform transform = gameObject.GetComponent<Transform>();
			transform.SetParent(ObjectPool.transform, true);
			gameObject.hideFlags |= kPoolHideFlags;
			gameObject.SetActive(false);

			hideFlags |= HideFlags.HideAndDontSave | HideFlags.HideInInspector;

			isUsing = false;
		}

		internal void Initialize(Object original, Object instance)
		{
			this.original = original;
			this.instance = instance;
		}
	}
}