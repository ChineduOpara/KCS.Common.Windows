using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using KCS.Common.Shared;

namespace KCS.Common.Controls
{
	/// <summary>
	/// Extends the generic ComboBox.
	/// </summary>
    public class KCSComboBox : System.Windows.Forms.ComboBox
    {
		private bool _readOnly;

		/// <summary>
		/// If TRUE, the control raises the SelectedIndexChanged event.
		/// </summary>
        [
            Category("Behavior"),
            DefaultValue(true),
			Description("If TRUE, the control raises the SelectedIndexChanged event.")
        ]
        public bool RaiseSelectedIndexChangedEvent { get; set; }

		/// <summary>
		/// If TRUE, the control does not allow selection.
		/// </summary>
		[
			Category("Behavior"),
			DefaultValue(false),
			Description("If TRUE, the control does not allow selection.")
		]
		public bool ReadOnly
		{
			get
			{
				return _readOnly;
			}
			set
			{
				_readOnly = value;
				base.Enabled = !value;
			}
		}

        /// <summary>
        /// Constructor.
        /// </summary>
        public KCSComboBox()
        {
            DisplayMember = "Name";
            ValueMember = "Id";
            DropDownStyle = ComboBoxStyle.DropDownList;
            RaiseSelectedIndexChangedEvent = true;
			ReadOnly = false;
        }

        /// <summary>
        /// Raised when an item is selected or deselected.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
			if (RaiseSelectedIndexChangedEvent)
            {
                base.OnSelectedIndexChanged(e);
            }
        }

        public virtual void ClearSelection()
        {
            var raiseEvent = RaiseSelectedIndexChangedEvent;
            SelectedIndex = -1;
            RaiseSelectedIndexChangedEvent = raiseEvent;
        }

		/// <summary>
		/// Adjusts the width of the combobox to acommodate the longest string.
		/// </summary>
		/// <remarks>
		/// Base class handles just a handful of types..
		/// </remarks>
		public virtual void ResizeToLongestItem()
		{
		    var list = new List<string>(Items.Count);

		    foreach (object obj in Items)
		    {
				if (obj is string)
				{
					list.Add(obj.ToString());
				}
				else
				{
					if (obj is System.Data.DataRowView)
					{
						list.Add(((System.Data.DataRowView)obj)[DisplayMember].ToString());
					}
					else
					{
						if (obj is System.Data.DataRow)
						{
							list.Add(((System.Data.DataRow)obj)[DisplayMember].ToString());
						}
						else
						{
							//list.Add(obj.ToString());
						}
					}
				}
		    }
			if (list.Count > 0)
			{
				Width = Convert.ToInt32(list.GetLongestString(this, Width)) + SystemInformation.VerticalScrollBarWidth + 10;	// Include padding
			}
		}
    }
}
