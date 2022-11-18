
using System.Collections.Generic;
using UnityEngine;
namespace KR
{

	public class FileDelegate : KR.Scripton<FileDelegate>
	{
        
		[System.Serializable]
		public struct Data {
			public string path;
			[TextArea]
			public string jsonData;

			public Data (string path, string jsonData){
				this.path = path;
				this.jsonData = jsonData;
			}
		}

		public  event System.Action<bool> onPause = delegate { };
		public  event System.Action onQuit = delegate { };
		[KR.ReadOnly]
		public  List<Data> openedFiles = new List<Data>();


		void OnApplicationQuit()
		{
			onQuit.Invoke();
		}
		void OnApplicationPause(bool status)
		{
			onPause.Invoke(status);

		}
	


	}
}