namespace GDPlotter
{
    public partial class BasePlotController
    {
        /// <summary>
        /// Shifts ControlArea by controlTargetPt - controlOriginPt
        /// </summary>
        /// <param name="controlOriginPt">Origin point of the shift, in control coordinates</param>
        /// <param name="controlTargetPt">Target point of the shift, in control coordinates</param>
        public void Control_CC_Shift(PointD controlOriginPt, PointD controlTargetPt)
        {
            ControlArea = RectangleD.Shift(ControlArea, controlTargetPt - controlOriginPt);
        }
        /// <summary>
        /// Shifts ControlArea by CC(plotTargetPt) - CC(plotOriginPt)
        /// </summary>
        /// <param name="plotOriginPt">Origin point of the shift, in plot coordinates</param>
        /// <param name="plotTargetPt">Target point of the shift, in plot coordinates</param>
        public void Control_PC_Shift(PointD plotOriginPt, PointD plotTargetPt)
        {
            ControlArea = RectangleD.Shift(ControlArea, RectangleD.ConvertPoint(plotTargetPt, PlotArea, ControlArea) - RectangleD.ConvertPoint(plotOriginPt, PlotArea, ControlArea));
        }
        /// <summary>
        /// Shifts PlotArea by PC(controlOriginPt) - PC(controlTargetPt)
        /// </summary>
        /// <param name="controlOriginPt">Origin point of the shift, in control coordinates</param>
        /// <param name="controlTargetPt">Target point of the shift, in control coordinates</param>
        public void Plot_CC_Shift(PointD controlOriginPt, PointD controlTargetPt)
        {
            PlotArea = RectangleD.Shift(PlotArea, RectangleD.ConvertPoint(controlOriginPt, ControlArea, PlotArea) - RectangleD.ConvertPoint(controlTargetPt, ControlArea, PlotArea));
        }
        /// <summary>
        /// Shifts PlotArea by plotOriginPt - plotTargetPt
        /// </summary>
        /// <param name="plotOriginPt">Origin point of the shift, in plot coordinates</param>
        /// <param name="plotTargetPt">Target point of the shift, in plot coordinates</param>
        public void Plot_PC_Shift(PointD plotOriginPt, PointD plotTargetPt)
        {
            PlotArea = RectangleD.Shift(PlotArea, plotOriginPt - plotTargetPt);
        }

        #region SHIFTMODE_PROP

        PointD shiftPt_origin = PointD.NaP;
        ModeTarget shiftMode = ModeTarget.None;
        public event ModeEventHandler ShiftModeChanged;
        /// <summary>
        /// Holds the shift flag of the current UI state
        /// </summary>
        public ModeTarget ShiftMode
        {
            get
            {
                return shiftMode;
            }
            private set
            {
                shiftMode = value;
                OnShiftModeChanged(new ModeEventArgs(value));
            }
        }
        protected virtual void OnShiftModeChanged(ModeEventArgs e)
        {
            if (ShiftModeChanged != null)
                ShiftModeChanged(this, e);
        }

        #endregion

        /// <summary>
        /// Activates shift mode, targeting the ControlArea rectangle
        /// </summary>
        /// <param name="controlPt">Initial point for shift calculations, in control coordinates</param>
        public void Control_ActivateShiftMode(PointD controlPt)
        {
            shiftPt_origin = controlPt;
            ShiftMode = ModeTarget.ControlArea;
        }

        /// <summary>
        /// Activates shift mode, targeting the PlotArea rectangle
        /// </summary>
        /// <param name="controlPt">Initial point for shift calculations, in control coordinates</param>
        public void Plot_ActivateShiftMode(PointD controlPt)
        {
            shiftPt_origin = RectangleD.ConvertPoint(controlPt, ControlArea, PlotArea);
            ShiftMode = ModeTarget.PlotArea;
        }

        /// <summary>
        /// Performs an iteration of the shift calculation based on the most recent coordinates
        /// </summary>
        /// <param name="controlPt">New point for shift calculations, in control coordinates</param>
        public void UpdateShiftModeCoord(PointD controlPt)
        {
            switch (ShiftMode)
            {
                case ModeTarget.None:
                    return;
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
        }

        /// <summary>
        /// Deactivates shift mode
        /// </summary>
        public void DeactivateShiftMode()
        {
            shiftPt_origin = PointD.NaP;
            ShiftMode = ModeTarget.None;
        }
    }
}