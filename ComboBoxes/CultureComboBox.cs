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
	public class CultureComboBox : KCSComboBox
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
		new public DataTable DataSource
		{
			get
			{
				return base.DataSource as DataTable;
			}
			set
			{
				base.DataSource = value;
			}
		}

		/// <summary>
		/// Gets or sets the selected Culture code.
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
					return KCS.Common.Shared.Utility.GetStringValue(base.SelectedValue);
				}
			}
			set
			{
				for (int i = 0; i < Items.Count; i++)
				{
					DataRowView drv = (DataRowView)Items[i];
					string v = KCS.Common.Shared.Utility.GetStringValue(drv[ValueMember]);
					if (string.Compare(v, value, true) == 0)
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
		public CultureComboBox()
		{
			base.DisplayMember = "Name";
			base.ValueMember = "Code";
		}

		/// <summary>
		/// Displays all the cultures.
		/// </summary>
		public void BindData()
		{
			DataTable dt = new DataTable("Cultures");
			dt.EnsureColumn("Code");
			dt.EnsureColumn("Name");

			foreach (CultureInfo ci in Utility.GetCultures())
			{
				dt.Rows.Add(ci.Name, ci.DisplayName);
			}
			dt.DefaultView.Sort = "Name";

			DataSource = dt;
		}
	}
}
