using System;

namespace GDPlotter
{
    public delegate void ModeEventHandler(object sender, ModeEventArgs e);
    public class ModeEventArgs : EventArgs
    {
        public ModeEventArgs(ModeTarget modeTarget)
        {
            ModeTarget = modeTarget;
        }
        public ModeTarget ModeTarget
        {
            get;
            set;
        }
    }
}