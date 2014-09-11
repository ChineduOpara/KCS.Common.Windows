using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using Shared = KCS.Common.Shared;

namespace KCS.Common.Controls
{
    /// <summary>
    /// Base class for all the specialized ListViews. Provides batch functions and special events. It also associates with
    /// an ExtendedListViewToolStrip, to allow for item operations.
    /// </summary>
    public class ExtendedListView : ListView
	{
		#region Members
		private int? _resizableColumnIndex = 0;
		private string _phantomText;
		#endregion

		//#region ToolStrip Properties
		///// <summary>
		///// Reference to the associated ToolStrip.
		///// </summary>
		//[
		//    Browsable(false)
		//]
		//public ExtendedToolStrip ToolStrip { get; private set; }

		///// <summary>
		///// Shows or hides the associated ToolStrip control.
		///// </summary>
		//[
		//    Description("Shows or hides the associated ToolStrip control."),
		//    DefaultValue(false),
		//    Category("Appearance")
		//]
		//public bool ShowToolStrip
		//{
		//    get
		//    {
		//        return ToolStrip.Visible;
		//    }
		//    set
		//    {
		//        ToolStrip.Visible = value;
		//    }
		//}

		///// <summary>
		///// Gets or sets the name of the object to Save.
		///// </summary>
		//[
		//    Description("Gets or sets the name of the object to Save."),
		//    Category("Data")
		//]
		//public string SaveObjectName { get; set; }

		///// <summary>
		///// Shows or hides the New command.
		///// </summary>
		//[
		//    Description("Shows or hides the New command."),
		//    DefaultValue(true),
		//    Category("Appearance")
		//]
		//public bool ShowNew { get; set; }

		///// <summary>
		///// Shows or hides the Refresh command.
		///// </summary>
		//[
		//    Description("Shows or hides the Refresh commands."),
		//    DefaultValue(false),
		//    Category("Appearance")
		//]
		//public bool ShowRefresh { get; set; }

		///// <summary>
		///// Shows or hides the Save command.
		///// </summary>
		//[
		//    Description("Shows or hides the Save commands."),
		//    DefaultValue(false),
		//    Category("Appearance")
		//]
		//public bool ShowSave { get; set; }

		///// <summary>
		///// Shows or hides the Save Object command.
		///// </summary>
		//[
		//    Description("Shows or hides the Save Object command."),
		//    DefaultValue(false),
		//    Category("Appearance")
		//]
		//public bool ShowSaveObject { get; set; }

		///// <summary>
		///// Enables or disables the Save Object command.
		///// </summary>
		//[
		//    Description("Enables or disables the Save Object command."),
		//    DefaultValue(false),
		//    Category("Appearance")
		//]
		//public bool EnableSaveObject { get; set; }

		///// <summary>
		///// Enables or disables the SaveAs command.
		///// </summary>
		//[
		//    Description("Enables or disables the Save As command."),
		//    DefaultValue(false),
		//    Category("Behavior")
		//]
		//public bool EnableSaveAs { get; set; }

		///// <summary>
		///// Shows or hides the Select commands.
		///// </summary>
		//[
		//    Description("Shows or hides the Select commands."),
		//    DefaultValue(true),
		//    Category("Appearance")
		//]
		//public bool ShowSelect { get; set; }

		///// <summary>
		///// Shows or hides the Delete command.
		///// </summary>
		//[
		//    Description("Shows or hides the Delete command."),
		//    DefaultValue(false),
		//    Category("Appearance")
		//]
		//public bool ShowDelete { get; set; }

        ///// <summary>
        ///// Enables or disables the Delete Selected command.
        ///// </summary>
        //[
        //    Description("Enables or disables the Delete Selected command."),
        //    DefaultValue(true),
        //    Category("Behavior")
        //]
        //public bool EnableDeleteSelected { get; set; }

        ///// <summary>
        ///// Enables or disables the DeleteAll command.
        ///// </summary>
        //[
        //    Description("Enables or disables the Delete All command."),
        //    DefaultValue(false),
        //    Category("Behavior")
        //]
        //public bool EnableDeleteAll { get; set; }
		//#endregion				

        #region Other Properties
        /// <summary>
        /// If set, the control displays some phantom text when there are no ListItems.
        /// </summary>
        [
            Description("If set, the toolStripItem displays some \"phantom\" text when there are no ListItems."),
            DefaultValue(true),
            Category("Appearance")
        ]
        public bool ShowPhantomText { get; set; }

        /// <summary>
        /// This is the phantom text that will be displayed when there are no ListItems.
        /// </summary>
        [
            Description("This is the \"phantom\" text that will be displayed when there are no ListItems."),
            DefaultValue(""),
            Category("Appearance")
        ]
        public string PhantomText
		{
			get
			{
				return _phantomText;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				_phantomText = value;
				//Refresh();
			}
		}

        /// <summary>
        /// This is the alternate phantom text that will be displayed when there are no ListItems.
        /// </summary>
        [
            Description("This is the alternate \"phantom\" text that will be displayed when there are no ListItems."),
            DefaultValue(""),
            Category("Appearance")
        ]
        public string AltPhantomText { get; set; }

        /// <summary>
        /// If set, the control bubbles up the SelectedIndexChanged event.
        /// </summary>
        [
            Description("If set, the toolStripItem bubbles up the SelectedIndexChanged event."),
            DefaultValue(true)
        ]
        public bool RaiseSelectedIndexChangedEvent { get; set; }

        /// <summary>
        /// If set, the control bubbles up the SelectedIndexChanged event ONLY when there's at least one item selected.
        /// </summary>
        [
            Description("If set, the toolStripItem bubbles up the SelectedIndexChanged event ONLY when there's at least one item selected."),
            DefaultValue(false)
        ]
        public bool RaiseSelectedIndexChangedEventOnSelection { get; set; }

        /// <summary>
        /// Contains the acceptable types that can be drag-and-dropped onto this ListView.
        /// </summary>
        [Browsable(false)]
        public List<Type> AcceptableDragDropTypes {get; private set;}        

        ///// <summary>
        ///// Gets or sets the flag indicating that this ListView's Items collection has changed.
        ///// </summary>
        //[Browsable(false)]
        //public bool IsListChanged { get; protected set; }

        /// <summary>
        /// Gets or sets the index of the column that will be automatically resized.
        /// </summary>
        [
        Description("Gets or sets the index of the column that will be automatically resized."),
        Category("Behavior"),
        DesignOnly(true)
        ]
        public int? FillColumnIndex
        {
            get
            {
                return _resizableColumnIndex;
            }
            set
            {
                if (value.HasValue)
                {
                    if (value < 0)
                    {
                        value = 0;
                    }
                }
                _resizableColumnIndex = value;
            }
        }

		/// <summary>
		/// If TRUE, the the DEL keystroke triggers Delete.
		/// </summary>
		[
			Description("If TRUE, the DEL keystroke triggers Delete."),
			DefaultValue(false),
			Category("Behavior")
		]
		public bool EnableDeleteSelectedByKey { get; set; }

		/// <summary>
		/// Contains the first selected item.
		/// </summary>
		public ListViewItem SelectedItem
		{
			get
			{
				return SelectedItems.Count == 0 ? null : SelectedItems[0];
			}
		}       
        #endregion         

		#region Events
		/// <summary>
		/// Raised when an item is clicked.
		/// </summary>
		public event EventHandler<Shared.ListViewItemClickEventArgs> ItemClick;

		/// <summary>
		/// Raised when an item is double-clicked.
		/// </summary>
		public event EventHandler<Shared.ListViewItemClickEventArgs> ItemDoubleClick;

		/// <summary>
		/// Raised when one or more selected items are being deleted.
		/// </summary>
		public event CancelEventHandler DeletingSelected;

		/// <summary>
		/// Raised after one or more selected items were deleted.
		/// </summary>
		public event EventHandler<Shared.SuccessEventArgs> DeletedSelected;

		/// <summary>
		/// Raised when all items are being deleted.
		/// </summary>
		public event EventHandler DeletingAll;

		/// <summary>
		/// Raised when all items were deleted.
		/// </summary>
		public event EventHandler<Shared.SuccessEventArgs> DeletedAll;

		///// <summary>
		///// Raised when the targed business entity is being Saved.
		///// </summary>
		//public event EventHandler SavingObject;

		///// <summary>
		///// Raised after the targed business entity was Saved.
		///// </summary>
		//public event EventHandler<Shared.SuccessEventArgs> SavedObject;

		///// <summary>
		///// Raised when the targed business entity is being Saved under a different name.
		///// </summary>
		//public event EventHandler SavingAs;

		///// <summary>
		///// Raised after the targed business entity was Saved under a different name.
		///// </summary>
		//public event EventHandler<Shared.SavedAsEventArgs> SavedAs;

		/// <summary>
		/// Raised when items are being drag-dropped onto this Control.
		/// </summary>
		public event EventHandler ItemsDragDropping;

		/// <summary>
		/// Raised after items were drag-dropped onto this Control.
		/// </summary>
		public event EventHandler<Shared.SuccessEventArgs> ItemsDragDropped;

		///// <summary>
		///// Raised after the control is cleared.
		///// </summary>
		///// <remarks>This is different from the DeletedAll event, because DeletedAll is "directly" user-initiated,
		///// while ItemCleared is indirectly called. Example, from inside an OnDragDrop event handler.</remarks>
		//public event EventHandler ItemsCleared;

		///// <summary>
		///// Raised when the data needs to be rebound to the grid.
		///// </summary>
		//public event EventHandler RefreshNeeded;

		///// <summary>
		///// Raised when the user clicks the New button in the toolstrip.
		///// </summary>
		//public EventHandler New { get; set; }

        ///// <summary>
        ///// Raised when the Items property has changed.
        ///// </summary>
        //[Browsable(false)]
        //public EventHandler ListChanged { get; set; }		
		#endregion

        #region Event-raising methods
		///// <summary>
		///// Raises the Items Cleared event.
		///// </summary>
		//public virtual void OnItemsCleared()
		//{
		//    if (ItemsCleared != null)
		//    {
		//        ItemsCleared(this, new EventArgs());
		//    }
		//}

        /// <summary>
        /// Raises the Items Drag Dropping event.
        /// </summary>
        public virtual void OnItemsDragDropping()
        {
            if (ItemsDragDropping != null)
            {
                ItemsDragDropping(this, new EventArgs());
            }
        }

        /// <summary>
        /// Raises the Items Drag Dropped event.
        /// </summary>
        /// <param name="success">True if the operation was successful.</param>
        public virtual void OnItemsDragDropped(bool success)
        {
            if (ItemsDragDropped != null)
            {
                ItemsDragDropped(this, new Shared.SuccessEventArgs(success));
            }
        }

        /// <summary>
        /// Raises the Deleting Selected event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnDeletingSelected(object sender, CancelEventArgs e)
        {
            if (DeletingSelected != null)
            {
                DeletingSelected(this, e);
            }
        }

        /// <summary>
        /// Raises the Deleted Selected event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnDeletedSelected(object sender, Shared.SuccessEventArgs e)
        {
            if (DeletedSelected != null)
            {
                DeletedSelected(this, e);
            }
        }

        /// <summary>
        /// Raises the Deleting All event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnDeletingAll(object sender, EventArgs e)
        {
            if (DeletingAll != null)
            {
                DeletingAll(this, e);
            }
        }

        /// <summary>
        /// Raises the Deleted All event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnDeletedAll(object sender, Shared.SuccessEventArgs e)
        {
            if (DeletedAll != null)
            {
                DeletedAll(this, e);
            }
        }

		///// <summary>
		///// Raises the Saving Object event.
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//protected void OnSavingObject(object sender, EventArgs e)
		//{
		//    if (SavingObject != null)
		//    {
		//        SavingObject(this, e);
		//    }
		//}

		///// <summary>
		///// Raises the Saved Object event.
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//protected void OnSavedObject(object sender, Shared.SuccessEventArgs e)
		//{
		//    if (SavedObject != null)
		//    {
		//        SavedObject(this, e);
		//    }
		//}

		///// <summary>
		///// Raises the Saving As event.
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//protected void OnSavingAs(object sender, EventArgs e)
		//{
		//    if (SavingAs != null)
		//    {
		//        SavingAs(this, e);
		//    }
		//}

		///// <summary>
		///// Raises the Saved As event.
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//protected void OnSavedAs(object sender, Shared.SavedAsEventArgs e)
		//{
		//    if (SavedAs != null)
		//    {
		//        SavedAs(this, e);
		//    }
		//}
        #endregion

        #region Other methods
        /// <summary>
        /// Paints the background, and paints the phantom text if necessary.
        /// </summary>
        /// <param name="m">Message being processed.</param>
        protected override void WndProc(ref Message m)
        {
            Font font;
            StringFormat sf = null;
            Point point;
            Graphics graphics = null;
			Brush brush;
            string textToDisplay = PhantomText;

            // If the alternate phantom text is available, use it instead.
            if (!string.IsNullOrEmpty(AltPhantomText))
            {
                textToDisplay = AltPhantomText;
            }

            try
            {
                base.WndProc(ref m);

                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    return;
                }

                point = new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2);

                // 0x000F = W_PAINT
                if (m.Msg == 0x000F && Items.Count == 0 && ShowPhantomText && !string.IsNullOrEmpty(textToDisplay))
                {
					brush = new SolidBrush(SystemColors.GrayText);
                    font = new Font(Font, FontStyle.Regular);
                    sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;

                    graphics = Graphics.FromHwnd(this.Handle);
                    graphics.DrawString(textToDisplay, font, brush, point, sf);
                }
            }
            catch// (/*System.Exception ex*/)
            {
				//throw ex;
            }
            finally
            {
                if (sf != null)
                {
                    sf.Dispose();
                }
                if (graphics != null)
                {
                    graphics.Dispose();
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExtendedListView()
        {
            ShowItemToolTips = true;
            HideSelection = false;
            AcceptableDragDropTypes = new List<Type>()
            {
                typeof(ListView.SelectedListViewItemCollection)
            };
            RaiseSelectedIndexChangedEvent = true;
            Application.Idle += new EventHandler(ApplicationIdle);

			//ToolStrip = new ExtendedToolStrip(this);
			//ShowNew = true;
			//ShowSelect = true;

			ShowPhantomText = true;
			_phantomText = "No imageData available.";
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Application.Idle -= new EventHandler(ApplicationIdle);
            base.OnHandleDestroyed(e);
        }

        /// <summary>
        /// Idle processing. Sets the properties of the embedded ToolStrip, if any.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ApplicationIdle(object sender, EventArgs e)
        {
			if (DesignMode)
			{
				return;
			}

			//ToolStrip.ShowNew = ShowNew;
			//ToolStrip.ShowRefresh = ShowRefresh;
			//ToolStrip.SaveObjectName = SaveObjectName;
			//ToolStrip.ShowSave = ShowSave;
			//ToolStrip.ShowSaveObject = ShowSaveObject;
			//ToolStrip.EnableSaveObject = EnableSaveObject && Items.Count > 0;
			//ToolStrip.EnableSaveAs = EnableSaveAs && Items.Count > 0;

			//ToolStrip.ShowSelect = ShowSelect;

			//ToolStrip.ShowDelete = ShowDelete;
			//ToolStrip.EnableDeleteSelected = EnableDeleteSelected;
			//ToolStrip.EnableDeleteAll = EnableDeleteAll;	
        }        

		///// <summary>
		///// This is raised when the control is created. It sets up the embedded ToolStrip control.
		///// </summary>
		//protected override void OnCreateControl()
		//{
		//    base.OnCreateControl();

		//    if (Parent != null && !Parent.Controls.Contains(ToolStrip))
		//    {
		//        Parent.Controls.Add(ToolStrip);
		//        ToolStrip.BringToFront();
		//        BringToFront();
		//    }

		//    ToolStrip.NewButtonClick += new EventHandler(ToolStripNewButtonClick);
		//    ToolStrip.RefreshButtonClick += new EventHandler(ToolStripRefreshButtonClick);
		//    ToolStrip.SaveObjectMenuItemClick += new EventHandler(ToolStripSaveObjectMenuItemClick);
		//    ToolStrip.SaveAsMenuItemClick += new EventHandler(ToolStripSaveAsMenuItemClick);
		//    ToolStrip.DeleteSelectedMenuItemClick += new EventHandler(ToolStripDeleteSelectedMenuItemClick);
		//    ToolStrip.DeleteAllMenuItemClick += new EventHandler(ToolStripDeleteAllMenuItemClick);
		//}

		/// <summary>
		/// Builds the ListView columns based on just their names.
		/// </summary>
		/// <param name="columns">Column names.</param>
		public virtual void InitializeColumns(params string[] columns)
		{
			Columns.Clear();
			foreach (string name in columns)
			{
				ColumnHeader ch = Columns.Add(name, name);
				ch.Width = 100;
			}
		}

		/// <summary>
		/// Builds the ListView columns, allowing user to specify the column sizes.
		/// </summary>
		/// <param name="columns">Column name/size pairs.</param>
		public virtual void InitializeColumns(params KeyValuePair<string, int>[] columns)
		{
			Columns.Clear();
			foreach (KeyValuePair<string, int> pair in columns)
			{
				ColumnHeader ch = Columns.Add(pair.Key, pair.Key);
				ch.Width = pair.Value;
			}
		}

        ///// <summary>
        ///// Resets the IsListChanged flag.
        ///// </summary>
        //public void ResetListChanged()
        //{
        //    IsListChanged = false;
        //}

        /// <summary>
        /// Selects all items.
        /// </summary>
        public virtual void SelectAll()
        {
            foreach (ListViewItem item in Items)
            {
                item.Selected = true;
            }
        }

        /// <summary>
        /// Deselects all items.
        /// </summary>
        public virtual void SelectNone()
        {
            foreach (ListViewItem item in Items)
            {
                item.Selected = false;
            }
        }

        /// <summary>
        /// Inverts the selection.
        /// </summary>
        public virtual void InvertSelection()
        {
            foreach (ListViewItem item in Items)
            {
                item.Selected = !item.Selected;
            }
        }

        /// <summary>
        /// Selects a single item, by key.
        /// </summary>
        public virtual void SelectSingle(string key)
        {
            ListViewItem[] items = Items.Find(key, false);

            SelectNone();
            if (items.Length > 0)
            {
                items[0].Selected = true;
                EnsureVisible(items[0].Index);
            }
        }

        /// <summary>
        /// Checks all items (only if the ListView's CheckBoxes property is TRUE, obviously).
        /// </summary>
        public virtual void CheckAll()
        {
            if (base.CheckBoxes)
            {
                foreach (ListViewItem item in Items)
                {
                    item.Checked = true;
                }
            }
        }

        /// <summary>
        /// Unchecks all items (only if the ListView's CheckBoxes property is TRUE, obviously)..
        /// </summary>
        public virtual void CheckNone()
        {
            if (base.CheckBoxes)
            {
                foreach (ListViewItem item in Items)
                {
                    item.Checked = false;
                }
            }
        }

        /// <summary>
        /// Inverts the Checks.
        /// </summary>
        public virtual void InvertChecked()
        {
            if (base.CheckBoxes)
            {
                foreach (ListViewItem item in Items)
                {
                    item.Checked = !item.Checked;
                }
            }
        }

        /// <summary>
        /// Called when an item is clicked. It guarantees that the ItemClick event is only bubbled up if an item was clicked.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Point point = PointToClient(System.Windows.Forms.Cursor.Position);
            ListViewHitTestInfo info = HitTest(point);

            if (info.Item != null)
            {
                if (ItemClick != null)
                {
                    ItemClick(this, new Shared.ListViewItemClickEventArgs(info.Item));
                }
            }
        }

        /// <summary>
        /// Called when an item is double-clicked. It guarantees that the ItemClick event is only bubbled up if an item was clicked.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            if (!IsDisposed)
            {
                Point point = PointToClient(System.Windows.Forms.Cursor.Position);
                ListViewHitTestInfo info = HitTest(point);

                if (info.Item != null)
                {
                    if (ItemDoubleClick != null)
                    {
                        ItemDoubleClick(this, new Shared.ListViewItemClickEventArgs(info.Item));
                    }
                }
            }
        }

        /// <summary>
        /// Called when an item is selected or deselected. It only bubbles up the SelectedIndexChanged event if
        /// RaiseSelectedIndexChangedEvent is TRUE. In addition, if RaiseSelectedIndexChangedEventOnSelection is TRUE,
        /// it only bubbles up if at least 1 item is selected.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (RaiseSelectedIndexChangedEvent)
            {
                if (!RaiseSelectedIndexChangedEventOnSelection || (RaiseSelectedIndexChangedEventOnSelection && this.SelectedIndices.Count > 0))
                {
                    base.OnSelectedIndexChanged(e);
                }
            }
        }        

        /// <summary>
        /// Adds an item to the ListView and sets the IsListChanged flag.
        /// </summary>
        /// <param name="listViewItem">ListViewItem to add.</param>
        public virtual void Add(ListViewItem listViewItem)
        {
            if (string.IsNullOrEmpty(listViewItem.ToolTipText))
            {
                listViewItem.ToolTipText = listViewItem.Text;
            }
            Items.Add(listViewItem);
            //IsListChanged = true;

            //// Raise event
            //if (ListChanged != null)
            //{
            //    ListChanged(this, new EventArgs());
            //}
        }

        /// <summary>
        /// Adds an item to the ListView and sets the IsListChanged flag.
        /// </summary>
        /// <param name="listViewItems">Array of ListViewItems to be added.</param>
        public virtual void AddRange(params ListViewItem[] listViewItems)
        {
            Items.AddRange(listViewItems);
            //IsListChanged = true;

            //// Raise event
            //if (ListChanged != null)
            //{
            //    ListChanged(this, new EventArgs());
            //}
        }

        /// <summary>
        /// Removes all items, with optional confirmation.
        /// </summary>
        /// <param name="requireConfirmation">If TRUE, the user is prompted to confirm the action.</param>
        public virtual void DeleteAll(bool requireConfirmation)
        {
            DialogResult dr = requireConfirmation ? MessageBox.Show(this, "Clear the list?", "Confirm Action",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) : DialogResult.OK;
            
            int itemCount = Items.Count;
            if (dr == DialogResult.OK)
            {
                OnDeletingAll(this, new EventArgs());
                Items.Clear();
                //if (itemCount > 0)
                //{
                //    IsListChanged = true;

                //    if (ListChanged != null)
                //    {
                //        ListChanged(this, new EventArgs());
                //    }
                //}
                OnDeletedAll(this, new Shared.SuccessEventArgs(true));
            }
            else
            {
                OnDeletedAll(this, new Shared.SuccessEventArgs(false));
            }
        }

        /// <summary>
        /// Removes selected items, with optional confirmation.
        /// </summary>
        /// <param name="requireConfirmation">If TRUE, the user is prompted to confirm the action.</param>
        public virtual void DeleteSelected(bool requireConfirmation)
        {
            int index = 0;
            ListViewItem lvi;
            DialogResult dr;
            bool success = false;

            // If there are no selected items, bail.
            if (SelectedItems.Count == 0)
            {
                return;
            }

            dr = requireConfirmation ? MessageBox.Show(this, "Remove the selected items from the list?", "Remove Items",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) : DialogResult.OK;

            if (dr == DialogResult.Cancel)
            {
                return;
            }

            CancelEventArgs cancelDelete = new CancelEventArgs(false);
            OnDeletingSelected(this, cancelDelete);
            
            try
            {
                if (!cancelDelete.Cancel)
                {
                    if (SelectedItems.Count == Items.Count)
                    {
                        Items.Clear();
                    }
                    else
                    {

                        while (index < Items.Count)
                        {
                            lvi = Items[index];
                            if (lvi.Selected)
                            {
                                Items.Remove(lvi);
                            }
                            else
                            {
                                index++;
                            }
                        };
                    }

                    //IsListChanged = true;
                    //if (ListChanged != null)
                    //{
                    //    ListChanged(this, new EventArgs());
                    //}
                    success = true;
                }
            }
            finally
            {
                OnDeletedSelected(this, new Shared.SuccessEventArgs(success));
            }
        }

        /// <summary>
        /// Begin drag-and-drop operations. It uses the custom DragDropObject, which includes a pointer to the source control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            base.OnItemDrag(e);
			DoDragDrop(new Shared.DragDropObject<SelectedListViewItemCollection>(this, SelectedItems), DragDropEffects.All);
        }

        /// <summary>
        /// Simulate dragging and dropping.
        /// </summary>
        /// <param name="e"></param>
        public void DoDragDrop(DragEventArgs e)
        {
            OnDragDrop(e);
        }

        /// <summary>
        /// Simulate dragging over.
        /// </summary>
        /// <param name="e"></param>
        public void DoDragOver(DragEventArgs e)
        {
            OnDragOver(e);
        }

        /// <summary>
        /// Ensures that the user cannot drag-and-drop an item from a control to SAME control. It also ensures that only
        /// the "acceptable" types can be dropped onto it. This includes FileDrops (so we can drag files in from Explorer).
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragOver(DragEventArgs e)
        {
			Shared.DragDropObject<SelectedListViewItemCollection> dragDropObject;

            base.OnDragOver(e);

            e.Effect = e.AllowedEffect;

            // We only want DragDropObject objects or filedrops.			
			if (!e.Data.GetDataPresent(DataFormats.FileDrop) && !e.Data.GetDataPresent(typeof(Shared.DragDropObject<SelectedListViewItemCollection>)))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

			// FileDrops are handled differently from DragDropObjects.
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				if (Control.ModifierKeys == Keys.Control)
				{
					e.Effect = DragDropEffects.Copy;
				}
				else
				{
					e.Effect = DragDropEffects.Move;
				}
			}
			else
			{
				dragDropObject = e.Data.GetData(typeof(Shared.DragDropObject<SelectedListViewItemCollection>)) as Shared.DragDropObject<SelectedListViewItemCollection>;

				// Cannot drag & drop to self
				if (dragDropObject.Source.Equals(this))
				{
					e.Effect = DragDropEffects.None;
					return;
				}

				// See what type of data we can accept. Holding CTRL means we want to ADD to the current selection, not replace it.
				if (dragDropObject.GetDataPresent(AcceptableDragDropTypes))
				{
					if (Control.ModifierKeys == Keys.Control)
					{
						e.Effect = DragDropEffects.Copy;
					}
					else
					{
						e.Effect = DragDropEffects.Move;
					}
				}
			}
        }        

        /// <summary>
        /// Called when the control is resized for any reason. If ResizableColumnIndex is set, resize the appropriate column.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            ColumnHeader columnToResize;
            int otherColumnsTotalWidth = 0;
            base.OnResize(e);

            if (View == View.Details &&
                FillColumnIndex.HasValue &&
                FillColumnIndex.Value < Columns.Count)
            {
                columnToResize = Columns[FillColumnIndex.Value];
                foreach (ColumnHeader ch in Columns)
                {
                    if (ch.Equals(columnToResize))
                    {
                        continue;
                    }
                    otherColumnsTotalWidth += ch.Width;
                }
                otherColumnsTotalWidth += SystemInformation.VerticalScrollBarWidth;
                if (Width < otherColumnsTotalWidth) Width = otherColumnsTotalWidth;
                columnToResize.Width = Width - otherColumnsTotalWidth;
            }
        }

        /// <summary>
        /// Handle the CTRL+A and DEL key sequences, for SELECT ALL and DELETE, respectively.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
			if ((e.KeyCode == Keys.Delete) && (EnableDeleteSelectedByKey))
			{
				DeleteSelected(true);
			}

			if ((e.KeyCode == Keys.A && e.Control))
            {
                SelectAll();
            }
            base.OnKeyDown(e);
        }
        #endregion

		//#region ToolStrip event-handler methods
		///// <summary>
		///// Indicates that a new item is needed.
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//protected virtual void ToolStripNewButtonClick(object sender, EventArgs e)
		//{
		//    if (New != null)
		//    {
		//        New(this, new EventArgs());
		//    }
		//}

		///// <summary>
		///// Reload and redisplay the data.
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//protected virtual void ToolStripRefreshButtonClick(object sender, EventArgs e)
		//{
		//    if (RefreshNeeded != null)
		//    {
		//        RefreshNeeded(this, new EventArgs());
		//    }
		//}

		///// <summary>
		///// Deletes the selected items only.
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//protected virtual void ToolStripDeleteSelectedMenuItemClick(object sender, EventArgs e)
		//{
		//    DeleteSelected(true);
		//}

		///// <summary>
		///// Deletes all items.
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//protected virtual void ToolStripDeleteAllMenuItemClick(object sender, EventArgs e)
		//{
		//    DeleteAll(true);
		//}

		///// <summary>
		///// Saves the current business entity. Since we don't know what business entity will be saved, this method must be overriden.
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//protected virtual void ToolStripSaveObjectMenuItemClick(object sender, EventArgs e)
		//{
		//    throw new NotImplementedException("Please override this method.");
		//}

		///// <summary>
		///// Save the current business entity under a different name. Since we don't know what business entity will be saved, this method must be overriden.
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//protected virtual void ToolStripSaveAsMenuItemClick(object sender, EventArgs e)
		//{
		//    throw new NotImplementedException("Please override this method.");
		//}
		//#endregion
    }
}
