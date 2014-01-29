using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using KCS.Common.Shared;

namespace KCS.Common.Controls
{
    /// <summary>
    /// Renders a colored dropdown of all the colors.
    /// </summary>
	public class ColorComboBox : KCSComboBox
	{
		#region Properties
		/// <summary>
		/// Hides DrawMode.
		/// </summary>
		[
			Browsable(false)
		]
		new public DrawMode DrawMode
		{
			get
			{
				return base.DrawMode;
			}
			set
			{
			}
		}

		/// <summary>
		/// If TRUE, the colors are sorted by shade. If FALSE, they are sorted by Name.
		/// </summary>
		[
			Category("Behavior"),
			Description("If TRUE, the colors are sorted by shade. If FALSE, they are sorted by Name."),
			DefaultValue(true)
		]
		public bool SortedByShade { get; set; }

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
		new public IEnumerable<Color> DataSource
		{
			get
			{
				return base.DataSource as IEnumerable<Color>;
			}
			set
			{
				base.DataSource = value;
			}
		}

		/// <summary>
		/// Gets or sets the selected value, by Color Name.
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
					return Color.FromKnownColor(KnownColor.Transparent).Name;
				}
				else
				{
					return ((Color)base.SelectedItem).Name;
				}
			}
			set
			{
				Color color;
				for (int i = 0; i < Items.Count; i++)
				{
					color = (Color)Items[i];
					if (string.Compare(color.Name, value, true) == 0)
					{
						SelectedIndex = i;
						break;
					}
				}
			}
		}

        /// <summary>
        /// Gets or sets the selected Color.
        /// </summary>
		[
			Browsable(false)
		]
        new public Color SelectedItem
        {
            get
            {
                if (SelectedIndex == -1)
                {
                    return Color.FromKnownColor(KnownColor.Transparent);
                }
                else
                {
                    return (Color)base.SelectedItem;
                }
            }
            set
            {
                Color color;
                for(int i=0; i < Items.Count; i++)
                {
                    color = (Color)Items[i];
                    if (color.ToArgb().Equals(value.ToArgb()))
                    {
                        SelectedIndex = i;
                        break;
                    }
                }
            }
		}
		#endregion

		/// <summary>
        /// Constructor.
        /// </summary>
        public ColorComboBox()
        {
			ValueMember = string.Empty;
            base.DrawMode = DrawMode.OwnerDrawFixed;
			base.DropDownStyle = ComboBoxStyle.DropDownList;
			SortedByShade = true;
        }

        /// <summary>
        /// Paint the background of each item.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Color color;
            SolidBrush brush = null;

            if (e.Index == -1)
                return;
            
            try
            {
                e.DrawBackground();
                e.DrawFocusRectangle();

                // Get the name of the current item to be drawn, and make a brush of it
                color = (Color)Items[e.Index];
                brush = new SolidBrush(color);

                // Draw a rectangle and fill it with the current color
                // and add the name to the right of the color
                e.Graphics.DrawRectangle(Pens.Black, 3, e.Bounds.Top + 1, 20, 11);
                e.Graphics.FillRectangle(brush, 4, e.Bounds.Top + 2, 19, 10);
                e.Graphics.DrawString(color.Name, this.Font, Brushes.Black, 25, e.Bounds.Top);
            }
            finally
            {
                if (brush != null)
                {
                    brush.Dispose();
                }
            }

        }

        /// <summary>
        /// Displays all the named colors.
        /// </summary>
        public void BindData()
        {
			DataSource = Shared.GraphicsExtensions.GetNamedColors(SortedByShade, false);
        }

		/// <summary>
		/// Displays a specific set of colors, sorted by shade.
		/// </summary>
		/// <param name="colors">Colors to be displayed.</param>
		public void BindData(IEnumerable<Color> colors)
		{
			if (SortedByShade)
			{
				var query = from color in colors
							orderby string.Format("{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B) descending
							select color;
				colors = query.ToList();
			}
			DataSource = colors;
		}

		/// <summary>
		/// Resizes the control to the longest color name + color sample.
		/// </summary>
		public override void ResizeToLongestItem()
		{
			List<string> list = new List<string>(Items.Count);

			foreach (Color c in Items)
			{
				list.Add(c.Name);
			}

			if (list.Count > 0)
			{
				Width = Convert.ToInt32(list.GetLongestString(this, Width)) + SystemInformation.VerticalScrollBarWidth + 30;	// Include padding
			}
		}
	}
}
