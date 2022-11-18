using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KR
{
	public class Scheduler : MonoBehaviour
	{


		public Data scheduler;

		[Serializable]
		public enum Event
		{
			Deactive,
			Destroy,
		}

		[Serializable]
		public struct Data
		{
			public float time;
			public Event Event;
		}


		private void OnEnable()
		{
			this.Schedule(scheduler.time, HandleSchedulerEvent);
		}

		void HandleSchedulerEvent(){
			switch(scheduler.Event){
				case Event.Deactive:
					gameObject.SetActive(false);
					break;
				case Event.Destroy:
					Destroy(gameObject);
					break;
			}
		}
	}
}