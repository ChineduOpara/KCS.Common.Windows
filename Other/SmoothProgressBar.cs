using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace KCS.Common.Controls
{
	/// <summary>
	/// A smooth progressbar control.
	/// </summary>
	[
		DefaultProperty("Value")
	]
	public class SmoothProgressBar : UserControl
	{
		int _minimum = 0;	// Minimum value for progress range
		int _maximum = 100;	// Maximum value for progress range
		int _value = 0;		// Current progress

		#region Properties
		[
			Description("Forecount color of the filled-in area."),
			Category("Appearance")
		]
		public override Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
				Invalidate();
			}
		}

		[
			Description("Minimum value."),
			Category("Data"),
			DefaultValue(0)
		]
		public int Minimum
		{
			get
			{
				return _minimum;
			}

			set
			{
				// Prevent a negative value.
				if (value < 0)
				{
					_minimum = 0;
				}

				// Make sure that the minimum value is never set higher than the maximum value.
				if (value > _maximum)
				{
					_minimum = value;
					_minimum = value;
				}

				// Ensure value is still in range
				if (_value < _minimum)
				{
					_value = _minimum;
				}

				// Invalidate the control to get a repaint.
				this.Invalidate();
			}
		}

		[
			Description("Maximum value."),
			Category("Data"),
			DefaultValue(100)
		]
		public int Maximum
		{
			get
			{
				return _maximum;
			}

			set
			{
				// Make sure that the maximum value is never set lower than the minimum value.
				if (value < _minimum)
				{
					_minimum = value;
				}

				_maximum = value;

				// Make sure that value is still in range.
				if (_value > _maximum)
				{
					_value = _maximum;
				}

				// Invalidate the control to get a repaint.
				this.Invalidate();
			}
		}

		[
			Description("Current value."),
			Category("Data"),
			DefaultValue(0)
		]
		public int Value
		{
			get
			{
				return _value;
			}

			set
			{
				int oldValue = _value;

				// Make sure that the value does not stray outside the valid range.
				if (value < _minimum)
				{
					_value = _minimum;
				}
				else if (value > _maximum)
				{
					_value = _maximum;
				}
				else
				{
					_value = value;
				}

				// Invalidate only the changed area.
				float percent;

				Rectangle newValueRect = this.ClientRectangle;
				Rectangle oldValueRect = this.ClientRectangle;

				// Use a new value to calculate the rectangle for progress.
				percent = (float)(_value - _minimum) / (float)(_maximum - _minimum);
				newValueRect.Width = (int)((float)newValueRect.Width * percent);

				// Use an old value to calculate the rectangle for progress.
				percent = (float)(oldValue - _minimum) / (float)(_maximum - _minimum);
				oldValueRect.Width = (int)((float)oldValueRect.Width * percent);

				Rectangle updateRect = new Rectangle();

				// Find only the part of the screen that must be updated.
				if (newValueRect.Width > oldValueRect.Width)
				{
					updateRect.X = oldValueRect.Size.Width;
					updateRect.Width = newValueRect.Width - oldValueRect.Width;
				}
				else
				{
					updateRect.X = newValueRect.Size.Width;
					updateRect.Width = oldValueRect.Width - newValueRect.Width;
				}

				updateRect.Height = this.Height;

				// Invalidate the intersection region only.
				this.Invalidate(updateRect);
			}
		}
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public SmoothProgressBar()
		{
			ForeColor = Color.FromKnownColor(KnownColor.ControlDark);
			BackColor = Color.FromKnownColor(KnownColor.Info);
			Minimum = 0;
			Maximum = 100;
		}

		/// <summary>
		/// Invalidate the control to get a repaint.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			this.Invalidate();
		}

		/// <summary>
		/// Perform painting.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			SolidBrush brush = new SolidBrush(ForeColor);
			float percent = (float)(_value - _minimum) / (float)(_maximum - _minimum);
			Rectangle rect = this.ClientRectangle;

			// Calculate area for drawing the progress.
			rect.Width = (int)((float)rect.Width * percent);

			// Draw the progress meter.
			g.FillRectangle(brush, rect);

			// Draw a three-dimensional border around the control.
			//if (BorderStyle == BorderStyle.None)
			//{
			//    ControlPaint.DrawBorder3D(g, e.ClipRectangle, Border3DStyle.SunkenInner);
			//}

			// Clean up.
			brush.Dispose();
			g.Dispose();
		}		
	}
}
