using System;

namespace GDPlotter
{
    public partial class BasePlotController
    {
        #region GRID_HSPACING_PROP

        double horizontalGridSpacing = H_GRID_SPACING_DFLT;
        public event EventHandler HorizontalGridSpacingChanged;
        /// <summary>
        /// Distance between two adjacent horizontal grid lines, in plot coordinates
        /// </summary>
        public double HorizontalGridSpacing
        {
            get
            {
                return horizontalGridSpacing;
            }
            set
            {
                horizontalGridSpacing = value;
                OnHorizontalGridSpacingChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnHorizontalGridSpacingChanged(EventArgs e)
        {
            if (HorizontalGridSpacingChanged != null)
                HorizontalGridSpacingChanged(this, e);
        }

        #endregion
        
        #region GRID_VSPACING_PROP

        double verticalGridSpacing = V_GRID_SPACING_DFLT;
        public event EventHandler VerticalGridSpacingChanged;
        /// <summary>
        /// Distance between two adjacent vertical grid lines, in plot coordinates
        /// </summary>
        public double VerticalGridSpacing
        {
            get
            {
                return verticalGridSpacing;
            }
            set
            {
                verticalGridSpacing = value;
                OnVerticalGridSpacingChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnVerticalGridSpacingChanged(EventArgs e)
        {
            if (VerticalGridSpacingChanged != null)
                VerticalGridSpacingChanged(this, e);
        }

        #endregion

        #region GRID_RENDERFLAG_PROP

        bool renderGrid = RENDER_GRID_DFLT;
        public event EventHandler RenderGridChanged;
        /// <summary>
        /// Indicates whether a grid shall be rendered
        /// </summary>
        public bool RenderGrid
        {
            get
            {
                return renderGrid;
            }
            set
            {
                renderGrid = value;
                OnRenderGridChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnRenderGridChanged(EventArgs e)
        {
            if (RenderGridChanged != null)
                RenderGridChanged(this, e);
        }

        #endregion

        /// <summary>
        /// Draws a grid according to current controller parameters: ControlArea, PlotArea, HorizontalGridSpacing, VerticalGridSpacing, RenderGrid
        /// </summary>
        /// <param name="gridLiner">Draws grid lines</param>
        public void RefreshGrid(Liner gridLiner)
        {
            if (CanRender && RenderGrid && gridLiner != null)
                Rendering.RenderGrid(gridLiner, HorizontalGridSpacing, VerticalGridSpacing, ControlArea, PlotArea);
        }
    }
}