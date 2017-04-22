using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

	public Control control;
	
	void On_Resume(){
		control.menu.enabled = control.in_menu = !control.in_menu;
	}
	void On_Settings(){

	}
	void On_Quit(){
		Application.LoadLevel("main_menu");
	}
}
