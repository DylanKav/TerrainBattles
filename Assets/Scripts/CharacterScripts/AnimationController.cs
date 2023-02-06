using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private bool _isJumping, _isWalking, _isRunning;
    public Animator Controller;
    private static readonly int Jumping = Animator.StringToHash("_isJumping");
    private static readonly int Walking = Animator.StringToHash("_isWalking");
    private static readonly int Running = Animator.StringToHash("_isRunning");
    [SerializeField] private Rigidbody[] RagdollParts;
    [SerializeField] private Collider[] RagdollColliders;
    [SerializeField] private Collider playerCollider;

    private void RagdollMode(bool isOn)
    {
        Controller.enabled = !isOn;
        //playerCollider.enabled = !isOn;
        if (isOn)
        {
            foreach (var part in RagdollParts)
            {
                part.isKinematic = !isOn;
            }

            /*
            foreach (var collider in RagdollColliders)
            {
                collider.enabled = isOn;
            }
            */
        }
    }

    public bool IsJumping
    {
        get
        {
            return _isJumping;
        }
        set
        {
            Controller.SetBool(Jumping, value);
            _isJumping = value;
        }
    }

    public bool IsWalking
    {
        get
        {
            return _isWalking;
        }
        set
        {
            Controller.SetBool(Walking, value);
            _isWalking = value;
        }
    }
    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }
        set
        {
            Controller.SetBool(Running, value);
            _isRunning = value;
        }
    }

    public int GetStateAsInt()
    {
        int state = 0;
        if (IsJumping) state += 1;
        if (IsWalking) state += 2;
        if (IsRunning) state += 4;
        return state;
    }

    public void SetState(int value)
    {
        if (value == 0)
        {
            RagdollMode(true);
            return;
        }
        if (value > 4)
        {
            value -= 4;
            IsRunning = true;
        }
        if (value > 2)
        {
            value -= 2;
            _isWalking = true;
        }
        if (value > 1)
        {
            value -= 1;
            IsJumping = true;
        }
    }
    


    
}
