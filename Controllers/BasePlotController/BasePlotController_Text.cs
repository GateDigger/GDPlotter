using System;

namespace GDPlotter
{
    public partial class BasePlotController
    {
        #region PLOT_AREA_BOUND_COORDS_RENDERFLAG

        CoordsRenderMode renderPlotAreaBoundCoords = COORDS_RENDER_MODE_DFLT;
        public event EventHandler RenderPlotAreaBoundCoordsChanged;
        public CoordsRenderMode RenderPlotAreaBoundCoords
        {
            get
            {
                return renderPlotAreaBoundCoords;
            }
            set
            {
                renderPlotAreaBoundCoords = value;
                OnRenderPlotAreaBoundCoordsChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnRenderPlotAreaBoundCoordsChanged(EventArgs e)
        {
            if (RenderPlotAreaBoundCoordsChanged != null)
                RenderPlotAreaBoundCoordsChanged(this, e);
        }

        #endregion

        /// <summary>
        /// Draws plot area coordinates according to current controller parameters: ControlArea, PlotArea, RenderPlotAreaBoundCoords
        /// </summary>
        /// <param name="horizontalWriter">Writes numbers horizontally</param>
        /// <param name="verticalWriter">Writes numbers vertically</param>
        public void RefreshPlotBoundCoords(DoubleWriter horizontalWriter, DoubleWriter verticalWriter)
        {
            if (!CanRender)
                return;

            switch (RenderPlotAreaBoundCoords)
            {
                case CoordsRenderMode.None:
                    return;

                case CoordsRenderMode.Inside:
                    if (horizontalWriter != null && verticalWriter != null)
                        Rendering.RenderCoordText_Int(horizontalWriter, verticalWriter, COORDS_RENDER_TEXTBOX_HEIGHT, PlotArea, ControlArea);
                    return;

                case CoordsRenderMode.Outside:
                default:
                    if (horizontalWriter != null && verticalWriter != null)
                        Rendering.RenderCoordText_Ext(horizontalWriter, verticalWriter, COORDS_RENDER_TEXTBOX_HEIGHT, PlotArea, ControlArea);
                    return;
            }
        }
    }
}