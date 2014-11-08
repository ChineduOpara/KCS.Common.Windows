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
        private CheckBox _checkBox = new CheckBox();

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
        /// Gets or sets the CheckBox FlatStyle.
        /// </summary>
        [
            Browsable(true),
            Category("Appearance")
        ]
        public FlatStyle FlatStyle
        {
            get
            {
                return CheckBox.FlatStyle;
            }
            set
            {
                CheckBox.FlatStyle = value;
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
                return _checkBox;
            }
        }
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public CheckBoxToolStripItem() : base(new FlowLayoutPanel())
        {
            AutoSize = true;

            _checkBox.AutoSize = true;
            _checkBox.FlatStyle = FlatStyle.Popup;
            _checkBox.Dock = DockStyle.Fill;
            _checkBox.CheckAlign = ContentAlignment.MiddleLeft;
            _checkBox.TextAlign = ContentAlignment.MiddleCenter;
            _checkBox.CheckedChanged += new EventHandler(CheckBoxCheckedChanged);

            // Set up flow layout panel
            _Panel = (FlowLayoutPanel)base.Control;
            _Panel.BackColor = Color.Transparent;            

            // Add the checkbox
            _Panel.Controls.Add(_checkBox);
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
