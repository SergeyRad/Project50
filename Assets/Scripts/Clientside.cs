using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerIOClient;


public class Clientside : MonoBehaviour {

    public float rot = 0f;
    public string room_name = "Test";

    public Canvas menu;
    public GameObject weapon;

    public Texture2D[] numeral = new Texture2D[10];
    public GameObject[] bullet = new GameObject[10];
    public GameObject[] playerPrefab = new GameObject[10];

    private int teamId = 0;

    private Client client;
    private GameObject player;
    private Connection connection;

    private const int SERVER_PORT = 8184;
    private const string SERVER_IP = "localhost";
    private const string MAX_PLAYERS = "16";
    private const string GAME_ID = "shooter-gpmw9uiee0uxk34a7hzp7w";


    private Dictionary<string, string> options = new Dictionary<string, string>();
    private List<PlayerIOClient.Message> msgList = new List<PlayerIOClient.Message>();

    public GUIStyle style = new GUIStyle();

    void Start() {
        options.Add("maxplayers", MAX_PLAYERS);
        string userId = Settings.email;
        PlayerIOClient.PlayerIO.Authenticate(
            GAME_ID,
            "public",
            new Dictionary<string, string> { { "userId", userId } },
            null,
            delegate (Client client) {
                Debug.Log("Authenticate");
                this.client = client;
                client.Multiplayer.DevelopmentServer = new ServerEndpoint(SERVER_IP, SERVER_PORT);
                client.Multiplayer.CreateJoinRoom(
                    "room114.3668",
                    "GameRoom",
                    true,
                    options,
                    null,
                    delegate (Connection connection) {
                        this.connection = connection;
                        this.connection.OnMessage += handlemessage;
                        connection.Send("Create", client.ConnectUserId);
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

    public void Hit(string name) {
        if (name == player.name)
            connection.Send("hit", player.name);
    }

    void FixedUpdate() {

        if (player != null) {
            float angle = player.transform.eulerAngles.z > 180 ? player.transform.eulerAngles.z - 360 : player.transform.eulerAngles.z;
            connection.Send("Move", player.name, player.transform.position.x, player.transform.position.y, angle);
        }
        foreach (PlayerIOClient.Message m in msgList) {
            switch (m.Type) {
                case "PlayerJoined":
                Debug.Log("It's not me");
                GameObject otherPlayer = GameObject.Instantiate(playerPrefab[m.GetInt(4) - 1]) as GameObject;
                otherPlayer.transform.position = new Vector3(m.GetFloat(1), m.GetFloat(2), 0);
                otherPlayer.transform.rotation = new Quaternion(0, 0, m.GetFloat(3), 0);
                otherPlayer.name = m.GetString(0);
                otherPlayer.GetComponent<Control>().enabled = false;
                Debug.Log(otherPlayer.name);
                break;
                case "Create":
                if (player == null) {
                    Debug.Log("It's me");
                    teamId = m.GetInt(4) - 1;
                    player = (GameObject)Instantiate(playerPrefab[teamId]);
                    player.transform.position = new Vector3(m.GetFloat(1), m.GetFloat(2), 0);
                    player.name = m.GetString(0);
                    player.GetComponent<Control>().enabled = true;
                    player.GetComponent<Control>().player = player;
                    Debug.Log(player.name);
                }
                break;
                case "Move":
                GameObject upplayer = GameObject.Find(m.GetString(0));
                if (upplayer != null) {
                    upplayer.transform.position = new Vector3(m.GetFloat(1), m.GetFloat(2), 0);
                    upplayer.transform.eulerAngles = new Vector3(0, 0, m.GetFloat(3));
                }
                break;
                case "Attack":
                GameObject othplayer = GameObject.Find(m.GetString(0));
                if (othplayer != null && player != null) {
                    float posX = othplayer.transform.position.x + (Mathf.Cos((othplayer.transform.localEulerAngles.z - 90) * Mathf.Deg2Rad)) * -player.GetComponent<Control>().shootingSpeed;
                    float posY = othplayer.transform.position.y + (Mathf.Sin((othplayer.transform.localEulerAngles.z - 90) * Mathf.Deg2Rad)) * -player.GetComponent<Control>().shootingSpeed;
                    GameObject game_bullet = Instantiate(bullet[m.GetInt(1)], othplayer.transform.position, othplayer.transform.rotation) as GameObject;
                    game_bullet.GetComponent<Bullet>().master = othplayer;
                    game_bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(posX, posY));
                }
                break;
                case "PlayerLeft":
                Destroy(GameObject.Find(m.GetString(0)));
                break;
                case "hp":
                if (client.ConnectUserId == m.GetString(0))
                    player.GetComponent<Control>().SetHealth(m.GetInt(1));
                break;
                case "Death":
                if (m.GetString(0) == player.name) {
                    player.GetComponent<Control>().SetHealth(0);
                    Destroy(player);
                    StartCoroutine(Reborn());
                } else {
                    Destroy(GameObject.Find(m.GetString(0)));
                }
                break;
                case "Reborn":
                if (m.GetString(0) == client.ConnectUserId) {
                    player = (GameObject)Instantiate(playerPrefab[teamId]);
                    player.transform.position = new Vector3(m.GetFloat(1), m.GetFloat(2), 0);
                    player.name = m.GetString(0);
                    player.GetComponent<Control>().enabled = true;
                    player.GetComponent<Control>().player = player;
                    player.GetComponent<Control>().SetHealth(100);
                } else {
                    GameObject rPlayer = GameObject.Instantiate(playerPrefab[m.GetInt(4) - 1]) as GameObject;
                    rPlayer.transform.position = new Vector3(m.GetFloat(1), m.GetFloat(2), 0);
                    rPlayer.transform.rotation = new Quaternion(0, 0, m.GetFloat(3), 0);
                    rPlayer.name = m.GetString(0);
                    rPlayer.GetComponent<Control>().enabled = false;
                }
                break;
            }
        }

        msgList.Clear();
    }

    public void Attack() {
        connection.Send("Attack", player.name, teamId);
    }

    IEnumerator Reborn() {
        yield return new WaitForSeconds(3f);
        connection.Send("Reborn");
    }
}
