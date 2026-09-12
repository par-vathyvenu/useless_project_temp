
using System;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace AnnoyingCat
{
    public partial class MainWindow : Window
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private readonly AssetManager _assets;
        private readonly DialogueManager _dialogue;
        private readonly MovementManager _movement;
        private readonly CatController _controller;
        private readonly DispatcherTimer _gameTimer;
        private DateTime _lastFrameTime;

        // Dragging state
        private bool _isDragging = false;
        private Point _dragStartPoint;
        private double _dragStartLeft;
        private double _dragStartTop;
        private bool _hasDraggedEnough = false;

        // Speech bubble timer
        private readonly DispatcherTimer _speechHideTimer;

        public CatController Controller
        {
            get { return _controller; }
        }

        public MainWindow(string baseDir = null)
        {
            InitializeComponent();

            string resolvedBaseDir = AssetManager.ResolveBaseDirectory(baseDir);
            _assets = new AssetManager(resolvedBaseDir);
            _dialogue = new DialogueManager();

            // Set initial position at bottom-right corner
            Rect workArea = SystemParameters.WorkArea;

            double startX = workArea.Right - this.Width - 40;
            double startY = workArea.Bottom - this.Height - 10;

            this.Left = startX;
            this.Top = startY;

            // Pass the actual window size to MovementManager
            _movement = new MovementManager(
                startX,
                startY,
                6,
                this.Width,
                this.Height
            );

            _controller = new CatController(_assets, _dialogue, _movement);

            // Wire up callbacks
            _controller.OnShowSpeechBubble = ShowSpeechBubble;
            _controller.OnHideSpeechBubble = HideSpeechBubble;
            _controller.OnPlayPopSound = PlayPopSound;

            // Immediately set initial sprite
            if (_controller.CurrentImage != null)
            {
                CatImage.Source = _controller.CurrentImage;
            }
            else
            {
                Console.WriteLine("[MainWindow] Warning: _controller.CurrentImage is null!");
            }

            // Speech bubble timer
            _speechHideTimer = new DispatcherTimer();

            _speechHideTimer.Tick += (s, e) =>
            {
                _speechHideTimer.Stop();
                HideSpeechBubble();
            };

            // Main update loop (~60 FPS)
            _lastFrameTime = DateTime.Now;

            _gameTimer = new DispatcherTimer(DispatcherPriority.Render);
            _gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            _gameTimer.Tick += GameLoop_Tick;
            _gameTimer.Start();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Update MovementManager with the actual rendered window size
            _movement.SetWindowSize(
                this.ActualWidth,
                this.ActualHeight
            );

            // Apply WS_EX_NOACTIVATE and WS_EX_TOOLWINDOW
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            int exStyle = GetWindowLong(
                hwnd,
                GWL_EXSTYLE
            );

            SetWindowLong(
                hwnd,
                GWL_EXSTYLE,
                exStyle | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW
            );

            if (CatImage.Source == null && _controller.CurrentImage != null)
            {
                CatImage.Source = _controller.CurrentImage;
            }

            // Say welcome quip after 1.2 seconds
            var welcomeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1.2)
            };

            welcomeTimer.Tick += (s, ev) =>
            {
                welcomeTimer.Stop();

                _controller.SayDialogue(
                    new DialogueItem(
                        "ഹലോ ബോസ്സ്! വീണ്ടും ശല്യം ചെയ്യാൻ ഞാൻ എത്തി!",
                        "Hello boss! Arrived to annoy you once again!"
                    )
                );
            };

            welcomeTimer.Start();
        }

        private void GameLoop_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            double delta =
                (now - _lastFrameTime).TotalSeconds;

            if (delta > 0.1)
                delta = 0.1;

            _lastFrameTime = now;

            if (!_isDragging)
            {
                _controller.Update(delta);

                // Sync Window position with movement
                this.Left = _movement.CurrentX;
                this.Top = _movement.CurrentY;
            }

            // Sync visual presentation
            if (_controller.CurrentImage != null &&
                CatImage.Source != _controller.CurrentImage)
            {
                CatImage.Source = _controller.CurrentImage;
            }

            CatScale.ScaleX =
                (_controller.IsFlippedHorizontally ? -1.0 : 1.0)
                * _controller.Scale;

            CatScale.ScaleY = _controller.Scale;

            CatTranslate.Y = _controller.VisualOffsetY;

            this.Opacity = _controller.Opacity;

            Visibility expectedVis =
                _controller.IsWindowVisible
                    ? Visibility.Visible
                    : Visibility.Hidden;

            if (this.Visibility != expectedVis)
            {
                this.Visibility = expectedVis;
            }
        }

        // ================= SPEECH BUBBLE =================

        private void ShowSpeechBubble(
            string malayalam,
            string english,
            double durationSeconds)
        {
            TxtMalayalam.Text = malayalam;
            TxtEnglish.Text = english;

            var fadeIn = new DoubleAnimation(
                0,
                1,
                TimeSpan.FromMilliseconds(250)
            );

            SpeechBubbleContainer.BeginAnimation(
                UIElement.OpacityProperty,
                fadeIn
            );

            _speechHideTimer.Stop();

            _speechHideTimer.Interval =
                TimeSpan.FromSeconds(durationSeconds);

            _speechHideTimer.Start();
        }

        private void HideSpeechBubble()
        {
            var fadeOut = new DoubleAnimation(
                SpeechBubbleContainer.Opacity,
                0,
                TimeSpan.FromMilliseconds(300)
            );

            SpeechBubbleContainer.BeginAnimation(
                UIElement.OpacityProperty,
                fadeOut
            );
        }

        private void PlayPopSound()
        {
            try
            {
                SystemSounds.Asterisk.Play();
            }
            catch
            {
            }
        }

        // ================= MOUSE INTERACTIONS =================

        private void Cat_MouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            _isDragging = true;
            _hasDraggedEnough = false;

            _dragStartPoint = e.GetPosition(this);

            _dragStartLeft = this.Left;
            _dragStartTop = this.Top;

            CatContainer.CaptureMouse();

            e.Handled = true;
        }

        private void Cat_MouseMove(
            object sender,
            MouseEventArgs e)
        {
            if (!_isDragging)
                return;

            Point currentPoint = e.GetPosition(this);

            double dx =
                currentPoint.X - _dragStartPoint.X;

            double dy =
                currentPoint.Y - _dragStartPoint.Y;

            if (!_hasDraggedEnough &&
                (Math.Abs(dx) > 6 ||
                 Math.Abs(dy) > 6))
            {
                _hasDraggedEnough = true;

                _controller.OnBeginDrag();
            }

            if (_hasDraggedEnough)
            {
                this.Left += dx;
                this.Top += dy;

                _movement.CurrentX = this.Left;
                _movement.CurrentY = this.Top;
            }
        }

        private void Cat_MouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs e)
        {
            if (!_isDragging)
                return;

            CatContainer.ReleaseMouseCapture();

            _isDragging = false;

            if (!_hasDraggedEnough)
            {
                _controller.OnClick();
            }
            else
            {
                _controller.OnEndDrag(
                    this.Left,
                    this.Top
                );
            }

            e.Handled = true;
        }

        private void Cat_MouseRightButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            ContextMenu menu = new ContextMenu();

            MenuItem miNap = new MenuItem
            {
                Header = "💤 ശല്യം ചെയ്യല്ലേ (Take a Nap - 2m)"
            };

            miNap.Click += (s, ev) =>
                _controller.TriggerSnooze(120.0);

            MenuItem miCenter = new MenuItem
            {
                Header = "🎯 ഇവിടെ വാ (Come to Center)"
            };

            miCenter.Click += (s, ev) =>
                _controller.TriggerBlockScreenCenter();

            MenuItem miQuip = new MenuItem
            {
                Header = "💬 ഒരു ഡയലോഗ് പറ (Say Something)"
            };

            miQuip.Click += (s, ev) =>
                _controller.SayRandomQuip();

            MenuItem miDance = new MenuItem
            {
                Header = "💃 ഡാൻസ് കളിക്ക് (Dance for me!)"
            };

            miDance.Click += (s, ev) =>
                _controller.TriggerDance();

            MenuItem miPounce = new MenuItem
            {
                Header = "🐾 ചാട്! (Pounce!)"
            };

            miPounce.Click += (s, ev) =>
                _controller.TriggerPlayfulPounce();

            MenuItem miDisappear = new MenuItem
            {
                Header = "👻 പോ അവിടുന്ന് (Disappear / Shoo)"
            };

            miDisappear.Click += (s, ev) =>
                _controller.TriggerDisappear();

            MenuItem miExit = new MenuItem
            {
                Header = "❌ വിട പറയുക (Exit)"
            };

            miExit.Click += (s, ev) =>
                Application.Current.Shutdown();

            menu.Items.Add(miNap);
            menu.Items.Add(miCenter);
            menu.Items.Add(miQuip);
            menu.Items.Add(miDance);
            menu.Items.Add(miPounce);
            menu.Items.Add(miDisappear);
            menu.Items.Add(new Separator());
            menu.Items.Add(miExit);

            menu.IsOpen = true;

            e.Handled = true;
        }
    }
}

