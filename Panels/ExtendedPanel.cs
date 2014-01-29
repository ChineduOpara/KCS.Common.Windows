using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using KCS.Common.Shared;

namespace KCS.Common.Controls
{
    /// <summary>
    /// Special Panel that offers some special functionality.
    /// </summary>
    public class ExtendedPanel : System.Windows.Forms.Panel
    {
        private Color _defaultChildControlBackColor;
        private Color _defaultChildControlForeColor;

        [Description("Default Backcolor of controls placed on this Panel.")]
        [DefaultValue(KnownColor.Control)]
        [Category("Appearance")]
        public Color DefaultChildControlBackColor
        {
            get { return _defaultChildControlBackColor; }
            set
            {                
                if (value == Color.Empty)
                {
                    value = BackColor;
                }

                _defaultChildControlBackColor = value;
                //var controls = this.GetControls(false);
                foreach (Control c in Controls)
                {
                    c.BackColor = value;
                }
            }
        }

        [Description("Default Forecolor of controls placed on this Panel.")]
        [DefaultValue(KnownColor.ControlText)]
        [Category("Appearance")]
        public Color DefaultChildControlForeColor
        {
            get { return _defaultChildControlForeColor; }
            set
            {
                if (value == Color.Empty)
                {
                    value = ForeColor;
                }

                _defaultChildControlForeColor = value;
                //var controls = this.GetControls(false);
                foreach (Control c in Controls)
                {
                    c.ForeColor = value;
                }
            }
        }

        public ExtendedPanel()
        {
            //if (!this.IsDesignerHosted())
            {
                DefaultChildControlBackColor = BackColor; // Color.FromKnownColor(KnownColor.Control);
                DefaultChildControlForeColor = ForeColor; // Color.FromKnownColor(KnownColor.ControlText);
            }
        }

        /// <summary>
        /// When a control is added, set the Fore and Back colors.
        /// </summary>
        /// <param name="e">Control info.</param>
        protected override void OnControlAdded(System.Windows.Forms.ControlEventArgs e)
        {
            base.OnControlAdded(e);
            e.Control.BackColor = DefaultChildControlBackColor;
            e.Control.ForeColor = DefaultChildControlForeColor;
        }
    }
}
