using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KR;
using UnityEngine.SceneManagement;

public class PoolEvent : MonoBehaviour {

	//[KR.ReadOnly, SerializeField]
	//private bool muteEvent;
	private void OnEnable()
	{
		//muteEvent = false;
		PoolManager.instance.runningPools.Add(this);
	}
	private void OnDisable()
	{
		if (PoolManager.instance == null ||  !PoolManager.instance.gameObject.activeInHierarchy) return;//pool is destroyed.

		if (PoolManager.instance.runningPools.Contains(this))
		{
			PoolManager.instance.runningPools.Remove(this);
			PoolManager.instance.Schedule(0, () => transform.SetParent(PoolManager.instance.transform, false));
		}
	}


}
