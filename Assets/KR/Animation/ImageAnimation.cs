using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace KR
{
	public class ImageAnimation : SimpleAnimation
	{

		Image _Image;
		public Image Image
		{
			get
			{
				return _Image ?? (_Image = GetComponent<Image>());
			}
		}
		protected override void OnFrameChange()
		{
			Image.sprite = Frames[currentFrame];
		}
	}
}