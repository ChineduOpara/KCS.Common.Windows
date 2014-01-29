using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Linq;
using System.Text;
using KCS.Common.Shared;

namespace KCS.Common.Controls
{
    /// <summary>
    /// Base class for specialized textbox controls. This class can control the OnTextChanged event.
    /// </summary>
    public class KCSTextBox : TextBox
    {
		/// <summary>
		/// Raised when text is being pasted into the control. If the Success property is true,
		/// that means that text was successfuly retrieved from the clipboard.
		/// </summary>
		public event EventHandler<SuccessEventArgs> TextPasting;

        /// <summary>
        /// If TRUE, this control raises its OnTextChanged event.
        /// </summary>
		[
			DefaultValue(true),
			Category("Behavior"),
			Description("If TRUE, this toolStripItem raises its OnTextChanged event.")
		]
        public bool RaiseTextChangedEvent { get; set; }

		///// <summary>
		///// If TRUE, this control discards the ENTER key.
		///// </summary>
		//[
		//    DefaultValue(true),
		//    Category("Behavior"),
		//    Description("If TRUE, this control discards the ENTER key.")
		//]
		//public bool IgnoreEnterKey { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public KCSTextBox()
        {
            RaiseTextChangedEvent = true;
			//IgnoreEnterKey = true;
        }

        /// <summary>
        /// Raised when an item is selected or deselected. This event is "bubbled up" only if RaiseTextChangedEvent is TRUE.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(EventArgs e)
        {
            if (RaiseTextChangedEvent)
            {
                base.OnTextChanged(e);
            }
        }

		///// <summary>
		///// Discard the Enter key, to avoid the OS-generated "ding" sound.
		///// </summary>
		///// <param name="e"></param>
		//protected override void OnKeyDown(KeyEventArgs e)
		//{
		//    if (e.KeyCode == Keys.Enter && IgnoreEnterKey)
		//    {
		//        e.SuppressKeyPress = true;
		//        e.Handled = true;
		//    }
		//    else
		//    {
		//        base.OnKeyDown(e);
		//    }
		//}

		/// <summary>
		/// Raises the TextPasting event.
		/// </summary>
		protected void OnTextPasting()
		{
			if (TextPasting != null)
			{
				string txt = string.Empty;
				bool success = true;
				SuccessEventArgs args;

				if (Clipboard.ContainsText(TextDataFormat.CommaSeparatedValue) || Clipboard.ContainsText(TextDataFormat.Text) || Clipboard.ContainsText(TextDataFormat.UnicodeText))
				{
					txt = Clipboard.GetText();
				}
				else
				{
					success = false;
				}
				args = new SuccessEventArgs(success, txt);
				
				TextPasting(this, args);
			}
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
			{
				return;
			}

			// Paste
			if (m.Msg == Shared.Win32API.Messages.Paste.ToNumber<int>())
			{
				OnTextPasting();
			}
		}

    }
}
