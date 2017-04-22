using UnityEngine;
using System.Collections;

public class Main_Menu_Validation : MonoBehaviour {

	public void onLogin(){
		Application.LoadLevel("main_menu_validation_login");
	}
	public void onRegistration(){
		Application.LoadLevel("main_menu_validation_registration");
	}
	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update(){
		if(Input.GetKeyDown(KeyCode.Return)){
			onLogin();
		}
	}
	
}
