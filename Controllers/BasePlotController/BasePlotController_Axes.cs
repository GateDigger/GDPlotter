using System;

namespace GDPlotter
{
    public partial class BasePlotController
    {
        #region AXES_RENDERFLAG_PROP

        bool renderAxes = RENDER_AXES_DFLT;
        public event EventHandler RenderAxesChanged;
        /// <summary>
        /// Indicates whether the x and y axes shall be rendered
        /// </summary>
        public bool RenderAxes
        {
            get
            {
                return renderAxes;
            }
            set
            {
                renderAxes = value;
                OnRenderAxesChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnRenderAxesChanged(EventArgs e)
        {
            if (RenderAxesChanged != null)
                RenderAxesChanged(this, e);
        }

        #endregion

        /// <summary>
        /// Draws x and y axes according to current controller parameters: ControlArea, PlotArea, RenderAxes
        /// </summary>
        /// <param name="axisLiner">Draws axis lines</param>
        public void RefreshAxes(Liner axisLiner)
        {
            if (CanRender && RenderAxes && axisLiner != null)
                Rendering.RenderAxes(axisLiner, ControlArea, PlotArea);
        }
    }
}