using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace KCS.Common.Controls
{
	/// <summary>
	/// Needs documentation.
	/// </summary>
    [ToolboxBitmap(typeof(AsyncProgressBarControl), "ToolBoxGraphic.bmp")]
    public partial class AsyncProgressBarControl : System.Windows.Forms.UserControl
	{
		#region Nested types
		/// <summary>
		/// Progress motion style.
		/// </summary>
		public enum OSProgressStyle
		{
			/// <summary>
			/// Left or right.
			/// </summary>
			LeftOrRight,

			/// <summary>
			/// Left and right.
			/// </summary>
			LeftAndRight
		}

		/// <summary>
		/// Progress display type.
		/// </summary>
		public enum OSProgressType
		{
			/// <summary>
			/// Box.
			/// </summary>
			Box,

			/// <summary>
			/// Graphic.
			/// </summary>
			Graphic
		}

		/// <summary>
		/// Progress box size style.
		/// </summary>
		public enum OSProgressBoxStyle
		{
			/// <summary>
			/// Solid, same size.
			/// </summary>
			SolidSameSize,			

			/// <summary>
			/// Solid, bigger.
			/// </summary>
			SolidBigger,

			/// <summary>
			/// Solid, smaller.
			/// </summary>
			SolidSmaller,

			/// <summary>
			/// Box, around.
			/// </summary>
			BoxAround
		}
		#endregion

		#region Members
		private System.ComponentModel.IContainer components;
		internal System.Windows.Forms.Timer tmrAutoProgress;
		private Image _NormalImage;
		private byte _SpeedMultiplier = 2;
		private bool _RequireClear = false;
		private Graphics _Graphics;
		private bool _Increasing = true;
		private Image _PointImage;		
		private OSProgressType _ProgressType;
		private Color _IndicatorColor = Color.Red;
		private OSProgressStyle _ProgressStyle = OSProgressStyle.LeftOrRight;
		private OSProgressBoxStyle _ProgressBoxStyle = OSProgressBoxStyle.SolidSameSize;		
		private bool _AutoProgress = false;
		private bool _ShowBorder = true;
		private int _NumPoints;
		private int _Position;
		private byte _AutoProgressSpeed = 100;
		#endregion

		#region Properties
		/// <summary>
		/// Needs documentation.
		/// </summary>
		private bool SerializeProgressType
		{
			get
			{
				return !((_ProgressType == OSProgressType.Box));
			}
		}

		/// <summary>
		/// Determines the type of progress bar.
		/// </summary>
		[
			Description("Determines the type of progress bar.")
		]
		public OSProgressType ProgressType
		{
			get
			{
				return _ProgressType;
			}
			set
			{
				_ProgressType = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Needs documentation.
		/// </summary>
		private bool SerializeNormalImage
		{
			get
			{
				return !(_NormalImage == null);
			}
		}

		/// <summary>
		/// Needs documentation.
		/// </summary>
		public Image NormalImage
		{
			get
			{
				return _NormalImage;
			}
			set
			{
				_NormalImage = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Needs documentation.
		/// </summary>
		private bool SerializePointImage
		{
			get
			{
				return !(_PointImage == null);
			}
		}

		/// <summary>
		/// Needs documentation.
		/// </summary>
		public Image PointImage
		{
			get
			{
				return _PointImage;
			}
			set
			{
				_PointImage = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Controls visibility of the border.
		/// </summary>
		[Description("Controls visibility of the border."), DefaultValue(true)]
		public bool ShowBorder
		{
			get
			{
				return _ShowBorder;
			}
			set
			{
				_ShowBorder = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Number of points in the progressbar.
		/// </summary>
		[Description("Number of points in the progressbar."), Browsable(false)]
		public int NumPoints
		{
			get
			{
				return _NumPoints;
			}
		}

		/// <summary>
		/// Position, in percent, of the progress indicator.
		/// </summary>
		[Description("Position, in percent, of the progress indicator."), Browsable(false)]
		public int Position
		{
			get
			{
				return _Position;
			}
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				else if (value > _NumPoints)
				{
					value = _NumPoints;
				}
				_Position = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Needs documentation.
		/// </summary>
		private bool SerializeIndicatorColor
		{
			get
			{
				return !((_IndicatorColor.Equals(Color.Red)));
			}
		}

		/// <summary>
		/// Color of the indicator.
		/// </summary>
		[
			Description("Color of the indicator.")
		]
		public Color IndicatorColor
		{
			get
			{
				return _IndicatorColor;
			}
			set
			{
				_IndicatorColor = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Needs documentation.
		/// </summary>
		private bool SerializeProgressStyle
		{
			get
			{
				return !((_ProgressStyle == OSProgressStyle.LeftOrRight));
			}
		}

		/// <summary>
		/// Indicates the progress indicator rotation style.
		/// </summary>
		[Description("Indicates the progress indicator rotation style.")]
		public OSProgressStyle ProgressStyle
		{
			get
			{
				return _ProgressStyle;
			}
			set
			{
				_ProgressStyle = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Indicates the progress indicator rotation style.
		/// </summary>
		[Description("Indicates whether auto-progress is enabled."), DefaultValue(false)]
		public bool AutoProgress
		{
			get
			{
				return _AutoProgress;
			}
			set
			{
				this.tmrAutoProgress.Interval = (255 - _AutoProgressSpeed) * _SpeedMultiplier;
				if (value)
				{
					this.tmrAutoProgress.Start();
				}
				else
				{
					this.tmrAutoProgress.Stop();
				}
				_AutoProgress = value;
			}
		}

		private bool SerializeAutoProgressSpeed
		{
			get
			{
				return _AutoProgressSpeed != 100;
			}
		}

		/// <summary>
		/// Indicates the speed of the progress indicator (1 [slower] to 255 [faster].
		/// </summary>
		[
			Description("Indicates the speed of the progress indicator (1 [slower] to 255 [faster].")
		]
		public byte AutoProgressSpeed
		{
			get
			{
				return _AutoProgressSpeed;
			}
			set
			{
				if (value < 1)
				{
					value = 1;
				}
				else if (value > 254)
				{
					value = 254;
				}
				this.tmrAutoProgress.Stop();
				this.tmrAutoProgress.Interval = (255 - value) * _SpeedMultiplier;
				this.tmrAutoProgress.Enabled = _AutoProgress;
				_AutoProgressSpeed = value;
			}
		}

		private bool SerializeProgressBoxStyle
		{
			get
			{
				return !((_ProgressBoxStyle == OSProgressBoxStyle.SolidSameSize));
			}
		}

		/// <summary>
		/// Indicates the style of the indicator box.
		/// </summary>
		[
			Description("Indicates the style of the indicator box.")
		]
		public OSProgressBoxStyle ProgressBoxStyle
		{
			get
			{
				return _ProgressBoxStyle;
			}
			set
			{
				_ProgressBoxStyle = value;
				this.Invalidate();
			}
		}
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
        public AsyncProgressBarControl()
        {
            InitializeComponent();
        }

		/// <summary>
		/// Dispose method, for IDisposable pattern.
		/// </summary>
		/// <param name="disposing">Disposing flag.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!((components == null)))
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }        		                

		/// <summary>
		/// Paint event handler. Needs documentation.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void NEProgressBar_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            this._Graphics = e.Graphics;
            this._Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            if (_RequireClear)
            {
                this._Graphics.Clear(this.BackColor);
            }
            DrawBackGround();
        }

		/// <summary>
		/// Needs documentation.
		/// </summary>
		/// <param name="rect">Area to be drawn.</param>
        private void PositionIndicator(Rectangle rect)
        {
            if (!(this._PointImage==null) && this._ProgressType == OSProgressType.Graphic)
            {
                this._Graphics.DrawImage(this._PointImage, rect);
            }
            else
            {
                if (this._ProgressBoxStyle == OSProgressBoxStyle.SolidSameSize)
                {
                    Rectangle R2 = new Rectangle(rect.Left + 3, rect.Top + 3, rect.Width - 5, rect.Height - 5);
                    this._Graphics.FillRectangle(new SolidBrush(_IndicatorColor), R2);
                }
                else if (this._ProgressBoxStyle == OSProgressBoxStyle.BoxAround)
                {
                    this._Graphics.DrawRectangle(new Pen(_IndicatorColor), rect);
                    Rectangle R2 = new Rectangle(rect.Left + 3, rect.Top + 3, rect.Width - 5, rect.Height - 5);
                    this._Graphics.FillRectangle(new SolidBrush(_IndicatorColor), R2);
                }
                else if (this._ProgressBoxStyle == OSProgressBoxStyle.SolidBigger)
                {
                    this._Graphics.FillRectangle(new SolidBrush(_IndicatorColor), rect);
                }
                else if (this._ProgressBoxStyle == OSProgressBoxStyle.SolidSmaller)
                {
                    Rectangle R2 = new Rectangle(rect.Left + 5, rect.Top + 5, rect.Width - 9, rect.Height - 9);
                    this._Graphics.FillRectangle(new SolidBrush(_IndicatorColor), R2);
                }
            }
        }

		/// <summary>
		/// Needs documentation.
		/// </summary>
        private void DrawBackGround()
        {
            this._NumPoints = 0;
            if (this.Width > 0 & this.Height > 0)
            {
                if (this._ShowBorder)
                {
                    this._Graphics.DrawRectangle(new Pen(SystemColors.ActiveBorder), new Rectangle(0, 0, this.Width - 1, this.Height - 1));
                }
                double iBoxSize = Convert.ToDouble(this.Height.ToString()) * 0.75;
                int iBoxLeft = (int)iBoxSize / 2;
                if (iBoxSize > 3)
                {
                    do
                    {
                        Rectangle r = new Rectangle(iBoxLeft, 0, this.Height - 1, this.Height - 1);
                        if (r.Left + r.Width > this.Width)
                        {
                            goto exitDoLoopStatement0;
                        }
                        if (this._NumPoints == this._Position)
                        {
                            PositionIndicator(r);
                        }
                        else
                        {
                            Rectangle R2 = new Rectangle(r.Left + 3, r.Top + 3, r.Width - 6, r.Height - 6);
                            if (!(this._NormalImage==null) && this._ProgressType == OSProgressType.Graphic)
                            {
                                this._Graphics.DrawImage(this._NormalImage, R2);
                            }
                            else
                            {
                                this._Graphics.FillRectangle(new SolidBrush(this.ForeColor), R2);
                            }
                        }
                        iBoxLeft += (int)(iBoxSize * 1.5);
                        this._NumPoints += 1;
                    } while (true);
                exitDoLoopStatement0: ;
                }
            }
        }

		/// <summary>
		/// Resize event handlers.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void NEProgressBar_Resize(object sender, System.EventArgs e)
        {
            this._RequireClear = true;
            this.Invalidate();
        }

		/// <summary>
		/// Tick event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void tmrAutoProgress_Tick(object sender, System.EventArgs e)
        {
            if (this._Position == this._NumPoints - 1)
            {
                if (this._ProgressStyle == OSProgressStyle.LeftOrRight)
                {
                    this._Position = 0;
                }
                else
                {
                    this._Position -= 1;
                    this._Increasing = false;
                }
            }
            else if (this._Position == 0 & !(this._Increasing))
            {
                this._Position += 1;
                this._Increasing = true;
            }
            else
            {
                if (this._Increasing)
                {
                    this._Position += 1;
                }
                else
                {
                    this._Position -= 1;
                }
            }
            this._RequireClear = false;
            this.Invalidate();
        }

		/// <summary>
		/// Initializes components.
		/// </summary>
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tmrAutoProgress = new System.Windows.Forms.Timer(this.components);
			this.Name = "AsyncProgressBarControl";
			this.Size = new System.Drawing.Size(257, 20);

			this.Paint += new PaintEventHandler(this.NEProgressBar_Paint);
			this.Resize += new EventHandler(this.NEProgressBar_Resize);
			this.tmrAutoProgress.Tick += new EventHandler(this.tmrAutoProgress_Tick);
		}
    }
}
