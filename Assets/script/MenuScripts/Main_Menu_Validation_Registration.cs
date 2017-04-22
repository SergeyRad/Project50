using UnityEngine;
using System.Collections;
using PlayerIOClient;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;

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
        connectToServer();
        okayEmail = true;
        okayPassword1 = true;
        okayPassword2 = true;

        emailError.enabled = !okayEmail;
        password1Error.enabled = !okayPassword1;
        password2Error.enabled = !okayPassword2;
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            onRegistration();
        }

        foreach (Message m in ms) {
            switch (m.Type) {
                case "RegApply":
                    if (m.GetBoolean(0)) {
                        Debug.Log("Регистрация успешна!");
                        Application.LoadLevel("main_menu_validation_login");
                    }
                    else {
                        Debug.Log("Регистрация не удалась!");
                    }
                    break;
                case "Err":
                    Debug.Log(m.GetString(0).ToString());
                    break;
            }
        }
        ms.Clear();
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
            //connectToServer();
            connect.Send("Reg", email, password1);
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