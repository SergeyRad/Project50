using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerIOClient;

public class Clientside : MonoBehaviour {
	public string name = "Test";
	public GameObject playerPrefab;
	public GameObject bullet;
	public GameObject weapon;

    public float rot = 0;

	private GameObject player;
	private Client client;
	private Connection connection;
	private List<PlayerIOClient.Message> msgList = new List<PlayerIOClient.Message>();
	private Dictionary<string,string> options = new Dictionary<string, string> ();
    
    void Start () {
		options.Add ("maxplayers", "16");
		string userId = Settings.email;
		PlayerIOClient.PlayerIO.Authenticate(
			"shooter-gpmw9uiee0uxk34a7hzp7w",
			"public",
			new Dictionary<string, string>{{"userId", userId}},
			null,
			delegate(Client client) {
				Debug.Log ("Authenticate");
				this.client = client;
				client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost",8184);
                client.Multiplayer.CreateJoinRoom(
                    Settings.roomId,
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

	public void SendAttack(){
		connection.Send ("Attack", player.name, player.transform.position.x, player.transform.position.y);
	}

	void handlemessage(object sender, PlayerIOClient.Message m) {
		msgList.Add(m);
	}

	void FixedUpdate() {
        
        if (player != null) {
            connection.Send("Move", player.name, player.transform.position.x, player.transform.position.y, player.transform.rotation.z);
            //player.transform.rotation = new Quaternion(0f, 0f, rot, 0f);
        }
		foreach(PlayerIOClient.Message m in msgList) {
            switch(m.Type) {
			case "PlayerJoined":
                Debug.Log("It's not me");
                GameObject otherPlayer = GameObject.Instantiate(playerPrefab) as GameObject;
                otherPlayer.transform.position = new Vector3(m.GetFloat(1), m.GetFloat(2), 0);
                otherPlayer.name = m.GetString(0);
                otherPlayer.GetComponent<Control>().enabled = false;
                Debug.Log(otherPlayer.name);
                //newplayer.transform.Find("NameTag").GetComponent<TextMesh>().text = m.GetString(0);
                break;
             case "Create":
                if (player == null) { 
                    Debug.Log("It's me");
                    player = (GameObject)Instantiate(playerPrefab);
                    player.transform.position = new Vector3(m.GetFloat(1), m.GetFloat(2), 0);
                    player.name = m.GetString(0);
                    player.GetComponent<Control>().enabled = true;
                    gameObject.GetComponent<Cam>().player = player.transform;
                    Debug.Log(player.name);
                }
				break;
			case "Move":
				GameObject upplayer = GameObject.Find (m.GetString (0));
                //upplayer.transform.LookAt (new Vector3 (m.GetFloat (1), 0, m.GetFloat (2)));
                //upplayer.transform.eulerAngles = new Vector3 (0, upplayer.transform.eulerAngles.y, upplayer.transform.eulerAngles.z);
                upplayer.transform.position = new Vector3(m.GetFloat(1), m.GetFloat(2), 0);
                upplayer.transform.eulerAngles = new Vector3(0,0,m.GetFloat(3));
				//float dist = Vector3.Distance (upplayer.transform.position, new Vector3 (m.GetFloat (1), 0, m.GetFloat (2)));
				//connection.Send ("Move", playerPrefab.transform.position.x, playerPrefab.transform.position.y);
				break;
			case "Attack":
				GameObject othplayer = GameObject.Find (m.GetString (0));
				float posX = othplayer.transform.position.x + (Mathf.Cos((othplayer.transform.localEulerAngles.z - 90) * Mathf.Deg2Rad)) * -player.GetComponent<Control>().shooting_speed;
				float posY = othplayer.transform.position.y + (Mathf.Sin((othplayer.transform.localEulerAngles.z - 90) * Mathf.Deg2Rad)) * -player.GetComponent<Control>().shooting_speed;
				GameObject game_bullet = Instantiate(bullet, othplayer.transform.position, othplayer.transform.rotation) as GameObject;
				game_bullet.GetComponent<Bullet>().master = othplayer;
				game_bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(posX, posY));
				break;
			case "PlayerLeft":
				GameObject playerd = GameObject.Find(m.GetString(0));
				Destroy(playerd);
				break;
			}
		}

		// clear message queue after it's been processed
		msgList.Clear();
	}
}
