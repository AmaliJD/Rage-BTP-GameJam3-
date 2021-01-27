using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicEnemyMovement : MonoBehaviour
{
    public float speed;
    public float regen;
    public float force;
    public float waittime = .5f;
    private Transform player;
    private Rigidbody2D body;

    public float health;

    public Slider healthbar;
    public GameObject explosion;

    private float hittimer = .1f;
    private float forcetimer;

    // sounds
    public AudioSource hit, hitexplode, bounce;

    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        body = GetComponent<Rigidbody2D>();

        healthbar.maxValue = health;
        healthbar.value = health;

        forcetimer = waittime;
    }

    
    void Update()
    {
        body.velocity = Vector2.Lerp(body.velocity, forcetimer >= waittime ? 5 * Vector3.Normalize(player.position - transform.position) : Vector3.zero, speed);

        if(body.velocity.x > 0) { transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1); }
        else if (body.velocity.x < 0) { transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1); }

        health += regen;
        if (health > healthbar.maxValue) { health = healthbar.maxValue; }

        healthbar.value = health;

        if (health <= 0)
        {
            GameObject ex = Instantiate(explosion, transform.position, Quaternion.identity);
            ex.transform.localScale = 0.7f * transform.localScale;
            Destroy(gameObject);
        }

        hittimer += Time.deltaTime;
        forcetimer += Time.deltaTime;
    }

    IEnumerator Pulse(Color color)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = color;

        float time = 0;

        while(time < 1)
        {
            sprite.color = Color.Lerp(color, Color.white, time);
            time += Time.deltaTime;

            yield return null;
        }

        sprite.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Weapon")
        {
            if (collision.GetComponent<Weapon>().nomercy || hittimer >= .1f)
            {
                hittimer = 0;
                StopAllCoroutines();
                body.AddForce(-.1f * (collision.GetComponent<Weapon>().damage * .7f) * (200 * speed) * Vector3.Normalize(player.position - transform.position));
                health -= collision.GetComponent<Weapon>().damage;
                StartCoroutine(Pulse(Color.red));

                AudioSource play = health > 0 ? hit : hitexplode;
                play.PlayOneShot(play.clip, 1);
            }
        }

        if(collision.tag == "Player" && force > 0)
        {
            forcetimer = 0;
            collision.GetComponent<Rigidbody2D>().AddForce(force * Vector3.Normalize(player.position - transform.position));
            player.GetComponent<PlayerController>().TakeDamage();
            bounce.PlayOneShot(bounce.clip, 1);
        }
    }
}
