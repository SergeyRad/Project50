using UnityEngine;
using PlayerIOClient;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class Main_Menu : MonoBehaviour {

    private Connection connection;
    private Client client;
    private List<PlayerIOClient.Message> msgList = new List<PlayerIOClient.Message>();

    public Slider graphics;
    public Slider sound_volume;
    public Slider music_volume;


    public GameObject main;
    public GameObject settings;



    void Start() {
        QualitySettings.SetQualityLevel(Settings.graphic);
        AudioListener.volume = Settings.music_volume;
        Settings.music_volume = music_volume.value;
        Settings.graphic = Mathf.RoundToInt(graphics.value);
        PlayerIO.Authenticate(
            "shooter-gpmw9uiee0uxk34a7hzp7w",
            "public",
            new Dictionary<string, string> { { "userId", Settings.email } },
            null,
            delegate (Client client) {
                Debug.Log("Authenticate");
                this.client = client;
                this.client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);
                this.client.Multiplayer.CreateJoinRoom(
                    null,
                    "LobbyRoom",
                    true,
                    null,
                    null,
                    delegate (Connection connection) {
                        Debug.Log("Create LobbyRoom");
                        this.connection = connection;
                        connection.OnMessage += handlemessage;
                    },
                    delegate (PlayerIOError error) {
                        Debug.Log("Error Joining Room: " + error.ToString());
                    }
                );
            }
        );
    }

    void handlemessage(object sender, PlayerIOClient.Message m) {
        msgList.Add(m);
    }

    public void OnPlay() {
        Debug.Log("Click");
        connection.Send("Access");
    }
    public void OnExit() {
        print("quit");
        this.connection.Disconnect();
        this.client.Logout();
        Application.Quit();
    }
    public void onBack() {
        main.SetActive(true);
        settings.SetActive(false);
    }
    public void OnSetting() {
        main.SetActive(false);
        settings.SetActive(true);
    }

    void FixedUpdate() {
        foreach (PlayerIOClient.Message m in msgList) {
            switch (m.Type) {
                case "Access":
                PlayerPrefs.SetString("roomId", m.GetString(0));
                SceneManager.LoadScene("main");
                break;
            }
        }
    }
    void Update() {
        QualitySettings.SetQualityLevel(Settings.graphic);
        AudioListener.volume = Settings.music_volume;
        Settings.sound_volume = sound_volume.value;
        Settings.music_volume = music_volume.value;
        Settings.graphic = Mathf.RoundToInt(graphics.value);
    }
}
