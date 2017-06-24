using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{

    private bool is_bot = false;
    private float vertical;
    private float horizontal;
    private Vector3 mouse_position;
    private float x, y;
    private Vector2 force_point;
    private Rigidbody2D cache_rigidbody;
    private Vector2 movement_check;
    private KeyCode key_shoot = KeyCode.Mouse0;
    private Camera camera;
    private Texture2D[] numeral = new Texture2D[10];

    public int ammo = 30;
    private bool isReload = false;
    private int health = 100;

    public GameObject player;
    public GameObject bullet;
    public GameObject weapon;
    public float shooting_speed;
    public float force = 15;
    public SpriteRenderer[] damage = new SpriteRenderer[3];
    public SpriteRenderer[] fire = new SpriteRenderer[3];
    public Canvas menu;
    public bool in_menu;
    
    void Start()
    {
        cache_rigidbody = GetComponent<Rigidbody2D>();
        camera = Camera.main;
        camera.GetComponent<Cam>().enabled = true;
        camera.GetComponent<Cam>().player = gameObject.transform;
        menu = camera.GetComponent<Clientside>().menu;
        camera.GetComponent<CursorController>().SetCursor();
        this.numeral = camera.GetComponent<Clientside>().numeral;
    }
    void Shoot()
    {
        float posX = this.transform.position.x + (Mathf.Cos((transform.localEulerAngles.z - 90) * Mathf.Deg2Rad)) * -shooting_speed;
        float posY = this.transform.position.y + (Mathf.Sin((transform.localEulerAngles.z - 90) * Mathf.Deg2Rad)) * -shooting_speed;
        GameObject game_bullet = Instantiate(bullet, weapon.transform.position, transform.rotation) as GameObject;
        game_bullet.GetComponent<Bullet>().master = gameObject;
        game_bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(posX, posY));
    }
    void Update()
    {
        // In-game Menu
        if (Input.GetKeyDown(Settings.keyMenu)) {
            if (!in_menu)
            {
                menu.enabled = in_menu = !in_menu;
                camera.GetComponent<CursorController>().SetMenuCursor();
            }
            else
            {
                menu.enabled = in_menu = !in_menu;
                camera.GetComponent<CursorController>().SetCursor();
            }
            
        }
        if (Input.GetKeyDown(Settings.keyShoot) && !in_menu) {
            this.SendAttack();
        }
        if (Input.GetKeyDown(Settings.keyReload)) {
            this.Reload();
        }
    }

    void FixedUpdate()
    {
        movement_check.x = transform.position.x;
        movement_check.y = transform.position.y;
        if (!is_bot)
        {
            // Movement
            mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
            var angle = Vector2.Angle(Vector2.right, mouse_position - transform.position); //угол между вектором от объекта к мыше и осью х
                                                                                           // TODO: make something better, than Elvis operator
            transform.eulerAngles = new Vector3(0f, 0f, transform.position.y < mouse_position.y ? angle - 90 : -angle - 90);
            cache_rigidbody.AddForce(new Vector2(force * horizontal, force * vertical));
        }

        if (isReload)
        {
            if (ammo < 30)
                ammo++;
            if (ammo >= 30)
            {
                this.Reload();
            }
        }
    }

    public void SendAttack()
    {
        if (ammo > 1)
        {
            if (!isReload)
            {
                ammo--;
                camera.GetComponent<Clientside>().Attack();
            }
        }
        else
        {
            this.Reload();
        }
    }

    public void Reload()
    {
        isReload = !isReload;
    }

    public void SetHealth(int hp) {
        this.health = hp;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Damage
        if (other.gameObject.tag == "Bullet" && other.GetComponent<Bullet>().master != gameObject)
        {
            Destroy(other.gameObject);
            Camera.main.GetComponent<Clientside>().Hit(this.name);
            if (!damage[0].enabled && !damage[1].enabled && !damage[2].enabled)
                StartCoroutine(hit(0));
            else if (damage[0].enabled && !damage[1].enabled && !damage[2].enabled)
                StartCoroutine(hit(1));
            else if (damage[0].enabled && damage[1].enabled && !damage[2].enabled)
                StartCoroutine(hit(2));
        }
    }
    IEnumerator hit(int dmg)
    {
        damage[dmg].enabled = true;
        yield return new WaitForSeconds(2f);
        if (damage[0].enabled && !damage[1].enabled && !damage[2].enabled)
            damage[0].enabled = false;
        else if (damage[0].enabled && damage[1].enabled && !damage[2].enabled)
            damage[1].enabled = false;
        else if (damage[0].enabled && damage[1].enabled && damage[2].enabled)
            damage[2].enabled = false;
    }

    private void OnGUI()
    {
        var mouse = Input.mousePosition;
        if (ammo > 9)
        {
            var x = ammo % 10;
            var y = (int)ammo / 10;
            GUI.DrawTexture(new Rect(mouse.x + 18, -mouse.y + Screen.height - 8, 16, 16), numeral[y]);
            GUI.DrawTexture(new Rect(mouse.x + 35, -mouse.y + Screen.height - 8, 16, 16), numeral[x]);
        }
        else
        {
            GUI.DrawTexture(new Rect(mouse.x + 18, -mouse.y + Screen.height - 8, 16, 16), numeral[0]);
            GUI.DrawTexture(new Rect(mouse.x + 35, -mouse.y + Screen.height - 8, 16, 16), numeral[ammo]);
        }
        GUI.Label(new Rect(120f, 24f, 100, 21), health.ToString(), camera.GetComponent<Clientside>().style);
    }
}