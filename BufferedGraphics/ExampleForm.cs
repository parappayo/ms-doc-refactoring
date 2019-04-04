using System;
using System.Drawing;
using System.Windows.Forms;

namespace BufferedGraphicsExample
{
    public class ExampleForm : Form
    {
        private readonly BufferedGraphicsContext _context;
        private BufferedGraphics _grafx;

        private byte _bufferingMode;
        private byte _updateCount;
        private byte _clearBufferFrequency = 5;

        private readonly string[] _bufferingModeStrings = {
            "Draw to Form without OptimizedDoubleBufferring control style",
            "Draw to Form using OptimizedDoubleBuffering control style",
            "Draw to HDC for form"
        };

        private readonly Timer _redrawTimer;

        public ExampleForm() : base()
        {
            // Configure the Form for this example.
            this.Text = "User double buffering";
            this.MouseDown += new MouseEventHandler(this.MouseDownHandler);
            this.Resize += new EventHandler(this.OnResize);
            this.SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true );

            _redrawTimer = new Timer
            {
                Interval = 200
            };
            _redrawTimer.Tick += OnRedrawTimer;

            _bufferingMode = 2;
            _updateCount = 0;

            // Retrieves the BufferedGraphicsContext for the
            // current application domain.
            _context = BufferedGraphicsManager.Current;

            // Sets the maximum size for the primary graphics buffer
            // of the buffered graphics context for the application
            // domain.  Any allocation requests for a buffer larger 
            // than this will create a temporary buffered graphics 
            // context to host the graphics buffer.
            _context.MaximumBuffer = new Size(this.Width+1, this.Height+1);

            // Allocates a graphics buffer the size of this form
            // using the pixel format of the Graphics created by 
            // the Form.CreateGraphics() method, which returns a 
            // Graphics object that matches the pixel format of the form.
            _grafx = _context.Allocate(this.CreateGraphics(),
                 new Rectangle( 0, 0, this.Width, this.Height ));

            // Draw the first frame to the buffer.
            DrawToBuffer(_grafx.Graphics);
        }

        private void MouseDownHandler(object sender, MouseEventArgs e)
        {
            if( e.Button == MouseButtons.Right )
            {
                // Cycle the buffering mode.
                if( ++_bufferingMode > 2 ) {
                    _bufferingMode = 0;
                }

                // If the previous buffering mode used
                // the OptimizedDoubleBuffering ControlStyle,
                // disable the control style.
                if( _bufferingMode == 1 ) {
                    this.SetStyle( ControlStyles.OptimizedDoubleBuffer, true );
                }

                // If the current buffering mode uses
                // the OptimizedDoubleBuffering ControlStyle,
                // enable the control style.
                if( _bufferingMode == 2 ) {
                    this.SetStyle( ControlStyles.OptimizedDoubleBuffer, false );
                }

                // Cause the background to be cleared and redraw.
                _updateCount = 6;
                DrawToBuffer(_grafx.Graphics);

                this.Refresh();
            }

            if( e.Button == MouseButtons.Left )
            {
                ToggleRedrawTimer();
            }
        }

        private void OnRedrawTimer(object sender, EventArgs e)
        {
            // Draw randomly positioned ellipses to the buffer.
            DrawToBuffer(_grafx.Graphics);

            // If in bufferingMode 2, draw to the form's HDC.
            if( _bufferingMode == 2 ) {
                // Render the graphics buffer to the form's HDC.
                _grafx.Render(Graphics.FromHwnd(this.Handle));
            }
            else {
                // If in bufferingMode 0 or 1, draw in the paint method.
                this.Refresh();
            }
        }

        private void OnResize(object sender, EventArgs e)
        {
           // Re-create the graphics buffer for a new window size.
           _context.MaximumBuffer = new Size(this.Width+1, this.Height+1);

            if( _grafx != null )
            {
                _grafx.Dispose();
                _grafx = null;
            }

            _grafx = _context.Allocate(
                this.CreateGraphics(), 
                new Rectangle( 0, 0, this.Width, this.Height ));
           
           ForceRefresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            _grafx.Render(e.Graphics);
        }

        private void ForceRefresh()
        {
            _updateCount = _clearBufferFrequency;
            DrawToBuffer(_grafx.Graphics);
            this.Refresh();
        }

        private void DrawToBuffer(Graphics g)
        {
            if( ++_updateCount > _clearBufferFrequency )
            {
                _updateCount = 0;
                _grafx.Graphics.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);
            }

            Random rnd = new Random();
            for( int i=0; i<20; i++ )
            {
                DrawRandomEllipse(g, rnd);
            }

            DrawInfoStrings(g);
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

        private Rectangle RandomRectangle(Random random, int margin, int minSize)
        {
            int px = random.Next(margin, this.Width - margin - minSize);
            int py = random.Next(margin, this.Height - margin - minSize);

            return new Rectangle(
                px, py,
                px + random.Next(0, this.Width - px - margin),
                py + random.Next(0, this.Height - py - margin));
        }

        private void DrawRandomEllipse(Graphics g, Random rnd)
        {
            var randomRectangle = RandomRectangle(rnd, 20, 20);
            int penWidth = 1;

            g.DrawEllipse(
                new Pen(RandomColor(rnd), penWidth),
                randomRectangle);
        }

        private void DrawInfoStrings(Graphics g)
        {
            g.DrawString("Buffering Mode: " + _bufferingModeStrings[_bufferingMode], new Font("Arial", 8), Brushes.White, 10,
                10);
            g.DrawString("Right-click to cycle buffering mode", new Font("Arial", 8), Brushes.White, 10, 22);
            g.DrawString("Left-click to toggle timed display refresh", new Font("Arial", 8), Brushes.White, 10, 34);
        }
    }
}
