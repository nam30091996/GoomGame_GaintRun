//-----------------------------------------------------
//            Arbor 3: FSM & BT Graph Editor
//		  Copyright(c) 2014-2020 caitsithware
//-----------------------------------------------------
namespace ArborEditor.UpdateCheck
{
	[System.Reflection.Obfuscation(Exclude = true)]
	[System.Serializable]
	public sealed class VersionInfo
	{
		public enum BuildType
		{
			Release,
			Patch,
			Trial,
			Beta,
		}

		public BuildType buildType = BuildType.Release;
		public string version = string.Empty;
		public string baseVersion = string.Empty;
		public string storeURL = string.Empty;
	}
}