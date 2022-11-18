using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicState : MonoBehaviour {

	AudioSource src;

	void Start (){
		src = GetComponent<AudioSource>();
		KR.Audio.instance.LoadMusicState();
	}
	private void OnEnable()
    {
		KR.Audio.onMusicStateChanged += Audio_OnSoundStateChanged;
    }

    private void OnDisable()
    {
		KR.Audio.onMusicStateChanged -= Audio_OnSoundStateChanged;
    }


    void Audio_OnSoundStateChanged(bool obj)
    {
		if(src)
		src.mute = !obj;
    }
}
