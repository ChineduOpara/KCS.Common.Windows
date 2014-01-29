using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KCS.Common.Controls
{
	/// <summary>
	/// Displays an instance of NEProgressBar, which indicates "indefinite" progress.
	/// Apparently this was designed to just relax the users.
	/// </summary>
    public partial class AsyncProgressBar : Form
    {
		/// <summary>
		/// Gets or sets the message displayed above the progress bar.
		/// </summary>
		[
			DefaultValue("Processing..."),
			Description("Gets or sets the message displayed above the progress bar")
		]
		public override string Text
		{
			get
			{
                return (lblHeading == null) ? base.Text : lblHeading.Text;
			}
			set
			{
				lblHeading.Text = value;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
        public AsyncProgressBar()
        {
            InitializeComponent();
        }
    }
}