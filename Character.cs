using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using System;

namespace OpenTK_Sprite_Animation
{
    public class Character
    {
        private AnimationController _animator;
        private CharacterState _state;

        // Physics
        private Vector2 _position;
        private bool _isGrounded;

        // Constants
        private const float WalkSpeed = 150f;
        private const float RunSpeed = 300f;
        private const float GroundY = 300f;

        public Character(int shaderProgram)
        {
            _animator = new AnimationController(shaderProgram);
            _position = new Vector2(400, GroundY);
            _isGrounded = true;
            _state = CharacterState.Idle;
            
        }

        public void Update(float deltaTime, KeyboardState keyboard)
        {
            // Determine desired state from input
            CharacterState newState = DetermineState(keyboard);

            // Validate state transition
            if (CanTransitionTo(newState))
            {
                if (_state != newState)
                {
                    _state = newState;
                    _animator.ChangeState(newState);
                }
            }

            // Update physics
            UpdatePhysics(deltaTime, keyboard);

            // Update animation
            // For Walk/Run: only update if currently moving (pause on release)
            // For Idle: never update (stays frozen on frame 0)
            // For non-looping (attacks, jump, shield): always update until finished
            if (_state == CharacterState.Walk || _state == CharacterState.Run)
            {
                // Only animate while keys are held
                if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.Left))
                {
                    _animator.Update(deltaTime);
                }
            }
            else if (_state != CharacterState.Idle)
            {
                // All other states (attacks, jump, shield) animate normally
                _animator.Update(deltaTime);
            }

            // Render idle after non-looping animations are over
            if (_animator.IsAnimationFinished())
            {
                if (_state == CharacterState.Attack1 ||
                    _state == CharacterState.Attack2 ||
                    _state == CharacterState.Attack3 ||
                    _state == CharacterState.Shield ||
                    _state == CharacterState.Jump)
                {
                    // Return to idle (frame 0 of walk)
                    _animator.ChangeState(CharacterState.Idle);
                    _animator.Update(deltaTime);
                }
            }
        }

        private CharacterState DetermineState(KeyboardState kb)
        {
            bool running = kb.IsKeyDown(Keys.LeftShift);
            bool shielding = kb.IsKeyDown(Keys.X);

            // Attacks 1/2/3 (A, S, D keys)
            if (kb.IsKeyPressed(Keys.A) && _isGrounded)
                return CharacterState.Attack1;
            
            if (kb.IsKeyPressed(Keys.S) && _isGrounded)
                return CharacterState.Attack2;
            
            if (kb.IsKeyPressed(Keys.D) && _isGrounded)
                return CharacterState.Attack3;

            // Shield
            if (shielding && _isGrounded)
                return CharacterState.Shield;

            // Jump: Check if Space is pressed, not if already in air
            if (kb.IsKeyPressed(Keys.Space) && _isGrounded)
                return CharacterState.Jump;

            // If in air but not from jump button (shouldn't happen), use jump state
            // if (!_isGrounded)
            //     return CharacterState.Jump;

            // Movement
            if (kb.IsKeyDown(Keys.Right) || kb.IsKeyDown(Keys.Left))
                return running ? CharacterState.Run : CharacterState.Walk;

            return CharacterState.Idle;
        }

        private bool CanTransitionTo(CharacterState newState)
        {
            // Can't interrupt attack animations - must finish first
            if (_state == CharacterState.Attack1 ||
                _state == CharacterState.Attack2 ||
                _state == CharacterState.Attack3)
            {
                return _animator.IsAnimationFinished();
            }

            // Can't interrupt jump animation - must finish first
            if (_state == CharacterState.Jump)
            {
                return _animator.IsAnimationFinished();
            }

            // Can't start attacks or shield in air
            if (newState == CharacterState.Attack1 ||
                newState == CharacterState.Attack2 ||
                newState == CharacterState.Attack3 ||
                newState == CharacterState.Shield)
                return _isGrounded;

            // Can only jump when grounded
            if (newState == CharacterState.Jump)
                return _isGrounded;

            return true;
        }

        private void UpdatePhysics(float dt, KeyboardState kb)
        {
            // Horizontal movement (can't move during attack or shield)
            if (_state != CharacterState.Attack1 &&
                _state != CharacterState.Attack2 &&
                _state != CharacterState.Attack3 &&
                _state != CharacterState.Shield)
            {
                float speed = (_state == CharacterState.Run) ? RunSpeed : WalkSpeed;

                if (kb.IsKeyDown(Keys.Right))
                {
                    _position.X += speed * dt;
                    _animator.SetFacing(FacingDirection.Right);
                }
                else if (kb.IsKeyDown(Keys.Left))
                {
                    _position.X -= speed * dt;
                    _animator.SetFacing(FacingDirection.Left);
                }
            }

            // Keep character on screen
            _position.X = Math.Clamp(_position.X, 64, 736);
        }
        public void Render(int shaderProgram)
        {
            int modelLoc = GL.GetUniformLocation(shaderProgram, "model");
            Matrix4 model = Matrix4.CreateTranslation(_position.X, _position.Y, 0);
            GL.UniformMatrix4(modelLoc, false, ref model);

            _animator.Render();
        }

        public Vector2 Position => _position;
    }
}