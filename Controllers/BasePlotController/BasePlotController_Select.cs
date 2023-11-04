using System;

namespace GDPlotter
{
    public partial class BasePlotController
    {
        /// <summary>
        /// Selects new ControlArea indicated by two points; Preserves orientation
        /// </summary>
        /// <param name="controlPt1">The first corner point of the new rectangle, in control coordinates</param>
        /// <param name="controlPt2">The second corner point of the new rectangle, in control coordinates</param>
        public void Control_CC_Select(PointD controlPt1, PointD controlPt2)
        {
            ControlArea = RectangleD.MakeRectangle(
                controlPt1,
                controlPt2,
                controlArea.X1 < controlArea.X2,
                controlArea.Y1 < controlArea.Y2
                );
        }

        /// <summary>
        /// Selects new ControlArea indicated by two points; Preserves orientation
        /// </summary>
        /// <param name="plotPt1">The first corner point of the new rectangle, in plot coordinates</param>
        /// <param name="plotPt2">The second corner point of the new rectangle, in plot coordinates</param>
        public void Control_PC_Select(PointD plotPt1, PointD plotPt2)
        {
            ControlArea = RectangleD.MakeRectangle(
                RectangleD.ConvertPoint(plotPt1, PlotArea, ControlArea),
                RectangleD.ConvertPoint(plotPt2, PlotArea, ControlArea),
                controlArea.X1 < controlArea.X2,
                controlArea.Y1 < controlArea.Y2
                );
        }

        /// <summary>
        /// Selects new PlotArea indicated by two points; Preserves orientation
        /// </summary>
        /// <param name="controlPt1">The first corner point of the new rectangle, in control coordinates</param>
        /// <param name="controlPt2">The second corner point of the new rectangle, in control coordinates</param>
        public void Plot_CC_Select(PointD controlPt1, PointD controlPt2)
        {
            PlotArea = RectangleD.MakeRectangle(
                RectangleD.ConvertPoint(controlPt1, ControlArea, PlotArea),
                RectangleD.ConvertPoint(controlPt2, ControlArea, PlotArea),
                PlotArea.X1 < plotArea.X2,
                PlotArea.Y1 < plotArea.Y2
                );
        }

        /// <summary>
        /// Selects new PlotArea indicated by two points; Preserves orientation
        /// </summary>
        /// <param name="plotPt1">The first corner point of the new rectangle, in plot coordinates</param>
        /// <param name="plotPt2">The second corner point of the new rectangle, in plot coordinates</param>
        public void Plot_PC_Select(PointD plotPt1, PointD plotPt2)
        {
            PlotArea = RectangleD.MakeRectangle(
                plotPt1,
                plotPt2,
                PlotArea.X1 < plotArea.X2,
                PlotArea.Y1 < plotArea.Y2
                );
        }

        #region SELECTIONMODE_PROP

        PointD selectPt_origin1,
            selectPt_origin2;
        ModeTarget selectionMode;
        public event ModeEventHandler SelectionModeChanged;
        /// <summary>
        /// Holds the selection flag of the current UI state
        /// </summary>
        public ModeTarget SelectionMode
        {
            get
            {
                return selectionMode;
            }
            private set
            {
                selectionMode = value;
                OnSelectionModeChanged(new ModeEventArgs(value));
            }
        }
        protected virtual void OnSelectionModeChanged(ModeEventArgs e)
        {
            if (SelectionModeChanged != null)
                SelectionModeChanged(this, e);
        }

        #endregion

        /// <summary>
        /// Activates selection mode, targeting the ControlArea rectangle
        /// </summary>
        /// <param name="controlPoint">An initial point to delimit the selection rectangle, in control coordinates</param>
        public void Control_ActivateSelectionMode(PointD controlPoint)
        {
            selectPt_origin2 = selectPt_origin1 = controlPoint;
            SelectionMode = ModeTarget.ControlArea;
        }

        /// <summary>
        /// Activates selection mode, targeting the PlotArea rectangle
        /// </summary>
        /// <param name="controlPoint">An initial point to delimit the selection rectangle, in control coordinates</param>
        public void Plot_ActivateSelectionMode(PointD controlPoint)
        {
            selectPt_origin2 = selectPt_origin1 = RectangleD.ConvertPoint(controlPoint, ControlArea, PlotArea);
            SelectionMode = ModeTarget.PlotArea;
        }

        /// <summary>
        /// Performs an iteration of the selection calculation based on the most recent rectangle delimiting point
        /// </summary>
        /// <param name="controlPoint">The new point, in control coordinates</param>
        public void UpdateSelectionModeCoord(PointD controlPoint)
        {
            switch (SelectionMode)
            {
                case ModeTarget.None:
                    return;
                case ModeTarget.ControlArea:
                    selectPt_origin2 = controlPoint;
                    CurrentSelection = RectangleD.MakeRectangle(selectPt_origin1, selectPt_origin2, ControlArea.X1 < ControlArea.X2, ControlArea.Y1 < ControlArea.Y2);
                    break;
                case ModeTarget.PlotArea:
                default:
                    selectPt_origin2 = RectangleD.ConvertPoint(controlPoint, ControlArea, PlotArea);
                    CurrentSelection = RectangleD.MakeRectangle(selectPt_origin1, selectPt_origin2, PlotArea.X1 < PlotArea.X2, PlotArea.Y1 < PlotArea.Y2);
                    break;
            }
        }

        /// <summary>
        /// Deactivates selection mode
        /// </summary>
        /// <param name="updateArea">The selection preview becomes the new area if true</param>
        public void DeactivateSelectionMode(bool updateArea)
        {
            if (!updateArea)
                goto CLEANUP;

            if (SelectionMode == ModeTarget.ControlArea &&
                selectPt_origin1.X != selectPt_origin2.X &&
                selectPt_origin1.Y != selectPt_origin2.Y)
                    Control_CC_Select(selectPt_origin1, selectPt_origin2);

            if (SelectionMode == ModeTarget.PlotArea &&
                selectPt_origin1.X != selectPt_origin2.X &&
                selectPt_origin1.Y != selectPt_origin2.Y)
                    Plot_PC_Select(selectPt_origin1, selectPt_origin2);

        CLEANUP:
            selectPt_origin1 = selectPt_origin2 = PointD.NaP;
            CurrentSelection = RectangleD.NaR;
            SelectionMode = ModeTarget.None;
        }

        #region RENDER_SELECTION_PROP

        bool renderSelection = RENDER_SELECTION_DFLT;
        public event EventHandler RenderSelectionChanged;
        /// <summary>
        /// Indicates whether a selection preview rectangle shall be rendered during active selection mode
        /// </summary>
        public bool RenderSelection
        {
            get
            {
                return renderSelection;
            }
            set
            {
                renderSelection = value;
                OnRenderSelectionChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnRenderSelectionChanged(EventArgs e)
        {
            if (RenderSelectionChanged != null)
                RenderSelectionChanged(this, e);
        }

        #endregion

        #region CURRENT_SELECTION

        RectangleD currentSelection = RectangleD.NaR;
        public event EventHandler CurrentSelectionChanged;
        /// <summary>
        /// A rectangle previewing area selection during active selection mode
        /// </summary>
        public RectangleD CurrentSelection
        {
            get
            {
                return currentSelection;
            }
            set
            {
                currentSelection = value;
                OnCurrentSelectionChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnCurrentSelectionChanged(EventArgs e)
        {
            if (CurrentSelectionChanged != null)
                CurrentSelectionChanged(this, e);
        }

        #endregion

        /// <summary>
        /// Draws a selection preview rectangle according to the current controller parameters: (ControlArea, PlotArea,) CurrentSelection, RenderSelection
        /// </summary>
        /// <param name="selectionLiner">Draws rectangle lines</param>
        public void RefreshSelection(Liner selectionLiner)
        {
            if (!CanRender || !RenderSelection)
                return;

            switch (SelectionMode)
            {
                case ModeTarget.None:
                    return;
                case ModeTarget.ControlArea:
                    Rendering.RenderRect(selectionLiner, CurrentSelection);
                    return;
                case ModeTarget.PlotArea:
                default:
                    Rendering.RenderRect(selectionLiner, CurrentSelection, ControlArea, PlotArea);
                    return;
            }
        }
    }
}