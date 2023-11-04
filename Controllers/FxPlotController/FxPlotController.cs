#define MT

using System;

namespace GDPlotter
{
    public partial class FxPlotController : BasePlotController
    {
        #region FX_PROP

        Func<double, double> fx;
        public event EventHandler FxChanged;
        /// <summary>
        /// The function to plot
        /// </summary>
        public Func<double, double> Fx
        {
            get
            {
                return fx;
            }
            set
            {
                fx = value;
                OnFxChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnFxChanged(EventArgs e)
        {
            if (FxChanged != null)
                FxChanged(this, e);
        }

        #endregion

        #region PLOTQUALITY_PROP

        int plotQuality = PLOT_QUALITY_DFLT;
        public event EventHandler PlotQualityChanged;
        /// <summary>
        /// The number of sample points to plot
        /// </summary>
        public int PlotQuality
        {
            get
            {
                return plotQuality;
            }
            set
            {
                plotQuality = value;
                OnPlotQualityChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnPlotQualityChanged(EventArgs e)
        {
            if (PlotQualityChanged != null)
                PlotQualityChanged(this, e);
        }

        #endregion

        #region FX_RENDERFLAG_PROP

        bool renderFx = RENDER_FX_DFLT;
        public event EventHandler RenderFxChanged;
        /// <summary>
        /// Indicates whether Fx shall be rendered
        /// </summary>
        public bool RenderFx
        {
            get
            {
                return renderFx;
            }
            set
            {
                renderFx = value;
                OnRenderFxChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnRenderFxChanged(EventArgs e)
        {
            if (RenderFxChanged != null)
                RenderFxChanged(this, e);
        }

        #endregion

        /// <summary>
        /// Call this
        /// </summary>
        /// <param name="dotter">Draws dots to a desired UI element</param>
        public void RefreshFx(Dotter dotter)
        {
            if (CanRender && RenderFx && Fx != null)
#if MT
                Rendering.RenderFx_Parallel(Fx, dotter, ControlArea, PlotArea, PlotQuality);
#else
                Rendering.RenderFx(Fx, dotter, ControlArea, PlotArea, PlotQuality);
#endif
        }
    }
}