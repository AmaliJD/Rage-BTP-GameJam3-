using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;

    void incrementDamage(float d)
    {
        damage += d;
    }
}
