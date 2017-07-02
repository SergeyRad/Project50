using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using PlayerIOClient;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Main_Menu_Validation : MonoBehaviour {

    // Those fields related to main page
    public GameObject main;
    public GameObject login;
    public GameObject registration;

    // Those fields related to login page
    public Text inputEmailLogin;
    public Text emailErrorLogin;
    public Text inputPassword;
    public Text passwordError;
    public InputField inputEmailLoginField;
    public InputField inputPasswordField;

    private bool okayEmailLogin;
    private bool okayPassword;
    private bool connected = false;
    private string emailLogin;
    private string password;

    private Client client;
    private List<PlayerIOClient.Message> msgList = new List<PlayerIOClient.Message>();

    // Those fields related to registration page
    public Text inputEmail;
    public Text emailError;
    public Text inputPassword1;
    public Text password1Error;
    public Text inputPassword2;
    public Text password2Error;
    public InputField inputEmailField;
    public InputField inputPassword1Field;
    public InputField inputPassword2Field;

    private bool okayEmail;
    private bool okayPassword1;
    private bool okayPassword2;
    private string email;
    private string password1;
    private string password2;


    private void Start() {
        // Login page
        okayEmailLogin = true;
        okayPassword = true;

        emailErrorLogin.enabled = !okayEmailLogin;
        passwordError.enabled = !okayPassword;

        // Registration page
        //connectToServer();
        okayEmail = false;
        okayPassword1 = false;
        okayPassword2 = false;

        emailError.enabled = !okayEmail;
        password1Error.enabled = !okayPassword1;
        password2Error.enabled = !okayPassword2;
    }

    // Login method
    public void onLogin() {
        emailLogin = inputEmailLogin.text;
        password = inputPassword.text;

        if (emailLogin.Length > 3 && checkEmail(emailLogin))
            okayEmailLogin = true;
        else
            okayEmailLogin = false;
        if (!(password.Contains(" ")) && password.Length >= 6)
            okayPassword = true;
        else
            okayPassword = false;
        if (okayEmailLogin && okayPassword)
            connectToServer();

        emailErrorLogin.enabled = !okayEmailLogin;
        passwordError.enabled = !okayPassword;
    }

    // Registration method
    public void onRegistration() {
        email = inputEmail.text;
        password1 = inputPassword1.text;
        password2 = inputPassword2.text;
        if (email.Length > 3 && checkEmail(email)) {
            okayEmail = true;
            Debug.Log("Email OK");
        } else
            okayEmail = false;

        if (!(password1.Contains(" ")) && password1.Length >= 6) {
            okayPassword1 = true;
            Debug.Log("Password OK");
        } else
            okayPassword1 = false;

        if (password1 == password2) {
            okayPassword2 = true;
            Debug.Log("Password2 OK");
        } else
            okayPassword2 = false;

        if (okayEmail && okayPassword1 && okayPassword2) {
            connectToServer();
            //connect.Send("Reg", email, password1);
        }

        emailError.enabled = !okayEmail;
        password1Error.enabled = !okayPassword1;
        password2Error.enabled = !okayPassword2;
    }

    static string GetMd5Hash(string input) {
        using (MD5 md5Hash = MD5.Create()) {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));
            return sBuilder.ToString();
        }
    }

    bool checkEmail(string str) {
        try {
            return Regex.IsMatch(str,
                  @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                  RegexOptions.IgnoreCase);
        } catch {
            return false;
        }
    }

    public void connectToServer() {
        emailLogin = emailLogin.Replace("@", " ");
        if (!connected)
            PlayerIO.Connect(
                "shooter-gpmw9uiee0uxk34a7hzp7w",
                "Public",
                emailLogin,
                null,
                null,
                null,
                delegate (Client cl) {
                    connected = true;
                    client = cl;
                    Debug.Log("Подключились к серверу");
                    client.BigDB.LoadSingle(
                        "users",
                        "email",
                        new object[] { emailLogin },
                        delegate (DatabaseObject value) {
                            if (value != null) {
                                Debug.Log("Авторизация успешна!");
                                Settings.email = emailLogin;
                                SceneManager.LoadScene("main_menu");
                            } else
                                Debug.Log("Неправильный пароль!");

                        },
                        delegate (PlayerIOError err) {
                            Debug.Log("Пользователь не найден!");
                        }
                    );

                }, delegate (PlayerIOError err) {
                    Debug.Log("Ошибка подключения: " + err);
                }
        );
        /*PlayerIO.Connect(
            "shooter-gpmw9uiee0uxk34a7hzp7w",
            "Public",
            email,
            null,
            null,
            null,
            delegate (Client cl) {
                Debug.Log("Authenticate");
                this.client = client;
                client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);
                this.client.Multiplayer.CreateJoinRoom(
                    null,
                    "LobbyRoom",
                    true,
                    null,
                    null,
                    delegate (Connection connection)
                    {
                        connection.Send("Login");
                        connection.OnMessage += handlemessage;
                    },
                    delegate (PlayerIOError error)
                    {
                        Debug.Log("Error Joining Room: " + error.ToString());
                    }
                );
            }
        );*/
        else {
            client.BigDB.LoadSingle(
                "users",
                "email",
                new object[] { emailLogin },
                delegate (DatabaseObject value) {
                    if (value != null) {
                        if (string.Equals(value.GetString("password"), password)) {
                            Debug.Log("Авторизация успешна!");
                            Settings.email = emailLogin;
                            SceneManager.LoadScene("main_menu");
                        } else
                            Debug.Log("Неправильный пароль!" + value.GetString("password"));
                    } else
                        Debug.Log("Пользователь не найден!" + value.GetString("password"));
                },
                delegate (PlayerIOError err) {
                    Debug.Log("Пользователь не найден!");
                }
            );
        }
    }
    void handlemessage(object sender, PlayerIOClient.Message m) {
        msgList.Add(m);
    }

    // Hotkeys listener
    void Update() {
        // Enter listener
        if (Input.GetKeyDown(KeyCode.Return))
            if (main.activeSelf && !login.activeSelf && !registration.activeSelf) // main menu
                onLoginMenu();
            else if (!main.activeSelf && login.activeSelf && !registration.activeSelf) // login menu
                onLogin();
            else if (!main.activeSelf && !login.activeSelf && registration.activeSelf) // registration menu
                onRegistration();

        // Escape listener
        if (Input.GetKeyDown(KeyCode.Escape))
            if(main.activeSelf && !login.activeSelf && !registration.activeSelf) // main menu
                // TODO: warning window, in which user will be asken will he quit the game - yes or not
                Debug.Log("TODO: warning window, in which user will be asken will he quit the game - yes or not");
            else if(!main.activeSelf && login.activeSelf && !registration.activeSelf) // login menu
                onBack();
            else if(!main.activeSelf && !login.activeSelf && registration.activeSelf)  // registration menu
                onBack();

        // Tab listener
        if(Input.GetKeyDown(KeyCode.Tab))
                if(EventSystem.current.currentSelectedGameObject == inputEmailLoginField.gameObject)
                    EventSystem.current.SetSelectedGameObject(inputPasswordField.gameObject);
                else if(EventSystem.current.currentSelectedGameObject == inputPasswordField.gameObject)
                    EventSystem.current.SetSelectedGameObject(inputEmailLoginField.gameObject);
                else if(EventSystem.current.currentSelectedGameObject == inputEmailField.gameObject)
                    EventSystem.current.SetSelectedGameObject(inputPassword1Field.gameObject);
                else if(EventSystem.current.currentSelectedGameObject == inputPassword1Field.gameObject)
                    EventSystem.current.SetSelectedGameObject(inputPassword2Field.gameObject);
                else if(EventSystem.current.currentSelectedGameObject == inputPassword2Field.gameObject)
                    EventSystem.current.SetSelectedGameObject(inputEmailField.gameObject);
    }


    // Menu buttons methods
    public void onLoginMenu() {
        main.SetActive(false);
        registration.SetActive(false);
        login.SetActive(true);
        EventSystem.current.SetSelectedGameObject(inputEmailLoginField.gameObject);
    }
    public void onRegistrationMenu() {
        main.SetActive(false);
        login.SetActive(false);
        registration.SetActive(true);
        EventSystem.current.SetSelectedGameObject(inputEmailField.gameObject);
    }
    public void onBack() {
        registration.SetActive(false);
        login.SetActive(false);
        main.SetActive(true);
    }
}
