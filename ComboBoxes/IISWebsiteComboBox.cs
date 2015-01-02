using System;
using System.ComponentModel;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Windows.Forms;
using KCS.Common.Shared;

namespace KCS.Common.Controls
{
	/// <summary>
	/// Displays websites.
	/// </summary>
	public class IISWebsiteComboBox : KCSComboBox
	{
        /// <summary>
        /// Hide the base Items property.
        /// </summary>
        [
            Browsable(false)
        ]
        new public ObjectCollection Items
        {
            get
            {
                return base.Items;
            }
            set
            {
            }
        }

		/// <summary>
		/// Hide the base DropDownStyle property.
		/// </summary>
		[
			Browsable(false)
		]
		new public ComboBoxStyle DropDownStyle
		{
			get
			{
				return base.DropDownStyle;
			}
			set
			{
			}
		}

		/// <summary>
		/// Hide the base DataSource property.
		/// </summary>
		[
			Browsable(false)
		]
		new public IEnumerable<IISWebsite> DataSource
		{
			get
			{
                return base.DataSource as IEnumerable<IISWebsite>;
			}
			set
			{
				base.DataSource = value;
			}
		}

        /// <summary>
        /// Gets or sets the selected Website.
        /// </summary>
        [
            Browsable(false)
        ]
        new public IISWebsite SelectedItem
        {
            get
            {
                if (SelectedIndex == -1)
                {
                    return null;
                }
                else
                {
                    return (IISWebsite)base.SelectedItem;
                }
            }
            set
            {
                ClearSelection();
                IISWebsite site;
                for (int i = 0; i < Items.Count; i++)
                {
                    site = (IISWebsite)Items[i];
                    if (site == value)
                    {
                        SelectedIndex = i;
                        break;
                    }
                }
            }
        }

		/// <summary>
		/// Gets or sets the selected Websites, by name.
		/// </summary>
		[
			Browsable(false)
		]
        new public string SelectedValue
        {
            get
            {
                if (SelectedIndex == -1)
                {
                    return null;
                }
                else
                {
                    return ((IISWebsite)base.SelectedItem).Name;
                }
            }
            set
            {
                ClearSelection();
                IISWebsite website;
                for (int i = 0; i < Items.Count; i++)
                {
                    website = (IISWebsite)Items[i];
                    if (website.Name.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                    {
                        SelectedIndex = i;
                        break;
                    }
                }
            }
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		public IISWebsiteComboBox()
		{
			base.DisplayMember = "Name";
            base.ValueMember = "Name";
		}

		/// <summary>
		/// Loads all the Websites, ordered by name.
		/// </summary>
		public void BindData()
		{
            DataSource = IIS.Websites.OrderBy(x => x.Name).ToList();
		}
	}
}
