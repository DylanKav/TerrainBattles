using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackHit : MonoBehaviour
{
    [SerializeField] private float force = 5f;
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Kaboom");
        PlayerInputListener listener;
        other.TryGetComponent<PlayerInputListener>(out listener);
        
        var position = this.transform.position;
        listener.SetRagdollMode(true, other.transform.position - position, force, other.ClosestPoint(position));
    }
}
