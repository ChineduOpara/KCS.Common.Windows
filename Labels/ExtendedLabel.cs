using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace KCS.Common.Controls
{
    /// <summary>
    /// Custom Label that supports angled text.
    /// </summary>
	/// <remarks>Before using this control, you MUST set AutoSize to FALSE.</remarks>
    public class ExtendedLabel : Label
    {
        private string _Text;
        private short _TextAngle = 0;

        /// <summary>
        /// Gets or sets the angle at which the Text will be rendered.
        /// </summary>
        [
            Category("Appearance"),
            DefaultValue(0),
            Description("Gets or sets the angle at which the Text will be rendered.")
        ]
        public short TextAngle
        {
            get
            {
                return _TextAngle;
            }
            set
            {
                _TextAngle = value;
                Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the Text to be rendered on the button's face.
        /// </summary>
        [
            Category("Appearance"),
            DefaultValue(""),
            Description("Gets or sets the Text to be rendered on the label's face.")
        ]
        new public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
                Refresh();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExtendedLabel()
        {
			// TODO: This does not work. Programmer must manually set auto-size to FALSE in the host control.
			base.AutoSize = false;
        }

        /// <summary>
        /// Paints the text onto the control at the desired angle.
        /// </summary>
        /// <param name="e">Event argument.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            Point point = new Point(ClientSize.Width / 2, ClientSize.Height / 2);
            Brush brush = new SolidBrush(ForeColor);
            
            base.OnPaint(e);

            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.TranslateTransform(point.X, point.Y);

            try
            {
                e.Graphics.RotateTransform(TextAngle);
                e.Graphics.DrawString(Text, Font, brush, 0, 0, sf);
            }
            finally
            {
                if (sf != null)
                {
                    sf.Dispose();
                }
            }
        }
    }
}
