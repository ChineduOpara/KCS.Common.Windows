using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.ComponentModel;

namespace KCS.Common.Controls
{
    /// <summary>
    /// GGeorgiev 10/27/2008
    /// Base class for specialized CheckedListBox controls. 
    /// This class can control CheckedListBox content (copy another CheckedListBox).  
    /// Extends the standard ListBox control, adding some "batch" methods. - CheckAll, CheckNone -Chinedu Opara
    /// Extends the standard CheckedListBox, adding DataSource, ValueMember, DisplayMember
    /// Extends the standard CheckedListBox, adding ability to copy checked list box and get a delimited list of checked values
    /// </summary>
    public class KCSCheckedListBox : CheckedListBox
    {
        private bool _showScrollBar = true;

        #region Properties
        /// <summary>
        /// Binds IListSource
        /// </summary>
        [DefaultValue(true), Category("Behavior"), Description("Show or hide the scrollbars")]
        public bool ShowScrollBar
        {
            get
            {
                return _showScrollBar;
            }
            set
            {
                if (value == _showScrollBar)
                    return;
                _showScrollBar = value;
                if (Handle != IntPtr.Zero)
                    RecreateHandle();
            }
        }

        ///// <summary>
        /////  The value member from the DataSource
        ///// </summary>
        //[DefaultValue("")]
        //public new string ValueMember
        //{
        //    get
        //    {
        //        return base.ValueMember;
        //    }
        //    set
        //    {
        //        base.ValueMember = value;

        //    }
        //}

        ///// <summary>
        ///// The member to be displayed from the DataSource
        ///// </summary>
        //[DefaultValue("")]
        //public new string DisplayMember
        //{
        //    get
        //    {
        //        return base.DisplayMember;
        //    }
        //    set
        //    {
        //        base.DisplayMember = value;

        //    }
        //}
        public bool HasCheckedItems
        {
            get
            {
                return CheckedIndices.Count == 0 ? false : true;
            }
            private set{;}
        }
        #endregion

        #region Methods

        #region CopyFrom CheckedListBox
        /// <summary>
        /// Copies CheckedListBox
        /// </summary>
        /// <param name="source">Source CheckedListBox.</param>
        public void CopyFrom(CheckedListBox source)
        {
            this.Items.Clear();

            DataTable dt = new DataTable();

            //if (this.IsCopyEnabled)
            //{

                if (dt.Columns.Count < 2)
                {
                    dt.Columns.Add("Index", typeof(bool));
                    dt.Columns.Add("Value", typeof(string));
                }

                for (int i = 0; i < source.Items.Count; i++)
                {
                    DataRow dr = dt.NewRow();

                    dr["Index"] = source.GetItemChecked(i);
                    dr["Value"] = source.Items[i].ToString();

                    dt.Rows.Add(dr);
                    if (source.GetItemChecked(i) == true)
                    {
                        source.SetItemChecked(i, false);
                    }
                    else
                    {
                        source.SetItemChecked(i, true);
                    }
                }

                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    this.Items.Add(dt.Rows[i][1].ToString(), (bool)dt.Rows[i][0]);
                }

            //}//end if
        } 
        #endregion

        #region CopyFrom ExtendedCheckedListBox
        /// <summary>
        /// Copies ExtendedCheckedListBox
        /// </summary>
        /// <param name="source">ExtendedCheckedListBox</param>
        /// <remarks>
        /// Comment from Chinedu: I am not sure why this is different from the OTHER CopyFrom method.
        /// They should all be in 1 method called Clone (realized from ICloneable).
        /// </remarks>
        public void CopyFrom(KCSCheckedListBox source)
        {
            this.DataSource = source.DataSource;
            this.ValueMember = source.ValueMember;
            this.DisplayMember = source.DisplayMember;

            //source.CheckNone();
            // Set up checked state for each item
            foreach (int indexChecked in source.CheckedIndices)
            {
                this.SetItemChecked(indexChecked, true);
                //this.SetItemCheckState(indexChecked, CheckState.Checked);
            }
        } 
        #endregion

		/// <summary>
		/// Copies the checked state of the items in an ExtendedCheckedListBox.
		/// Rename this later to CopyCheckedState.
		/// </summary>
		/// <param name="target">Target control.</param>
        public void CopyTo(KCSCheckedListBox target)
        {
            target.DataSource = this.DataSource;
            target.ValueMember = this.ValueMember;
            target.DisplayMember = this.DisplayMember;
           
            target.CheckNone();
            // Set up checked state for each item
            foreach (int indexChecked in this.CheckedIndices)
            {
                target.SetItemChecked(indexChecked, true);
                //this.SetItemCheckState(indexChecked, CheckState.Checked);
            }

        } 

        #region CheckAll
        /// <summary>
        /// Checks all the items.
        /// </summary>
        public void CheckAll()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                SetItemChecked(i, true);
            }
        }
        #endregion

        #region CheckNone
        /// <summary>
        /// Unchecks all the items.
        /// </summary>
        public void CheckNone()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                SetItemChecked(i, false);
            }
        }
        #endregion

		/// <summary>
		/// Gets all the checked items as a specific type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T[] GetCheckedItems<T>()
		{
			List<T> list = new List<T>(CheckedIndices.Count);
			foreach (int i in CheckedIndices)
			{
				list.Add((T)Items[i]);
			}
			return list.ToArray();
		}

        /// <summary>
        /// Confirms that at least one item is checked.
        /// </summary>
        /// <returns>True or false.</returns>
        public bool IsItemChecked
        {
			get
			{
                return CheckedIndices.Count == 0 ? false : true;
				//return CheckedIndices.Count > 0;
			}
        }

        public int GetSingleItemChecked()
        {
            int c = 0;
            if (CheckedIndices.Count == 1)
            {
                int.TryParse(this.CheckedItems[0].ToString(), out c);

                foreach (int i in this.CheckedIndices)
                {
                    KeyValuePair<object, object> kvp = (KeyValuePair<object, object>)this.Items[i];

                    //string s = ((DataRowView)this.Items[i]).Row[0].ToString();
                    int.TryParse(kvp.Key.ToString(), out c);
                }
            }
            else
            {
                c = 0;
            }
            return c;
 
        } 
        #endregion
    }
}
