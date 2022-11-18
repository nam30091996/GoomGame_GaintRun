using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Timers;

namespace KR
{
	public class SpriteAnimation : SimpleAnimation
	{

		private SpriteRenderer _spriteRenderer;
		public SpriteRenderer spriteRenderer
		{
			get
			{
				return _spriteRenderer ?? (_spriteRenderer = GetComponent<SpriteRenderer>());
			}
		}

		protected override void OnFrameChange()
		{

			spriteRenderer.sprite = Frames[currentFrame];

		}

	}
}