using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KR
{
	[System.Flags]
	public enum LogType
	{
		Info = 0x01 << 0x00,
		Error = 0x01 << 0x01,
	}

	[System.Flags]
	public enum SaveEvent
	{
		OnQuit = 0x00 << 0x00,
		OnPause = 0x01 << 0x00,
	}

	public class Device : KR.ScriptableSingleton<Device>
	{

#if UNITY_EDITOR
		[MenuItem("KR/File")]
		public static void FileSetting()
		{
			KR.Scriptable.CreateAsset<Device>("Assets/KR/File/Resources/");
		}
#endif

		[EnumFlags]
		public LogType logType;

		[EnumFlags]
		public SaveEvent saveEvent;

		[HideWhenPlaying]
		public string fileDirectory = "/KR/File/Files";
		[HideWhenPlaying]
		public string fileType = ".ea";


		//[ValidateInput("PasswordHashVal", "Password hash size must equal 20")]
		public string passwordHash = "11010101010101010101";

		//[ValidateInput("SaltKeyVal", "Salt key size must equal 18")]
		public string saltKey = "010101010101010110";

		//[ValidateInput("VIKeyVal", "VI key size must equal 16")]
		public string viKey = "0101010101011010";


		private bool PasswordHashVal(string ph){
			return ph.Length == 20;
		}
		private bool SaltKeyVal(string sk)
        {
			return sk.Length == 18;
        }
		public bool VIKeyVal (string vi){
			return vi.Length == 16;
		}



		public bool encrypt;
			

		public static string filePath<T>() where T : ISerializable
		{
			return (Path.Combine(path(instance.fileDirectory), typeof(T).Name.insert("(", ")") + instance.fileType));
		}
		public static string filePath(string fileName)
		{
			return (Path.Combine(path(instance.fileDirectory), fileName));
		}


		public static string path(string directory)
		{

#if !UNITY_EDITOR
		return Application.persistentDataPath;
#else
			if (!Directory.Exists(Application.dataPath + directory))
				Directory.CreateDirectory(Application.dataPath + directory);
			return Application.dataPath + directory;
#endif

		}
	}
}