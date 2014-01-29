using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace KCS.Common.Controls
{
    /// <summary>
    /// Extends the ToolStripComboBox, allowing control of the SelectedIndexChanged event.
    /// </summary>
    [DefaultEvent("SelectedIndexChangedEvent")]
    public class KCSToolStripComboBox : ToolStripComboBox
    {
        /// <summary>
        /// If TRUE, the control raises the SelectedIndexChanged event.
        /// </summary>
        [
            Category("Behavior"),
            DefaultValue(true),
            Description("If TRUE, the toolStripItem raises the SelectedIndexChanged event.")
        ]
        public bool RaiseSelectedIndexChangedEvent { get; set; }

        [
            Category("Data"),
            Description("Gets or sets the property to display.")
        ]
        public string DisplayMember
        {
            get { return base.ComboBox.DisplayMember; }
            set { base.ComboBox.DisplayMember = value; }
        }

        [
            Category("Data"),
            Description("Gets or sets the property to use as the actual value.")
        ]
        public string ValueMember
        {
            get { return base.ComboBox.ValueMember; }
            set { base.ComboBox.ValueMember = value; }
        }

        /// <summary>
        /// Constructor. Sets some default property values.
        /// </summary>
        public KCSToolStripComboBox()
        {
            RaiseSelectedIndexChangedEvent = true;
        }

        /// <summary>
        /// Raised when the selection is changed. The event is only bubbled up if RaiseSelectedIndexChangedEvent is TRUE.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (RaiseSelectedIndexChangedEvent)
            {
                base.OnSelectedIndexChanged(e);
            }
        }
    }
}
