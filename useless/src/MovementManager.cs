
using System;
using System.Windows;

namespace AnnoyingCat
{
    public class MovementManager
    {
        private readonly Random _rand = new Random();

        private double _currentX;
        private double _currentY;
        private double _targetX;
        private double _targetY;

        private double _speed = 130.0;
        private bool _isMoving = false;
        private bool _isFacingLeft = false;

        private double _accumulatedDistance = 0.0;
        private const double DistancePerFrame = 22.0;

        private int _walkFrameIndex = 0;
        private int _totalWalkFrames = 6;

        // Pounce arc variables
        private bool _isPouncing = false;
        private double _pounceStartX;
        private double _pounceStartY;
        private double _pounceTargetX;
        private double _pounceTargetY;
        private double _pounceProgress = 0.0;
        private double _pounceDuration = 0.55;

        private const double PounceHeight = 90.0;

        // Actual window size
        private double _windowWidth = 360.0;
        private double _windowHeight = 420.0;

        public double CurrentX
        {
            get { return _currentX; }
            set
            {
                Point safePosition = ClampPosition(value, _currentY);
                _currentX = safePosition.X;
                _currentY = safePosition.Y;
            }
        }

        public double CurrentY
        {
            get { return _currentY; }
            set
            {
                Point safePosition = ClampPosition(_currentX, value);
                _currentX = safePosition.X;
                _currentY = safePosition.Y;
            }
        }

        public bool IsMoving
        {
            get { return _isMoving || _isPouncing; }
        }

        public bool IsFacingLeft
        {
            get { return _isFacingLeft; }
        }

        public int WalkFrameIndex
        {
            get { return _walkFrameIndex; }
        }

        public bool IsPouncing
        {
            get { return _isPouncing; }
        }

        public MovementManager(
            double startX,
            double startY,
            int totalWalkFrames = 6,
            double windowWidth = 360.0,
            double windowHeight = 420.0)
        {
            _totalWalkFrames = Math.Max(1, totalWalkFrames);

            _windowWidth = windowWidth;
            _windowHeight = windowHeight;

            Point safeStart = ClampPosition(startX, startY);

            _currentX = safeStart.X;
            _currentY = safeStart.Y;

            _targetX = safeStart.X;
            _targetY = safeStart.Y;
        }

        public void SetWindowSize(double width, double height)
        {
            if (width > 0)
                _windowWidth = width;

            if (height > 0)
                _windowHeight = height;

            // Immediately correct position if the window size changed
            Point safePosition =
                ClampPosition(_currentX, _currentY);

            _currentX = safePosition.X;
            _currentY = safePosition.Y;

            Point safeTarget =
                ClampPosition(_targetX, _targetY);

            _targetX = safeTarget.X;
            _targetY = safeTarget.Y;
        }

        public void SetTotalWalkFrames(int count)
        {
            _totalWalkFrames = Math.Max(1, count);
        }

        public Rect GetWorkArea()
        {
            // WorkArea excludes the Windows taskbar.
            return SystemParameters.WorkArea;
        }

        private Point ClampPosition(double x, double y)
        {
            Rect workArea = GetWorkArea();

            // Left and top limits
            double minX = workArea.Left;
            double minY = workArea.Top;

            // Right and bottom limits.
            // Subtract the complete window size so the cat
            // never goes underneath the taskbar or outside
            // the usable desktop area.
            double maxX =
                workArea.Right - _windowWidth;

            double maxY =
                workArea.Bottom - _windowHeight;

            // Prevent invalid ranges if the window is larger
            // than the available work area.
            if (maxX < minX)
                maxX = minX;

            if (maxY < minY)
                maxY = minY;

            double clampedX =
                Math.Max(
                    minX,
                    Math.Min(maxX, x)
                );

            double clampedY =
                Math.Max(
                    minY,
                    Math.Min(maxY, y)
                );

            return new Point(
                clampedX,
                clampedY
            );
        }

        public void SetPosition(double x, double y)
        {
            Point safePosition =
                ClampPosition(x, y);

            _currentX = safePosition.X;
            _currentY = safePosition.Y;

            _targetX = safePosition.X;
            _targetY = safePosition.Y;
        }

        public void MoveTo(
            double x,
            double y,
            double speed = 140.0)
        {
            Point position =
                ClampPosition(x, y);

            _targetX = position.X;
            _targetY = position.Y;

            _speed = speed;

            _isMoving = true;
            _isPouncing = false;

            _isFacingLeft =
                (_targetX < _currentX);
        }

        public void PounceTo(
            double targetX,
            double targetY)
        {
            Point position =
                ClampPosition(
                    targetX,
                    targetY
                );

            _pounceStartX = _currentX;
            _pounceStartY = _currentY;

            _pounceTargetX = position.X;
            _pounceTargetY = position.Y;

            _pounceProgress = 0.0;

            _isPouncing = true;
            _isMoving = false;

            _isFacingLeft =
                (_pounceTargetX < _currentX);
        }

        public Point PickRandomBottomPosition()
        {
            Rect workArea = GetWorkArea();

            double minX =
                workArea.Left + 20;

            double maxX =
                Math.Max(
                    minX,
                    workArea.Right -
                    _windowWidth -
                    20
                );

            double x =
                minX +
                _rand.NextDouble() *
                (maxX - minX);

            // IMPORTANT:
            // WorkArea.Bottom is already above the taskbar.
            double y =
                workArea.Bottom -
                _windowHeight -
                10;

            return ClampPosition(x, y);
        }
        public Point GetExactScreenCenterPosition()
{
    Rect workArea = GetWorkArea();

    double x =
        workArea.Left +
        (workArea.Width -
         _windowWidth) /
        2.0;

    double y =
        workArea.Top +
        (workArea.Height -
         _windowHeight) /
        2.0;

    return ClampPosition(x, y);
}
        
    public Point PickScreenCenterPosition()
{
    return GetExactScreenCenterPosition();
}
        public Point PickRandomScreenPosition()
        {
            Rect workArea = GetWorkArea();

            double minX =
                workArea.Left + 50;

            double maxX =
                Math.Max(
                    minX,
                    workArea.Right -
                    _windowWidth -
                    50
                );

            double minY =
                workArea.Top + 50;

            double maxY =
                Math.Max(
                    minY,
                    workArea.Bottom -
                    _windowHeight -
                    50
                );

            double x =
                minX +
                _rand.NextDouble() *
                (maxX - minX);

            double y =
                minY +
                _rand.NextDouble() *
                (maxY - minY);

            return ClampPosition(x, y);
        }

        public void Stop()
        {
            _isMoving = false;
            _isPouncing = false;

            _targetX = _currentX;
            _targetY = _currentY;
        }

        public bool Update(double deltaSeconds)
        {
            // -------------------------
            // POUNCE
            // -------------------------
            if (_isPouncing)
            {
                _pounceProgress +=
                    deltaSeconds /
                    _pounceDuration;

                if (_pounceProgress >= 1.0)
                {
                    _pounceProgress = 1.0;

                    Point safePosition =
                        ClampPosition(
                            _pounceTargetX,
                            _pounceTargetY
                        );

                    _currentX =
                        safePosition.X;

                    _currentY =
                        safePosition.Y;

                    _isPouncing = false;

                    return true;
                }

                double t =
                    _pounceProgress;

                // Linear X
                double pounceNextX =
                    _pounceStartX +
                    (_pounceTargetX -
                     _pounceStartX) *
                    t;

                // Base Y
                double baseY =
                    _pounceStartY +
                    (_pounceTargetY -
                     _pounceStartY) *
                    t;

                // Parabolic jump arc
                double arc =
                    4.0 *
                    PounceHeight *
                    t *
                    (1.0 - t);

                double pounceNextY =
                    baseY - arc;

                // Keep the window inside the usable
                // work area even during the jump.
                Point safePouncePosition =
                    ClampPosition(
                       pounceNextX,
                        pounceNextY
                    );

                _currentX =
                    safePouncePosition.X;

                _currentY =
                    safePouncePosition.Y;

                return false;
            }

            // -------------------------
            // NORMAL MOVEMENT
            // -------------------------
            if (!_isMoving)
                return true;

            double dx =
                _targetX - _currentX;

            double dy =
                _targetY - _currentY;

            double dist =
                Math.Sqrt(
                    dx * dx +
                    dy * dy
                );

            if (dist < 4.0)
            {
                Point safePosition =
                    ClampPosition(
                        _targetX,
                        _targetY
                    );

                _currentX =
                    safePosition.X;

                _currentY =
                    safePosition.Y;

                _isMoving = false;

                return true;
            }

            double step =
                _speed *
                deltaSeconds;

            if (step >= dist)
            {
                Point safePosition =
                    ClampPosition(
                        _targetX,
                        _targetY
                    );

                _currentX =
                    safePosition.X;

                _currentY =
                    safePosition.Y;

                _isMoving = false;

                return true;
            }

            double ratio =
                step / dist;

            double movementnextX =
                _currentX +
                dx * ratio;

            double movementnextY =
                _currentY +
                dy * ratio;

            Point safeMovement =
                ClampPosition(
                movementnextX,
                movementnextY
                );

            _currentX =
                safeMovement.X;

            _currentY =
                safeMovement.Y;

            _isFacingLeft =
                (dx < 0);

            // -------------------------
            // WALK ANIMATION
            // -------------------------
            _accumulatedDistance += step;

            if (_accumulatedDistance >=
                DistancePerFrame)
            {
                _walkFrameIndex =
                    (_walkFrameIndex + 1) %
                    _totalWalkFrames;

                _accumulatedDistance -=
                    DistancePerFrame;
            }

            return false;
        }
    }
}


