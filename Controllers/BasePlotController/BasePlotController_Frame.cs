using System;

namespace GDPlotter
{
    public partial class BasePlotController
    {
        #region FRAME_RENDERFLAG_PROP

        bool renderFrame = RENDER_FRAME_DFLT;
        public event EventHandler RenderFrameChanged;
        /// <summary>
        /// Indicates whether a frame around ControlArea shall be rendered
        /// </summary>
        public bool RenderFrame
        {
            get
            {
                return renderFrame;
            }
            set
            {
                renderFrame = value;
                OnRenderFrameChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnRenderFrameChanged(EventArgs e)
        {
            if (RenderFrameChanged != null)
                RenderFrameChanged(this, e);
        }

        #endregion

        /// <summary>
        /// Draws a frame according to current controller parameters: ControlArea, RenderFrame
        /// </summary>
        /// <param name="frameLiner">Draws frame lines</param>
        public void RefreshFrame(Liner frameLiner)
        {
            if (CanRender && RenderFrame && frameLiner != null)
                Rendering.RenderRect(frameLiner, ControlArea);
        }
    }
}