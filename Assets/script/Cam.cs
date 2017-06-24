using UnityEngine;
using System.Collections;

public class Cam : MonoBehaviour {

	public Transform player;
	private Vector3 mousePosition;
	private Vector3 direction;	
	public float mouseEffect = -0.1f; 
	void Update(){
        if (player)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction.x = player.position.x + (player.position.x - mousePosition.x) * mouseEffect;
            direction.y = player.position.y + (player.position.y - mousePosition.y) * mouseEffect;
            direction.z = transform.position.z;
            transform.position = direction;
        }
	}
	
}