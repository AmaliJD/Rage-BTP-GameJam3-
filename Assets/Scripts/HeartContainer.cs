using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartContainer : MonoBehaviour
{
    public int health;

    public Vector3 velocity;
    private Rigidbody2D body;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.velocity = velocity;
    }

    /*private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            Destroy(GetComponent<CircleCollider2D>());
        }
    }*/
}
