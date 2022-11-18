using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KR
{
	public class MeshAnimation : SimpleAnimation
	{

		Material _Material;
		Material Material {
			get{
				return _Material ?? (_Material = meshRenderer.material); 
			}
		}
        
		MeshRenderer _meshRenderer;
		public MeshRenderer meshRenderer
		{
			get
			{
				return _meshRenderer ?? (_meshRenderer = GetComponent<MeshRenderer>());
			}
		}
		protected override void OnFrameChange()
		{
			if(Application.isPlaying)
			Material.mainTexture = Frames[currentFrame].texture;
		}

	}
}