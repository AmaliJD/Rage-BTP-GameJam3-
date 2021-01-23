using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class PlayerController : MonoBehaviour
{
    // Player
    public float speed = .15f, acceleration = .3f;
    public int health, maxHealth;

    private float moveX, moveY;
    private float minAcc = 0.1f, maxAcc = .2f, accDiff;
    private float minSpeed = 1.6f/*0.05f*/, maxSpeed = 7f/*.2f*/, speedDiff;

    private Rigidbody2D playerbody;

    // Weapons
    public GameObject sword;
    public BoxCollider2D swordCollider;
    public Animator swordAnim;
    public TrailRenderer swordtrail;

    // timers
    private float health_timer = 0, sword_timer, max_sword_timer_value = 5;

    // UI
    public Text health_indicator;

    // System
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)] private static extern short GetKeyState(int keyCode);
    [DllImport("user32.dll")] private static extern int GetKeyboardState(byte[] lpKeyState);
    [DllImport("user32.dll", EntryPoint = "keybd_event")] private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
    private const byte VK_NUMLOCK = 0x90; private const uint KEYEVENTF_EXTENDEDKEY = 1; private const int KEYEVENTF_KEYUP = 0x2; private const int KEYEVENTF_KEYDOWN = 0x0;

    private void Start()
    {
        // player
        speed = minSpeed;
        acceleration = minAcc;
        health = maxHealth;

        accDiff = maxAcc - minAcc;
        speedDiff = maxSpeed - minSpeed;

        playerbody = GetComponent<Rigidbody2D>();

        // timers
        health_timer = 0;
        sword_timer = max_sword_timer_value;

        // system
        if (((ushort)GetKeyState(0x90) & 0xffff) != 1)
        {
            keybd_event(VK_NUMLOCK, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN, 0);
            keybd_event(VK_NUMLOCK, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }
    }

    void Update()
    {
        // movement
        moveX = Mathf.Lerp(moveX, Input.GetAxisRaw("Horizontal") * speed, acceleration);
        moveY = Mathf.Lerp(moveY, Input.GetAxisRaw("Vertical") * speed, acceleration);

        //transform.Translate(new Vector3(moveX, moveY, 0));
        playerbody.velocity = new Vector3(moveX, moveY, 0);

        // health effects
        float health_ratio = (float)health / (float)maxHealth;
        speed = maxSpeed - (speedDiff * health_ratio);
        acceleration = maxAcc - (accDiff * health_ratio);

        health_indicator.text = "Health: " + health + "/" + maxHealth + "\nSword: " + (float)((int)((max_sword_timer_value - sword_timer) * 100) / 100f);

        // attacks
        RotateWeapon();
        if (Input.GetMouseButtonDown(0) && sword_timer >= max_sword_timer_value)
        {
            max_sword_timer_value = max_sword_timer_value * health_ratio;
            sword_timer = 0;
            swordtrail.Clear();
            swordtrail.emitting = false;
            swordAnim.Play("SwordSwipe");
        }

        // timer
        //health_timer += Time.deltaTime;
        //if(health_timer > 5) { health -= health == 0 ? 0 : 1; health_timer = 0; }
        if (health_timer > 0)
        {
            health_timer -= Time.deltaTime;
        }

        if (sword_timer < max_sword_timer_value)
        {
            sword_timer += Time.deltaTime;
        }
    }

    void RotateWeapon()
    {
        Vector2 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //float X = Input.GetAxisRaw("Horizontal"), Y = Input.GetAxisRaw("Vertical");
        float X = currentMousePosition.x - transform.position.x, Y = currentMousePosition.y - transform.position.y;

        if (swordCollider.enabled) { return; }

        if (X == 0 && Y == 0) { }
        else if (X >= 0)
        {
            Vector3 angle = new Vector3(0, 0, (Mathf.Rad2Deg * Mathf.Atan(Y / X)));
            sword.transform.rotation = Quaternion.Lerp(sword.transform.rotation, Quaternion.Euler(angle), .5f);

        }
        else if (X < 0)
        {
            Vector3 angle = new Vector3(0, 0, 180 + (Mathf.Rad2Deg * Mathf.Atan(Y / X)));
            sword.transform.rotation = Quaternion.Lerp(sword.transform.rotation, Quaternion.Euler(angle), .5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Enemy" && health_timer <= 0)
        {
            Debug.Log("hit!");
            health -= health > 0 ? 1 : 0;
            health_timer = 1;
        }
    }
}
