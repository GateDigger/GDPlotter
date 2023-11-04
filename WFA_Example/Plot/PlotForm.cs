using System;
using System.Drawing;
using System.Windows.Forms;

namespace GDPlotter
{
    class PlotForm : Form
    {
        PlotDisplay display;
        public PlotForm()
        {
            InitializeComponent();
        }

        System.ComponentModel.IContainer components;
        private void InitializeComponent()
        {
            SuspendLayout();

            this.Text = "AAAAAAA";
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            display = new PlotDisplay();
            display.PlotController.SuspendRender();
            display.PlotController.PlotArea = new RectangleD(-5.0, -5.0, 5.0, 5.0);
            display.BackColor = Color.White;
            display.ForeColor = Color.Black;
            display.Font = new Font("Arial", 10, FontStyle.Regular);
            display.Location = new Point(Padding.Left, Padding.Top);
            display.Size = new Size(600, 600);
            display.GraphColor = Color.Blue;
            display.GridPen = new Pen(Color.LightGray, 1F);
            display.CrossPen = new Pen(Color.Black, 1F);
            display.FramePen = new Pen(Color.BlueViolet, 1F);
            display.CoordStringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            Controls.Add(display);
            display.PlotController.ResumeRender();
            display.RefreshRender(this, EventArgs.Empty);

            ResumeLayout(true);
        }

        public PlotDisplay Display
        {
            get
            {
                return display;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}