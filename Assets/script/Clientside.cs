using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerIOClient;

public class Clientside : MonoBehaviour {
	public string name = "Test";
	public GameObject playerPrefab;
	private GameObject player;
	private Client client;
	private Connection connection;
	private List<PlayerIOClient.Message> msgList = new List<PlayerIOClient.Message>();


	void Start () {
		string userId = name + new System.Random ().Next (0, 10000);
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
					null,
					"GameRoom",
					true,
					null,
					null,
					delegate(Connection connection) {
						Debug.Log ("CreateJoinRoom");
						this.connection = connection;
					},
					delegate (PlayerIOError error) {
						Debug.Log("Error Joining Room: " + error.ToString());
					}
				);
			}
		);
	}

	void FixedUpdate() {
		foreach(PlayerIOClient.Message m in msgList) {
			switch(m.Type) {
			case "Joined":
				Debug.Log ("Send joined");
				player = GameObject.Instantiate (playerPrefab) as GameObject;
				player.transform.position = new Vector3 (m.GetFloat (1), m.GetFloat (2), 0);
				player.name = m.GetString (0);
				Debug.Log (player.name);
				break;
			case "PlayerJoined":
				Debug.Log ("Send PlayerJoined");
				GameObject otherPlayer = GameObject.Instantiate (playerPrefab) as GameObject;
				otherPlayer.transform.position = new Vector3 (m.GetFloat (1), m.GetFloat (2), 0);
				otherPlayer.name = m.GetString (0);
				otherPlayer.GetComponent<Control> ().enabled = false;
				Debug.Log (otherPlayer.name);
				//newplayer.transform.Find("NameTag").GetComponent<TextMesh>().text = m.GetString(0);
				break;
			case "Move":
				GameObject upplayer = GameObject.Find (m.GetString (0));
				upplayer.transform.LookAt (new Vector3 (m.GetFloat (1), 0, m.GetFloat (2)));
				upplayer.transform.eulerAngles = new Vector3 (0, upplayer.transform.eulerAngles.y, upplayer.transform.eulerAngles.z);

				float dist = Vector3.Distance (upplayer.transform.position, new Vector3 (m.GetFloat (1), 0, m.GetFloat (2)));
				connection.Send ("Move", playerPrefab.transform.position.x, playerPrefab.transform.position.y);
				break;
			case "Chat":
				
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
