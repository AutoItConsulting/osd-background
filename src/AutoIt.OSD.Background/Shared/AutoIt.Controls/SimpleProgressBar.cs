//  
// Copyright (c) AutoIt Consulting Ltd. All rights reserved.  
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.  
// using System;
//

using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoIt.Controls
{
    public partial class SimpleProgressBar : UserControl
    {
        private int _max = 100; // Maximum value for progress range

        private int _min; // Minimum value for progress range
        private int _val; // Current progress

        public SimpleProgressBar()
        {
            InitializeComponent();

            DrawBorder = false;
        }

        public bool DrawBorder { get; set; }

        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }

            set
            {
                base.ForeColor = value;

                // Invalidate the control to get a repaint.
                Invalidate();
            }
        }

        public int Maximum
        {
            get
            {
                return _max;
            }

            set
            {
                // Make sure that the maximum value is never set lower than the minimum value.
                if (value < _min)
                {
                    _min = value;
                }

                _max = value;

                // Make sure that value is still in range.
                if (_val > _max)
                {
                    _val = _max;
                }

                // Invalidate the control to get a repaint.
                Invalidate();
            }
        }

        public int Minimum
        {
            get
            {
                return _min;
            }

            set
            {
                // Prevent a negative value.
                if (value < 0)
                {
                    _min = 0;
                }

                // Make sure that the minimum value is never set higher than the maximum value.
                if (value > _max)
                {
                    _min = value;
                    _min = value;
                }

                // Ensure value is still in range
                if (_val < _min)
                {
                    _val = _min;
                }

                // Invalidate the control to get a repaint.
                Invalidate();
            }
        }

        public int Value
        {
            get
            {
                return _val;
            }

            set
            {
                int oldValue = _val;

                // Make sure that the value does not stray outside the valid range.
                if (value < _min)
                {
                    _val = _min;
                }
                else if (value > _max)
                {
                    _val = _max;
                }
                else
                {
                    _val = value;
                }

                // Invalidate only the changed area.
                Rectangle newValueRect = ClientRectangle;
                Rectangle oldValueRect = ClientRectangle;

                // Use a new value to calculate the rectangle for progress.
                float percent = (_val - _min) / (float)(_max - _min);
                newValueRect.Width = (int)(newValueRect.Width * percent);

                // Use an old value to calculate the rectangle for progress.
                percent = (oldValue - _min) / (float)(_max - _min);
                oldValueRect.Width = (int)(oldValueRect.Width * percent);

                var updateRect = new Rectangle();

                // Find only the part of the screen that must be updated.
                if (newValueRect.Width > oldValueRect.Width)
                {
                    updateRect.X = oldValueRect.Size.Width;
                    updateRect.Width = newValueRect.Width - oldValueRect.Width;
                }
                else
                {
                    updateRect.X = newValueRect.Size.Width;
                    updateRect.Width = oldValueRect.Width - newValueRect.Width;
                }

                updateRect.Height = Height;

                // Invalidate the intersection region only.
                Invalidate(updateRect);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            var brush = new SolidBrush(ForeColor);
            float percent = (_val - _min) / (float)(_max - _min);
            Rectangle rect = ClientRectangle;

            // Calculate area for drawing the progress.
            rect.Width = (int)(rect.Width * percent);

            // Draw the progress meter.
            g.FillRectangle(brush, rect);

            // Draw a three-dimensional border around the control.
            if (DrawBorder)
            {
                Draw3DBorder(g);
            }

            // Clean up.
            brush.Dispose();
            g.Dispose();
        }

        protected override void OnResize(EventArgs e)
        {
            // Invalidate the control to get a repaint.
            Invalidate();
        }

        private void Draw3DBorder(Graphics g)
        {
            var penWidth = (int)Pens.White.Width;

            g.DrawLine(
                Pens.DarkGray,
                new Point(ClientRectangle.Left, ClientRectangle.Top),
                new Point(ClientRectangle.Width - penWidth, ClientRectangle.Top));
            g.DrawLine(
                Pens.DarkGray,
                new Point(ClientRectangle.Left, ClientRectangle.Top),
                new Point(ClientRectangle.Left, ClientRectangle.Height - penWidth));
            g.DrawLine(
                Pens.White,
                new Point(ClientRectangle.Left, ClientRectangle.Height - penWidth),
                new Point(ClientRectangle.Width - penWidth, ClientRectangle.Height - penWidth));
            g.DrawLine(
                Pens.White,
                new Point(ClientRectangle.Width - penWidth, ClientRectangle.Top),
                new Point(ClientRectangle.Width - penWidth, ClientRectangle.Height - penWidth));
        }
    }
}