using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KR;

#pragma warning disable
namespace KR
{
	public class HealthManager : KR.Singleton<HealthManager>
	{

		public Health Source;
		public Vector3 position;
		public bool lockRotation;
		//[ShowIf("lockRotation")]
		public Vector3 rotationConstraint;


		private List<Health> Healths = new List<Health>();
        
		public void Remove(Health health){
			if(Healths.Contains(health)){
				Healths.Remove(health);
			}
		}
		public Health CreateHealth(int Health, Vector3 offset, Transform handler){
			var health = Instantiate(Source, handler);
			health.LoadHealth(Health);
			health.transform.localPosition = position + offset;
			health.transform.localScale = Vector3.one;
			health.transform.localScale = new Vector3(Source.transform.localScale.x / health.transform.lossyScale.x,
													  Source.transform.localScale.y / health.transform.lossyScale.y,
													  Source.transform.localScale.z / health.transform.lossyScale.z);
			Healths.Add(health);
			return health;

		}
		public Health CreateHealth(int Health, Vector3 offset, Quaternion rotation, Transform handler)
        {
			var health = CreateHealth(Health, offset, handler);
			health.transform.rotation = rotation;
            
            return health;

        }

		Quaternion euler;
		private void Start()
		{
			euler = Quaternion.Euler(rotationConstraint);	
		}

		private void Update()
		{
			if (!lockRotation) return;
			int length = Healths.Count;
			for (int i = 0; i < length; i++)
			{
				if (Healths[i])
				{

					Healths[i].transform.rotation = euler;
				    
				}
			}
			
		}

	}
}
