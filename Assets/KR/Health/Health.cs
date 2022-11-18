using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace KR
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class Health : MonoBehaviour
	{

		public string property = "_Horizontal";
		private SpriteRenderer _spriteRenderer;
		public SpriteRenderer spriteRenderer
		{
			get
			{
				return _spriteRenderer ?? (_spriteRenderer = GetComponent<SpriteRenderer>());
			}
		}
		[SerializeField, KR.ReadOnly]
		int startHealth, _health;

		//[KR.ReadOnly]
		//public Vector3 offset;

		public int health
		{
			get
			{
				return _health;
			}
		 	set
			{
				_health = value;
				spriteRenderer.material.SetFloat(property, _health / (float)startHealth);
				spriteRenderer.enabled = health > 0 && health < startHealth;
				onValueChanged?.Invoke(_health);
			}

		}

		public Action<int> onValueChanged;

		public void LoadHealth(int Health)
		{
			this.health = startHealth = Health;
			//this.offset = offset;
		}

		private void OnDisable()
		{
			HealthManager.instance.Remove(this);
		}

	}
}