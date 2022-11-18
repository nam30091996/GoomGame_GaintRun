﻿//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Arbor.ObjectPooling
{
	[AddComponentMenu("")]
	internal sealed class AdvancedPoolingInternal : MonoBehaviour
	{
		private Queue<PoolingItem> _PoolingItems = new Queue<PoolingItem>();

		private Coroutine _Coroutine;

		public bool isReady
		{
			get
			{
				return _PoolingItems.Count == 0;
			}
		}

		public void AddItems(IEnumerable<PoolingItem> items)
		{
			foreach (PoolingItem item in items)
			{
				_PoolingItems.Enqueue(new PoolingItem(item));
			}

			if (_Coroutine == null)
			{
				_Coroutine = StartCoroutine(Load());
			}
		}

		static float GetMaxTimePerFrame()
		{
			int frameRate = 0;

			if (ObjectPool.advancedRatePerFrame > 0)
			{
				int vSyncCount = QualitySettings.vSyncCount;
				if (Application.isEditor || vSyncCount > 0)
				{
					Resolution currentResolution = Screen.currentResolution;
					vSyncCount = (vSyncCount > 0 ? vSyncCount : 1);
					frameRate = currentResolution.refreshRate / vSyncCount;
				}
				else
				{
					frameRate = Application.targetFrameRate;
				}

				frameRate *= ObjectPool.advancedRatePerFrame;
			}
			else
			{
				frameRate = ObjectPool.advancedFrameRate;
			}

			if (frameRate > 0)
			{
				return (1f / frameRate);
			}

			return 0f;
		}

		private IEnumerator Load()
		{
			float lastTime = Time.realtimeSinceStartup;

			while (_PoolingItems.Count > 0)
			{
				PoolingItem item = _PoolingItems.Peek();

				if (item.amount > 0)
				{
					ObjectPool.CreatePool(item.original);
					item.amount--;
				}

				if (item.amount == 0)
				{
					_PoolingItems.Dequeue();
				}

				float maxTime = GetMaxTimePerFrame();
				if (maxTime > 0f && Time.realtimeSinceStartup - lastTime >= maxTime)
				{
					yield return null;
					lastTime = Time.realtimeSinceStartup;
				}
			}

			_Coroutine = null;
		}
	}
}