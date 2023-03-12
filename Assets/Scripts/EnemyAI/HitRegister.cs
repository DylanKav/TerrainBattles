using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class HitRegister : MonoBehaviour
{
    [SerializeField] private HitReaction _reaction;
    private Collider _collider;
    void Start()
    {
        if (this.gameObject.TryGetComponent<Collider>(out var collider))
        {
            _collider = collider;
        }
        else
        {
            Destroy(this);
        }

        if (_reaction == null) Destroy(this);

    }

    public void RegisterHit(Collision collision, float hitForce)
    {
        _reaction.Hit(collision.collider, (collision.GetContact(0).normal * -1) * hitForce, collision.GetContact(0).point);
    }
}
