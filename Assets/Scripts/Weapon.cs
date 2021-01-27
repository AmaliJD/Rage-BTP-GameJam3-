using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;
    public bool nomercy;

    public void incrementDamage(float d)
    {
        damage += d;
    }
}
