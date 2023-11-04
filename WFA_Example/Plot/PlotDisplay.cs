using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

using static GDPlotter.Utility;

namespace GDPlotter
{
    public sealed class PlotDisplay : Control
    {
        public PlotDisplay()
        {
            DoubleBuffered = true;
            BackgroundImageLayout = ImageLayout.None;

            PlotController = new FxPlotController();
            PlotController.ControlAreaChanged += RefreshRender;
            PlotController.PlotAreaChanged += RefreshRender;
            PlotController.CurrentSelectionChanged += Invalidate;
            GraphColorChanged += RefreshRender;
            GridPenChanged += RefreshRender;
            CrossPenChanged += RefreshRender;
            FramePenChanged += RefreshRender;
            PlotControllerChanged += RefreshRender;
        }

        #region Properties

        ARGB graphColor = Color.Blue;
        public event EventHandler GraphColorChanged;
        public ARGB GraphColor
        {
            get
            {
                return graphColor;
            }
            set
            {
                graphColor = value;
                if (GraphColorChanged != null)
                    GraphColorChanged(this, EventArgs.Empty);
            }
        }

        Pen gridPen = Pens.LightGray;
        public event EventHandler GridPenChanged;
        public Pen GridPen
        {
            get
            {
                return gridPen;
            }
            set
            {
                gridPen = value;
                if (GridPenChanged != null)
                    GridPenChanged(this, EventArgs.Empty);
            }
        }

        Pen crossPen = Pens.Black;
        public event EventHandler CrossPenChanged;
        public Pen CrossPen
        {
            get
            {
                return crossPen;
            }
            set
            {
                crossPen = value;
                if (CrossPenChanged != null)
                    CrossPenChanged(this, EventArgs.Empty);
            }
        }

        Pen framePen = Pens.Black;
        public event EventHandler FramePenChanged;
        public Pen FramePen
        {
            get
            {
                return framePen;
            }
            set
            {
                framePen = value;
                if (FramePenChanged != null)
                    FramePenChanged(this, EventArgs.Empty);
            }
        }

        Brush coordBrush = Brushes.Black;
        public event EventHandler CoordBrushChanged;
        public Brush CoordBrush
        {
            get
            {
                return coordBrush;
            }
            set
            {
                coordBrush = value;
                if (CoordBrushChanged != null) CoordBrushChanged(this, EventArgs.Empty);
            }
        }

        StringFormat coordStringFormat,
            horizontalCoordStringFormat,
            verticalCoordStringFormat;
        public event EventHandler CoordStringFormatChanged;
        public StringFormat CoordStringFormat
        {
            get
            {
                return coordStringFormat;
            }
            set
            {
                coordStringFormat = value;
                horizontalCoordStringFormat = new StringFormat(value);
                verticalCoordStringFormat = new StringFormat(value) { FormatFlags = StringFormatFlags.DirectionVertical };
                if (CoordStringFormatChanged != null)
                    CoordStringFormatChanged(this, EventArgs.Empty);
            }
        }

        FxPlotController plotController;
        public event EventHandler PlotControllerChanged;
        public FxPlotController PlotController
        {
            get
            {
                return plotController;
            }
            set
            {
                plotController = value;
                if (PlotControllerChanged != null)
                    PlotControllerChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        #region INPUT_LOGIC

        #region MOUSE

        static PointD zoomRefPt = PointD.NaP;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (ShiftDown)
                        plotController.Control_ActivateSelectionMode(e.Location);
                    else
                        plotController.Plot_ActivateSelectionMode(e.Location);
                    break;
                case MouseButtons.Middle:
                    if (ShiftDown)
                        plotController.Control_ActivateZoomMode(zoomRefPt, e.Location);
                    else
                        plotController.Plot_ActivateZoomMode(zoomRefPt, e.Location);
                    break;
                case MouseButtons.Right:
                    if (ShiftDown)
                        plotController.Control_ActivateShiftMode(e.Location);
                    else
                        plotController.Plot_ActivateShiftMode(e.Location);
                    break;
                default:
                    zoomRefPt = e.Location;
                    break;
            }
        }

        PointD lastMouseCoord = PointD.NaP;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            lastMouseCoord = e.Location;
            PlotController.UpdateCoords(lastMouseCoord);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    plotController.DeactivateSelectionMode(true);
                    break;
                case MouseButtons.Middle:
                    plotController.DeactivateZoomMode();
                    break;
                case MouseButtons.Right:
                    plotController.DeactivateShiftMode();
                    break;
            }
        }

        const double ZOOM_IN = 0.94,
            ZOOM_OUT = 1.0 / ZOOM_IN;
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (ShiftDown)
                plotController.Control_CC_Zoom(new PointD(e.X, e.Y), e.Delta > 0 ? ZOOM_IN : ZOOM_OUT);
            else
                plotController.Plot_CC_Zoom(new PointD(e.X, e.Y), e.Delta > 0 ? ZOOM_IN : ZOOM_OUT);
        }

        #endregion

        #region KEYBOARD

        public bool ShiftDown
        {
            get
            {
                return (ModifierKeys & Keys.Shift) != 0;
            }
        }

        const double CONTROL_PAD = 5.0,
            PLOT_PAD = 0.05,
            CONTROL_SHIFT = 10.0,
            PLOT_SHIFT = 0.3;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.V:
                    if (ShiftDown)
                        PlotController.ControlArea = RectangleD.ScaleX(PlotController.ControlArea, lastMouseCoord.X, -1.0);
                    else
                        PlotController.PlotArea = RectangleD.ScaleX(PlotController.PlotArea, RectangleD.ConvertPoint(lastMouseCoord, PlotController.ControlArea, PlotController.PlotArea).X, -1.0);
                    break;

                case Keys.B:
                    if (ShiftDown)
                        PlotController.ControlArea = RectangleD.ScaleY(PlotController.ControlArea, lastMouseCoord.Y, -1.0);
                    else
                        PlotController.PlotArea = RectangleD.ScaleY(PlotController.PlotArea, RectangleD.ConvertPoint(lastMouseCoord, PlotController.ControlArea, PlotController.PlotArea).Y, -1.0);
                    break;

                case Keys.W:
                    if (ShiftDown)
                        PlotController.ControlArea = RectangleD.Pad(PlotController.ControlArea, 0.0, CONTROL_PAD, 0.0, 0.0);
                    else
                        PlotController.PlotArea = RectangleD.Pad(PlotController.PlotArea, 0.0, PLOT_PAD, 0.0, 0.0);
                    break;

                case Keys.A:
                    if (ShiftDown)
                        PlotController.ControlArea = RectangleD.Pad(PlotController.ControlArea, CONTROL_PAD, 0.0, 0.0, 0.0);
                    else
                        PlotController.PlotArea = RectangleD.Pad(PlotController.PlotArea, PLOT_PAD, 0.0, 0.0, 0.0);
                    break;

                case Keys.S:
                    if (ShiftDown)
                        PlotController.ControlArea = RectangleD.Pad(PlotController.ControlArea, 0.0, 0.0, 0.0, CONTROL_PAD);
                    else
                        PlotController.PlotArea = RectangleD.Pad(PlotController.PlotArea, 0.0, 0.0, 0.0, PLOT_PAD);
                    break;

                case Keys.D:
                    if (ShiftDown)
                        PlotController.ControlArea = RectangleD.Pad(PlotController.ControlArea, 0.0, 0.0, CONTROL_PAD, 0.0);
                    else
                        PlotController.PlotArea = RectangleD.Pad(PlotController.PlotArea, 0.0, 0.0, PLOT_PAD, 0.0);
                    break;

                case Keys.Left:
                    if (ShiftDown)
                        PlotController.Control_CC_Shift(PointD.Zero, new PointD(-CONTROL_SHIFT, 0));
                    else
                        PlotController.Plot_CC_Shift(PointD.Zero, new PointD(-PLOT_SHIFT, 0));
                    break;

                case Keys.Up:
                    if (ShiftDown)
                        PlotController.Control_CC_Shift(PointD.Zero, new PointD(0, CONTROL_SHIFT));
                    else
                        PlotController.Plot_CC_Shift(PointD.Zero, new PointD(0, PLOT_SHIFT));
                    break;

                case Keys.Right:
                    if (ShiftDown)
                        PlotController.Control_CC_Shift(PointD.Zero, new PointD(CONTROL_SHIFT, 0));
                    else
                        PlotController.Plot_CC_Shift(PointD.Zero, new PointD(PLOT_SHIFT, 0));
                    break;

                case Keys.Down:
                    if (ShiftDown)
                        PlotController.Control_CC_Shift(PointD.Zero, new PointD(0, -CONTROL_SHIFT));
                    else
                        PlotController.Plot_CC_Shift(PointD.Zero, new PointD(0, -PLOT_SHIFT));
                    break;

                case Keys.Escape:
                    plotController.DeactivateSelectionMode(false);
                    RefreshRender(this, EventArgs.Empty);
                    break;
            }
        }

        #endregion

        #endregion

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            BackgroundImage = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            if (plotController != null)
                plotController.ControlArea = RectangleD.Pad(DisplayRectangle, 30, 30, 30, 30);
        }

        void Invalidate(object sender, EventArgs e)
        {
            Invalidate();
        }

        public void RefreshRender(object sender, EventArgs e)
        {
            if (BackgroundImage == null)
                return;

            Pixelmap32 map = new Pixelmap32(true, (Bitmap)BackgroundImage);
            Graphics g = map.GetGraphics();

            Liner crossLiner = MakeLiner(g, CrossPen),
                gridLiner = MakeLiner(g, GridPen),
                frameLiner = MakeLiner(g, FramePen);
            Dotter fxDotter = (x, y) => map[ToInt(x), ToInt(y)] = graphColor;
            DoubleWriter horizontalTextWriter = MakeDoubleWriter(g, Font, CoordBrush, horizontalCoordStringFormat),
                verticalTextWriter = MakeDoubleWriter(g, Font, CoordBrush, verticalCoordStringFormat);

            g.Clear(BackColor);

            plotController.RefreshGrid(gridLiner);
            plotController.RefreshAxes(crossLiner);
            plotController.RefreshFrame(frameLiner);
            plotController.RefreshPlotBoundCoords(horizontalTextWriter, verticalTextWriter);

            map.Lock();
            plotController.RefreshFx(fxDotter);
            map.Unlock();

            Invalidate();
        }

        static Liner MakeLiner(Graphics g, Pen pen)
        {
            return (x1, y1, x2, y2) => g.DrawLine(pen, ToInt(x1), ToInt(y1), ToInt(x2), ToInt(y2));
        }

        static DoubleWriter MakeDoubleWriter(Graphics g, Font font, Brush brush, StringFormat stringFormat)
        {
            return (x, x1, y1, w, h) => g.DrawString(x.ToString(),
                                            font,
                                            brush,
                                            new RectangleF((float)x1, (float)y1, (float)w, (float)h),
                                            stringFormat);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (PlotController.SelectionMode != ModeTarget.None)
            {
                Liner selectionLiner = (x1, y1, x2, y2) => e.Graphics.DrawLine(framePen, ToInt(x1), ToInt(y1), ToInt(x2), ToInt(y2));
                plotController.RefreshSelection(selectionLiner);
            }
        }
    }
}