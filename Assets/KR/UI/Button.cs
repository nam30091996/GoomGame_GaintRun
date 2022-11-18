using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KR.UI
{
	public class Button : UnityEngine.UI.Button
	{
		[HideInInspector]
		public bool isPointerDown;

		[SerializeField, HideInInspector]
		private Button.ButtonClickedEvent onPointerDown = new Button.ButtonClickedEvent();
		[SerializeField, HideInInspector]
		private Button.ButtonClickedEvent onPointerUp = new Button.ButtonClickedEvent();

		[SerializeField, HideInInspector]
		private Button.ButtonClickedEvent onPointerHover = new Button.ButtonClickedEvent();

      
		public override void OnPointerDown(PointerEventData eventData)
		{
			isPointerDown = true;
			onPointerDown?.Invoke();
			base.OnPointerDown(eventData);
		}
		public override void OnPointerUp(PointerEventData eventData)
		{
			isPointerDown = false;
			onPointerUp?.Invoke();
			base.OnPointerUp(eventData);
		}

		private void Update()
		{
			if(isPointerDown){
				onPointerHover?.Invoke();
			}
		}
	}
			
}
