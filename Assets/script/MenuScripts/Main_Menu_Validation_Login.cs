using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using PlayerIOClient;
using System.Collections.Generic;
public class Main_Menu_Validation_Login : MonoBehaviour {

    public Text inputEmail;
    public Text emailError;
    public Text inputPassword;
    public Text passwordError;
    private string email;
    private string password;
    private bool okayEmail;
    private bool okayPassword;
	private bool connected = false;
	private Client client;

    void Start() {
        okayEmail = true;
        okayPassword = true;

        emailError.enabled = !okayEmail;
        passwordError.enabled = !okayPassword;
    }

    public void onLogIn() {
        email = inputEmail.text;
        password = inputPassword.text;
        if (email.Length > 3 && checkEmail(email)) {
            okayEmail = true;
        }
        else {
            okayEmail = false;
        }
        if (!(password.Contains(" ")) && password.Length >= 6) {
            okayPassword = true;
        }
        else {
            okayPassword = false;
        }
        if (okayEmail && okayPassword) {
            connectToServer();
        }

        emailError.enabled = !okayEmail;
        passwordError.enabled = !okayPassword;
    }
    public void onBack() {
        Application.LoadLevel("main_menu_validation");
    }
    public void onRegister() {
        Application.LoadLevel("main_menu_validation_registration");
    }

    bool checkEmail(string str) {
        try {
            return Regex.IsMatch(str,
                  @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                  RegexOptions.IgnoreCase);
        }
        catch {
            return false;
        }
    }

    
    public void connectToServer() {
		email = email.Replace ("@", " ");
		if (!connected)
			PlayerIO.Connect (
				"shooter-gpmw9uiee0uxk34a7hzp7w", 
				"Public", 
				email, 
				null, 
				null, 
				null, 
				delegate(Client cl) {
					connected = true;
					client = cl;
					Debug.Log ("Подключились к серверу");
					client.BigDB.LoadSingle (
						"users",
						"email",
						new object[]{email},
						delegate(DatabaseObject value) {
							if (value != null){
								Debug.Log ("Авторизация успешна!");
								Settings.email = email;
								Application.LoadLevel ("main_menu");
							} else {
								Debug.Log ("Неправильный пароль!");
							}
						},
						delegate(PlayerIOError err) {
							Debug.Log ("Пользователь не найден!");
						}
					);
            	
				}, delegate(PlayerIOError err) {
				Debug.Log ("Ошибка подключения: " + err);
			}
		);
		else {
			client.BigDB.LoadSingle (
				"users",
				"email",
				new object[]{email},
				delegate(DatabaseObject value) {
					if (value != null){
						if (string.Equals(value.GetString("password"), password) ){
							Debug.Log ("Авторизация успешна!");
							Settings.email = email;
							Application.LoadLevel ("main_menu");
						} else {
							Debug.Log ("Неправильный пароль!" + value.GetString("password"));
						}
					} else {
						Debug.Log ("Пользователь не найден!" + value.GetString("password"));
					}
				},
				delegate(PlayerIOError err) {
					Debug.Log ("Пользователь не найден!");
				}
			);
		}
    }
}