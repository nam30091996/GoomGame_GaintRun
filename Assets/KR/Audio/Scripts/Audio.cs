using System.Collections.Generic;
using UnityEngine;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
using KR.Editor;
#endif
using Sirenix.OdinInspector;

using UnityEngine.Audio;
using System;
using System.Text.RegularExpressions;
#pragma warning disable


namespace KR
{

	[Serializable]
	public class AudioSetting
	{
		public AudioClip audioClip;
		public AudioMixerGroup output;
		public bool mute, bypassEffects, bypassListenerEffects, bypassReverbZones, playOnAwake = true, loop;

		[Range(0, 256)]
		public int priority = 128;
		[Range(0, 1)]
		public float volume = 1;
		[Range(-3, 3)]
		public float pitch = 1;


		[Range(-1, 1)]
		public float stereoPan;

		[Range(0, 1)]
		public float spatialBlend;

		[Range(0, 1.1f)]
		public float preverbZoneMix = 1;


	}
	public enum AudioCategory
	{
		SoundEffect,
		BackgroundMusic,
	}
	public class Audio : KR.ScriptableSingleton<Audio>
	{

		const string defaultResourcePath = "Assets/KR/Audio/Resources/";
		private readonly bool music;

		public static event Action<bool>  onSoundStateChanged;
		public static event Action<bool> onMusicStateChanged;


#if UNITY_EDITOR

		[MenuItem("KR/Audio/Init")]
		public static void CreateInitiator()
		{
			KR.Scriptable.CreateInitiator<AudioInitiator>();
		}

		[MenuItem("KR/Audio/Settings")]
		public static void CreateAsset()
		{
			KR.Scriptable.CreateAsset<Audio>(defaultResourcePath);
		}



#endif


		public List<AudioInfo> audioList = new List<AudioInfo>();


		//[TabGroup("G", "Sound Settings"), PropertyOrder(-1)]
		public AudioSetting soundSetting;


		//[TabGroup("G", "Music Settings"), PropertyOrder(-1)]
		public AudioSetting musicSetting;



		public static AudioSource soundEffect { get; set; }
		public static AudioSource backgroundMusic { get; set; }

		public override void OnInitialized()
		{
			base.OnInitialized();
			soundEffect = new GameObject("SOUND EFFECT").AddComponent<AudioSource>();
			backgroundMusic = new GameObject("BACKGROUND MUSIC").AddComponent<AudioSource>();
			LoadSetting(soundEffect, soundSetting);
			LoadSetting(backgroundMusic, musicSetting);

			DontDestroyOnLoad(backgroundMusic.gameObject);
			DontDestroyOnLoad(soundEffect.gameObject);
		}

		void LoadSetting(AudioSource source, AudioSetting setting)
		{
			source.clip = setting.audioClip;
			source.outputAudioMixerGroup = setting.output;
			source.mute = setting.mute;
			source.bypassEffects = setting.bypassEffects;
			source.bypassListenerEffects = setting.bypassListenerEffects;
			source.bypassReverbZones = setting.bypassReverbZones;
			source.playOnAwake = setting.playOnAwake;
			source.loop = setting.loop;
			source.priority = setting.priority;
			source.volume = setting.volume;
			source.pitch = setting.pitch;
			source.panStereo = setting.stereoPan;
			source.spatialBlend = setting.spatialBlend;
			source.reverbZoneMix = setting.preverbZoneMix;

		}

#if UNITY_EDITOR

		[Button, ButtonGroup("Action"), PropertyOrder(-2)]
		private void GenerateAudio()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("namespace KR ");
			builder.AppendLine("{");
			builder.AppendLine("\tpublic enum AudioList");
			builder.AppendLine("\t{");
			for (int i = 0; i < audioList.Count; i++)
			{
				if (audioList[i].clip != null && !string.IsNullOrEmpty(audioList[i].key))
				{
					builder.AppendLine("\t\t " + audioList[i].key + ",");
				}
			}
			builder.AppendLine("\t} ");
			builder.AppendLine("} ");

			//Debug.Log(builder.ToString());
			KR.Editor.Utility.DisplayProgressBar("Generating Pool", "Total audios: " + audioList.Count, 0.25f, () =>
			{
				System.IO.File.WriteAllText(Application.dataPath + "/KR/Audio/Resources/" + "AudioList.cs", builder.ToString());
                //Debug.Log(Application.dataPath + "/Ea/Audio/Resources/EaAudioList.cs");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
			});

		
		}
#endif
		public static void Play(AudioList key, float volume  =1)
		{
			//if (!AudioKey.isAudioOn) return;


			for (int i = 0; i < instance.audioList.Count; i++)
			{
				if (instance.audioList[i].key == key.ToString())
				{
					soundEffect.PlayOneShot(instance.audioList[i].clip, volume);
					return;
				}

			}
		}



		public void LoadSoundState()
		{
			soundEffect.mute = !Data.instance.enableSound;
			onSoundStateChanged?.Invoke(Data.instance.enableSound);


		}
		public void LoadMusicState (){
			backgroundMusic.mute = !Data.instance.enableMusic;
            onMusicStateChanged?.Invoke(Data.instance.enableMusic);
		}



		public void ChangeSound(bool enabled)
		{
			Data.instance.enableSound = enabled;
			LoadSoundState();
		}
		public void ToggleMusic(){
			ChangeMusic(!Data.instance.enableMusic);
		}
		public void ToggleSound(){
			//Debug.Log("TS: " + Data.instance.enableSound);
			ChangeSound(!Data.instance.enableSound);
		}

		public void ChangeMusic(bool enabled){
			Data.instance.enableMusic = enabled;
			LoadMusicState();
		}
        
	
		public static string GetKey(string input)
        {
            if (Regex.IsMatch(input, @"^\d"))
                input = "_" + input;

            Regex r = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return r.Replace(input, string.Empty).Replace(" ", "");

            //return input.Replace(" ", "").Replace("(", "").Replace(")", "");

        }
      

		public class Data : KR.Serializable<Data>
        {
            public bool enableMusic = true, enableSound = true;
        }

	}

	[System.Serializable]
    public class AudioInfo
    {
		string Title 
		{
			get{
				return string.IsNullOrEmpty(key)  ? string.Empty : Regex.Replace(Audio.GetKey(key), "([a-z])([A-Z])", "$1 $2");
			}
		}

        //[FoldoutGroup("$Title", Expanded = true)]
        public string key;

		//[FoldoutGroup("$Title", Expanded = true)]
		//[ValidateInput("AutoSetName")]
		public AudioClip clip;
        
		private bool AutoSetName(AudioClip s)
        {
			if (string.IsNullOrEmpty(key) && clip != null)
            {
				key = Audio.GetKey(clip.name);
            }
            return true;
        }
    }



}
	
