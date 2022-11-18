
namespace KR
{
	public class AudioInitiator : KR.ManagerSingleton<AudioInitiator>
	{

		protected override void Awake()
		{
			base.Awake();
			KR.Audio.Init();
		}
	}
}