using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyMovement : MonoBehaviour
{
    public float speed;
    private Transform player;
    private Rigidbody2D body;

    public float health;

    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        body = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        body.velocity = Vector2.Lerp(body.velocity, 5 * Vector3.Normalize(player.position - transform.position), 0.01f);

        if(body.velocity.x > 0) { transform.localScale = new Vector3(-1, 1, 1); }
        else if (body.velocity.x < 0) { transform.localScale = new Vector3(1, 1, 1); }

        if (health <= 0)
        {
            Destroy(gameObject);
        }
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
            StopAllCoroutines();
            body.AddForce(-.02f * Vector3.Normalize(player.position - transform.position));
            health -= collision.GetComponent<Weapon>().damage;
            StartCoroutine(Pulse(Color.red));
        }
    }
}
