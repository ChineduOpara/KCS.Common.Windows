using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace KCS.Common.Controls
{
    /// <summary>
    /// A CheckBox that can be placed on a ToolStrip.
    /// </summary>
    [
    ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip |
    ToolStripItemDesignerAvailability.StatusStrip),
    Description("Represents a CheckBox toolStripItem that can be placed in a ToolStrip."),
    DefaultEvent("CheckedChanged")
    ]
    public class CheckBoxToolStripItem : ToolStripControlHost
    {
        private FlowLayoutPanel _Panel;
        private CheckBox _CheckBox = new CheckBox();

        /// <summary>
        /// Raised when the Checked property changes.
        /// </summary>
        public event EventHandler CheckedChanged;

        #region Exposed Properties
        /// <summary>
        /// Gets or sets the CheckBox text.
        /// </summary>
        [
            Browsable(true),
            Category("Appearance")
        ]
        public override string Text
        {
            get
            {
                return CheckBox.Text;
            }
            set
            {
                CheckBox.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the Checked state.
        /// </summary>
        [
            Browsable(true),
            DefaultValue(false)
        ]
        public bool Checked
        {
            get
            {
                return CheckBox.Checked;
            }
            set
            {
                CheckBox.Checked = value;
            }
        }

        /// <summary>
        /// Contains the child CheckBox control.
        /// </summary>
        [Browsable(false)]
        public CheckBox CheckBox
        {
            get
            {
                return _CheckBox;
            }
        }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public CheckBoxToolStripItem() : base(new FlowLayoutPanel())
        {
            AutoSize = true;

            _CheckBox.AutoSize = true;
            _CheckBox.Dock = DockStyle.Fill;
            _CheckBox.CheckAlign = ContentAlignment.MiddleLeft;
            _CheckBox.TextAlign = ContentAlignment.MiddleCenter;
            _CheckBox.CheckedChanged += new EventHandler(CheckBoxCheckedChanged);

            // Set up flow layout panel
            _Panel = (FlowLayoutPanel)base.Control;
            _Panel.BackColor = Color.Transparent;            

            // Add the checkbox
            _Panel.Controls.Add(_CheckBox);
        }

        /// <summary>
        /// Raises the CheckedChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (CheckedChanged != null)
            {
                CheckedChanged(this, e);
            }
        }        
    }
}
