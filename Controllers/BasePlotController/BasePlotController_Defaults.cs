namespace GDPlotter
{
    public partial class BasePlotController
    {
        const bool RENDER_AXES_DFLT = true;

        const double H_GRID_SPACING_DFLT = 1.0,
            V_GRID_SPACING_DFLT = 1.0;
        const bool RENDER_GRID_DFLT = true;

        const bool RENDER_FRAME_DFLT = true;

        const bool RENDER_SELECTION_DFLT = true;

        const CoordsRenderMode COORDS_RENDER_MODE_DFLT = CoordsRenderMode.Outside;
        const double COORDS_RENDER_TEXTBOX_HEIGHT = 20.0;

        //Bounds for zoom operations
        const double CTRL_SCALE_LOW = 20.0,
            CTRL_SCALE_HIGH = 16000.0,
            PLOT_SCALE_LOW = 0.0001,
            PLOT_SCALE_HIGH = 1000000;
    }
}