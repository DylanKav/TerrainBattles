using System;
using System.Collections;
using System.Collections.Generic;
using TerrainBattlesCore.Core;
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

    private int GetAnimationLayerState()
    {
        var activeLayers = 0;
        for (int i = 0; i < Controller.layerCount; i++)
        {
            if (Math.Abs(Controller.GetLayerWeight(i) - 1) < .1)
            {
                activeLayers += i ^ 2;
            }
        }
        return activeLayers;
    }

    private void SetActiveLayers(int hashCode)
    {
        var decreasingHash = hashCode;
        for (int i = Controller.layerCount; i == 0; i--)
        {
            var currentLayerHash = i ^ 2;
            if (decreasingHash > currentLayerHash)
            {
                decreasingHash -= currentLayerHash;
                Controller.SetLayerWeight(i, 1);
            }
            else
            {
                Controller.SetLayerWeight(i, 0);
            }
        }
    }

    public PlayerAnimationState GetCurrentState()
    {
        var state = new PlayerAnimationState();
        state.AnimationLayerState = GetAnimationLayerState();
        state.InputX = Controller.GetFloat("PosX");
        state.InputY = Controller.GetFloat("PosY");
        state.IsGrounded = Controller.GetBool("isGrounded");
        state.IsBlocking = Controller.GetBool("Block");
        state.IsAttack = Controller.GetBool("Attack");
        return state;
    }

    public void SetAnimationState(PlayerAnimationState state)
    {
        SetActiveLayers(state.AnimationLayerState);
        Controller.SetFloat("PosX", state.InputX);
        Controller.SetFloat("PosY", state.InputY);
        Controller.SetBool("isGrounded", state.IsGrounded);
        Controller.SetBool("Block", state.IsBlocking);
        Controller.SetBool("Attack", state.IsAttack);
    }
    
}
