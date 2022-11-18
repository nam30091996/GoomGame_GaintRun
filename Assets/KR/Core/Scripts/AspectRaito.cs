using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KR;

[System.Serializable]
public struct AspectFilter {
	public float width, height;
	public float orthoSize;
	public float Aspect{
		get{
			if (width == 0 || height == 0)
				return 0;
			
			return width / height;
		}
	}

	public override string ToString()
	{
		return string.Format("Width: {0}, Height: {1}, Aspect: {2}", width, height, Aspect);
	}
} 

[RequireComponent(typeof(Camera))]
public class AspectRaito : KR.Singleton<AspectRaito> {
	public List<AspectFilter> filters;
    
	private Camera _Camera;
	public Camera Camera{
		get{
			return _Camera ?? (_Camera = GetComponent<Camera>());
		}
	}


	private void Start()
	{
		var screenAspect = Screen.width / (float) Screen.height;
		AspectFilter filter = default(AspectFilter);
		for (int i = 0; i < filters.Count; i++){
			if (filter.Aspect == 0)
				filter = filters[i];
			else
				if (filter.Aspect.distance(screenAspect) > filters[i].Aspect.distance(screenAspect))
				filter = filters[i];
		}
		var Camera = GetComponent<Camera>();
		Camera.orthographicSize = filter.orthoSize;
		Debug.Log(filter);
	}
}
