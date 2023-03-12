using System;

namespace TerrainBattlesCore.Core
{
    [Serializable]
    public struct PlayerAnimationState
    {
        public int AnimationLayerState;
        public float InputX; //for movement animation
        public float InputY; //for movement animation
        public bool IsGrounded;
        public bool IsBlocking;
        public bool IsAttack;
    }
}