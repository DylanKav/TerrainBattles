using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{

    //Editor Values
    public Collider WeaponCollider;
    public int BaseDamage;
    public float HitForce = 1;
    public bool PhysicalAttackEnabled = false;

    internal virtual void Start()
    {
        WeaponCollider.enabled = false;
    }

    public abstract void Attack(int attackNum);

    public abstract void Block();

    /*
    internal virtual void OnCollisionEnter(Collision collision)
    {
        
    }
    */
}
