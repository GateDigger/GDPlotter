namespace GDPlotter
{
    public partial class BasePlotController
    {
        /// <summary>
        /// Attempts to zoom in the ControlArea rectangle by scalar around controlFocusPt; does nothing if supplied a NaN value
        /// </summary>
        /// <param name="controlFocusPt">The focal zoom point, in control coordinates</param>
        /// <param name="scalar">The scale of the zoom operation</param>
        /// <returns>false if the operation overflows/underflows</returns>
        public bool Control_CC_Zoom(PointD controlFocusPt, double scalar)
        {
            RectangleD result = RectangleD.Zoom(ControlArea, controlFocusPt, scalar);
            if (RectangleD.CheckScale(result, CTRL_SCALE_LOW, CTRL_SCALE_HIGH))
            {
                ControlArea = result;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Attempts to zoom in the ControlArea rectangle around a weighted average of controlPt_new1 and controlPt_new2; checks for NaN values and ignores relevant axis if such value is found
        /// </summary>
        /// <param name="controlPt_origin1">First initial point, in control coordinates</param>
        /// <param name="controlPt_origin2">Second initial point, in control coordinates</param>
        /// <param name="controlPt_new1">First new point, in control coordinates</param>
        /// <param name="controlPt_new2">Second new point, in control coordinates</param>
        /// <returns>false if the operation overflows/underflows</returns>
        public bool Control_CC_Zoom(PointD controlPt_origin1, PointD controlPt_origin2, PointD controlPt_new1, PointD controlPt_new2)
        {
            double dx1 = controlPt_new1.X - controlPt_origin1.X,
                dx2 = controlPt_new2.X - controlPt_origin2.X,
                dy1 = controlPt_new1.Y - controlPt_origin1.Y,
                dy2 = controlPt_new2.Y - controlPt_origin2.Y;

            RectangleD result = RectangleD.ScaleXY(ControlArea,
                (dx2 * controlPt_new1.X + dx1 * controlPt_new2.X) / (dx1 + dx2),
                (controlPt_new2.X - controlPt_new1.X) / (controlPt_origin2.X - controlPt_origin1.X),
                (dy2 * controlPt_new1.Y + dy1 * controlPt_new2.Y) / (dy1 + dy2),
                (controlPt_new2.Y - controlPt_new1.Y) / (controlPt_origin2.Y - controlPt_origin1.Y));

            if (RectangleD.CheckScale(result, CTRL_SCALE_LOW, CTRL_SCALE_HIGH))
            {
                ControlArea = result;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Attempts to zoom in the ControlArea rectangle by scalar around plotFocusPt; does nothing if supplied a NaN value
        /// </summary>
        /// <param name="plotFocusPt">The focal zoom point, in plot coordinates</param>
        /// <param name="scalar">The scale of the zoom operation</param>
        /// <returns>false if the operation overflows/underflows</returns>
        public bool Control_PC_Zoom(PointD plotFocusPt, double scalar)
        {
            RectangleD result = RectangleD.Zoom(ControlArea, RectangleD.ConvertPoint(plotFocusPt, PlotArea, ControlArea), scalar);
            if (RectangleD.CheckScale(result, CTRL_SCALE_LOW, CTRL_SCALE_HIGH))
            {
                ControlArea = result;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Attempts to zoom in the PlotArea rectangle by scalar around controlFocusPt; does nothing if supplied a NaN value
        /// </summary>
        /// <param name="controlFocusPt">The focal zoom point, in control coordinates</param>
        /// <param name="scalar">The scale of the zoom operation</param>
        /// <returns>false if the operation overflows/underflows</returns>
        public bool Plot_CC_Zoom(PointD controlFocusPt, double scalar)
        {
            RectangleD result = RectangleD.Zoom(PlotArea, RectangleD.ConvertPoint(controlFocusPt, ControlArea, PlotArea), scalar);
            if (RectangleD.CheckScale(result, PLOT_SCALE_LOW, PLOT_SCALE_HIGH))
            {
                PlotArea = result;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Attempts to zoom in the PlotArea rectangle by scalar around plotFocusPt; does nothing if supplied a NaN value
        /// </summary>
        /// <param name="plotFocusPt">The focal zoom point, in plot coordinates</param>
        /// <param name="scalar">The scale of the zoom operation</param>
        /// <returns>false if the operation overflows/underflows</returns>
        public bool Plot_PC_Zoom(PointD plotFocusPt, double scalar)
        {
            RectangleD result = RectangleD.Zoom(PlotArea, plotFocusPt, scalar);
            if (RectangleD.CheckScale(result, PLOT_SCALE_LOW, PLOT_SCALE_HIGH))
            {
                PlotArea = result;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Attempts to zoom in the PlotArea rectangle around a weighted average of plotPt_new1 and plotPt_new2; checks for NaN values and ignores relevant axis if such value is found
        /// </summary>
        /// <param name="plotPt_origin1">First initial point, in plot coordinates</param>
        /// <param name="plotPt_origin2">Second initial point, in plot coordinates</param>
        /// <param name="plotPt_new1">First new point, in plot coordinates</param>
        /// <param name="plotPt_new2">Second new point, in plot coordinates</param>
        /// <returns>false if the operation overflows/underflows</returns>
        public bool Plot_PC_Zoom(PointD plotPt_origin1, PointD plotPt_origin2, PointD plotPt_new1, PointD plotPt_new2)
        {
            double dx1 = plotPt_new1.X - plotPt_origin1.X,
                dx2 = plotPt_new2.X - plotPt_origin2.X,
                dy1 = plotPt_new1.Y - plotPt_origin1.Y,
                dy2 = plotPt_new2.Y - plotPt_origin2.Y;

            RectangleD result = RectangleD.ScaleXY(PlotArea,
                (dx2 * plotPt_new1.X + dx1 * plotPt_new2.X) / (dx1 + dx2),
                (plotPt_origin2.X - plotPt_origin1.X) / (plotPt_new2.X - plotPt_new1.X),
                (dy2 * plotPt_new1.Y + dy1 * plotPt_new2.Y) / (dy1 + dy2),
                (plotPt_origin2.Y - plotPt_origin1.Y) / (plotPt_new2.Y - plotPt_new1.Y));

            if (RectangleD.CheckScale(result, PLOT_SCALE_LOW, PLOT_SCALE_HIGH))
            {
                PlotArea = result;
                return true;
            }
            return false;
        }

        #region ZOOMMODE_PROP

        PointD zoomPt_origin1 = PointD.NaP,
            zoomPt_origin2 = PointD.NaP;
        ModeTarget zoomMode = ModeTarget.None;
        public event ModeEventHandler ZoomModeChanged;
        /// <summary>
        /// Holds the zoom flag of the current UI state
        /// </summary>
        public ModeTarget ZoomMode
        {
            get
            {
                return zoomMode;
            }
            set
            {
                zoomMode = value;
                OnZoomModeChanged(new ModeEventArgs(value));
            }
        }
        protected virtual void OnZoomModeChanged(ModeEventArgs e)
        {
            if (ZoomModeChanged != null)
                ZoomModeChanged(this, e);
        }

        #endregion

        /// <summary>
        /// Activates zoom mode, targeting the ControlArea rectangle
        /// </summary>
        /// <param name="controlPt1">The first initial point, in control coordinates</param>
        /// <param name="controlPt2">The second initial point, in control coordinates</param>
        public void Control_ActivateZoomMode(PointD controlPt1, PointD controlPt2)
        {
            zoomPt_origin1 = controlPt1;
            zoomPt_origin2 = controlPt2;
            ZoomMode = ModeTarget.ControlArea;
        }

        /// <summary>
        /// Activates zoom mode, targeting the PlotArea rectangle
        /// </summary>
        /// <param name="controlPt1">The first initial point, in control coordinates</param>
        /// <param name="controlPt2">The second initial point, in control coordinates</param>
        public void Plot_ActivateZoomMode(PointD controlPt1, PointD controlPt2)
        {
            zoomPt_origin1 = RectangleD.ConvertPoint(controlPt1, ControlArea, PlotArea);
            zoomPt_origin2 = RectangleD.ConvertPoint(controlPt2, ControlArea, PlotArea);
            ZoomMode = ModeTarget.PlotArea;
        }

        /// <summary>
        /// Performs an iteration of the zoom calculation based on the most recent coordinates
        /// </summary>
        /// <param name="controlPt1">The first new point, in control coordinates</param>
        /// <param name="controlPt2">The second new point, in control coordinates</param>
        public void UpdateZoomModeCoord(PointD controlPt1, PointD controlPt2)
        {
            switch (ZoomMode)
            {
                case ModeTarget.None:
                    return;
                case ModeTarget.ControlArea:
                    if (Control_CC_Zoom(zoomPt_origin1, zoomPt_origin2, controlPt1, controlPt2))
                    {
                        zoomPt_origin1 = controlPt1;
                        zoomPt_origin2 = controlPt2;
                    }
                    break;
                case ModeTarget.PlotArea:
                    if (Plot_PC_Zoom(zoomPt_origin1, zoomPt_origin2, RectangleD.ConvertPoint(controlPt1, ControlArea, PlotArea), RectangleD.ConvertPoint(controlPt2, ControlArea, PlotArea)))
                    {
                        zoomPt_origin1 = RectangleD.ConvertPoint(controlPt1, ControlArea, PlotArea);
                        zoomPt_origin2 = RectangleD.ConvertPoint(controlPt2, ControlArea, PlotArea);
                    }
                    break;
            }
        }

        /// <summary>
        /// Deactivates zoom mode
        /// </summary>
        public void DeactivateZoomMode()
        {
            zoomPt_origin1 = zoomPt_origin2 = PointD.NaP;
            ZoomMode = ModeTarget.None;
        }
    }
}