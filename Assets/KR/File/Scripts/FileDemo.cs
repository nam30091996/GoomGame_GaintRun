using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KR;
namespace FileDemo{
public class FileDemo : MonoBehaviour {
		public class DataDemo : KR.Serializable{
			public int gold;
		}
		DataDemo data;
		DataDemo data1;
		void Start(){
			OpenOrCreateData ();
//			data.gold = 100; 
//			data1.gold = 50;
			ReadData ();
		}
		void OpenOrCreateData(){
			data = KR.File.Open<DataDemo> ();
			//data1 = KR.File.Open<DataDemo> ("anotherData");
		}
		void ReadData(){
			
			Debug.Log (data.gold);
			//Debug.Log (data1.gold);
			data.gold += 1; 
		}
	}

}
