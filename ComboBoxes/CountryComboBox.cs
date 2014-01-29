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
	/// Displays cultures.
	/// </summary>
	/// <remarks>This control can later be extended to custom-draw country flags or even fonts.</remarks>
	public class CountryComboBox : KCSComboBox
	{
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
		new public IEnumerable<Country> DataSource
		{
			get
			{
                return base.DataSource as IEnumerable<Country>;
			}
			set
			{
				base.DataSource = value;
			}
		}

        /// <summary>
        /// Gets or sets the selected Country.
        /// </summary>
        [
            Browsable(false)
        ]
        new public Country SelectedItem
        {
            get
            {
                if (SelectedIndex == -1)
                {
                    return null;
                }
                else
                {
                    return (Country)base.SelectedItem;
                }
            }
            set
            {
                Country country;
                for (int i = 0; i < Items.Count; i++)
                {
                    country = (Country)Items[i];
                    if (string.Compare(country.ISO3166Code, value.ISO3166Code, true) == 0)
                    {
                        SelectedIndex = i;
                        break;
                    }
                }
            }
        }

		/// <summary>
		/// Gets or sets the selected Country code.
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
                    return ((Country)base.SelectedItem).ISO3166Code;
                }
            }
            set
            {
                Country country;
                for (int i = 0; i < Items.Count; i++)
                {
                    country = (Country)Items[i];
                    if (string.Compare(country.ISO3166Code, value, true) == 0)
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
		public CountryComboBox()
		{
			base.DisplayMember = "Name";
            base.ValueMember = "ISO3166Code";
		}

		/// <summary>
		/// Displays all the countries.
		/// </summary>
		public void BindData()
		{
            DataSource = Countries.Instance.ToList();
		}
	}
}
