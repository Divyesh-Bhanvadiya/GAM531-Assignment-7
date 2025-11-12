using System;

namespace OpenTK_Sprite_Animation
{
    public enum CharacterState
    {
        Idle,
        Walk,
        Run,
        Jump,
        Attack1,
        Attack2,
        Attack3,
        Shield
    }

    public enum FacingDirection
    {
        Right = 1,
        Left = -1
    }

    public class AnimationState
    {
        public CharacterState State { get; set; }
        public int Row { get; set; }                    // row in the spritesheet
        public int FrameCount { get; set; }             // no of frames in this row
        public float FrameTime { get; set; }            // time btn frame switches
        public bool Loop { get; set; }                  // continuous animation or not
    }
}