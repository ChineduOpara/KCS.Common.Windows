using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KCS.Common.Controls
{
    /// <summary>
    /// Represents a button that is not selectable (user cannot set focus to it).
    /// </summary>
    /// <remarks>This is for the "tall" buttons used for expand/collapse functions.</remarks>
    public class NonFocusableButton : KCSButton
    {
		/// <summary>
		/// Constructor.
		/// </summary>
        public NonFocusableButton()
        {
            SetStyle(ControlStyles.Selectable, false);
        }
    }
}
