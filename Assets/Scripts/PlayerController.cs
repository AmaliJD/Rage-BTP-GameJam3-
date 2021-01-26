using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Game
    [Header("Game")]
    public int level_number;
    public Text level_number_text;
    private bool start;

    public Animator UIAnimator;

    // Player
    [Header("Player")]
    public float speed = .15f;
    public float acceleration = .3f;
    public int health, maxHealth;

    private float moveX, moveY;
    private float minAcc = 0.1f, maxAcc = .2f, accDiff;
    private float minSpeed = 1.6f/*0.05f*/, maxSpeed = 7f/*.2f*/, speedDiff;

    private Rigidbody2D playerbody;
    public Transform enemyholder;

    private bool alive = true;
    private bool once = false, once1 = false;

    public float[] timeawards;

    [Header("Player Renderer")]
    // Player Renderer
    public SpriteRenderer body_renderer;
    public SpriteRenderer eyes_renderer;
    public Sprite[] eyes_list;
    public Color[] body_colors, eyes_colors;

    public ParticleSystem flames;
    public ParticleSystem explosion;

    // Sounds
    [Header("Sounds")]
    public AudioSource hurt;
    public AudioSource powerup;
    public AudioSource explode;
    public AudioSource lifeup;
    public AudioSource starsfx;
    public AudioSource fail;

    // Screen
    [Header("Screen")]
    public Volume rage_postprocessing;

    // Weapons
    [Header("Weapons")]
    public GameObject sword;
    public BoxCollider2D swordCollider;
    public Animator swordAnim;
    public TrailRenderer swordtrail;
    private Weapon weapon;

    // timers
    [Header("Timers")]
    public float heartattack_timer;
    private float MAXHEARTATTACKTIMERVALUE;

    private float health_timer = 0, sword_timer, max_sword_timer_value;
    private const float MAXSWORDTIMERVALUE = 5;
    private float timer = 0;
    private int milli = 0, m = 0, sec = 0, s = 0, min = 0;

    // UI
    [Header("UI")]
    public Text time_text;
    public Sprite fullheart;
    public Sprite halfheart;
    public Grid heartgrid;
    private List<Image> hearts;
    public Slider sword_timer_slider;
    public Image sword_image;
    public Slider heartattack_timer_slider;
    public GameObject heartattack_ui;
    public GameObject[] stars;

    // System
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)] private static extern short GetKeyState(int keyCode);
    [DllImport("user32.dll")] private static extern int GetKeyboardState(byte[] lpKeyState);
    [DllImport("user32.dll", EntryPoint = "keybd_event")] private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
    private const byte VK_NUMLOCK = 0x90; private const uint KEYEVENTF_EXTENDEDKEY = 1; private const int KEYEVENTF_KEYUP = 0x2; private const int KEYEVENTF_KEYDOWN = 0x0;

    private void Start()
    {
        // game
        level_number_text.text = ""+level_number;

        // player
        speed = minSpeed;
        acceleration = minAcc;
        health = maxHealth;

        accDiff = maxAcc - minAcc;
        speedDiff = maxSpeed - minSpeed;

        playerbody = GetComponent<Rigidbody2D>();

        // player renderer
        body_renderer.color = body_colors[0];
        eyes_renderer.sprite = eyes_list[0];
        eyes_renderer.color = eyes_colors[0];

        // screen
        rage_postprocessing.weight = 0;

        // weapons
        weapon = swordCollider.gameObject.GetComponent<Weapon>();

        // timers
        health_timer = 0;
        max_sword_timer_value = MAXSWORDTIMERVALUE;
        sword_timer = max_sword_timer_value;
        MAXHEARTATTACKTIMERVALUE = heartattack_timer;

        // ui
        hearts = new List<Image>();
        for(int i = 0; i < maxHealth / 2; i++)
        {
            GameObject obj = new GameObject();
            Image newHeart = obj.AddComponent<Image>();
            newHeart.sprite = fullheart;
            hearts.Add(newHeart);

            obj.transform.parent = heartgrid.gameObject.transform;
            obj.transform.localScale = new Vector3(1, 1, 1);
        }

        sword_timer_slider.maxValue = max_sword_timer_value;
        sword_timer_slider.value = sword_timer;

        heartattack_timer_slider.maxValue = MAXHEARTATTACKTIMERVALUE;
        heartattack_timer_slider.value = MAXHEARTATTACKTIMERVALUE;

        // system
        if (((ushort)GetKeyState(0x90) & 0xffff) != 1)
        {
            keybd_event(VK_NUMLOCK, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN, 0);
            keybd_event(VK_NUMLOCK, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        Time.timeScale = 1;
    }

    void Update()
    {
        if(!start && level_number_text.gameObject.activeSelf) { return; }
        start = true;
        if (alive)
        {
            // movement
            moveX = Mathf.Lerp(moveX, Input.GetAxisRaw("Horizontal") * speed, acceleration);
            moveY = Mathf.Lerp(moveY, Input.GetAxisRaw("Vertical") * speed, acceleration);

            //transform.Translate(new Vector3(moveX, moveY, 0));
            //if (swordCollider.enabled) { moveX = moveY = 0; }
            playerbody.velocity = new Vector3(moveX, moveY, 0);

            // health effects
            float health_ratio = (float)health / (float)maxHealth;
            speed = maxSpeed - (speedDiff * health_ratio);
            acceleration = maxAcc - (accDiff * health_ratio);

            for (int i = 1; i <= maxHealth; i += 2)
            {
                if (i <= health)
                {
                    hearts[((i + 1) / 2) - 1].gameObject.SetActive(true);
                    hearts[((i + 1) / 2) - 1].sprite = i + 1 <= health ? fullheart : halfheart;
                }
                else
                {
                    hearts[((i + 1) / 2) - 1].gameObject.SetActive(false);
                }
            }

            // visual effects
            body_renderer.color = Color.Lerp(body_colors[0], body_colors[1], 1 - health_ratio);
            int eyes_index = (eyes_list.Length - 1) - Mathf.RoundToInt((health_ratio * (eyes_list.Length - 1)));
            eyes_renderer.sprite = eyes_list[eyes_index];
            eyes_renderer.color = Color.Lerp(eyes_colors[0], eyes_colors[1], 1 - health_ratio);

            rage_postprocessing.weight = 1 - health_ratio;

            // attacks
            RotateWeapon();
            /*if (health == 0) { */max_sword_timer_value = (MAXSWORDTIMERVALUE * health_ratio) + .001f;// }
            if (Input.GetMouseButtonDown(0) && sword_timer >= max_sword_timer_value)
            {
                max_sword_timer_value = (MAXSWORDTIMERVALUE * health_ratio) + .001f;
                sword_timer = 0;
                swordtrail.Clear();
                swordtrail.emitting = false;
                swordAnim.Play("SwordSwipe");
            }

            // internal timers
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
        else
        {
            if (!once && enemyholder.childCount > 0)
            {
                once = true;
                explode.PlayOneShot(explode.clip, 1);
                explosion.Play();
                swordCollider.enabled = false;
                body_renderer.enabled = false;
                eyes_renderer.enabled = false;

                StartCoroutine(Restart());
            }
        }

        // ui
        sword_timer_slider.maxValue = max_sword_timer_value;
        sword_timer_slider.value = sword_timer;
        if(sword_timer_slider.value == max_sword_timer_value)
        {
            sword_timer_slider.fillRect.GetComponentInChildren<Image>().color = Color.yellow;
            sword_image.color = Color.white;
        }
        else
        {
            sword_timer_slider.fillRect.GetComponentInChildren<Image>().color = Color.white;
            sword_image.color = new Color(1, 1, 1, .26f);
        }

        milli = (int)(timer * 1000) % 1000;
        sec = (int)(timer);
        min = (int)(sec / 60);

        milli -= (1000 * m);
        sec -= (60 * s);

        if (milli == 1000) { m++; };
        if (sec == 60) { s++; };

        time_text.text = /*min + ":" + */(sec < 10 ? "0" : "") + sec + " : " + (milli < 100 ? "0" : "") + (milli < 10 ? "0" : "") + milli + " ";
        if (enemyholder.childCount > 0 && alive)
        {
            timer += Time.deltaTime;

            if(health == 0)
            {
                if(heartattack_timer <= 0)
                {
                    alive = false;
                }

                heartattack_ui.SetActive(true);
                heartattack_timer -= Time.deltaTime;
                heartattack_timer_slider.value = heartattack_timer;
            }
            else
            {
                heartattack_ui.SetActive(false);
                heartattack_timer = MAXHEARTATTACKTIMERVALUE;
                heartattack_timer_slider.value = MAXHEARTATTACKTIMERVALUE;
            }
        }
        else
        {
            if (!once1) { once1 = true;  Time.timeScale = .2f; }
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z), .1f);
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 2, .1f);

            if(!once)
            {
                once = true;
                StartCoroutine(NextLevel());
            }
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

    IEnumerator Restart()
    {
        float time = 0;

        while(time < .5f)
        {
            //Debug.Log(time);
            time += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1.0f;
    }

    IEnumerator NextLevel()
    {
        float time = 0;

        while (time < .3f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        Time.timeScale = 1.0f;
        UIAnimator.SetTrigger("Score");

        time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        for(int i = timeawards.Length - 1; i >= 0; i--)
        {
            if(timer < timeawards[i])
            {
                starsfx.PlayOneShot(starsfx.clip, 1);
                stars[i].SetActive(true);

                time = 0;
                while (time < .3f)
                {
                    time += Time.deltaTime;
                    yield return null;
                }
            }
        }

        if(!stars[0].activeSelf && !stars[1].activeSelf && !stars[2].activeSelf)
        {
            fail.PlayOneShot(fail.clip, 1);
        }

        level_number += 1;
        level_number_text.text = ""+level_number;
        UIAnimator.SetTrigger("End");

        time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        GlobalData.Index = (GlobalData.Index + 1 == GlobalData.Scene.Length) ? 0 : GlobalData.Index + 1;
        SceneManager.LoadScene(GlobalData.Scene[GlobalData.Index]);
        
    }
    
    private void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.tag == "Enemy" && health_timer <= 0 && !swordCollider.enabled)
        {
            int damage = health > 0 ? 1 : 0;
            health -= damage;
            health_timer = .4f;

            if (damage != 0)
            {
                weapon.incrementDamage(.5f * damage);
                if (health == 0)
                {
                    hurt.PlayOneShot(hurt.clip, 1);
                    powerup.PlayOneShot(powerup.clip, 1);
                    flames.Play();
                }
                else
                {
                    hurt.PlayOneShot(hurt.clip, 1);
                }
            }
        }
        else if (collider.tag == "Healing")
        {
            int life = health < maxHealth ? collider.GetComponent<HeartContainer>().health : 0;
            health += life;
            health = Mathf.Min(health, maxHealth);
            health_timer = 0;
            flames.Stop();

            if (life != 0)
            {
                heartattack_ui.SetActive(false);
                weapon.incrementDamage(-.5f * life);
                lifeup.PlayOneShot(lifeup.clip, 1);
                Destroy(collider.gameObject);
            }
        }
    }
}
