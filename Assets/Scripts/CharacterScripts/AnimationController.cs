using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator Controller;
    private Vector2 _input;
    public float TransitionSpeed = 1f;

    public void SetMovement(Vector2 input)
    {
        _input = input;
    }

    public void SetBlock(bool value)
    {
        Controller.SetBool("Block", value);
    }
    
    public void SetAttack(bool value)
    {
        Controller.SetBool("Attack", value);
    }
    public bool GetIsGrounded()
    {
        return Controller.GetBool("isGrounded");
    }

    public void SetIsGrounded(bool value)
    {
        Controller.SetBool("isGrounded", value);
    }

    private void Update()
    {
        Controller.SetFloat("PosX", Mathf.Lerp(Controller.GetFloat("PosX"), Mathf.Round(_input.y), TransitionSpeed * Time.deltaTime));
        Controller.SetFloat("PosY", Mathf.Lerp(Controller.GetFloat("PosY"), Mathf.Round(_input.x), TransitionSpeed * Time.deltaTime));
    }
}
