using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AnnoyingCat
{
    public enum CatState
    {
        Idle,
        Walking,
        BlockScreenCenter,
        Dancing,
        PlayfulPounce,
        Disappearing,
        Hidden,
        Reappearing,
        ClickReaction,
        Dragged,
        ReturningToCenter,
        Snoozing
    }

    public class CatController
    {
        private readonly Random _rand = new Random();
        private readonly AssetManager _assets;
        private readonly DialogueManager _dialogue;
        private readonly MovementManager _movement;

        // Main cat size.
        // 1.0 = original size.
        // 0.78 = 78% of original size.
        private const double BaseScale = 0.78;

        private CatState _currentState = CatState.Idle;
        private double _stateTimer = 0.0;
        private double _danceTimer = 0.0;
        private int _danceFrame = 0;
        private int _clickCount = 0;
        private double _clickCoolDown = 0.0;

        // Talking mouth sync
        private bool _isTalking = false;
        private double _talkTimer = 0.0;
        private double _talkDuration = 0.0;
        private int _mouthFrameIndex = 0;
        private double _mouthFrameTimer = 0.0;

        // Current visual presentation
        private BitmapImage _currentImage;
        private bool _isFlippedHorizontally = false;
        private double _visualOffsetY = 0.0;
        private double _scale = BaseScale;
        private double _opacity = 1.0;
        private bool _isWindowVisible = true;

        // Callbacks for MainWindow
        public Action<string, string, double> OnShowSpeechBubble;
        public Action OnHideSpeechBubble;
        public Action OnPlayPopSound;

        public CatState CurrentState
        {
            get { return _currentState; }
        }

        public BitmapImage CurrentImage
        {
            get { return _currentImage; }
        }

        public bool IsFlippedHorizontally
        {
            get { return _isFlippedHorizontally; }
        }

        public double VisualOffsetY
        {
            get { return _visualOffsetY; }
        }

        public double Scale
        {
            get { return _scale; }
        }

        public double Opacity
        {
            get { return _opacity; }
        }

        public bool IsWindowVisible
        {
            get { return _isWindowVisible; }
        }

        public MovementManager Movement
        {
            get { return _movement; }
        }

        public CatController(
            AssetManager assets,
            DialogueManager dialogue,
            MovementManager movement)
        {
            _assets = assets;
            _dialogue = dialogue;
            _movement = movement;

            _currentImage = _assets.GetFrame("idle_stare");

            EnterIdleState(2.5);
        }

        public void Update(double deltaSeconds)
        {
            _stateTimer -= deltaSeconds;

            if (_clickCoolDown > 0)
                _clickCoolDown -= deltaSeconds;

            // =========================
            // TALKING MOUTH ANIMATION
            // =========================

            if (_isTalking)
            {
                _talkTimer -= deltaSeconds;
                _mouthFrameTimer += deltaSeconds;

                if (_mouthFrameTimer >= 0.12)
                {
                    _mouthFrameTimer = 0.0;
                    _mouthFrameIndex =
                        (_mouthFrameIndex + 1) % 6;
                }

                if (_talkTimer <= 0)
                {
                    _isTalking = false;
                }
            }

            // =========================
            // STATE UPDATE
            // =========================

            switch (_currentState)
            {
                case CatState.Idle:
                    UpdateIdle(deltaSeconds);
                    break;

                case CatState.Walking:
                    UpdateWalking(deltaSeconds);
                    break;

                case CatState.BlockScreenCenter:
                    UpdateBlockScreenCenter(deltaSeconds);
                    break;

                case CatState.Dancing:
                    UpdateDancing(deltaSeconds);
                    break;

                case CatState.PlayfulPounce:
                    UpdatePlayfulPounce(deltaSeconds);
                    break;

                case CatState.Disappearing:
                    UpdateDisappearing(deltaSeconds);
                    break;

                case CatState.Hidden:
                    UpdateHidden(deltaSeconds);
                    break;

                case CatState.Reappearing:
                    UpdateReappearing(deltaSeconds);
                    break;

                case CatState.ClickReaction:
                    UpdateClickReaction(deltaSeconds);
                    break;

                case CatState.Dragged:
                    UpdateDragged(deltaSeconds);
                    break;

                case CatState.ReturningToCenter:
                    UpdateReturningToCenter(deltaSeconds);
                    break;

                case CatState.Snoozing:
                    UpdateSnoozing(deltaSeconds);
                    break;
            }
        }

        // =========================
        // IDLE
        // =========================

        private void EnterIdleState(double duration)
        {
            _currentState = CatState.Idle;
            _stateTimer = duration;

            _visualOffsetY = 0.0;
            _scale = BaseScale;
            _opacity = 1.0;
            _isWindowVisible = true;

            // Randomize idle pose
            int r = _rand.Next(10);

            if (r < 5)
            {
                _currentImage =
                    _assets.GetFrame("idle_stare");
            }
            else if (r < 7)
            {
                _currentImage =
                    _assets.GetFrame("idle_look_up");
            }
            else if (r < 8)
            {
                _currentImage =
                    _assets.GetFrame("idle_happy");
            }
            else if (r < 9)
            {
                _currentImage =
                    _assets.GetFrame("idle_curious");
            }
            else
            {
                _currentImage =
                    _assets.GetFrame("idle_proud");
            }
        }

        private void UpdateIdle(double deltaSeconds)
        {
            if (_isTalking)
            {
                _currentImage =
                    _assets.GetFrame(
                        "mouth_" + _mouthFrameIndex);
            }

            if (_stateTimer <= 0)
            {
                PickNextAction();
            }
        }

        // =========================
        // NORMAL WALKING
        // =========================

        private void EnterWalkingState(
            Point target,
            double speed = 140.0)
        {
            _currentState = CatState.Walking;

            _movement.MoveTo(
                target.X,
                target.Y,
                speed);

            _isFlippedHorizontally =
                _movement.IsFacingLeft;

            _visualOffsetY = 0.0;
        }

        private void UpdateWalking(double deltaSeconds)
        {
            bool finished =
                _movement.Update(deltaSeconds);

            _isFlippedHorizontally =
                _movement.IsFacingLeft;

            var walkFrames =
                _assets.GetAnimation("walk");

            if (walkFrames.Count > 0)
            {
                int frameIndex =
                    _movement.WalkFrameIndex %
                    walkFrames.Count;

                _currentImage =
                    walkFrames[frameIndex];
            }

            if (finished)
            {
                // Occasionally make a productivity comment.
                if (_rand.Next(3) == 0)
                {
                    SayDialogue(
                        _dialogue.GetProductivityJab());
                }

                EnterIdleState(
                    3.0 +
                    _rand.NextDouble() * 4.0);
            }
        }

        // =========================
        // BLOCK SCREEN CENTER
        // =========================

        public void TriggerBlockScreenCenter()
        {
            Point center =
                _movement.PickScreenCenterPosition();

            EnterWalkingState(
                center,
                210.0);

            _currentState =
                CatState.BlockScreenCenter;
        }

        private void UpdateBlockScreenCenter(
            double deltaSeconds)
        {
            bool finished =
                _movement.Update(deltaSeconds);

            _isFlippedHorizontally =
                _movement.IsFacingLeft;

            var walkFrames =
                _assets.GetAnimation("walk");

            if (walkFrames.Count > 0)
            {
                int frameIndex =
                    _movement.WalkFrameIndex %
                    walkFrames.Count;

                _currentImage =
                    walkFrames[frameIndex];
            }

            if (finished)
            {
                _currentImage =
                    _assets.GetFrame("idle_stare");

                SayDialogue(
                    _dialogue.GetBlockWindowJab());

                EnterIdleState(
                    5.0 +
                    _rand.NextDouble() * 5.0);
            }
        }

        // =========================
        // DANCING
        // =========================

        public void TriggerDance()
        {
            _currentState =
                CatState.Dancing;

            _stateTimer =
                4.0 +
                _rand.NextDouble() * 3.0;

            _danceTimer = 0.0;
            _danceFrame = 0;
            _visualOffsetY = 0.0;

            SayDialogue(
                _dialogue.GetDanceJab());
        }

        private void UpdateDancing(
            double deltaSeconds)
        {
            _danceTimer += deltaSeconds;

            if (_danceTimer >= 0.16)
            {
                _danceTimer = 0.0;

                _danceFrame =
                    (_danceFrame + 1) % 2;

                _isFlippedHorizontally =
                    (_danceFrame == 1);
            }

            _currentImage =
                (_danceFrame == 0)
                    ? _assets.GetFrame("dance_0")
                    : _assets.GetFrame("dance_1");

            _visualOffsetY =
                (_danceFrame == 0)
                    ? -12.0
                    : 0.0;

            if (_stateTimer <= 0)
            {
                _visualOffsetY = 0.0;

                EnterIdleState(3.0);
            }
        }

        // =========================
        // PLAYFUL POUNCE
        // =========================

        public void TriggerPlayfulPounce()
        {
            _currentState =
                CatState.PlayfulPounce;

            _stateTimer = 1.3;

            _currentImage =
                _assets.GetFrame("playful");

            _visualOffsetY = 0.0;
        }

        private void UpdatePlayfulPounce(
            double deltaSeconds)
        {
            if (_stateTimer > 0)
            {
                _visualOffsetY =
                    Math.Sin(
                        _stateTimer * 20.0) * 4.0;

                if (_stateTimer <= deltaSeconds)
                {
                    _visualOffsetY = 0.0;

                    _currentImage =
                        _assets.GetFrame("pounce");

                    double targetX =
                        _movement.CurrentX +
                        (_movement.IsFacingLeft
                            ? -260
                            : 260);

                    _movement.PounceTo(
                        targetX,
                        _movement.CurrentY);
                }
            }
            else
            {
                bool leapDone =
                    _movement.Update(deltaSeconds);

                if (leapDone)
                {
                    _currentImage =
                        _assets.GetFrame("sit_paw");

                    EnterIdleState(3.0);
                }
            }
        }

        // =========================
        // DISAPPEAR
        // =========================

        public void TriggerDisappear()
        {
            _currentState =
                CatState.Disappearing;

            _stateTimer = 0.8;

            SayDialogue(
                _dialogue.GetDisappearLine());
        }

        private void UpdateDisappearing(
            double deltaSeconds)
        {
            double progress =
                Math.Max(
                    0.0,
                    _stateTimer / 0.8);

            // Shrink relative to the new base size.
            _scale =
                Math.Max(
                    0.1,
                    BaseScale * progress);

            _opacity =
                Math.Max(
                    0.0,
                    progress);

            if (_stateTimer <= 0)
            {
                _isWindowVisible = false;

                _currentState =
                    CatState.Hidden;

                _stateTimer =
                    15.0 +
                    _rand.NextDouble() * 25.0;

                if (OnHideSpeechBubble != null)
                    OnHideSpeechBubble();
            }
        }

        // =========================
        // HIDDEN
        // =========================

        private void UpdateHidden(
            double deltaSeconds)
        {
            if (_stateTimer <= 0)
            {
                // Time to reappear.
                int spot = _rand.Next(3);

                if (spot == 0)
                {
                    Point center =
                        _movement.PickScreenCenterPosition();

                    _movement.CurrentX =
                        center.X;

                    _movement.CurrentY =
                        center.Y;
                }
                else
                {
                    Point bottom =
                        _movement.PickRandomBottomPosition();

                    _movement.CurrentX =
                        bottom.X;

                    _movement.CurrentY =
                        bottom.Y;
                }

                _currentState =
                    CatState.Reappearing;

                _stateTimer = 0.6;

                _scale = 0.1;
                _opacity = 0.0;

                _isWindowVisible = true;

                _currentImage =
                    _assets.GetFrame("idle_stare");
            }
        }

        private void UpdateReappearing(
            double deltaSeconds)
        {
            double progress =
                Math.Min(
                    1.0,
                    1.0 -
                    (_stateTimer / 0.6));

            // Grow back to the new smaller size.
            _scale =
                0.1 +
                progress *
                (BaseScale - 0.1);

            _opacity = progress;

            if (_stateTimer <= 0)
            {
                _scale = BaseScale;
                _opacity = 1.0;

                SayDialogue(
                    _dialogue.GetReappearLine());

                EnterIdleState(4.0);
            }
        }

        // =========================
        // CLICK REACTION
        // =========================

        public void OnClick()
        {
            if (_currentState ==
                    CatState.Disappearing ||
                _currentState ==
                    CatState.Hidden)
            {
                return;
            }

            _clickCount++;

            _currentState =
                CatState.ClickReaction;

            _stateTimer = 1.8;

            _currentImage =
                _assets.GetFrame("surprised");

            _visualOffsetY = -15.0;

            if (OnPlayPopSound != null)
                OnPlayPopSound();

            SayDialogue(
                _dialogue.GetClickReaction());

            // If clicked 3 times, run away.
            if (_clickCount >= 3)
            {
                _clickCount = 0;

                Point runTarget =
                    _movement.PickRandomBottomPosition();

                EnterWalkingState(
                    runTarget,
                    260.0);
            }
        }

        private void UpdateClickReaction(
            double deltaSeconds)
        {
            if (_visualOffsetY < 0)
            {
                _visualOffsetY +=
                    deltaSeconds * 30.0;

                if (_visualOffsetY > 0)
                    _visualOffsetY = 0.0;
            }

            if (_stateTimer <= 0)
            {
                _visualOffsetY = 0.0;

                EnterIdleState(3.0);
            }
        }

        // =========================
        // DRAGGING
        // =========================

        public void OnBeginDrag()
        {
            _currentState =
                CatState.Dragged;

            _danceTimer = 0.0;
            _danceFrame = 0;

            _currentImage =
                _assets.GetFrame("dance_0");

            // Context-specific drag dialogue.
            SayDialogue(
                _dialogue.GetDragReaction());
        }

        public void OnEndDrag(
            double finalX,
            double finalY)
        {
            // Keep the actual dropped position for one moment.
            _movement.SetPosition(
                finalX,
                finalY);

            _currentImage =
                _assets.GetFrame("idle_proud");

            // Immediately start returning to center.
            Point center =
                _movement.GetExactScreenCenterPosition();

            _currentState =
                CatState.ReturningToCenter;

            _stateTimer = 0.0;

            _visualOffsetY = 0.0;

            _movement.MoveTo(
                center.X,
                center.Y,
                300.0);

            _isFlippedHorizontally =
                _movement.IsFacingLeft;
        }

        private void UpdateDragged(
            double deltaSeconds)
        {
            _danceTimer += deltaSeconds;

            if (_danceTimer >= 0.12)
            {
                _danceTimer = 0.0;

                _danceFrame =
                    (_danceFrame + 1) % 2;

                _currentImage =
                    (_danceFrame == 0)
                        ? _assets.GetFrame("dance_0")
                        : _assets.GetFrame("dance_1");
            }
        }

        // =========================
        // RETURN TO SCREEN CENTER
        // =========================

        private void UpdateReturningToCenter(
            double deltaSeconds)
        {
            bool finished =
                _movement.Update(deltaSeconds);

            _isFlippedHorizontally =
                _movement.IsFacingLeft;

            var walkFrames =
                _assets.GetAnimation("walk");

            if (walkFrames.Count > 0)
            {
                int frameIndex =
                    _movement.WalkFrameIndex %
                    walkFrames.Count;

                _currentImage =
                    walkFrames[frameIndex];
            }

            if (finished)
            {
                // Make absolutely sure the final position
                // is the exact center.
                Point center =
                    _movement.GetExactScreenCenterPosition();

                _movement.SetPosition(
                    center.X,
                    center.Y);

                _currentImage =
                    _assets.GetFrame("idle_proud");

                // Context-specific return dialogue.
                SayDialogue(
                    _dialogue.GetReturnToCenterLine());

                EnterIdleState(4.0);
            }
        }

        // =========================
        // SNOOZING
        // =========================

        public void TriggerSnooze(
            double durationSeconds = 90.0)
        {
            _currentState =
                CatState.Snoozing;

            _stateTimer =
                durationSeconds;

            _currentImage =
                _assets.GetFrame("idle_happy");

            if (OnHideSpeechBubble != null)
                OnHideSpeechBubble();
        }

        private void UpdateSnoozing(
            double deltaSeconds)
        {
            if (_stateTimer <= 0)
            {
                SayDialogue(
                    new DialogueItem(
                        "ഞാൻ ഉണർന്നു! വീണ്ടും തുടങ്ങാം!",
                        "I'm awake! Let the annoyance resume!"));

                EnterIdleState(3.0);
            }
        }

        // =========================
        // DIALOGUE
        // =========================

        public void SayDialogue(
            DialogueItem item)
        {
            _isTalking = true;

            _talkDuration =
                Math.Min(
                    item.DurationSeconds,
                    5.0);

            _talkTimer =
                _talkDuration;

            _mouthFrameIndex = 0;
            _mouthFrameTimer = 0.0;

            if (OnShowSpeechBubble != null)
            {
                OnShowSpeechBubble(
                    item.Malayalam,
                    item.English,
                    item.DurationSeconds);
            }
        }

        public void SayRandomQuip()
        {
            SayDialogue(
                _dialogue.GetRandomQuip());
        }

        // =========================
        // RANDOM ACTION SELECTION
        // =========================

        private void PickNextAction()
        {
            int roll =
                _rand.Next(100);

            if (roll < 35)
            {
                // Walk along bottom.
                Point target =
                    _movement.PickRandomBottomPosition();

                EnterWalkingState(
                    target,
                    120.0 +
                    _rand.Next(40));
            }
            else if (roll < 55)
            {
                // Block center.
                TriggerBlockScreenCenter();
            }
            else if (roll < 70)
            {
                // Dance.
                TriggerDance();
            }
            else if (roll < 82)
            {
                // Pounce.
                TriggerPlayfulPounce();
            }
            else if (roll < 92)
            {
                // Productivity sarcasm.
                SayDialogue(
                    _dialogue.GetProductivityJab());

                EnterIdleState(4.0);
            }
            else
            {
                // Disappear.
                TriggerDisappear();
            }
        }
    }
}