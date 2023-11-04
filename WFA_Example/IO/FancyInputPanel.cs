using System;
using System.Drawing;
using System.Windows.Forms;

namespace GDPlotter
{
    public class FancyInputPanel : Panel
    {
        Label inputHeader;
        RichTextBox inputBody;
        Label inputFooter;
        ColorDialogButton colorDialogButton;

        public FancyInputPanel()
        {
            SuspendLayout();
            Controls.AddRange(new Control[]
            {
                inputHeader = new Label()
                {
                    TextAlign = ContentAlignment.MiddleLeft,
                },
                inputBody = new RichTextBox()
                {
                    Multiline = true,
                    ScrollBars = RichTextBoxScrollBars.Vertical
                },
                inputFooter = new Label()
                {
                    TextAlign = ContentAlignment.MiddleLeft,
                },
                colorDialogButton = new ColorDialogButton()
                {
                    DialogButtons = MouseButtons.Right,
                    BackColor = Color.Black
                }
            });

            UpdateLayout_H();
            UpdateLayout_V();
            ResumeLayout(true);
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);

            SuspendLayout();
            UpdateLayout_H();
            UpdateLayout_V();
            ResumeLayout(true);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateLayout_H();
        }

        public string HeaderText
        {
            get
            {
                return inputHeader.Text;
            }
            set
            {
                inputHeader.Text = value;
            }
        }
        public string BodyText
        {
            get
            {
                return inputBody.Text;
            }
            set
            {
                inputBody.Text = value;
            }
        }

        public string FooterText
        {
            get
            {
                return inputFooter.Text;
            }
            set
            {
                inputFooter.Text = value;
            }
        }

        public event MouseEventHandler ButtonMouseDown
        {
            add
            {
                colorDialogButton.MouseDown += value;
            }
            remove
            {
                colorDialogButton.MouseDown -= value;
            }
        }

        public Color SelectedColor
        {
            get
            {
                return colorDialogButton.Color;
            }
        }

        public int HeaderHeight
        {
            get
            {
                return inputHeader.Height;
            }
            set
            {
                inputHeader.Height = value;
                SuspendLayout();
                UpdateLayout_V();
                ResumeLayout(true);
            }
        }

        public int BodyHeight
        {
            get
            {
                return inputBody.Height;
            }
            set
            {
                inputBody.Height = value;
                SuspendLayout();
                UpdateLayout_V();
                ResumeLayout(true);
            }
        }
        public int FooterHeight
        {
            get
            {
                return inputFooter.Height;
            }
            set
            {
                inputFooter.Height = value;
                SuspendLayout();
                UpdateLayout_V();
                ResumeLayout(true);
            }
        }

        void UpdateLayout_H()
        {
            int w = Width - Padding.Horizontal;
            inputHeader.Left = Padding.Left;
            inputHeader.Width = w;
            inputBody.Left = Padding.Left;
            inputBody.Width = w;
            inputFooter.Left = Padding.Left;
            colorDialogButton.Width = colorDialogButton.Height;
            inputFooter.Width = w - colorDialogButton.Width - 2 * Padding.Horizontal;
            colorDialogButton.Left = inputFooter.Right + Padding.Horizontal;
            Height = colorDialogButton.Bottom + Padding.Bottom;
        }

        void UpdateLayout_V()
        {
            inputHeader.Top = Padding.Top;
            inputBody.Top = inputHeader.Bottom + Padding.Vertical;
            inputFooter.Top = inputBody.Bottom + Padding.Vertical;
            colorDialogButton.Top = inputFooter.Top;
            colorDialogButton.Height = colorDialogButton.Width = inputFooter.Height;
            Height = colorDialogButton.Bottom + Padding.Bottom;
        }
    }
}