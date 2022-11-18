using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KR
{
	public class PoolManager : KR.ManagerSingleton<PoolManager>
	{
		//[Sirenix.OdinInspector.ReadOnly, SerializeField]
		private List<PoolEvent> _runningPools;
		public List<PoolEvent> runningPools{
			get{
				return _runningPools ?? (_runningPools = new List<PoolEvent>());
			}
			private set{
				_runningPools = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			Pool.Init();
		}

		string baseName;
		private void Start()
		{
			baseName = gameObject.name;
		}

		public void StopAll(){
			int totals = runningPools.Count - 1;
			for (int i = totals; i >= 0; i--){
				runningPools[i].transform.parent = transform;
				runningPools[i].gameObject.SetActive(false);

			}
		}
#if UNITY_EDITOR
		private void Update()
		{
			gameObject.name = baseName + " [Running [{0}]]".format(runningPools.Count);
		}
#endif

	}
}
