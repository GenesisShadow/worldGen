using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
	public GameObject MenuUI;
	public GameObject GameUI;
	public GameObject DebugUI;
	public GameObject WorldObjects;
	public GameObject MenuObjects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

	public void SetGameUI(){
		MenuUI.SetActive(false);
		GameUI.SetActive(true);
		MenuObjects.SetActive(false);
		WorldObjects.SetActive(true);
	}

	public void SetMenuUI(){
		MenuUI.SetActive(true);
		GameUI.SetActive(false);
		MenuObjects.SetActive(true);
		WorldObjects.SetActive(false);
	}
}
