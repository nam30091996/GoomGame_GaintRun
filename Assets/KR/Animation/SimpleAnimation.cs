using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KR
{
	[ExecuteInEditMode]
	public abstract class SimpleAnimation : MonoBehaviour
	{

		[Range(1, 60)]
		public int frameRate;
		public bool playOnEnable = true, Loop;
		public bool Reverse;
		private System.Action onCompleteCallback, onRestartCallback;


		public void OnComplete(System.Action callback)
		{
			onCompleteCallback = callback;
		}

		public void OnRestart(System.Action callback)
		{
			onRestartCallback = callback;
		}

		//[Button, HideIf("isPlaying", false), ButtonGroup("ActionBar")]
		public void Play()
		{
			isPlaying = true;
		}

		//[Button]
		public void Restart()
		{
			ResetFrame();
			onRestartCallback?.Invoke();
			isPlaying = true;
		}


		//[Button, ShowIf("isPlaying", false), ButtonGroup("ActionBar")]
		public void Pause()
		{
			isPlaying = false;
		}

		//[Button, EnableIf("isPlaying", false), ButtonGroup("ActionBar")]
		public void Stop()
		{
			isPlaying = false;
			ResetFrame();
		}

		[SerializeField, KR.ReadOnly]
		private bool isPlaying;
		[KR.ReadOnly, SerializeField]
		protected int currentFrame;

		[KR.ReadOnly, SerializeField]
		private float lastFrame;

		private void OnEnable()
		{
			isPlaying = playOnEnable;
			lastFrame = Time.realtimeSinceStartup;
			ResetFrame();
		}

		private void ResetFrame()
		{
			currentFrame = Reverse ? Frames.Count - 1 : 0;

		}




		public List<Sprite> Frames;





		private void Update()
		{
			OnPlayMode();

		}

		void OnPlayMode()
		{
			if (isPlaying && Time.realtimeSinceStartup - (1 / (float)frameRate) >= lastFrame)
			{
				lastFrame = Time.realtimeSinceStartup;
				MoveNext();
			}
		}



		protected abstract void OnFrameChange();

		void MoveNext()
		{


			currentFrame = Reverse ? (currentFrame - 1 >= 0 ? currentFrame - 1 : Frames.Count - 1) : currentFrame + 1 < Frames.Count ? currentFrame + 1 : 0;

			if ((currentFrame == 0 || currentFrame == Frames.Count - 1))
			{
				onCompleteCallback?.Invoke();
				if (!Loop)
					isPlaying = false;
			}
			OnFrameChange();

		}

	}
}