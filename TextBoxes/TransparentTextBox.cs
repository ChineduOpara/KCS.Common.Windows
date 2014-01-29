using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace KCS.Common.Controls
{
	/// <summary>
	/// A transparent TextBox.
	/// </summary>
	public class TransparentTextBox : TextBox
	{
		public TransparentTextBox()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			BackColor = Color.Transparent;
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			// Do nothing.
			//base.OnPaintBackground(pevent);
		}
	}
}
