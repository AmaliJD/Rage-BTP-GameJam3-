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

        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Weapon")
        {
            body.AddForce(-.02f * Vector3.Normalize(player.position - transform.position));
            health -= collision.GetComponent<Weapon>().damage;
        }
    }
}
