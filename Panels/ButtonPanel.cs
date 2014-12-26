using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace KCS.Common.Controls
{
	/// <summary>
	/// Puts standard OK and Cancel buttons on a Bottom-docked Panel. Also exposes properties to set their text.
	/// </summary>
	[
		DefaultEvent("AcceptClicked")
	]
	public class ButtonPanel : Panel
	{
		public const string DefaultOKButtonText = "&OK";
		public const string DefaultCancelButtonText = "&Cancel";
		public new const int DefaultPadding = 5;

		#region Members
		private Button _btnOK;
        private Button _btnCancel;
		private bool _showSeparator;
		private bool _autoCloseForm;
		private bool _isShown = false;
		#endregion

		#region Properties
        /// <summary>
        /// If set, the OK button is always the form's Accept button.
        /// </summary>
        [Category("Layout"), Description("Gets or sets the width of the OK button.")]
        public int OKButtonWidth
        {
            get { return _btnOK.Width; }
            set { _btnOK.Width = value; }
        }

		/// <summary>
		/// If set, the OK button is always the form's Accept button.
		/// </summary>
		[Category("Behavior"), Description("If set, the OK button is always the form's AcceptButton."), DefaultValue(false)
		]
		public bool AutoAcceptButton
		{
			get;
			set;
		}

		/// <summary>
		/// If set, the Cancel button is always the form's Cancel button.
		/// </summary>
		[
            Category("Behavior"),
			Description("If set, the Cancel button is always the form's CancelButton."),
			DefaultValue(true)
		]
		public bool AutoCancelButton
		{
			get;
			set;
		}

		/// <summary>
		/// Enables or disables the OK button.
		/// </summary>
		[
            Category("Appearance"),
			Description("Enables or disables the OK button."),
			DefaultValue(true)
		]
		public bool EnableOKButton
		{
			get
			{
				return _btnOK.Enabled;
			}
			set
			{
				_btnOK.Enabled = value;
			}
		}

        /// <summary>
        /// Enables or disables the Close button.
        /// </summary>
        [
            Category("Appearance"),
            Description("Enables or disables the Close button."),
            DefaultValue(true)
        ]
        public bool EnableCancelButton
        {
            get
            {
                return _btnCancel.Enabled;
            }
            set
            {
                _btnCancel.Enabled = value;
            }
        }


		/// <summary>
		/// Shows or hides the OK button.
		/// </summary>
		[
            Category("Appearance"),
			Description("Shows or hides the OK button"),
			DefaultValue(true)
		]
		public bool ShowOKButton
		{
			get
			{
				return _btnOK.Visible;
			}
			set
			{
				_btnOK.Visible = value;
			}
		}

        /// <summary>
        /// Shows or hides the Cancel button.
        /// </summary>
        [
            Category("Appearance"),
            Description("Shows or hides the Cancel button"),
            DefaultValue(true)
        ]
        public bool ShowCancelButton
        {
            get
            {
                return _btnCancel.Visible;
            }
            set
            {
                _btnCancel.Visible = value;
            }
        }

		/// <summary>
		/// If TRUE, draw a horizontal line to separate these buttons from the rest of the form.
		/// </summary>
		[
            Category("Appearance"),
			Description("If TRUE, draw a horizontal line to separate these buttons from the rest of the form."),
			DefaultValue(false)
		]
		public bool ShowSeparator
		{
			get
			{
				return _showSeparator;
			}
			set
			{
				_showSeparator = value;
				Refresh();
			}
		}

		/// <summary>
		/// If TRUE, the parent form automatically closes when the buttons are clicked.
		/// </summary>
		[
			Description("If TRUE, the parent form automatically closes when the buttons are clicked."),
			Category("Behavior"),
			DefaultValue(true)
		]
		public bool AutoCloseForm
		{
			get
			{
				return _autoCloseForm;
			}
			set
			{
				_autoCloseForm = value;
				_btnOK.DialogResult = value ? DialogResult.OK : DialogResult.None;
				_btnCancel.DialogResult = value ? DialogResult.Cancel : DialogResult.None;
			}
		}

		/// <summary>
		/// Contains the DialogResult of the button that was clicked.
		/// </summary>
		[
			Browsable(false)
		]
		public DialogResult DialogResult
		{
			get;
			private set;
		}

		new public Size Size
		{
			get
			{
				return base.Size;
			}
			set
			{
			}
		}

		[
			Browsable(false)
		]
		public override Size MinimumSize
		{
			get
			{
				return base.MinimumSize;
			}
			set
			{
			}
		}

		[
			Browsable(false)
		]
		public override Size MaximumSize
		{
			get
			{
				return base.MaximumSize;
			}
			set
			{
			}
		}

		[
			Browsable(false)
		]
		new public int Height
		{
			get
			{
				return base.Height;
			}
			set
			{
			}
		}

		[
			DefaultValue(true),
			ReadOnly(true)
		]
		public override bool AutoSize
		{
			get
			{
				return base.AutoSize;
			}
			set
			{
				//base.AutoSize = value;
			}
		}

		[
			Browsable(false)
		]
		new public Padding Padding
		{
			get
			{
				return base.Padding;
			}
			set
			{
			}
		}

		[
			DefaultValue(AutoSizeMode.GrowAndShrink),
			ReadOnly(true)
		]
		new public AutoSizeMode AutoSizeMode
		{
			get
			{
				return base.AutoSizeMode;
			}
			set
			{
			}
		}

		[
			DefaultValue(DockStyle.Bottom),
			ReadOnly(true)
		]
		public override DockStyle Dock
		{
			get
			{
				return base.Dock;
			}
			set
			{
			}
		}

		[
			Description("Gets or sets the text of the OK button."),
			DefaultValue(DefaultOKButtonText),
			Category("Appearance")
		]
		public string OKButtonText
		{
			get
			{
				return _btnOK.Text;
			}
			set
			{
				_btnOK.Text = value;
				FormatOKButtonText();
			}
		}

		[
			Description("Gets or sets the text of the Cancel button."),
			DefaultValue(DefaultCancelButtonText),
			Category("Appearance")
		]
		public string CancelButtonText
		{
			get
			{
				return _btnCancel.Text;
			}
			set
			{
				_btnCancel.Text = value;
			}
		}

		[
            Category("Appearance"),
			Description("Gets or sets the bitmap that is displayed on the OK button.")
		]
		public Image OKButtonImage
		{
			get
			{
				return _btnOK.Image;
			}
			set
			{
				_btnOK.Image = value;
				FormatOKButtonText();
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Raised when the Accept button is clicked.
		/// </summary>
		public event EventHandler AcceptClicked;

		/// <summary>
		/// Raised when the Cancel button is clicked.
		/// </summary>
		public event EventHandler CancelClicked;

		/// <summary>
		/// Forwards the MouseDown event of the Accept button.
		/// </summary>
		public event MouseEventHandler AcceptMouseDown;

		/// <summary>
		/// Forwards the MouseDown event of the Cancel button.
		/// </summary>
		public event MouseEventHandler CancelMouseDown;

		/// <summary>
		/// Forwards the MouseUp event of the Accept button.
		/// </summary>
		public event MouseEventHandler AcceptMouseUp;

		/// <summary>
		/// Forwards the MouseUp event of the Cancel button.
		/// </summary>
		public event MouseEventHandler CancelMouseUp;
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ButtonPanel() : base()
		{
			base.Dock = DockStyle.Bottom;
			base.Padding = new Padding(DefaultPadding);
			base.AutoSize = true;
			base.AutoSizeMode = AutoSizeMode.GrowAndShrink;			

			CreateControls();
			AutoCancelButton = true;
			AutoCloseForm = true;

            base.Height = KCSButton.DefaultSize.Height + base.Padding.Top + base.Padding.Bottom;
			base.MinimumSize = new Size(0, base.Height);
			base.MaximumSize = new Size(0, base.Height);

			_btnOK.ImageAlign = ContentAlignment.MiddleLeft;
		}

		/// <summary>
		/// Formats the OK Button Text, based on whether the Image property has a value or not.
		/// </summary>
		private void FormatOKButtonText()
		{
			if (_btnOK.Image == null)
			{
				_btnOK.Text = _btnOK.Text.Trim();
				_btnOK.TextAlign = ContentAlignment.MiddleCenter;
			}
			else
			{
				_btnOK.Text = "  " + _btnOK.Text.Trim();
                //_btnOK.Text = _btnOK.Text.Trim();
				//_btnOK.TextAlign = ContentAlignment.MiddleRight;
                _btnOK.TextAlign = ContentAlignment.MiddleCenter;
			}
		}

		///// <summary>
		///// When the control is made visible, make the OK button the default button for the parent form,
		///// and make the Cancel button the... well, you know.
		///// </summary>
		///// <remarks>
		///// Unfortunately this does not seem to work properly because the buttons involved need to be
		///// direct children of the Form.
		///// </remarks>
		///// <param name="e"></param>
		//protected override void OnVisibleChanged(EventArgs e)
		//{
		//    Form form = TopLevelControl as Form;

		//    base.OnVisibleChanged(e);

		//    if (AutoCancelButton)
		//    {				
		//        form.AcceptButton = _BtnOK;				
		//    }
		//    if (AutoCancelButton)
		//    {
		//        form.CancelButton = _BtnCancel;
		//    }
		//}

		/// <summary>
		/// Creates the embedded controls.
		/// </summary>
		private void CreateControls()
		{
			if (_btnCancel == null)
			{
                _btnCancel = new Button();
				_btnCancel.TabIndex = 1;
				_btnCancel.Text = DefaultCancelButtonText;
				_btnCancel.Dock = DockStyle.Right;
				_btnCancel.Click += new EventHandler(_Btn_Click);
				_btnCancel.MouseDown += new MouseEventHandler(OnCancelMouseDown);
				_btnCancel.MouseUp += new MouseEventHandler(OnCancelMouseUp);

				Controls.Add(_btnCancel);
			}

			if (_btnOK == null)
			{
                _btnOK = new Button();
				_btnOK.TabIndex = 0;
				_btnOK.Text = DefaultOKButtonText;
				_btnOK.Dock = DockStyle.Right;
				_btnOK.Click += new EventHandler(_Btn_Click);
				_btnOK.MouseDown += new MouseEventHandler(OnAcceptMouseDown);
				_btnOK.MouseUp += new MouseEventHandler(OnAcceptMouseUp);

				Controls.Add(_btnOK);
				_btnOK.BringToFront();
			}
		}

		/// <summary>
		/// Raised when any of the buttons is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _Btn_Click(object sender, EventArgs e)
		{
            DialogResult = ((Button)sender).DialogResult;
			
			// Raise event for Accept button
			if (sender == _btnOK)
			{
				OnAcceptClicked();
			}

			// Raise event for Cancel button
			if (sender == _btnCancel)
			{				
				OnCancelClicked();
				
				// If AutoCloseForm is set to true, then always close the top-level form.
				// This is redundant for forms opened with ShowDialog(), but it is necessary for
				// non-modal forms.
				if (AutoCloseForm)
				{
                    var form = this.FindForm();
                    form.Close();
				}
			}			
		}

		/// <summary>
		/// Draw a horizontal line to separate the panel from the rest of the form.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			Pen pen;
			Point start, end;
			Form form;
			
			base.OnPaint(e);

			if (_showSeparator)
			{
				pen = new Pen(Color.FromKnownColor(KnownColor.ControlDark), 1.0f);
				//start = new Point(DefaultPadding, 0);
				//end = new Point(e.ClipRectangle.Width - DefaultPadding, 0);
				start = new Point(0, 0);
				end = new Point(e.ClipRectangle.Width, 0);
				e.Graphics.DrawLine(pen, start, end);

				pen = new Pen(Color.FromKnownColor(KnownColor.ControlLightLight), 1.0f);
				//start = new Point(DefaultPadding-1, 1);
				//end = new Point(e.ClipRectangle.Width - DefaultPadding+1, 1);
				start = new Point(0, 1);
				end = new Point(e.ClipRectangle.Width, 1);
				e.Graphics.DrawLine(pen, start, end);
			}			

			// Set the Accept or Cancel buttons
			if (!_isShown)
			{
				form = TopLevelControl as Form;
				_isShown = true;
				if (form != null)
				{
					if (AutoAcceptButton)
					{
						form.AcceptButton = _btnOK;
					}
					if (AutoCancelButton)
					{
						form.CancelButton = _btnCancel;
					}
				}
			}
		}

		#region Event-Raising Methods
		/// <summary>
		/// Raises the OKClicked event.
		/// </summary>
		protected virtual void OnAcceptClicked()
		{
			if (AcceptClicked != null)
			{
				AcceptClicked(this, new EventArgs());
			}
		}

		/// <summary>
		/// Raises the OKClicked event.
		/// </summary>
		protected virtual void OnCancelClicked()
		{
			if (CancelClicked != null)
			{
				CancelClicked(this, new EventArgs());
			}
		}

		/// <summary>
		/// Raises the AcceptMouseDown event.
		/// </summary>
		protected virtual void OnAcceptMouseDown(object sender, MouseEventArgs e)
		{
			if (AcceptMouseDown != null)
			{
				AcceptMouseDown(this, e);
			}
		}

		/// <summary>
		/// Raises the CancelMouseDown event.
		/// </summary>
		protected virtual void OnCancelMouseDown(object sender, MouseEventArgs e)
		{
			if (CancelMouseDown != null)
			{
				CancelMouseDown(this, e);
			}
		}

		/// <summary>
		/// Raises the AcceptMouseUp event.
		/// </summary>
		protected virtual void OnAcceptMouseUp(object sender, MouseEventArgs e)
		{
			if (AcceptMouseUp != null)
			{
				AcceptMouseUp(this, e);
			}
		}

		/// <summary>
		/// Raises the CancelMouseUp event.
		/// </summary>
		protected virtual void OnCancelMouseUp(object sender, MouseEventArgs e)
		{
			if (CancelMouseUp != null)
			{
				CancelMouseUp(this, e);
			}
		}
		#endregion
	}
}
