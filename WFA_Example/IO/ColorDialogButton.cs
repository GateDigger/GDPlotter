using System;
using System.Drawing;
using System.Windows.Forms;

namespace GDPlotter
{
    sealed class ColorDialogButton : Button
    {
        ColorDialog colorDialog;
        public ColorDialogButton()
        {
            colorDialog = new ColorDialog()
            {
                AllowFullOpen = true,
                FullOpen = true,
                AnyColor = true,
            };
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if ((DialogButtons & e.Button) != 0 && colorDialog.ShowDialog() == DialogResult.OK)
                BackColor = colorDialog.Color;
            base.OnMouseDown(e);
        }

        const MouseButtons DFLT_DIALOG_BTNS = MouseButtons.Right;
        MouseButtons dialogButtons = DFLT_DIALOG_BTNS;
        public event EventHandler DialogButtonsChanged;
        public MouseButtons DialogButtons
        {
            get
            {
                return dialogButtons;
            }
            set
            {
                dialogButtons = value;
                if (DialogButtonsChanged != null)
                    DialogButtonsChanged(this, EventArgs.Empty);
            }
        }

        public Color Color
        {
            get
            {
                return BackColor;
            }
        }
    }
}