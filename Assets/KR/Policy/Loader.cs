using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using KR;


public class Loader : MonoBehaviour {
	public class Data : KR.Serializable<Data>
    {
        public bool isPolicyAccepted;

    }
	public static Loader instance;
	public GameObject policyPopup;
	public Button policyBtn, acceptPolicyBtn;
	public string policyUrl;
	public float delay = 2f;

	public string sceneToLoad;

	//private void Awake()
	//{

	//	//DontDestroyOnLoad(gameObject);
	//	//instance = this;
	//}
	void CreateDelegates(){
		policyBtn.onClick.AddListener(() => {
			Application.OpenURL(policyUrl);
		});
		acceptPolicyBtn.onClick.AddListener(() => {
			Data.instance.isPolicyAccepted = true;
			LoadGame();
		}); 
	}
	void ValidatePolicy(){
		//if (!Data.instance.isPolicyAccepted)
		//{
		//	policyPopup.SetActive(true);
		//}
		//else
			LoadGame();
	}
	void  Start()
	{
        //CreateDelegates();
        ValidatePolicy();
    }

	void LoadGame(){

		policyPopup.SetActive(false);
		this.Schedule(delay, () => {
			//UI.LoadLevel();
			//SceneManager.LoadScene(sceneToLoad);
			//UI.MoveNext();	
		});
	}
	public void Destruct(){
		instance = null;
		Destroy(gameObject);

	}
}
