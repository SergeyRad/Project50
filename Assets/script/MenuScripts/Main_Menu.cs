using UnityEngine;
using PlayerIOClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Main_Menu : MonoBehaviour
{
	public Slider graphics;
	public Slider sound_volume;
	public Slider music_volume;
    private Connection connection;
    private Client client;
    private List<PlayerIOClient.Message> msgList = new List<PlayerIOClient.Message>();
    private bool access = false;
    public GameObject main;
    public GameObject settings;
    
    void Start()
    {
        QualitySettings.SetQualityLevel(Settings.graphic);
        AudioListener.volume = Settings.music_volume;
        PlayerIOClient.PlayerIO.Authenticate(
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

    public void onPlay()
    {
        Debug.Log("Click");
        client.BigDB.LoadSingle(
            "roomsId", 
            "isFull", 
            new object[] { false }, 
            delegate(DatabaseObject result) {
                if (result != null)
                {
                    Debug.Log("Find room: " + result.GetString("id"));
                    result.Set("countPlayers", (result.GetInt("countPlayers") + 1));
                    if (result.GetInt("countPlayers") == 16)
                    {
                        result.Set("isFull", true);
                    }
                    result.Save();
                    connection.Send("Access", result.Key);
                }
                else
                {
                    Debug.Log("Not find room");
                    DatabaseObject obj = new DatabaseObject();
                    obj.Set("id", client.ConnectUserId);
                    obj.Set("countPlayers", 1);
                    obj.Set("isFull", false);
                    client.BigDB.CreateObject(
                        "roomsId",
                        "room" + (Time.deltaTime + Time.time),
                        obj,
                        delegate (DatabaseObject res)
                        {
                            Debug.Log("Create room");
                            res.Save();
                            connection.Send("Access", res.GetString("id"));
                        }
                    );
                }
            },
            delegate(PlayerIOError err) {
                Debug.Log("Not find room");
                DatabaseObject obj = new DatabaseObject();
                obj.Set("id", client.ConnectUserId);
                obj.Set("countPlayers", 1);
                obj.Set("isFull", false);
                client.BigDB.CreateObject(
                    "roomsId",
                    "room" + (Time.deltaTime + Time.time),
                    obj,
                    delegate(DatabaseObject result) {
                        Debug.Log("Create room");
                        result.Save();
                        connection.Send("Access", result.GetString("id"));
                    }
                );
            }
        );
    }
    public void onExit()
    {
        print("quit");
        this.connection.Disconnect();
        this.client.Logout();
        Application.Quit();
    }
    public void onSetting()
    {
        main.SetActive(false);
        settings.SetActive(true);
    }

    void FixedUpdate()
    {
        foreach (PlayerIOClient.Message m in msgList)
        {
            switch (m.Type)
            {
                case "Access":
                    this.access = true;
                    Settings.roomId = m.GetString(0);
                    Application.LoadLevel("main");
                    break;
                case "Attack":

                    break;
                case "PlayerLeft":

                    break;
            }
        }
    }
    public void onBack(){
        main.SetActive(true);
        settings.SetActive(false);
	}
    void Update () {
		QualitySettings.SetQualityLevel(Settings.graphic);
		AudioListener.volume = Settings.music_volume;
		Settings.sound_volume = sound_volume.value;
		Settings.music_volume = music_volume.value;
		Settings.graphic = Mathf.RoundToInt(graphics.value);
	}	
}
