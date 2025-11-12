using OpenTK.Graphics.OpenGL4;

namespace OpenTK_Sprite_Animation
{
    public class AnimationController
    {
        private List<AnimationState> _animations;
        private AnimationState _currentAnimation;
        private FacingDirection _facing;
        private float _timer;
        private int _currentFrame;
        private int _shaderProgram;

        // Sprite sheet constants 
        private const float FrameW = 128f;
        private const float FrameH = 126f;
        private const float Gap = 0f;
        private const float SheetW = 12 * FrameW;  // 12 columns max
        private const float SheetH = 7 * FrameH;  // 7 rows

        public AnimationController(int shaderProgram)
        {
            _shaderProgram = shaderProgram;
            _facing = FacingDirection.Right;
            _animations = new List<AnimationState>();
            InitializeAnimations();
            
            // // Start with first frame of walk cycle (idle)
            _currentAnimation = _animations.First();
            UpdateShaderUniforms();
        }

        private void InitializeAnimations()
        {
            _animations.Add(new AnimationState
            {
                State = CharacterState.Idle,
                Row = 0,
                FrameCount = 1,  
                FrameTime = 0.2f,
                Loop = false
            });

            _animations.Add(new AnimationState
            {
                State = CharacterState.Walk,
                Row = 0,
                FrameCount = 8,
                FrameTime = 0.15f,
                Loop = true
            });

            _animations.Add(new AnimationState
            {
                State = CharacterState.Run,
                Row = 1,
                FrameCount = 8,
                FrameTime = 0.08f,
                Loop = true
            });

            _animations.Add(new AnimationState
            {
                State = CharacterState.Jump,
                Row = 2,
                FrameCount = 12,
                FrameTime = 0.08f,
                Loop = false
            });

            _animations.Add(new AnimationState
            {
                State = CharacterState.Attack1,
                Row = 3,
                FrameCount = 5,
                FrameTime = 0.2f,
                Loop = false
            });

            _animations.Add(new AnimationState
            {
                State = CharacterState.Attack2,
                Row = 4,
                FrameCount = 3,
                FrameTime = 0.2f,
                Loop = false
            });

            _animations.Add(new AnimationState
            {
                State = CharacterState.Attack3,
                Row = 5,
                FrameCount = 4,
                FrameTime = 0.2f,
                Loop = false
            });

            _animations.Add(new AnimationState
            {
                State = CharacterState.Shield,
                Row = 6,
                FrameCount = 4,
                FrameTime = 0.15f,
                Loop = true
            });
        }

        public void SetFacing(FacingDirection direction)
        {
            _facing = direction;
        }

        public void SetToFrame(int frame)
        {
            _currentFrame = frame;
            _timer = 0f;
        }
        

        public void ChangeState(CharacterState newState)
        {
            var newAnim = _animations.FirstOrDefault(a => a.State == newState);
            if (newAnim == null)
                throw new Exception($"No animation defined for state: {newState}");

            if (_currentAnimation?.State == newState) return;

            _currentAnimation = newAnim;
            _timer = 0f;
            _currentFrame = 0;
        }

        public void Update(float deltaTime)
        {
            if (_currentAnimation == null) return;

            _timer += deltaTime;
            if (_timer >= _currentAnimation.FrameTime)
            {
                _timer -= _currentAnimation.FrameTime;
                _currentFrame++;

                if (_currentFrame >= _currentAnimation.FrameCount)
                {
                    if (_currentAnimation.Loop)
                        _currentFrame = 0;
                    else
                        _currentFrame = _currentAnimation.FrameCount - 1; // Hold last frame
                }
            }

            UpdateShaderUniforms();
        }

        private void UpdateShaderUniforms()
        {
            float totalW = FrameW + Gap;
            float x = (_currentFrame * totalW) / SheetW;
            float y = (_currentAnimation.Row * FrameH) / SheetH;
            float w = FrameW / SheetW;
            float h = FrameH / SheetH;

            GL.UseProgram(_shaderProgram);

            int off = GL.GetUniformLocation(_shaderProgram, "uOffset");
            int sz = GL.GetUniformLocation(_shaderProgram, "uSize");
            GL.Uniform2(off, x, y);
            GL.Uniform2(sz, w, h);

            // Send flip flag
            int flipLoc = GL.GetUniformLocation(_shaderProgram, "uFlipX");
            GL.Uniform1(flipLoc, _facing == FacingDirection.Left ? 1 : 0);
        }

        public void Render()
        {
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
        }

        public CharacterState CurrentState => _currentAnimation?.State ?? CharacterState.Idle;
        public FacingDirection Facing => _facing;

        public bool IsAnimationFinished()
        {
            if (_currentAnimation == null) return true;
            return !_currentAnimation.Loop &&
                   _currentFrame >= _currentAnimation.FrameCount - 1;
        }
    }
}