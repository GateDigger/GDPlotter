using System;
using System.Windows.Forms;

namespace GDPlotter
{
    public partial class BasePlotController
    {
        #region CONTROLAREA_PROP

        RectangleD controlArea = RectangleD.Unit;
        public event EventHandler ControlAreaChanged;
        /// <summary>
        /// Section of the control display area to render upon
        /// </summary>
        public RectangleD ControlArea
        {
            get
            {
                return controlArea;
            }
            set
            {
                controlArea = value;
                OnControlAreaChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnControlAreaChanged(EventArgs e)
        {
            if (ControlAreaChanged != null)
                ControlAreaChanged(this, e);
        }

        #endregion

        #region PLOTAREA_PROP

        RectangleD plotArea = RectangleD.Unit;
        public event EventHandler PlotAreaChanged;
        /// <summary>
        /// Section of a Cartesian plane to render
        /// </summary>
        public RectangleD PlotArea
        {
            get
            {
                return plotArea;
            }
            set
            {
                plotArea = value;
                OnPlotAreaChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnPlotAreaChanged(EventArgs e)
        {
            if (PlotAreaChanged != null)
                PlotAreaChanged(this, e);
        }

        #endregion

        #region SUSPEND_RENDER

        const int CAN_RENDER = 0;
        int renderCounter = CAN_RENDER;
        /// <summary>
        /// Indicates whether anything shall be rendered
        /// </summary>
        public bool CanRender
        {
            get
            {
                return renderCounter == CAN_RENDER;
            }
        }
        /// <summary>
        /// Suspends all rendering until ResumeRender is called corresponding amount of times
        /// </summary>
        public void SuspendRender()
        {
            if (renderCounter != CAN_RENDER - 1)
                renderCounter++;
        }
        /// <summary>
        /// Undoes one SuspendRender() call
        /// </summary>
        public void ResumeRender()
        {
            if (renderCounter != CAN_RENDER)
                renderCounter--;
        }

        #endregion

        #region UPDATE_COORD

        /// <summary>
        /// Performs an iteration of all calculations based on the most recent coordinate; first coordinate for zoom is fixed
        /// </summary>
        /// <param name="controlPt">New point, in control coordinates</param>
        public void UpdateCoords(PointD controlPt)
        {
            switch (ZoomMode)
            {
                case ModeTarget.None:
                    break;

                case ModeTarget.ControlArea:
                    if (Control_CC_Zoom(zoomPt_origin1, zoomPt_origin2, zoomPt_origin1, controlPt))
                        zoomPt_origin2 = controlPt;
                    break;

                case ModeTarget.PlotArea:
                    if (Plot_PC_Zoom(zoomPt_origin1, zoomPt_origin2, zoomPt_origin1, RectangleD.ConvertPoint(controlPt, ControlArea, PlotArea)))
                        zoomPt_origin2 = RectangleD.ConvertPoint(controlPt, ControlArea, PlotArea);
                    break;
            }

            switch (ShiftMode)
            {
                case ModeTarget.None:
                    break;

                case ModeTarget.ControlArea:
                    Control_CC_Shift(shiftPt_origin, controlPt);
                    shiftPt_origin = controlPt;
                    break;

                case ModeTarget.PlotArea:
                default:
                    Plot_PC_Shift(shiftPt_origin, RectangleD.ConvertPoint(controlPt, ControlArea, PlotArea));
                    shiftPt_origin = RectangleD.ConvertPoint(controlPt, ControlArea, PlotArea);
                    break;
            }

            switch (SelectionMode)
            {
                case ModeTarget.None:
                    break;
                case ModeTarget.ControlArea:
                    selectPt_origin2 = controlPt;
                    CurrentSelection = RectangleD.MakeRectangle(selectPt_origin1, selectPt_origin2, ControlArea.X1 < ControlArea.X2, ControlArea.Y1 < ControlArea.Y2);
                    break;
                case ModeTarget.PlotArea:
                default:
                    selectPt_origin2 = RectangleD.ConvertPoint(controlPt, ControlArea, PlotArea);
                    CurrentSelection = RectangleD.MakeRectangle(selectPt_origin1, selectPt_origin2, PlotArea.X1 < PlotArea.X2, PlotArea.Y1 < PlotArea.Y2);
                    break;
            }
        }

        #endregion
    }
}