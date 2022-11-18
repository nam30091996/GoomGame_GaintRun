using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Alerter : KR.ManagerSingleton<Alerter>
{
    public Text alert;
    public GameObject alertPanel;
    private void Start()
    {
        alertPanel.GetComponent<Button>().onClick.AddListener(CloseAlert);
    }


    public void ShowAlert(string msg)
    {
        alert.text = msg;
        alertPanel.SetActive(true);
        Time.timeScale = 0;
    }
    void CloseAlert()
    {
        Time.timeScale = 1;
        alertPanel.gameObject.SetActive(false);
    }
}
