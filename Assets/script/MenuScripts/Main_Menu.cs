using UnityEngine;
using PlayerIOClient;
using System.Collections;
using System.Collections.Generic;

public class Main_Menu : MonoBehaviour
{

    private Connection connection;
    private Client client;
    private List<PlayerIOClient.Message> msgList = new List<PlayerIOClient.Message>();

    void Start()
    {
        QualitySettings.SetQualityLevel(Settings.graphic);
        AudioListener.volume = Settings.music_volume;
        PlayerIO.Authenticate(
            "shooter-gpmw9uiee0uxk34a7hzp7w",
            "public",
            new Dictionary<string, string> { { "userId", Settings.email } },
            null,
            delegate (Client client)
            {
                Debug.Log("Authenticate");
                this.client = client;
                this.client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);
                this.client.Multiplayer.CreateJoinRoom(
                    null,
                    "LobbyRoom",
                    true,
                    null,
                    null,
                    delegate (Connection connection)
                    {
                        Debug.Log("Create LobbyRoom");
                        this.connection = connection;
                        connection.OnMessage += handlemessage;
                    },
                    delegate (PlayerIOError error)
                    {
                        Debug.Log("Error Joining Room: " + error.ToString());
                    }
                );
            }
        );
    }

    void handlemessage(object sender, PlayerIOClient.Message m)
    {
        msgList.Add(m);
    }

    public void OnPlay()
    {
        Debug.Log("Click");
        connection.Send("Access");
    }
    public void OnExit()
    {
        print("quit");
        this.connection.Disconnect();
        this.client.Logout();
        Application.Quit();
    }
    public void OnSetting()
    {
        Application.LoadLevel("main_menu_settings");
    }

    void FixedUpdate()
    {
        foreach (PlayerIOClient.Message m in msgList)
        {
            switch (m.Type)
            {
                case "Access":
                    PlayerPrefs.SetString("roomId", m.GetString(0));
                    Application.LoadLevel("main");
                    break;
            }
        }
    }
}
