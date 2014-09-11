using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ComponentModel;
using System.Data;
using KCS.Common.Shared;
using Shared = KCS.Common.Shared;
using System.Windows.Forms;

namespace KCS.Common.Controls
{
	/// <summary>
	/// Base class for all the specialized DataGridViews. Provides batch functions and special events. It also associates with
	/// an AssociatedToolStrip, to allow for item operations.
	/// </summary>
	public class KCSDataGridView : DataGridView
	{
		#region Members
		private string _phantomText;

        private int _clickedRow = -1;
        private Rectangle _dragBounds = Rectangle.Empty;
        private MouseEventArgs _mouseDownArgs = null;
        protected bool ChildHandlesMouseClicks { get; set; }

        private Brush _headerCellBackColorBrush;        
        private Brush _cellForeColorBrush;

		#endregion

        public List<DataGridViewColumnSpec> ColumnSpecs { get; private set; }

        /// <summary>
        /// Brush with the fore color of the cell text.
        /// </summary>
        public Brush CellForeColorBrush
        {
            get
            {
                if (_cellForeColorBrush == null)
                {
                    _cellForeColorBrush = new SolidBrush(ColumnHeadersDefaultCellStyle.ForeColor);
                }
                return _cellForeColorBrush;
            }
            private set { _cellForeColorBrush = value; }
        }

        /// <summary>
        /// Brush with the background color of the header cells.
        /// </summary>
        public Brush HeaderCellBackColorBrush
        {
            get
            {
                if (_headerCellBackColorBrush == null)
                {
                    _headerCellBackColorBrush = new SolidBrush(ColumnHeadersDefaultCellStyle.BackColor);
                }
                return _headerCellBackColorBrush;
            }
            private set { _headerCellBackColorBrush = value; }
        }

		#region Other Properties
        /// <summary>
        /// Gets or sets the color that's used to indicate a changed or added row.
        /// </summary>
        [
            Description("Gets or sets the color that's used to indicate a changed or added row."), Category("Appearance")
        ]
        public Color ChangedRowHeaderBackColor { get; set; }

		[
			Browsable(false),
			DefaultValue(false)
		]
		new public bool EnableHeadersVisualStyles
		{
			get { return base.EnableHeadersVisualStyles; }
			set { }
		}

		/// <summary>
		/// If anything was copied, this table will contain it.
		/// </summary>
		[
			Browsable(false),
			DefaultValue(null)
		]
		public DataTable CopiedData
		{
			get;
			private set;
		}
		/// <summary>
		/// Indicates that the grid has uncommitted changes.
		/// </summary>
		public bool IsDirty
		{
			get;
			set;
		}

		/// <summary>
		/// Contains the DataSource, cast as a DataTable. Use this property only when the control has been
		/// bound to a DataTable.
		/// </summary>
		public DataTable DataTable
		{
			get
			{
				return DataSource as DataTable;
			}
		}

		/// <summary>
		/// Contains the DataSource, cast as a DataView. Use this property only when the control has been
		/// bound to a DataView.
		/// </summary>
		public DataView DataView
		{
			get
			{
				return DataSource as DataView;
			}
		}

		/// <summary>
		/// If set, rows can be dragged out of the control.
		/// </summary>
		[
			Description("If set, rows can be dragged out of the toolStripItem."), DefaultValue(false), Category("Behavior")
		]
		public bool AllowDrag { get; set; }

        [Category("Behavior"), DefaultValue(false),
         Description("Enable or disable drag selection of rows.  Disabling this will make dragging of unselected rows smoother.")]
        public bool SuppressDragSelection {get; set;}

		/// <summary>
		/// If set, the control displays some phantom text when there are no Rows.
		/// </summary>
		[
			Description("If set, the toolStripItem displays some \"phantom\" text when there are no Rows."),
			DefaultValue(true),
			Category("Appearance")
		]
		public bool ShowPhantomText { get; set; }

		/// <summary>
		/// This is the phantom text that will be displayed when there are no Rows.
		/// </summary>
		[
			Description("This is the \"phantom\" text that will be displayed when there are no Rows."),
			DefaultValue("No imageData available."),
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
				Refresh();
			}
		}

		/// <summary>
		/// This is the alternate phantom text that will be displayed when there are no ListItems.
		/// </summary>
		[
			Description("This is the alternate \"phantom\" text that will be displayed when there are no Rows."),
			DefaultValue(null),
			Category("Appearance")
		]
		public string AltPhantomText { get; set; }		

		/// <summary>
		/// Contains the acceptable types that can be drag-and-dropped onto this ListView.
		/// </summary>
		[Browsable(false)]
		public List<Type> AcceptableDragDropTypes { get; private set; }

		/// <summary>
		/// Returns true if any cell is selected, in any mode.
		/// </summary>
		[Browsable(false)]
		public bool IsAnyCellSelected
		{
			get
			{
				return SelectedCells.Count + SelectedRows.Count + SelectedColumns.Count > 0;
			}
		}
		#endregion		

		#region Events
        //[Category("Action"), Description("Occurs when the user begins dragging a row.")]
        //public event ItemDragEventHandler BeginRowDrag;

		/// <summary>
		/// Raised when a row is clicked.
		/// </summary>
		public event EventHandler<Shared.DataGridViewRowClickEventArgs> RowClick;

		/// <summary>
		/// Raised when a row is double-clicked.
		/// </summary>
		public event EventHandler<Shared.DataGridViewRowClickEventArgs> RowDoubleClick;

		/// <summary>
		/// Raised when items are being drag-dropped onto this Control.
		/// </summary>
		public event EventHandler ItemsDragDropping;

		/// <summary>
		/// Raised after items were drag-dropped onto this Control.
		/// </summary>
		public event EventHandler<Shared.SuccessEventArgs> ItemsDragDropped;

		/// <summary>
		/// Raised just before data is copied into the internal buffer.
		/// </summary>
		public event CancelEventHandler DataCopying;

		/// <summary>
		/// Raised just before data is pasted back into the cell.
		/// </summary>
		public event CancelEventHandler DataPasting;

        public delegate void DataPastedIntoCellDelegate(DataRowView dr, string columnName, object data);
        public event DataPastedIntoCellDelegate DataPastedIntoCell;

		/// <summary>
		/// Raised after data is pasted back into the cell.
		/// </summary>
		//public event EventHandler DataPasted;

        ///// <summary>
        ///// Raised when the Items property has changed.
        ///// </summary>
        //[Browsable(false)]
        //public EventHandler ListChanged { get; set; }		
		#endregion

		#region Event-raising methods
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
		/// Intelligently returns the currently-selected row.
		/// </summary>
		new public DataGridViewRow CurrentRow
		{
			get
			{
				if (base.CurrentRow != null)
				{
					return base.CurrentRow;
				}

				if (base.CurrentCell != null)
				{
					return Rows[base.CurrentCell.RowIndex];
				}

				return null;
			}
		}

        #endregion

        #region Other methods

        public List<DataGridViewColumnGroup> ColumnGroups{ get; private set;}

        /// <summary>
        /// Clean up.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (_headerCellBackColorBrush != null)
            {
                _headerCellBackColorBrush.Dispose();
                _headerCellBackColorBrush = null;
            }

            if (_cellForeColorBrush != null)
            {
                _cellForeColorBrush.Dispose();
                _cellForeColorBrush = null;
            }

            base.OnHandleDestroyed(e);
        }

        /// <summary>
        /// Gets all rows displayed in the grid's window.
        /// </summary>
        /// <returns>Rows.</returns>
        public IEnumerable<DataGridViewRow> GetVisibleRows()
        {
            var list = new List<DataGridViewRow>(1);
            if (FirstDisplayedCell == null)
            {
                yield return null;                
            }

            int rowIndex = FirstDisplayedCell.RowIndex;
            list.Add(FirstDisplayedCell.OwningRow);
            DataGridViewRow dgvr = Rows[rowIndex];

            while (rowIndex < Rows.Count && dgvr.Displayed)
            {
                dgvr = Rows[rowIndex++];
                if (dgvr.Displayed)
                {
                    yield return dgvr;
                    //list.Add(dgvr);
                }
            }

            //return list;
        }

        public List<DataGridViewRow> GetSelectedRows()
        {
            return GetSelectedRows(false);
        }

		/// <summary>
		/// Gets the physically selected rows, taking into consideration the DataGridView's SelectionMode.
		/// </summary>
		/// <returns>Selected rows.</returns>
        public List<DataGridViewRow> GetSelectedRows(bool skipUnboundColumns)
		{
			List<DataGridViewRow> list = new List<DataGridViewRow>(1);
			DataGridViewRow row;
			DataGridViewCell cell;

			if (RowCount == 0)
			{
				return list;
			}

			// TODO: This needs to take into consideration other selection modes.
			if (SelectionMode == DataGridViewSelectionMode.FullRowSelect)
			{
				DataGridViewRow[] array = new DataGridViewRow[SelectedRows.Count];
				SelectedRows.CopyTo(array, 0);
				list.AddRange(array);
			}

			if (SelectionMode == DataGridViewSelectionMode.CellSelect || SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
			{
				for (int i = 0; i < Rows.Count; i++)
				{
					row = Rows[i];
					for (int j = 0; j < row.Cells.Count; j++)
					{
						cell = row.Cells[j];
                        
                        if (skipUnboundColumns)
                        {
                            DataGridViewColumn col = Columns[cell.ColumnIndex];
                            if (cell.ValueType == null)
                            {
                                cell.Selected = false;
                            }
                        }

						if (cell.Selected)
						{
							list.Add(row);
							break;
						}
					}
				}
			}

			return list;
		}

		/// <summary>
		/// Gets all the selected cells, keyed by Row.
		/// </summary>
		/// <returns></returns>
		public Dictionary<int, List<DataGridViewCell>> GetSelectedCells()
		{
			Dictionary<int, List<DataGridViewCell>> dic = new Dictionary<int, List<DataGridViewCell>>(1);
			List<DataGridViewCell> list;
			DataGridViewRow row;
			DataGridViewCell cell;
			int key = 0;

			for (int i=0; i < Rows.Count; i++)
			{
				row = Rows[i];
				list = new List<DataGridViewCell>();				
				for (int j = 0; j < row.Cells.Count; j++)
				{
					cell = row.Cells[j];
					if (cell.Selected)
					{
						list.Add(cell);
					}
				}

				if (list.Count > 0)
				{
					dic.Add(key++, list);
				}
			}
			return dic;
		}

		/// <summary>
		/// Enables or disables sorting on the columns. When enabled, sorting
		/// is automatic. Call this method only AFTER columns have been added.
		/// </summary>
		/// <param name="enable">Enabled setting.</param>
		public void EnableColumnSorting(bool enable)
		{
			foreach (DataGridViewColumn column in Columns)
			{
				column.SortMode = enable ? DataGridViewColumnSortMode.Automatic : DataGridViewColumnSortMode.NotSortable;
			}
		}

		/// <summary>
		/// Gets the first column with the given DataPropertyName.
		/// </summary>
		/// <param name="dataPropertyName">DataPropertyName for which to search.</param>
		/// <returns>A single column.</returns>
		private DataGridViewColumn GetColumnByDataPropertyName(string dataPropertyName)
		{
			foreach (DataGridViewColumn column in Columns)
			{
				if (string.Compare(column.DataPropertyName, dataPropertyName, true) == 0)
				{
					return column;
				}
			}
			return null;
		}

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
			Brush brush = null;
			string textToDisplay = PhantomText;						

			try
			{
				base.WndProc(ref m);

				if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
				{
					return;
				}

				// If the alternate phantom text is available, use it instead.
				if (!string.IsNullOrEmpty(AltPhantomText))
				{
					textToDisplay = AltPhantomText;
				}

				point = new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2);

				// 0x000F = W_PAINT
				if (m.Msg == Shared.Win32API.Messages.Paint.ToNumber<int>() && Rows.Count == 0 && ShowPhantomText && !string.IsNullOrEmpty(textToDisplay))
				{
					graphics = Graphics.FromHwnd(this.Handle);

					//// First paint the background clean
					//if (this.Region != null)
					//{
					//    brush = new SolidBrush(BackgroundColor);
					//    graphics.FillRegion(brush, this.Region);
					//}

					brush = new SolidBrush(SystemColors.GrayText);
					font = new Font(Font, FontStyle.Regular);
					sf = new StringFormat();
					sf.LineAlignment = StringAlignment.Center;
					sf.Alignment = StringAlignment.Center;
					
					graphics.DrawString(textToDisplay, font, brush, point, sf);
				}

				// Copy
				if (m.Msg == Shared.Win32API.Messages.Copy.ToNumber<int>())
				{
					CopySelectedCells();
				}

				// Cut
				if (m.Msg == Shared.Win32API.Messages.Cut.ToNumber<int>())
				{
					CutSelectedCells();					
				}

				// Paste
				if (m.Msg == Shared.Win32API.Messages.Paste.ToNumber<int>())
				{
					PasteIntoSelectedCells();
				}
			}
			finally
			{
				if (sf != null)
				{
					sf.Dispose();
				}
				if (brush != null)
				{
					brush.Dispose();
				}
				if (graphics != null)
				{
					graphics.Dispose();
				}
			}
		}

        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

		/// <summary>
		/// "Clean" the background.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInvalidated(InvalidateEventArgs e)
		{
			Graphics graphics = null;
			Brush brush = null;			

			base.OnInvalidated(e);

			if (InvokeRequired)		// Control is not cross-thread friendly
				return;

			try
			{				
				brush = new SolidBrush(BackgroundColor);
				graphics = Graphics.FromHwnd(this.Handle);
				graphics.FillRegion(brush, new Region(e.InvalidRect));
			}
			finally
			{
				if (brush != null)
				{
					brush.Dispose();
				}
				if (graphics != null)
				{
					graphics.Dispose();
				}
			}
		}

		/// <summary>
		/// Copies the selected cells, into the clipboard and into the CopiedData property.
		/// </summary>
		public virtual void CopySelectedCells()
		{
			DataGridViewRow dgrv;
			DataGridViewCell cell;
			object cellValue = DBNull.Value;			
			List<DataGridViewRow> selectedRows;
			List<string> dataList = new List<string>(1);
			DataTable copiedData;

            // Deselect all cells belonging to columns that have no data type

			// Raise event
			if (DataCopying != null)
			{
				CancelEventArgs args = new CancelEventArgs();
				DataCopying(this, args);
				if (args.Cancel)
				{
					return;
				}
			}

			selectedRows = GetSelectedRows(true);
			if (selectedRows.Count == 0)
			{
				return;
			}

			// Generate the Copied Data table so that the types match those in the grid.
			copiedData = new DataTable("CopiedData");
			foreach (DataGridViewCell c in selectedRows.First().Cells)
			{
                if (c.Selected && Columns[c.ColumnIndex].ValueType != null)
				{
					copiedData.Columns.Add(Columns[c.ColumnIndex].DataPropertyName, Columns[c.ColumnIndex].ValueType);
				}
			}

			try
			{
				for (int i = 0; i < selectedRows.Count; i++)
				{
					dgrv = selectedRows[i];

					// Gather all the cells selected on a per-row basis
					for (int j = 0; j < dgrv.Cells.Count; j++)
					{
						cell = dgrv.Cells[j];
                        if (cell.Selected && Columns[cell.ColumnIndex].ValueType != null)
						{
							cellValue = cell.Value;                            
							if (Utility.IsEmpty2(cellValue))
							{
								if (Columns[j].ValueType.Equals(typeof(DateTime)))
								{
									cellValue = DateTime.MinValue;
								}
								else
								{
									if (Columns[j].ValueType.IsValueType)
									{
										cellValue = 0;
									}
									else
									{
										cellValue = DBNull.Value;
									}
								}
							}
							dataList.Add(Utility.GetStringValue(cellValue));
						}
					}

					copiedData.Rows.Add(dataList.ToArray());					
					dataList.Clear();
				}

				this.CopiedData = copiedData.Copy();

				// Convert the copied data table to XML, and save it to the clipboard
				//StringWriter sw = new StringWriter();
				//copiedData.WriteXml(sw, XmlWriteMode.WriteSchema);
				//sw.Flush();
				//string clipBoardText = sw.ToString();
				//Clipboard.SetText(clipBoardText);
				//Clipboard.SetDataObject(copiedData);
				//Clipboard.SetData(typeof(DataTable).FullName, copiedData);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Unable to copy cells: " + ex.Message);
				copiedData = null;
			}
		}

		/// <summary>
		/// Copies and clears the selected cells.
		/// </summary>
		public virtual void CutSelectedCells()
		{
			CopySelectedCells();

			foreach (DataGridViewCell cell in SelectedCells)
			{
				if (Columns[cell.ColumnIndex].ValueType.IsValueType)
				{
					cell.Value = 0;
				}
				else
				{
					cell.Value = DBNull.Value;
				}
			}
		}

		/// <summary>
		/// Pastes from the CopiedData table. This assumes that the DataSource is a DataTable.
		/// If it is not, then handle the DataPasting event, set Cancel to TRUE, and implement
		/// the pasting yourself.
		/// </summary>
		public virtual void PasteIntoSelectedCells()
		{
			List<DataGridViewRow> selectedRows;
			List<DataGridViewCell> selectedCells;
			DataRow dr, drSource;
			DataGridViewCell cell;
			DataGridViewColumn col;
			DataRowView drvTarget;
			DataGridViewRow dgrv;
			CancelEventArgs args = new CancelEventArgs();
			DataTable copiedData = CopiedData == null ? null : this.CopiedData;
			//string clipBoardText = string.Empty;

			// Retrieve the copied data table from the the clipboard. If it is not a valid table, then bail out
			//try
			//{
			//    if (DataTable != null && Clipboard.ContainsText(TextDataFormat.Text))
			//    {
			//        clipBoardText = Clipboard.GetText();
			//        DataObject dobj = (DataObject)Clipboard.GetDataObject();
			//        if (Utility.IsEmpty2(clipBoardText))
			//        {
			//            return;
			//        }
			//    }
			//    else
			//    {
			//        return;
			//    }
			//    StringReader sr = new StringReader(clipBoardText);
			//    copiedData = new DataTable();
			//    copiedData.ReadXml(sr);
			//}
			//catch
			//{
			//    return;
			//}

			if (DataTable == null || copiedData == null)
			{
				return;
			}

			if (DataPasting != null)
			{
				DataPasting(this, args);
			}

			// If the subscriber cancelled the built-in pasting, then abort.
			if (args.Cancel)
			{
				return;
			}

			selectedRows = GetSelectedRows();
			selectedCells = new List<DataGridViewCell>(1);

			// Gather the cells in the first row of the selected rows
			foreach (DataGridViewCell c in selectedRows[0].Cells)
			{
                if (c.Selected && Columns[c.ColumnIndex].ValueType != null)
				{
					selectedCells.Add(c);
				}
			}

			// First, make sure the number of columns match up
			if (selectedCells.Count != copiedData.Columns.Count)
			{
				MessageBox.Show(this, "Target column NUMBER mismatch!\r\nPlease select the exact number of columns as were copied.", "Paste Failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				return;
			}

			// Next make sure the TYPES match up
			for (int i = 0; i < selectedCells.Count; i++)
			{
				if (copiedData.Columns[i].DataType != selectedCells[i].ValueType)
				{
					MessageBox.Show(this, "Target column TYPE mismatch!\r\nPlease select the exact types of columns as were copied.", "Paste Failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					return;
				}
			}

			// Begin the paste operation. We are pasting always DOWN, so make sure there are enough rows in the grid. We do this on the fly
			bool allowUserToAddRows = AllowUserToAddRows;
			AllowUserToAddRows = false;			// Turn this off so we have no confusion

			int targetRowIndex = selectedRows[0].Index;
			if (targetRowIndex < 0)
			{
				targetRowIndex = DataTable.Rows.Count == 0 ? 0 : DataTable.Rows.Count - 1;
			}

			// We are only copying ONE row, so fill in as many as needed
			drSource = CopiedData.Rows[0];
			for (int i = 0; i < selectedRows.Count; i++)
			{
				if (targetRowIndex >= DataTable.Rows.Count)
				{
					dr = DataTable.NewRow();
					DataTable.Rows.Add(dr);
					targetRowIndex = DataTable.Rows.Count - 1;
				}
				dgrv = Rows[targetRowIndex];
				drvTarget = (DataRowView)dgrv.DataBoundItem;

				// Copy each column/cell
				for (int j = 0; j < copiedData.Columns.Count; j++)
				{
					col = GetColumnByDataPropertyName(copiedData.Columns[j].ColumnName);
					cell = dgrv.Cells[j];

					if (!col.ReadOnly /*&& !cell.ReadOnly*/)
					{   
						drvTarget[col.DataPropertyName] = drSource[col.DataPropertyName];

                        if (DataPastedIntoCell != null)
                        {
                            DataPastedIntoCell(drvTarget, col.DataPropertyName, drSource[col.DataPropertyName]);
                        }
					}
				}
				targetRowIndex++;
			}

			AllowUserToAddRows = allowUserToAddRows;
		}

        /// <summary>
        /// Constructor.
        /// </summary>
        public KCSDataGridView()
        {
			InitializeComponent();
            ColumnSpecs = new List<DataGridViewColumnSpec>();
            ColumnGroups = new List<DataGridViewColumnGroup>();
			AcceptableDragDropTypes = new List<Type>();
            SuppressDragSelection = false;
			_phantomText = "No imageData available.";
            ChangedRowHeaderBackColor = Color.Yellow;
			base.EnableHeadersVisualStyles = false;
			ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            DoubleBuffered = true;
            ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomLeft;
        }

        #region Column group methods
        public DataGridViewColumnGroup AddColumnGroup(string id, string text, int top, int height, Color foreColor, Color backColor, params DataGridViewColumn[] columns)
        {
            var sortedColumns = columns.OrderBy(x => x.Index).ToList();
            DataGridViewColumnGroup cg = new DataGridViewColumnGroup(this, id, text);
            cg.AddRange(sortedColumns);
            if (!ColumnGroups.Contains(cg))         // TODO: This needs to be properly tested. Groups cannot have duplicate IDs.
            {
                cg.Top = (uint)top;
                cg.Height = (uint)height;
                cg.ForeColor = foreColor;
                cg.BackColor = backColor;
                ColumnGroups.Add(cg);
                return cg;
            }

            return null;
        }

        public DataGridViewColumnGroup AddColumnGroup(string id, string text, int top, int height, Color foreColor, Color backColor, IEnumerable<DataGridViewColumn> columns)
        {
            return AddColumnGroup(id, text, top, height, foreColor, backColor, columns.ToArray());
        }

        public DataGridViewColumnGroup AddColumnGroup(string id, string text, int height, IEnumerable<DataGridViewColumn> columns)
        {
            return AddColumnGroup(id, text, SystemInformation.BorderSize.Height, height, DefaultCellStyle.BackColor, GridColor, columns.ToArray());
        }

        public DataGridViewColumnGroup AddColumnGroup(string id, string text, int height, params DataGridViewColumn[] columns)
        {
            return AddColumnGroup(id, text, SystemInformation.BorderSize.Height, height, DefaultCellStyle.BackColor, GridColor, columns);
        }
        #endregion

        /// <summary>
        /// Selects all rows.
        /// </summary>
        public new virtual void SelectAll()
        {
			foreach (DataGridViewRow row in Rows)
			{
				row.Selected = true;
			}
        }

        /// <summary>
        /// Deselects all items.
        /// </summary>
        public virtual void SelectNone()
        {
			foreach (DataGridViewRow row in Rows)
			{
				row.Selected = false;
			}
        }

        /// <summary>
        /// Inverts the selection.
        /// </summary>
        public virtual void InvertSelection()
        {
			foreach (DataGridViewRow row in Rows)
			{
				row.Selected = !row.Selected;
			}
        }

		///// <summary>
		///// Selects a single row, by value of a particular column.
		///// </summary>
		//public virtual bool Contains(string column, string value)
		//{
		//    ListViewItem[] items = Rows.Find(key, false);

		//    SelectNone();
		//    if (items.Length > 0)
		//    {
		//        items[0].Selected = true;
		//        EnsureVisible(items[0].Index);
		//    }
		//}

		/// <summary>
		/// Simulate clicking a Row.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnCellClick(DataGridViewCellEventArgs e)
		{
			DataGridViewRow row;

			base.OnCellClick(e);

			if (e.RowIndex >= 0 && RowClick != null)
			{
				row = this.Rows[e.RowIndex];
				if (row != CurrentRow)
				{
					return;
				}
				RowClick(this, new Shared.DataGridViewRowClickEventArgs(row));
			}
		}

		/// <summary>
		/// Simulate double-clicking a Row.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnCellDoubleClick(DataGridViewCellEventArgs e)
		{
			DataGridViewRow row;

			base.OnCellDoubleClick(e);

			if (e.RowIndex >= 0 && RowDoubleClick != null)
			{
				row = this.Rows[e.RowIndex];
				if (e.ColumnIndex >= 0)
				{
					this.CurrentCell = Rows[e.RowIndex].Cells[e.ColumnIndex];
				}
				else
				{
					this.CurrentCell = Rows[e.RowIndex].Cells[FirstDisplayedScrollingColumnIndex];
				}
				RowDoubleClick(this, new Shared.DataGridViewRowClickEventArgs(row));
			}
		}

		///// <summary>
		///// Simulate row doubleclick, if selection mode is FullRow.
		///// </summary>
		///// <param name="e"></param>
		//protected override void OnDoubleClick(EventArgs e)
		//{
		//    HitTestInfo ht = base.HitTest(Cursor.Position.X, Cursor.Position.Y);
		//    DataGridViewCell cell;

		//    base.OnDoubleClick(e);

		//    if (ht != null && ht.RowIndex > 0 && ht.ColumnIndex > 0 && RowDoubleClick != null)
		//    {
		//        cell = Rows[ht.RowIndex].Cells[ht.ColumnIndex];
		//        this.CurrentCell = cell;
		//        RowDoubleClick(this, new Shared.DataGridViewRowClickEventArgs(Rows[cell.RowIndex]));
		//    }
		//}

		/// <summary>
		/// When a row is added, set the Dirty Flag.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
		{
			base.OnRowsAdded(e);
			//IsDirty = true;
		}

		/// <summary>
		/// When a row is removed, set the Dirty flag.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e)
		{
			base.OnRowsRemoved(e);
			//IsDirty = true;
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
            Shared.DragDropObject<List<DataGridViewRow>> dragDropObject;

            base.OnDragOver(e);

            e.Effect = e.AllowedEffect;

            // We only want DragDropObject objects or filedrops.			
			if ((!e.Data.GetDataPresent(DataFormats.FileDrop) && !e.Data.GetDataPresent(typeof(Shared.DragDropObject<List<DataGridViewRow>>))))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

			// FileDrops are handled differently from DragDropObjects.
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				if (Control.ModifierKeys == Keys.Control)
				{
					e.Effect = DragDropEffects.Copy | DragDropEffects.Link;
				}
				else
				{
					e.Effect = DragDropEffects.Move;
				}
			}
			else
			{
				dragDropObject = e.Data.GetData(typeof(Shared.DragDropObject<List<DataGridViewRow>>)) as Shared.DragDropObject<List<DataGridViewRow>>;

				// See what type of data we can accept.
				if (dragDropObject.GetDataPresent(AcceptableDragDropTypes))
				{
					e.Effect = DragDropEffects.Copy | DragDropEffects.Link;
				}
			}
        }

        protected virtual void OnBeginRowDrag()
        {            
            //Point p = PointToClient(Cursor.Position);
            //DataGridView.HitTestInfo hit = HitTest(p.X, p.Y);

            //DataGridViewCell clickedCell = Rows[hit.RowIndex].Cells[hit.ColumnIndex];
            //DoDragDrop(clickedCell, DragDropEffects.Copy);
            DoDragDrop(new Shared.DragDropObject<List<DataGridViewRow>>(this, GetSelectedRows()), DragDropEffects.All);

            //if (BeginRowDrag != null)
            //    BeginRowDrag(this, e);
        }

        /// <summary>
        /// Simulate the beginning of a drag event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!ChildHandlesMouseClicks && AllowDrag && e.Button == MouseButtons.Left)
            {
                int row = this.HitTest(e.X, e.Y).RowIndex;

                // We only drag selected rows so the user can drag multiple rows.  If we call
                // base.OnMouseDown(e), the click will deselect all rows apart from the current row.
                if (row >= 0 && row < this.Rows.Count)
                {
                    // if this click is not in the previously selected rows or there is no select row, let the selection happens first
                    if (SelectedRows.Count == 0 || !this.SelectedRows.Contains(this.Rows[row]))
                    {
                        base.OnMouseDown(e);  
                    }

                    if (SuppressDragSelection || this.SelectedRows.Contains(this.Rows[row]))
                    {
                        _clickedRow = row;
                        Size dragSize = SystemInformation.DragSize;

                        _dragBounds = new Rectangle(new Point(
                          e.X - (dragSize.Width / 2),
                          e.Y - (dragSize.Height / 2)), dragSize);

                        _mouseDownArgs = e; // Record for future use if they abort drag/drop and are just clicking.

                        // If we are not suppressing it means this row has been selected, so we exit before base.OnMouseDown.
                        if (!SuppressDragSelection) return;
                    }
                    else
                    {
                        _dragBounds = Rectangle.Empty;
                    }
                }
                else
                {
                    _dragBounds = Rectangle.Empty;
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            // The drag drop operation was not concluded.
            if (!ChildHandlesMouseClicks && _dragBounds.Contains(e.X, e.Y) && _mouseDownArgs != null)
            {
                if (_clickedRow >= 0 && _clickedRow < this.Rows.Count)
                {
                    // Let the grid continue processing the click we hijacked earlier...
                    // Hopefully it will be on the same row!  Tolerance is good enough.
                    base.OnMouseDown(_mouseDownArgs);
                    _mouseDownArgs = null;
                }
            }

            base.OnMouseUp(e);
        }

		/// <summary>
		/// Raised when the mouse moves over the control.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
            base.OnMouseMove(e);

            if (!ChildHandlesMouseClicks && AllowDrag && e.Button == MouseButtons.Left)
            {
                int row = this.HitTest(e.X, e.Y).RowIndex;

                if (row < 0) // Ignore column headers.
                {
                    _dragBounds = Rectangle.Empty;
                    return;
                }

                if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    // If the mouse has been click-dragged outside our grid.
                    if (_dragBounds != Rectangle.Empty && !_dragBounds.Contains(e.X, e.Y))
                    {
                        //ItemDragEventArgs dragArgs = new ItemDragEventArgs(MouseButtons.Left, this.SelectedRows);
                        OnBeginRowDrag();
                    }
                }
            }
		}

        /// <summary>
        /// As soon as a cell value is changed, set the row to Modified, so that we don't have to leave that row.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
        {
            //base.OnCellValueChanged(e);

            if (Rows.Count == 0 || e.RowIndex < 0)
                return;

            object cellValue = null;
            try
            {
                DataRowView drv = Rows[e.RowIndex].DataBoundItem as DataRowView;
                DataGridViewColumn col = Columns[e.ColumnIndex];

                if (!Rows[e.RowIndex].IsNewRow && drv != null && !string.IsNullOrEmpty(col.DataPropertyName))
                {
                    cellValue = drv[col.DataPropertyName];

                    if (drv.Row.RowState == DataRowState.Unchanged)
                    {
                        drv.Row.SetModified();
                        drv[col.DataPropertyName] = cellValue;
                        InvalidateCell(this[e.ColumnIndex, e.RowIndex]);
                        this.IsDirty = true;
                    }

                    if (drv.Row.RowState == DataRowState.Detached)
                    {
                        DataTable.Rows.Add(drv.Row);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Annoying error from custom DataGridView" + ex.Message);
            }

            base.OnCellValueChanged(e);
        }

        protected override void OnColumnWidthChanged(DataGridViewColumnEventArgs e)
        {
            base.OnColumnWidthChanged(e);

            // Make sure to redraw the column group headers
            if (ColumnGroups.Count > 0)
            {
                var group = ColumnGroups.Where(x => x.Contains(e.Column)).FirstOrDefault();
                if (group != null) group.SetImage();
                Refresh();
            }
        }

        protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
        {
            base.OnDataError(displayErrorDialogIfNoHandler, e);
        }

        /// <summary>
        /// Trim spaces!
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellValidated(DataGridViewCellEventArgs e)
        {
            if (ReadOnly)
            {
                return;
            }
            base.OnCellValidated(e);
            
            DataGridViewCell cell = this[e.ColumnIndex, e.RowIndex];
            if ((cell.ReadOnly || cell.OwningRow.ReadOnly || cell.OwningColumn.ReadOnly) || !cell.OwningColumn.CellType.Equals(typeof(DataGridViewTextBoxCell)))
            {
                return;
            }

            string strValue = Utility.GetStringValue(this[e.ColumnIndex, e.RowIndex].Value).Trim();
            if (string.IsNullOrEmpty(strValue))
            {
                cell.Value = DBNull.Value;
            }
            else
            {
                cell.Value = Convert.ChangeType(strValue, cell.ValueType);
            }
        }

        /// <summary>
        /// Paint column group headers, if any.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);

            // Paint the row header if it has changed
            if (e.RowIndex > -1 && e.ColumnIndex < 0)
            {
                DataGridViewRow dgvr = Rows[e.RowIndex];
                DataRowView drv = dgvr.DataBoundItem as DataRowView;
                DataGridViewCell cell = dgvr.HeaderCell;

                // Changed rows get yellow in their headers
                if (drv != null)
                {
                    if (drv.Row.RowState == DataRowState.Added || Utility.IsRowReallyChanged(drv.Row))
                    {
                        cell.Style.BackColor = ChangedRowHeaderBackColor;
                    }
                    else
                    {
                        cell.Style.BackColor = RowHeadersDefaultCellStyle.BackColor;
                    }
                }
            }

            // Paint the columns, if applicable
            if (e.RowIndex == -1 && e.ColumnIndex > -1)
            {
                DataGridViewColumn currentColumn = Columns[e.ColumnIndex];
                DataGridViewColumnGroup columnGroup = GetColumnGroup(currentColumn).FirstOrDefault();
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                if (ColumnSpecs != null && ColumnSpecs.Count > 0)
                {                    
                    DataGridViewColumnSpec spec = ColumnSpecs.Where(x => x.Column == currentColumn).FirstOrDefault();
                    if (spec != null)
                    {
                        if (spec.Image != null && spec.UseImage)
                        {
                            Rectangle rect = e.CellBounds;
                            rect.Inflate(-1, -1);
                            e.Graphics.FillRectangle(Brushes.White, rect);      // White background
                            rect = new Rectangle(e.CellBounds.X, e.CellBounds.Y + (int)columnGroup.Height, e.CellBounds.Width, e.CellBounds.Height - (int)columnGroup.Height);
                            Point p = rect.FitRectangle(spec.Image.Size);
                            e.Graphics.DrawImage(spec.Image, p);
                            e.Handled = true;
                        }
                        else
                        {
                            currentColumn.HeaderCell.Style.BackColor = spec.BackColor;
                        }
                    }
                }                

                // Paint the column header groups if any (there should be just one)
                if (columnGroup != null)
                {
                    Image img = columnGroup.GetColumnImage(e.ColumnIndex);
                    e.Graphics.DrawImage(img, e.CellBounds.Left - 1, columnGroup.Top);
                    e.Handled = true;
                }
            }


            //if (e.RowIndex == -1 && e.ColumnIndex > -1)
            //{
            //    e.Paint(e.CellBounds, DataGridViewPaintParts.All);

            //    // Paint the column header groups, if any
            //    var query1 = GetColumnGroup(Columns[e.ColumnIndex]);
            //    if (query1.Count > 0)
            //    {
            //        foreach (var group in query1)
            //        {
            //            Image img = group.GetColumnImage(e.ColumnIndex);
            //            e.Graphics.DrawImage(img, e.CellBounds.Left - 1, group.Top);
            //        }
            //        e.Handled = true;
            //    }
            //}
        }

        /// <summary>
        /// Gets Column groups by column.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public List<DataGridViewColumnGroup> GetColumnGroup(DataGridViewColumn column)
        {
            var query = from cg in ColumnGroups
                        where cg.Contains(column)
                        select cg;
            return query.ToList();
        }

        /// <summary>
        /// Gets Column groups by GroupName.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public List<DataGridViewColumnGroup> GetColumnGroupByGroupName(string groupName)
        {
            var query = from cg in ColumnGroups
                        where string.Compare(cg.GroupName, groupName, true) == 0
                        select cg;
            return query.ToList();
        }

        /// <summary>
        /// Gets Column groups by ID.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public List<DataGridViewColumnGroup> GetColumnGroup(string id)
        {
            var query = from cg in ColumnGroups
                        where string.Compare(cg.ID, id, true) == 0
                        select cg;
            return query.ToList();
        }

		/// <summary>
		/// Selects the row with the matching value in a particular column. It examines
		/// only the visible columns.
		/// </summary>
		/// <param name="columnName">Column name to examine.</param>
		/// <param name="value">Value to match against.</param>
		/// <returns>Index of matching row. If nothing was found, returns -1</returns>
		public virtual int SelectRow<T>(string columnName, T value)
		{			
			DataGridViewColumn col = GetColumnByDataPropertyName(columnName);

            // If no matching column, then there's nothing to do.
            if (col == null) return -1;

			ClearSelection();
			foreach (DataGridViewRow row in Rows)
			{
				if (row.IsNewRow)
				{
					continue;
				}
				if (Convert.ChangeType(row.Cells[col.Name].Value, typeof(T)).Equals(value))
				{
					row.Selected = true;

					// Ensures that the row is visible. I wish there was a VisibleColumns property, to avoid this inner loop
					foreach(DataGridViewColumn c in Columns)
					{
						if (c.Visible)
						{							
							CurrentCell = row.Cells[c.Name];
							break;
						}
					}					
					return row.Index;
				}
			}
			return -1;
		}
        #endregion

		private void InitializeComponent()
		{
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			this.SuspendLayout();
			// 
			// ExtendedDataGridView
			// 
			base.BackgroundColor = System.Drawing.SystemColors.Window;
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();
			this.ResumeLayout(false);
			base.AllowUserToDeleteRows = false;
			base.AllowUserToResizeRows = false;
			base.AllowUserToAddRows = false;
			base.RowHeadersVisible = false;
			base.MultiSelect = false;
			AutoGenerateColumns = false;
			ShowPhantomText = true;
		}

	}
}
