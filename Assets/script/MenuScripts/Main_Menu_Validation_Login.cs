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
    void Start() {
        okayEmail = true;
        okayPassword = true;

        emailError.enabled = !okayEmail;
        passwordError.enabled = !okayPassword;
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            onLogIn();
        }

        foreach (Message m in ms) {
            switch (m.Type) {
                case "Aut":
                    if (m.GetBoolean(0)) {
                        Debug.Log("Авторизация успешна!");
                        Application.LoadLevel("main_menu");
                    }
                    else {
                        Debug.Log("Авторизация не удалась!");
                    }
                    break;
                case "ErrAut":
                    Debug.Log(m.GetString(0).ToString());
                    break;
                case "Err":
                    Debug.Log(m.GetString(0).ToString());
                    break;
            }
        }
        ms.Clear();
    }
    public void onLogIn() {
        //TODO Alexander
        /** if feedback from server is false, 
        variables okayEmail and okayPassword should be false,
        (if bad email, okayEmail = false, if bad password, okayPassword = false)
        if data is correct, those variables should be true,
        variables email and password are having userdata from input fields*/

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

    Client client;
    Connection connect;
    List<Message> ms = new List<Message>();
    public void connectToServer() {
        PlayerIO.Connect("shooter-gpmw9uiee0uxk34a7hzp7w", "Public", name, null, null, null, delegate(Client cl) {
            Debug.Log("Подключились к серверу");
            cl.Multiplayer.CreateJoinRoom(System.DateTime.Now.Ticks.ToString(), "regAutRoom", true, null, null, delegate(Connection con) {
                connect = con;
                Debug.Log("Подключились к комнате");
                connect.OnMessage += connect_OnMessage;
                con.Send("Aut", email, password);
            }, delegate(PlayerIOError error) {
                Debug.Log("Ошибка подключения к комнате: " + error);
            });
        }, delegate(PlayerIOError err) {
            Debug.Log("Ошибка подключения: " + err);
        });
    }

    void connect_OnMessage(object sender, Message e) {
        ms.Add(e);
    }
}