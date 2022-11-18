using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KR;


public class CameraAspect3D : MonoBehaviour {

	public float width = 960f, height = 1600f;
	[Range(-50, 50)]
	public float depth = 5f;

    
	// Use this for initialization
	Camera Cam;
	float startZ;
	void Awake () {
		Cam = GetComponent<Camera>();

		//if (is3DAspect)
		//{
			//transform.position = transform.position.set(y: initHeight);
		///	Cam.orthographic = false;
		//}
		startZ = Cam.transform.localPosition.z;
		UpdateAspect();
        
	}


	void UpdateAspect(){
		float asp = width / height;
		float currentAsp = Screen.width / (float)Screen.height;
		float raito  = (asp  - currentAsp);
		//Debug.Log("Width: " + screen.width raito);
		Vector3 pos = transform.localPosition.set(z: startZ + (Mathf.Lerp(0, depth, raito)));
		transform.localPosition = pos;

	}

	// Update is called once per frame
	#if UNITY_EDITOR
	void Update()
	{
		UpdateAspect();
	}
	#endif

}
