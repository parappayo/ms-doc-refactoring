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
        private byte _clearBufferFrequency = 5;

        enum BufferingMode {
            DrawToForm,
            DrawToFormWithDoubleBuffering,
            DrawToHDC,
        };

        private BufferingMode _bufferingMode;
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

            _bufferingMode = BufferingMode.DrawToHDC;
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
            _bufferedGraphics = _context.Allocate(this.CreateGraphics(),
                 new Rectangle( 0, 0, this.Width, this.Height ));

            // Draw the first frame to the buffer.
            DrawToBuffer(_bufferedGraphics.Graphics);
        }

        private void MouseDownHandler(object sender, MouseEventArgs e)
        {
            if( e.Button == MouseButtons.Right )
            {
                NextBufferingMode();

                this.SetStyle(
                    ControlStyles.OptimizedDoubleBuffer,
                    _bufferingMode == BufferingMode.DrawToFormWithDoubleBuffering);

                ForceRefresh();
            }

            if( e.Button == MouseButtons.Left )
            {
                ToggleRedrawTimer();
            }
        }

        private void OnRedrawTimer(object sender, EventArgs e)
        {
            DrawToBuffer(_bufferedGraphics.Graphics);

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
           // Re-create the graphics buffer for a new window size.
           _context.MaximumBuffer = new Size(this.Width+1, this.Height+1);

            if( _bufferedGraphics != null )
            {
                _bufferedGraphics.Dispose();
                _bufferedGraphics = null;
            }

            _bufferedGraphics = _context.Allocate(
                this.CreateGraphics(), 
                new Rectangle( 0, 0, this.Width, this.Height ));
           
           ForceRefresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            _bufferedGraphics.Render(e.Graphics);
        }

        // TODO: needs a better name - not so much refresh and restarting the cycle
        private void ForceRefresh()
        {
            _updateCount = _clearBufferFrequency;
            DrawToBuffer(_bufferedGraphics.Graphics);
            this.Refresh();
        }

        private void DrawToBuffer(Graphics g)
        {
            // TODO: separate concerns here, clearing is outside the scope of this method
            if( ++_updateCount > _clearBufferFrequency )
            {
                _updateCount = 0;
                _bufferedGraphics.Graphics.FillRectangle(Brushes.Black, 0, 0, this.Width, this.Height);
            }

            Random rnd = new Random(); // TODO: don't alloc every frame
            for( int i=0; i<20; i++ )
            {
                DrawRandomEllipse(g, rnd);
            }

            DrawInfoStrings(g);
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
            g.DrawString("Buffering Mode: " + _bufferingMode.ToString(), new Font("Arial", 8), Brushes.White, 10,
                10);
            g.DrawString("Right-click to cycle buffering mode", new Font("Arial", 8), Brushes.White, 10, 22);
            g.DrawString("Left-click to toggle timed display refresh", new Font("Arial", 8), Brushes.White, 10, 34);
        }
    }
}
