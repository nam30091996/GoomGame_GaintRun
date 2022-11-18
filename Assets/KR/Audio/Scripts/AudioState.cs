using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioState : MonoBehaviour {

	AudioSource _src;
	AudioSource src {
		get{
			return _src ?? (_src = GetComponent<AudioSource>());
		}
	}

    private void OnEnable()
    {
		KR.Audio.onSoundStateChanged += Audio_OnSoundStateChanged;
    }

	private void OnDisable()
	{
		KR.Audio.onSoundStateChanged -= Audio_OnSoundStateChanged;
	}
	void Audio_OnSoundStateChanged(bool obj)
	{
		src.mute = !obj;
	}

}
