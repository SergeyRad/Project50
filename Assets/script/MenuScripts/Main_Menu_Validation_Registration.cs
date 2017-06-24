using UnityEngine;
using System.Security.Cryptography;
using PlayerIOClient;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using PlayerIOClient;
using System.Text;

public class Main_Menu_Validation_Registration : MonoBehaviour {

    public Text inputEmail;
    public Text emailError;
    public Text inputPassword1;
    public Text password1Error;
    public Text inputPassword2;
    public Text password2Error;
    private string email;
    private string password1;
    private string password2;
    private bool okayEmail;
    private bool okayPassword1;
    private bool okayPassword2;

    void Start() {
        //connectToServer();
        okayEmail = false;
        okayPassword1 = false;
        okayPassword2 = false;

        emailError.enabled = !okayEmail;
        password1Error.enabled = !okayPassword1;
        password2Error.enabled = !okayPassword2;
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            onRegistration();
        }
    }

    public void onRegistration() {
        email = inputEmail.text;
        password1 = inputPassword1.text;
        password2 = inputPassword2.text;
        if (email.Length > 3 && checkEmail(email)) {
            okayEmail = true;
            Debug.Log("Email OK");
        }
        else {
            okayEmail = false;
        }
        if (!(password1.Contains(" ")) && password1.Length >= 6) {
            okayPassword1 = true;
            Debug.Log("Password OK");
        }
        else {
            okayPassword1 = false;
        }
        if (password1 == password2) {
            okayPassword2 = true;
            Debug.Log("Password2 OK");
        }
        else {
            okayPassword2 = false;
        }

        if (okayEmail && okayPassword1 && okayPassword2) {
            connectToServer();
            //connect.Send("Reg", email, password1);
        }

        emailError.enabled = !okayEmail;
        password1Error.enabled = !okayPassword1;
        password2Error.enabled = !okayPassword2;
    }
    public void onBack() {
        Application.LoadLevel("main_menu_validation");
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

    static string GetMd5Hash(string input)
    {
        using (MD5 md5Hash = MD5.Create())
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }

    public void connectToServer() {
		email = email.Replace ("@", " ");
        PlayerIO.Connect(
			"shooter-gpmw9uiee0uxk34a7hzp7w", 
			"public", 
			name, 
			null, 
			null, 
			null, 
			delegate(Client cl) {
            	Debug.Log("Подключились к серверу");

				cl.BigDB.LoadSingle (
					"users",
					"email",
					new object[]{email},
					delegate(DatabaseObject value) {
						if (value != null){
							Debug.Log ("Пользователь уже зарегистрирован!");
						} else {
							DatabaseObject obj = new DatabaseObject();
							obj.Set("email", email);
							obj.Set("password", GetMd5Hash(password1));
							cl.BigDB.CreateObject(
								"users",
								email,
								obj,
								delegate(DatabaseObject data) {
									data.Save();
									Application.LoadLevel("main_menu_validation_login");
								},
								delegate(PlayerIOError error) {
									Debug.Log("Ошибка регистрации: " + error);
								}
							);
						}
					},
					delegate(PlayerIOError err) {
						Debug.Log("Ошибка регистрации: " + err);
					}
				);
            	
	        }, delegate(PlayerIOError err) {
	            Debug.Log("Ошибка подключения: " + err);
	        }
		);
    }
}