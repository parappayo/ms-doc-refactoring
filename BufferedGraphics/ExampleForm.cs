using System;
using System.Drawing;
using System.Windows.Forms;

namespace BufferedGraphicsExample
{
    public class ExampleForm : Form
    {
        private readonly BufferedGraphicsContext _context;
        private BufferedGraphics _bufferedGraphics;

        private byte _updateCount;
        private byte _clearBufferInterval = 5;

        enum BufferingMode {
            DrawToForm,
            DrawToFormWithDoubleBuffering,
            DrawToHDC,
        };

        private BufferingMode _bufferingMode;
        private readonly Timer _redrawTimer;
        private readonly Random _random = new Random();

        public ExampleForm() : base()
        {
            this.Text = "Double Buffering Example";
            this.MouseDown += this.MouseDownHandler;
            this.Resize += this.OnResize;
            this.SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true );

            _redrawTimer = new Timer
            {
                Interval = 200
            };
            _redrawTimer.Tick += OnRedrawTimer;

            _bufferingMode = BufferingMode.DrawToHDC;
            _updateCount = 0;

            _context = BufferedGraphicsManager.Current;
            ResizeBuffer();
            UpdateBuffer();
        }

        private void MouseDownHandler(object sender, MouseEventArgs e)
        {
            if( e.Button == MouseButtons.Right )
            {
                NextBufferingMode();

                this.SetStyle(
                    ControlStyles.OptimizedDoubleBuffer,
                    _bufferingMode == BufferingMode.DrawToFormWithDoubleBuffering);

                Clear();
            }

            if( e.Button == MouseButtons.Left )
            {
                ToggleRedrawTimer();
            }
        }

        private void OnRedrawTimer(object sender, EventArgs e)
        {
            UpdateBuffer();

            if( _bufferingMode == BufferingMode.DrawToHDC )
            {
                _bufferedGraphics.Render(Graphics.FromHwnd(this.Handle));
            }
            else
            {
                this.Refresh();
            }
        }

        private void OnResize(object sender, EventArgs e)
        {
            ResizeBuffer();
            Clear();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            _bufferedGraphics.Render(e.Graphics);
        }

        private void ResizeBuffer()
        {
            _context.MaximumBuffer = new Size(this.Width+1, this.Height+1);

            if( _bufferedGraphics != null )
            {
                _bufferedGraphics.Dispose();
            }

            _bufferedGraphics = _context.Allocate(
                this.CreateGraphics(), 
                new Rectangle( 0, 0, this.Width, this.Height ));
        }

        private void Clear()
        {
            _updateCount = _clearBufferInterval;
            UpdateBuffer();
            this.Refresh();
        }

        private void UpdateBuffer()
        {
            if( ++_updateCount > _clearBufferInterval )
            {
                _updateCount = 0;
                ClearBuffer(_bufferedGraphics, _context, Brushes.Black);
                DrawInfoStrings(_bufferedGraphics.Graphics);
            }

            DrawRandomEllipses(_bufferedGraphics.Graphics, _random, 20);
        }

        private void DrawRandomEllipses(Graphics graphics, Random random, int count)
        {
            for (int i = 0; i < count; i++)
            {
                DrawRandomEllipse(_bufferedGraphics.Graphics, random);
            }
        }

        private void ClearBuffer(BufferedGraphics graphics, BufferedGraphicsContext context, Brush clearColor)
        {
            var clearRect = new Rectangle(new Point(0, 0), context.MaximumBuffer);
            graphics.Graphics.FillRectangle(clearColor, clearRect);
        }

        private void NextBufferingMode()
        {
            if( ++_bufferingMode > BufferingMode.DrawToHDC )
            {
                _bufferingMode = 0;
            }
        }

        private void ToggleRedrawTimer()
        {
            if (_redrawTimer.Enabled)
            {
                _redrawTimer.Stop();
            }
            else
            {
                _redrawTimer.Start();
            }
        }

        private Color RandomColor(Random random)
        {
            return Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
        }

        private Rectangle RandomRectangle(Random random, int margin)
        {
            int minX = margin;
            int minY = margin;
            int maxX = Math.Max(minX, this.Width - margin);
            int maxY = Math.Max(minY, this.Height - margin);

            int px = random.Next(minX, maxX);
            int py = random.Next(minY, maxY);

            return new Rectangle(
                px,
                py,
                random.Next(0, maxX - px),
                random.Next(0, maxY - py));
        }

        private void DrawRandomEllipse(Graphics g, Random rnd)
        {
            var randomRectangle = RandomRectangle(rnd, 20);
            int penWidth = 1;

            g.DrawEllipse(
                new Pen(RandomColor(rnd), penWidth),
                randomRectangle);
        }

        private void DrawInfoStrings(Graphics g)
        {
            var font = new Font("Arial", 8);
            var x = 10;
            var y = 10;
            var spacing = 12;

            g.DrawString("Buffering Mode: " + _bufferingMode.ToString(), font, Brushes.White, x, y);
            y += spacing;
            g.DrawString("Right-click to cycle buffering mode", font, Brushes.White, x, y);
            y += spacing;
            g.DrawString("Left-click to toggle timed display refresh", font, Brushes.White, x, y);
        }
    }
}
