using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using KCS.Common.Shared;

namespace KCS.Common.Controls
{
	/// <summary>
	/// A panel that can be expanded and collapsed.
	/// </summary>
	[
		DefaultProperty("Direction")
	]
	public class CollapsiblePanel : Panel
	{
		#region Constants
		/// <summary>
		/// Default width of the expand/collapse button.
		/// </summary>
		public const int DefaultButtonWidth = 23;

		/// <summary>
		/// Default preferred width of the explanded panel.
		/// </summary>
		public const int DefaultPreferredExpandedWidth = 300;

		/// <summary>
		/// Default button text when the panel is collapsed.
		/// </summary>
		public const string DefaultExpandCommandText = "Open";

		/// <summary>
		/// Default button text when the panel is expanded.
		/// </summary>
		public const string DefaultCollapseCommandText = "Close";
		#endregion

		#region Members
		private NonFocusableButton _Btn = null;
		private Enumerations.ExpandDirection _ExpandDirection;
		private bool _ShowButton;
		private int _ButtonWidth = DefaultButtonWidth;
		private bool _IsExpanded = true;
		private int _PreferredExpandedWidth = DefaultPreferredExpandedWidth;
		private string _ExpandCommandText = DefaultExpandCommandText;
		private string _CollapseCommandText = DefaultCollapseCommandText;
		private Image _ExpandCommandImage;
		private Image _CollapseCommandImage;
		#endregion

		#region Events
		/// <summary>
		/// Raised just before the panel expands.
		/// </summary>
		public event EventHandler Expanding;

		/// <summary>
		/// Raised just before the panel collapses.
		/// </summary>
		public event EventHandler Collapsing;

		/// <summary>
		/// Raised after the panel expands.
		/// </summary>
		public event EventHandler Expanded;

		/// <summary>
		/// Raised after the panel collapses.
		/// </summary>
		public event EventHandler Collapsed;
		#endregion

		#region Properties
		/// <summary>
		/// Shows or hides the expand/collapse panel.
		/// </summary>
		[
			Browsable(true),
			DefaultValue(true)
		]
		public bool ShowButton
		{
			get
			{
				return _ShowButton;
			}
			set
			{
				_ShowButton = value;
				_Btn.Visible = value;
			}
		}

		/// <summary>
		/// The width of the control when it is collapsed. This is useful when it is docked and no longer
		/// controls its own dimensions.
		/// </summary>
		[
			Browsable(false)
		]
		public int CollapsedWidth { get; private set; }

		/// <summary>
		/// The width of the control when it is expanded. This is useful when it is docked and no longer
		/// controls its own dimensions.
		/// </summary>
		[
			Browsable(false)
		]
		public int ExpandedWidth { get; private set; }

		/// <summary>
		/// Gets or sets the preferred width (or height) of the expanded panel.
		/// </summary>
		[
			Description("Gets or sets the preferred width of the expanded panel."),
			Category("Appearance"),
			DefaultValue(DefaultPreferredExpandedWidth),
		]
		public int PreferredExpandedWidth
		{
			get
			{
				return _PreferredExpandedWidth;
			}
			set
			{
				if (value < DefaultButtonWidth * 2)
				{
					value = DefaultButtonWidth * 2;
				}
				_PreferredExpandedWidth = value;
				ResetSize();
			}
		}

		/// <summary>
		/// Gets or sets Expanded state of the Panel.
		/// </summary>
		[
			Description("Gets or sets Expanded state of the Panel."),
			Category("Appearance"),
			DefaultValue(true)
		]
		public bool IsExpanded
		{
			get
			{
				return _IsExpanded;
			}
			private set
			{
				_IsExpanded = value;

				CreateChildControls();
				ExpandedWidth = PreferredExpandedWidth;
				CollapsedWidth = DefaultButtonWidth + 0;
				switch (Direction)
				{
					case Enumerations.ExpandDirection.Right:
						Width = value ? ExpandedWidth : CollapsedWidth;
						break;
					case Enumerations.ExpandDirection.Left:
						Width = value ? ExpandedWidth : CollapsedWidth;
						break;
					case Enumerations.ExpandDirection.Top:
						Height = value ? ExpandedWidth : CollapsedWidth;
						break;
					case Enumerations.ExpandDirection.Bottom:
						Height = value ? ExpandedWidth : CollapsedWidth;
						break;
				}
				SetButtonText();
			}
		}

		/// <summary>
		/// The width of the expand/collapse button.
		/// </summary>
		[
			Description("The width of the expand/collapse button."),
			Category("Appearance"),
			DefaultValue(DefaultButtonWidth)
		]
		public int ButtonWidth
		{
			get
			{
				return _ButtonWidth;
			}
			set
			{				
				_ButtonWidth = value;
				CreateChildControls();

				switch (Direction)
				{
					case Enumerations.ExpandDirection.Right:
						_Btn.Width = DefaultButtonWidth;
						break;
					case Enumerations.ExpandDirection.Left:
						_Btn.Width = DefaultButtonWidth;
						break;
					case Enumerations.ExpandDirection.Top:
						_Btn.Height = DefaultButtonWidth;
						break;
					case Enumerations.ExpandDirection.Bottom:
						_Btn.Height = DefaultButtonWidth;
						break;
				}
			}
		}

		/// <summary>
		/// Direction of the panel's contents.
		/// </summary>
		[
			Description("Direction of the panel's contents."),
			Category("Appearance"),
			DefaultValue(Enumerations.ExpandDirection.Right)
		]
		public Enumerations.ExpandDirection Direction
		{
			get
			{
				return _ExpandDirection;
			}
			set
			{
				CreateChildControls();
				_ExpandDirection = value;

				switch (value)
				{
					case Enumerations.ExpandDirection.Right:
						base.Dock = DockStyle.Right;
						_Btn.Dock = DockStyle.Left;
						_Btn.TextAngle = 270;
						break;
					case Enumerations.ExpandDirection.Left:
						base.Dock = DockStyle.Left;
						_Btn.Dock = DockStyle.Right;
						_Btn.TextAngle = 90;
						break;
					case Enumerations.ExpandDirection.Top:
						base.Dock = DockStyle.Top;
						_Btn.Dock = DockStyle.Bottom;
						_Btn.TextAngle = 0;
						break;
					case Enumerations.ExpandDirection.Bottom:
						base.Dock = DockStyle.Bottom;
						_Btn.Dock = DockStyle.Top;
						_Btn.TextAngle = 0;
						break;
				}
				ResetSize();
			}
		}

		/// <summary>
		/// Text to display on the expand/collapse button when the Panel is collapsed.
		/// </summary>
		[
			Description("Text to display on the expand/collapse button when the Panel is collapsed."),
			Category("Appearance"),
			DefaultValue(DefaultExpandCommandText)
		]
		public string ExpandCommandText
		{
			get { return _ExpandCommandText; }
			set
			{
				_ExpandCommandText = value;
				_Btn.Text = IsExpanded ? CollapseCommandText : ExpandCommandText;
			}
		}

		/// <summary>
		/// Text to display on the expand/collapse button when the Panel is expanded.
		/// </summary>
		[
			Description("Text to display on the expand/collapse button when the Panel is expanded."),
			Category("Appearance"),
			DefaultValue(DefaultCollapseCommandText)
		]
		public string CollapseCommandText
		{
			get { return _CollapseCommandText; }
			set
			{
				_CollapseCommandText = value;
				_Btn.Text = IsExpanded ? CollapseCommandText : ExpandCommandText;
			}
		}

		/// <summary>
		/// Image to display on the expand/collapse button when the Panel is collapsed.
		/// </summary>
		[
			Description("Image to display on the expand/collapse button when the Panel is collapsed."),
			Category("Appearance")
		]
		public Image ExpandCommandImage
		{
			get { return _ExpandCommandImage; }
			set
			{
				_ExpandCommandImage = value;
				_Btn.Image = IsExpanded ? CollapseCommandImage : ExpandCommandImage;
			}
		}

		/// <summary>
		/// Image to display on the expand/collapse button when the Panel is expanded.
		/// </summary>
		[
			Description("Image to display on the expand/collapse button when the Panel is expanded."),
			Category("Appearance")
		]
		public Image CollapseCommandImage
		{
			get { return _CollapseCommandImage; }
			set
			{
				_CollapseCommandImage = value;
				_Btn.Image = IsExpanded ? CollapseCommandImage : ExpandCommandImage;
			}
		}
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public CollapsiblePanel() : base()
		{
			CreateChildControls();
			ButtonWidth = DefaultButtonWidth;
			Direction = Enumerations.ExpandDirection.Right;
			ShowButton = true;
			Expand();
		}

		/// <summary>
		/// Creates the child controls.
		/// </summary>
		private void CreateChildControls()
		{
			if (_Btn == null)
			{
				_Btn = new NonFocusableButton();
				_Btn.Text = CollapseCommandText;
                _Btn.BringToFront();              
				Controls.Add(_Btn);
				_Btn.Click += new EventHandler(_BtnClick);
			}			
		}

		/// <summary>
		/// Anytime a control is added, make sure the expand/collapse button is at the back.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnControlAdded(ControlEventArgs e)
		{
			base.OnControlAdded(e);
			//_Btn.SendToBack(); //GG 10/22/2008 Ensures that when IsExpanded=false Expand button is on top of all panel controls
		}

		/// <summary>
		/// Expands or collapses the panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _BtnClick(object sender, EventArgs e)
		{
			if (IsExpanded)
			{
				Collapse();
			}
			else
			{
				Expand();
			}
		}

		/// <summary>
		/// Expands the Panel.
		/// </summary>
		public void Expand()
		{
			if (!IsExpanded)
			{
				OnExpanding();
				IsExpanded = true;
				OnExpanded();				
			}
		}

		/// <summary>
		/// Collapses the Panel.
		/// </summary>
		public void Collapse()
		{
			if (IsExpanded)
			{
				OnCollapsing();
				IsExpanded = false;
				OnCollapsed();
			}
		}

		/// <summary>
		/// Displays the appropriate image or text in the button.
		/// </summary>
		private void SetButtonText()
		{
			_Btn.Text = IsExpanded ? CollapseCommandText : ExpandCommandText;
			_Btn.Image = IsExpanded ? CollapseCommandImage : ExpandCommandImage;
		}

		/// <summary>
		/// Immediately adjust the width or height, based on the direction.
		/// </summary>
		private void ResetSize()
		{
			if (IsExpanded)
			{
				switch (Direction)
				{
					case Enumerations.ExpandDirection.Right:
					case Enumerations.ExpandDirection.Left:
						Width = PreferredExpandedWidth;
						break;
					case Enumerations.ExpandDirection.Top:
					case Enumerations.ExpandDirection.Bottom:
						Height = PreferredExpandedWidth;
						break;
				}
			}
		}

		#region Event-raising methods
		/// <summary>
		/// Raises the Expanding event.
		/// </summary>
		protected virtual void OnExpanding()
		{
			if (Expanding != null)
			{
				Expanding(this, new EventArgs());
			}
		}

		/// <summary>
		/// Raises the Expanded event.
		/// </summary>
		protected virtual void OnExpanded()
		{
			if (Expanded != null)
			{
				Expanded(this, new EventArgs());
			}
		}

		/// <summary>
		/// Raises the Collapsing event.
		/// </summary>
		protected virtual void OnCollapsing()
		{
            this.Controls.SetChildIndex(_Btn, 0);//GG 12/30/2009 move the collapsible button to the top of the z-order
			if (Collapsing != null)
			{
				Collapsing(this, new EventArgs());
			}
		}

		/// <summary>
		/// Raises the Collapsed event.
		/// </summary>
		protected virtual void OnCollapsed()
		{
			if (Collapsed != null)
			{
				Collapsed(this, new EventArgs());
			}
		}
		#endregion
	}
}
