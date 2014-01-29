using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KCS.Common.Controls
{
    /// <summary>
    /// Extends the standard ListBox control, adding some "batch" methods.
    /// It also allows us to control the SelectedIndexChangedEvent event.
    /// </summary>
    public class KCSListBox : ListBox
    {
        #region Properties
		/// <summary>
		/// If TRUE, the control raises the SelectedIndexChanged event.
		/// </summary>
        [
            Description("If TRUE, the toolStripItem raises the SelectedIndexChanged event."),
            DefaultValue(true),
            Category("Behavior")
        ]
        public bool RaiseSelectedIndexChangedEvent { get; set; }

		/// <summary>
		/// If TRUE, the control raises the SelectedIndexChanged event ONLY when there's at least one item selected.
		/// </summary>
        [
            Description("If TRUE, the toolStripItem raises the SelectedIndexChanged event ONLY when there's at least one item selected."),
            DefaultValue(false),
            Category("Behavior")
        ]
        public bool RaiseSelectedIndexChangedEventOnSelection { get; set; }

		/// <summary>
		/// Raised when an item in the ListBox is double-clicked.
		/// </summary>
        [
            Browsable(true),
            Description("Raised when an item in the ListBox is double-clicked."),
            Category("Action")
        ]
        public event EventHandler<Shared.ListBoxItemClickEventArgs> ItemDoubleClick;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public KCSListBox()
        {
            RaiseSelectedIndexChangedEvent = true;
        }

        /// <summary>
        /// Selected all items.
        /// </summary>
        public virtual void SelectAll()
        {
            RaiseSelectedIndexChangedEvent = false;
            for (int i = 0; i < Items.Count; i++)
            {
                SetSelected(i, true);
            }
            RaiseSelectedIndexChangedEvent = true;
        }

        /// <summary>
        /// Deselects all items.
        /// </summary>
        public virtual void SelectNone()
        {
            RaiseSelectedIndexChangedEvent = false;
            for (int i = 0; i < Items.Count; i++)
            {
                SetSelected(i, false);
            }
            RaiseSelectedIndexChangedEvent = true;
        }

        /// <summary>
        /// Remove selected items.
        /// </summary>
        public virtual void RemoveSelectedItems()
        {
            RaiseSelectedIndexChangedEvent = false;
            while (SelectedItems.Count > 0)
            {
                Items.Remove(SelectedItems[0]);
            }
            RaiseSelectedIndexChangedEvent = true;
        }

        /// <summary>
        /// Is raised when an item is selected or deselected.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (RaiseSelectedIndexChangedEvent)
            {
                if (!RaiseSelectedIndexChangedEventOnSelection || (RaiseSelectedIndexChangedEventOnSelection && SelectedItems.Count > 0))
                {
                    base.OnSelectedIndexChanged(e);
                }
            }
        }

        /// <summary>
        /// Handle the double-click event on an item.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            int index = IndexFromPoint(PointToClient(Cursor.Position));
            if (index > -1)
            {
                if (ItemDoubleClick != null)
                {
                    ItemDoubleClick(this, new Shared.ListBoxItemClickEventArgs(Items[index]));
                }
            }
        }
    }
}
