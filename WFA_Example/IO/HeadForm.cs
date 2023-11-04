using System;
using System.Drawing;
using System.Windows.Forms;

namespace GDPlotter
{
    class HeadForm : Form
    {
        FancyInputPanel inputPanel;
        PlotForm plot;
        public HeadForm()
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


            inputPanel = new FancyInputPanel();

            inputPanel.SuspendLayout();

            inputPanel.Width = 400;
            inputPanel.HeaderText = "double f(double x) { ";
            inputPanel.HeaderHeight = 35;
            inputPanel.BodyText = "return Math.Sin(1.0 + x + Math.Cos(x));";
            inputPanel.FooterText = "  }";
            inputPanel.FooterHeight = 35;
            inputPanel.BodyHeight = 100;
            inputPanel.Padding = new Padding(10, 5, 10, 5);
            inputPanel.Font = new Font("Arial", 13F);
            Controls.Add(inputPanel);

            inputPanel.ButtonMouseDown += PlotInputButtonMouseDown;

            inputPanel.ResumeLayout(true);

            ResumeLayout(true);

            plot = new PlotForm();
            plot.FormClosed += PlotClosed;

            plot.Show();
            UpdateFx();
        }

        void PlotInputButtonMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            UpdateFx();
        }

        void UpdateFx()
        {
            FxPlotController controller = plot.Display.PlotController;
            controller.SuspendRender();
            plot.Display.GraphColor = inputPanel.SelectedColor;
            controller.Fx = RuntimeCompiler.CompileFunction("x", inputPanel.BodyText);
            controller.ResumeRender();
            plot.Display.RefreshRender(this, EventArgs.Empty);
        }

        void PlotClosed(object sender, EventArgs e)
        {
            Close();
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