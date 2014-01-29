using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;

namespace LS.Common.Controls
{
	/// <summary>
	/// ToolStrip specifically associated with a ListView control. Provides common functions.
	/// </summary>
	public class ExtendedToolStrip : ToolStrip
	{
		#region Members
		/// <summary>
		/// Associated ListView.
		/// </summary>
		protected ExtendedListView ListView = null;

		/// <summary>
		/// Associated GridView.
		/// </summary>
		protected ExtendedDataGridView DataGridView = null;

		private ToolStripButton NewToolStripButton;

		private ToolStripButton RefreshToolStripButton;

		private ToolStripDropDownButton SaveToolStripDropDownButton;
		private ToolStripMenuItem SaveObjectToolStripMenuItem;
		private ToolStripMenuItem SaveAsToolStripMenuItem;

		private ToolStripDropDownButton SelectToolStripDropDownButton;
		/// <summary>
		/// Menu item for "Select All".
		/// </summary>
		protected ToolStripMenuItem SelectAllToolStripMenuItem;
		private ToolStripMenuItem SelectNoneToolStripMenuItem;
		private ToolStripMenuItem InvertSelectionToolStripMenuItem;

		private ToolStripDropDownButton DeleteToolStripDropDownButton;
		private ToolStripMenuItem DeleteSelectedToolStripMenuItem;
		private ToolStripMenuItem DeleteAllToolStripMenuItem;

		private ToolStripSeparator RefreshToolStripSeparator;
		private ToolStripSeparator SaveToolStripSeparator;
		private ToolStripSeparator SelectToolStripSeparator;
		#endregion

		#region Events
		/// <summary>
		/// Raised when the New item is clicked.
		/// </summary>
		internal event EventHandler NewButtonClick;

		/// <summary>
		/// Raised when the Refresh item is clicked.
		/// </summary>
		internal event EventHandler RefreshButtonClick;

		/// <summary>
		/// Raised when the Save Object item is clicked.
		/// </summary>
		internal event EventHandler SaveObjectMenuItemClick;

		/// <summary>
		/// Raised when the Save As item is clicked.
		/// </summary>
		internal event EventHandler SaveAsMenuItemClick;

		/// <summary>
		/// Raised when the Delete Selected item is clicked.
		/// </summary>
		internal event EventHandler DeleteSelectedMenuItemClick;

		/// <summary>
		/// Raised when the Delete All item is clicked.
		/// </summary>
		internal event EventHandler DeleteAllMenuItemClick;
		#endregion

		#region Command-related Properties
		/// <summary>
		/// Gets or sets the name of the object to Save.
		/// </summary>
		[
			Description("Gets or sets the name of the object to Save."),
			Category("Data")
		]
		public string SaveObjectName { get; set; }

		/// <summary>
		/// Shows or hides the New command.
		/// </summary>
		[
			Description("Shows or hides the New command."),
			DefaultValue(true),
			Category("Appearance")
		]
		public bool ShowNew { get; set; }

		/// <summary>
		/// Shows or hides the Refresh command.
		/// </summary>
		[
			Description("Shows or hides the Refresh commands."),
			DefaultValue(false),
			Category("Appearance")
		]
		public bool ShowRefresh { get; set; }

		/// <summary>
		/// Shows or hides the Save command.
		/// </summary>
		[
			Description("Shows or hides the Save commands."),
			DefaultValue(false),
			Category("Appearance")
		]
		public bool ShowSave { get; set; }		

		/// <summary>
		/// Shows or hides the Save Object command.
		/// </summary>
		[
			Description("Shows or hides the Save Object command."),
			DefaultValue(false),
			Category("Appearance")
		]
		public bool ShowSaveObject { get; set; }

		/// <summary>
		/// Enables or disables the Save Object command.
		/// </summary>
		[
			Description("Enables or disables the Save Object command."),
			DefaultValue(false),
			Category("Appearance")
		]
		public bool EnableSaveObject { get; set; }

		/// <summary>
		/// Enables or disables the SaveAs command.
		/// </summary>
		[
			Description("Enables or disables the Save As command."),
			DefaultValue(false),
			Category("Behavior")
		]
		public bool EnableSaveAs { get; set; }

		/// <summary>
		/// Shows or hides the Select commands.
		/// </summary>
		[
			Description("Shows or hides the Select commands."),
			DefaultValue(true),
			Category("Appearance")
		]
		public bool ShowSelect { get; set; }

		/// <summary>
		/// Shows or hides the Delete command.
		/// </summary>
		[
			Description("Shows or hides the Delete command."),
			DefaultValue(false),
			Category("Appearance")
		]
		public bool ShowDelete { get; set; }

		/// <summary>
		/// Enables or disables the Delete Selected command.
		/// </summary>
		[
			Description("Enables or disables the Delete Selected command."),
			DefaultValue(true),
			Category("Behavior")
		]
		public bool EnableDeleteSelected { get; set; }

		/// <summary>
		/// Enables or disables the DeleteAll command.
		/// </summary>
		[
			Description("Enables or disables the Delete All command."),
			DefaultValue(false),
			Category("Behavior")
		]
		public bool EnableDeleteAll { get; set; }
		#endregion

		#region Other properties
		#endregion

		/// <summary>
		/// Private contstructor.
		/// </summary>
		private ExtendedToolStrip() : base()
		{
			ShowNew = true;
			ShowSelect = true;
			EnableDeleteSelected = true;

			InitializeComponent();
			Application.Idle += new EventHandler(ApplicationIdle);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="listView">ListView with which this ToolStrip will be associated.</param>
		public ExtendedToolStrip(ExtendedListView listView) : this()
		{
			this.ListView = listView;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dataGridView">DataGridView with which this ToolStrip will be associated.</param>
		public ExtendedToolStrip(ExtendedDataGridView dataGridView) : this()
		{
			this.DataGridView = dataGridView;
		}

		/// <summary>
		/// Remove the idle event handler.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed(e);
			Application.Idle -= new EventHandler(ApplicationIdle);
		}

		/// <summary>
		/// Initialize components.
		/// </summary>
		protected virtual void InitializeComponent()
		{
			// New button
			this.NewToolStripButton = new ToolStripButton("&New...");

			// Refresh button
			this.RefreshToolStripButton = new ToolStripButton("&Refresh");

			// Save-related menu
			this.SaveToolStripDropDownButton = new ToolStripDropDownButton("Save");
			this.SaveObjectToolStripMenuItem = new ToolStripMenuItem("New");
			this.SaveAsToolStripMenuItem = new ToolStripMenuItem("&As...");

			// Selection-related menu
			this.SelectToolStripDropDownButton = new ToolStripDropDownButton("Select");
			this.SelectAllToolStripMenuItem = new ToolStripMenuItem("&All");
			this.SelectNoneToolStripMenuItem = new ToolStripMenuItem("&None");
			this.InvertSelectionToolStripMenuItem = new ToolStripMenuItem("&Invert");

			// Deletion-related menu
			this.DeleteToolStripDropDownButton = new ToolStripDropDownButton("Remove");
			this.DeleteSelectedToolStripMenuItem = new ToolStripMenuItem("&Selected");
			this.DeleteAllToolStripMenuItem = new ToolStripMenuItem("&All");

			// Separators
			this.RefreshToolStripSeparator = new ToolStripSeparator();
			this.SaveToolStripSeparator = new ToolStripSeparator();
			this.SelectToolStripSeparator = new ToolStripSeparator();

			this.SuspendLayout();

			// Populate the ToolStrip
			this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.NewToolStripButton,
                this.RefreshToolStripButton,
                this.RefreshToolStripSeparator,                
                this.SaveToolStripDropDownButton,
                this.SaveToolStripSeparator,
                this.SelectToolStripDropDownButton,
                this.SelectToolStripSeparator,
                this.DeleteToolStripDropDownButton
                }
			);		

			#region NewToolStripButton
			this.NewToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
			this.NewToolStripButton.Image = Properties.Resources.New;
			this.NewToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.NewToolStripButton.Name = "NewToolStripButton";
			this.NewToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.NewToolStripButton.Click += new EventHandler(NewToolStripButtonClick);
			#endregion

			#region RefreshToolStripButton
			this.RefreshToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.RefreshToolStripButton.Image = Properties.Resources.Refresh;
			this.RefreshToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.RefreshToolStripButton.Name = "RefreshToolStripButton";
			this.RefreshToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.RefreshToolStripButton.Click += new EventHandler(RefreshToolStripButtonClick);
			#endregion

			#region SaveToolStripDropDownButton
			SaveToolStripDropDownButton.DropDownItems.AddRange(new ToolStripItem[]
                {
                    SaveObjectToolStripMenuItem,
                    SaveAsToolStripMenuItem
                }
			);
			SaveObjectToolStripMenuItem.Image = Properties.Resources.Save;
			SaveObjectToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
			SaveObjectToolStripMenuItem.Click += new EventHandler(SaveObjectToolStripMenuItemClick);
			SaveAsToolStripMenuItem.Click += new EventHandler(SaveAsToolStripMenuItemClick);
			#endregion

			#region SelectToolStripDropDownButton
			SelectToolStripDropDownButton.DropDownItems.AddRange(new ToolStripItem[]
                {
                    SelectAllToolStripMenuItem,
                    SelectNoneToolStripMenuItem,
                    InvertSelectionToolStripMenuItem
                }
			);
			SelectAllToolStripMenuItem.Click += new EventHandler(SelectAllToolStripMenuItemClick);
			SelectNoneToolStripMenuItem.Click += new EventHandler(SelectNoneToolStripMenuItemClick);
			InvertSelectionToolStripMenuItem.Click += new EventHandler(InvertSelectionToolStripMenuItemClick);
			#endregion

			#region DeleteToolStripDropDownButton
			DeleteToolStripDropDownButton.DropDownItems.AddRange(new ToolStripItem[]
                {
                    DeleteSelectedToolStripMenuItem,
                    DeleteAllToolStripMenuItem
                }
			);
			DeleteSelectedToolStripMenuItem.Image = Properties.Resources.Delete;
			DeleteSelectedToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
			DeleteSelectedToolStripMenuItem.Click += new EventHandler(DeleteSelectedToolStripMenuItemClick);
			DeleteAllToolStripMenuItem.Click += new EventHandler(DeleteAllToolStripMenuItemClick);
			#endregion

			this.Name = "ToolStrip";
			this.GripStyle = ToolStripGripStyle.Hidden;

			this.ResumeLayout(false);
			this.PerformLayout();
		}

		/// <summary>
		/// Idle processing. Shows or hides the buttons/items depending on conditions.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void ApplicationIdle(object sender, EventArgs e)
		{
			if (Shared.Extensions.IsDesignerHosted(this))
			{
				return;
			}			

			int itemCount = 0;
			int selectedItemCount = 0;			

			if (ListView != null)
			{
				itemCount = this.ListView.Items.Count;
				selectedItemCount = this.ListView.SelectedIndices.Count;
			}
			if (DataGridView != null)
			{
				itemCount = this.DataGridView.Rows.Count;
				selectedItemCount = this.DataGridView.SelectedRows.Count;
			}

			// Show or hide the New button, taking into account the ReadOnly status of the associated DataGridView
			// (if any)
			NewToolStripButton.Visible = ShowNew;
			if (DataGridView != null)
			{
				NewToolStripButton.Visible = ShowNew && !DataGridView.ReadOnly;
			}

			// Show or hide the Refresh button
			RefreshToolStripButton.Visible = ShowRefresh;
			RefreshToolStripSeparator.Visible = RefreshToolStripButton.Visible || NewToolStripButton.Visible;

			// Show or hide the Save button
			SaveToolStripDropDownButton.Visible = ShowSave;
			if (ShowSave)
			{
				SaveToolStripDropDownButton.Enabled = itemCount > 0;
			}
			SaveToolStripSeparator.Visible = SaveToolStripDropDownButton.Visible;
			SaveObjectToolStripMenuItem.Visible = ShowSaveObject;
			if (ShowSaveObject)
			{
				SaveObjectToolStripMenuItem.Enabled = EnableSaveObject;
			}
			SaveAsToolStripMenuItem.Enabled = EnableSaveAs;
			SaveObjectToolStripMenuItem.Text = string.IsNullOrEmpty(SaveObjectName) ? "Current" : SaveObjectName;

			// Show or hide the Select button
			SelectToolStripDropDownButton.Visible = ShowSelect;
			if (ShowSelect)
			{
				SelectToolStripDropDownButton.Enabled = itemCount > 0;
			}
			SelectToolStripSeparator.Visible = SelectToolStripDropDownButton.Visible;

			// Show or hide the Delete button, taking into account the ReadOnly status of the associated DataGridView
			// (if any)
			DeleteToolStripDropDownButton.Visible = ShowDelete;
			if (DataGridView != null)
			{
				DeleteToolStripDropDownButton.Visible = ShowDelete && !DataGridView.ReadOnly;
			}
			if (ShowDelete)
			{
				DeleteToolStripDropDownButton.Enabled = itemCount > 0;
				if (DataGridView != null)
				{
					DeleteToolStripDropDownButton.Enabled = itemCount > 0 && !DataGridView.ReadOnly;
				}
			}
			DeleteSelectedToolStripMenuItem.Enabled = EnableDeleteSelected && selectedItemCount > 0;
			DeleteAllToolStripMenuItem.Enabled = EnableDeleteAll && itemCount > 0;
			if (DataGridView != null)
			{
				DeleteSelectedToolStripMenuItem.Enabled = EnableDeleteSelected && selectedItemCount > 0 && !DataGridView.ReadOnly;
				DeleteAllToolStripMenuItem.Enabled = EnableDeleteAll && itemCount > 0 && !DataGridView.ReadOnly;
			}
		}

		/// <summary>
		/// When mouse enters the control, set focus to it.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			Focus();
		}

		#region Command event handlers
		/// <summary>
		/// Selects all items.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void SelectAllToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (ListView != null)
			{
				ListView.SelectAll();
			}

			if (DataGridView != null)
			{
				this.DataGridView.SelectAll();
			}
		}

		/// <summary>
		/// Deselects all items.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SelectNoneToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (ListView != null)
			{
				ListView.SelectNone();
			}

			if (DataGridView != null)
			{
				this.DataGridView.SelectNone();
			}
		}

		/// <summary>
		/// Inverts the selection.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void InvertSelectionToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (ListView != null)
			{
				ListView.InvertSelection();
			}

			if (DataGridView != null)
			{
				this.DataGridView.InvertSelection();
			}
		}

		/// <summary>
		/// Raises the Delete Selected event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeleteSelectedToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (DeleteSelectedMenuItemClick != null)
			{
				DeleteSelectedMenuItemClick(this, e);
			}
		}

		/// <summary>
		/// Raises the Delete All event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeleteAllToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (DeleteAllMenuItemClick != null)
			{
				DeleteAllMenuItemClick(this, e);
			}
		}

		/// <summary>
		/// Raises the Save Object event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveObjectToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (SaveObjectMenuItemClick != null)
			{
				SaveObjectMenuItemClick(this, e);
			}
		}

		/// <summary>
		/// Raises the Save As event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
		{
			if (SaveAsMenuItemClick != null)
			{
				SaveAsMenuItemClick(this, e);
			}
		}

		/// <summary>
		/// Raises the New event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NewToolStripButtonClick(object sender, EventArgs e)
		{
			if (NewButtonClick != null)
			{
				NewButtonClick(this, e);
			}
		}

		/// <summary>
		/// Raises the Refresh event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RefreshToolStripButtonClick(object sender, EventArgs e)
		{
			if (RefreshButtonClick != null)
			{
				RefreshButtonClick(this, e);
			}
		}
		#endregion
	}
}
