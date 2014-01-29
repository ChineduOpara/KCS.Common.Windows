using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KCS.Common.Controls
{
	/// <summary>
	/// Enhances the standard SplitContainer by allowing coloring of the splitter bar (defaults to ControlDark),
	/// and making the splitter slightly larger.
	/// </summary>
	[
		DefaultProperty("SplitterColor")
	]
	public class ExtendedSplitContainer : SplitContainer
	{
		/// <summary>
		/// Default color for the splitter bar.
		/// </summary>
		public static Color DefaultSplitterColor = Color.FromKnownColor(KnownColor.HotTrack);

		private Color _SplitterColor = DefaultSplitterColor;

		/// <summary>
		/// Gets or sets the Splitter color.
		/// </summary>
		[
			Description("Gets or sets the Splitter color."),
			DefaultValue(KnownColor.HotTrack),
			Category("Appearance")
		]
		public Color SplitterColor
		{
			get
			{
				return _SplitterColor;
			}
			set
			{
				if (value == Color.Transparent || value == null)
				{
					value = DefaultSplitterColor;
				}
				_SplitterColor = value;
				Refresh();
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExtendedSplitContainer()
		{
			SplitterWidth = 4;
		}

		/// <summary>
		/// Paints the splitter.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			Brush brush = new SolidBrush(SplitterColor);

			base.OnPaint(e);

			e.Graphics.FillRectangle(brush, this.SplitterRectangle);
		}
	}
}
