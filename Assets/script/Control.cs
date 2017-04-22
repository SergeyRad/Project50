using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

	public bool is_bot = false;
    private float vertical;
	private float horizontal;
	private Vector3 mouse_position;
	private float x, y;
	public GameObject bullet;
	public GameObject weapon;
	public float shooting_speed;
	public float force = 15;
	private Vector2 force_point;
	private Rigidbody2D cache_rigidbody;
	public SpriteRenderer[] damage = new SpriteRenderer[3];
	private Vector2 movement_check;
	public SpriteRenderer[] fire = new SpriteRenderer[3];
	public Canvas menu;
	public bool in_menu;
	void Start(){
		cache_rigidbody = GetComponent<Rigidbody2D>();
	}
	void Shoot(){
        float posX = this.transform.position.x + (Mathf.Cos((transform.localEulerAngles.z - 90) * Mathf.Deg2Rad)) * -shooting_speed;
        float posY = this.transform.position.y + (Mathf.Sin((transform.localEulerAngles.z - 90) * Mathf.Deg2Rad)) * -shooting_speed;
        GameObject game_bullet = Instantiate(bullet, weapon.transform.position, transform.rotation) as GameObject;
		game_bullet.GetComponent<Bullet>().master = gameObject;
		game_bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(posX, posY));
	}
	void Update(){
		// In-game Menu
		if(Input.GetKeyDown(Settings.key_menu) && !is_bot)
			menu.enabled = in_menu = !in_menu;
	}
	void LateUpdate(){
		// if(Mathf.Abs(movement_check.x - transform.position.x) > 0.1 
		// || Mathf.Abs(movement_check.y - transform.position.y) > 0.1)
		// 	for(int i = 0; i < fire.Length; i++)
		// 		fire[i].enabled = true;
		// else
		// 	for(int i = 0; i < fire.Length; i++)
		// 		fire[i].enabled = false;
	}

	void FixedUpdate()	{
		movement_check.x = transform.position.x;
		movement_check.y = transform.position.y;
		if(!is_bot){
			// Movement
			mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			vertical = Input.GetAxis("Vertical");
			horizontal = Input.GetAxis("Horizontal");
			var angle = Vector2.Angle(Vector2.right, mouse_position - transform.position); //угол между вектором от объекта к мыше и осью х
			// TODO: make something better, than Elvis operator
			transform.eulerAngles = new Vector3(0f, 0f, transform.position.y < mouse_position.y ? angle - 90 : -angle - 90);
			cache_rigidbody.AddForce(new Vector2(force*horizontal, force*vertical));
			// Shooting 
			if(Input.GetKeyDown(Settings.key_shoot) && !in_menu)
				Shoot();
		}
	}
	void OnTriggerEnter2D(Collider2D other) {
		// Damage
		if(other.gameObject.tag == "Bullet" && other.GetComponent<Bullet>().master != gameObject){
			Destroy(other.gameObject);
			if(!damage[0].enabled && !damage[1].enabled && !damage[2].enabled)
				StartCoroutine(hit(0));
			else if(damage[0].enabled && !damage[1].enabled && !damage[2].enabled)
				StartCoroutine(hit(1));
			else if(damage[0].enabled && damage[1].enabled && !damage[2].enabled)
				StartCoroutine(hit(2));
		}
	}
	IEnumerator hit(int dmg){
		damage[dmg].enabled = true;
		yield return new WaitForSeconds(2f);
		if(damage[0].enabled && !damage[1].enabled && !damage[2].enabled)
			damage[0].enabled = false;
		else if(damage[0].enabled && damage[1].enabled && !damage[2].enabled)
			damage[1].enabled = false;
		else if(damage[0].enabled && damage[1].enabled && damage[2].enabled)
			damage[2].enabled = false;
	}
}