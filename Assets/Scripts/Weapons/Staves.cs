using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staves : WeaponBase
{
    internal override void Start()
    {
        base.Start();
    }

    public override void Attack(int attackNum)
    {
        WeaponCollider.enabled = true;
        if (attackNum == 0) PhysicalAttackEnabled = true;
    }

    public override void Block()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (PhysicalAttackEnabled == false) return;
        if (collision.transform.gameObject.CompareTag("EnemyHitPoint"))
        {
            if (collision.transform.gameObject.TryGetComponent<HitRegister>(out var register))
            {
                PhysicalAttackEnabled = false;
                WeaponCollider.enabled = false;
                register.RegisterHit(collision, HitForce);
            }
        }
    }

    private IEnumerator PhysicalAttack()
    { 
        yield return new WaitForSeconds(1.5f);
        PhysicalAttackEnabled = false;
        WeaponCollider.enabled = false;
    }

}

