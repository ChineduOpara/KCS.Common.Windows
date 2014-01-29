using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KCS.Common.Controls
{
    /// <summary>
    /// Adds auto-dropdown functionality to the base ToolStripDropDownButton.
    /// </summary>
    public class KCSToolStripDropDownButton : ToolStripDropDownButton
    {
        /// <summary>
        /// When the mouse hovers over the control, immediately dropdown the items.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            this.ShowDropDown();
        }
    }
}
