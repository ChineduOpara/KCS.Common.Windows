using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using KCS.Common.Shared;

namespace KCS.Common.Controls
{
    /// <summary>
    /// Represents a single column group.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataGridViewColumnGroup : List<DataGridViewColumn>, IEqualityComparer< DataGridViewColumnGroup>
    {
        private Image _image;
        private uint _height;
        private uint _top;
        private DataGridView _gvw;
        //private bool _readOnly;
        //private string _toolTipText;

        #region Properties        
        public object Tag { get; set; }
        public Color ForeColor { get; set; }
        public Color BackColor { get; set; }
        public Color BorderColor { get; set; }
        public Font Font { get; set; }

        /// <summary>
        /// This cannot be a negative number.
        /// </summary>
        public uint Top
        {
            get { return _top; }
            set { if (value < 0) value = 0; _top = value; }
        }

        /// <summary>
        /// This cannot be a negative number.
        /// </summary>
        public uint Height
        {
            get { return _height; }
            set {if (value < 1) value = 1; _height = value;}
        }

        public const int DefaultHeight = 30;

        /// <summary>
        /// Contains the group's Unique ID.
        /// </summary>
        public string ID { get; private set; }

        ///// <summary>
        ///// Alternate ID.
        ///// </summary>
        //public string AlternateID { get; set; }

        /// <summary>
        /// Name of the group. This is different from the Text.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Text to display.
        /// </summary>
        public string Text { get; private set; }

        ///// <summary>
        ///// Gets or sets the readonly property of all columns in this group.
        ///// </summary>
        //public bool ReadOnly
        //{
        //    get { return _readOnly; }
        //    set
        //    {
        //        _readOnly = value;
        //        foreach (DataGridViewColumn column in this)
        //        {
        //            column.ReadOnly = value;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the ToolTipText property of all columns in this group.
        ///// </summary>
        //public string ToolTipText
        //{
        //    get { return _toolTipText; }
        //    set
        //    {
        //        _toolTipText = value;
        //        foreach (DataGridViewColumn column in this)
        //        {
        //            column.ToolTipText = value;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the ToolTipText property of all columns in this group.
        ///// </summary>
        //public Color BackColor
        //{
        //    get { return _toolTipText; }
        //    set
        //    {
        //        _toolTipText = value;
        //        foreach (DataGridViewColumn column in this)
        //        {
        //            column.ToolTipText = value;
        //        }
        //    }
        //}
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Key.</param>
        /// <param name="headerText">Header Text.</param>
        public DataGridViewColumnGroup(DataGridView gvw, string id, string headerText)
        {
            _gvw = gvw;
            this.ID = id;
            Text = headerText;
            if (string.IsNullOrEmpty(headerText))
            {
                Text = ID.ToString();
            }
            ForeColor = _gvw.ColumnHeadersDefaultCellStyle.ForeColor;
            BackColor = _gvw.ColumnHeadersDefaultCellStyle.BackColor;
            Font = _gvw.ColumnHeadersDefaultCellStyle.Font;
            BorderColor = _gvw.GridColor;
            Height = DefaultHeight;
        }        

        /// <summary>
        /// Sets the image that will be used for the header.
        /// </summary>
        /// <param name="refresh">Force a re-generation of the image.</param>
        public void SetImage()
        {
            Graphics g = null;
            SolidBrush fillBrush = null;
            SolidBrush textBrush = null;
            int totalWidth = 0;

            try
            {
                StringFormat sf;
                sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                totalWidth = GetTotalWidth();

                // Generate blank bitmap
                _image = new Bitmap(totalWidth, (int)Height);
                g = Graphics.FromImage(_image);
                textBrush = new SolidBrush(ForeColor);
                fillBrush = new SolidBrush(BackColor);

                GraphicsUnit gu = GraphicsUnit.Display;
                RectangleF rectf = _image.GetBounds(ref gu);
                g.FillRectangle(fillBrush, rectf);
                g.DrawString(Text, this.Font, textBrush, rectf, sf);

                // Draw 3D border
                ControlPaint.DrawBorder(g, Rectangle.Round(rectf), BorderColor, ButtonBorderStyle.Solid);

                _image = (Image)_image.Clone();
            }
            finally
            {
                if (textBrush != null) textBrush.Dispose();
                if (fillBrush != null) fillBrush.Dispose();
                if (g != null) g.Dispose();
            }
        }

        public Image GetColumnImage(int columnDisplayIndex)
        {
            return GetColumnImage(columnDisplayIndex, false);
        }

        private Image GetColumnImage(int columnDisplayIndex, bool refresh)
        {
            Bitmap bmp = null;
            Graphics g = null;

            // Make sure the image is available.
            if (_image == null || refresh)
            {
                SetImage();
            }

            try
            {
                // Get the left starting position and width of resulting image
                int relativeIndex = columnDisplayIndex - this[0].DisplayIndex;
                int targetLeft = GetLeftWidth(columnDisplayIndex);
                int targetWidth = this[relativeIndex].Width;

                bmp = new Bitmap(targetWidth, _image.Height);
                g = Graphics.FromImage(bmp);
                Rectangle destRect = new Rectangle(0, 0, targetWidth, _image.Height);
                Rectangle srcRect = new Rectangle(targetLeft, 0, targetWidth, _image.Height);
                g.DrawImage(_image, destRect, srcRect, GraphicsUnit.Pixel);
            }
            finally
            {
                if (g != null) g.Dispose();
            }

            return bmp;
        }

        /// <summary>
        /// Gets the total width of all the columns.
        /// </summary>
        /// <returns></returns>
        private int GetTotalWidth()
        {
            return this.Where(x => x.Visible).Sum(x => x.Width);
        }

        /// <summary>
        /// Get the total widths of the columns BEFORE this a given column.
        /// </summary>
        /// <param name="columnRelativeIndex">Index of the column relative to the others in the DataGridView.</param>
        /// <returns></returns>
        private int GetLeftWidth(int columnDisplayIndex)
        {
            int leftWidth = 0;
            int firstColumnAbsoluteIndex = this[0].DisplayIndex;
            int relativeIndex = columnDisplayIndex - this[0].DisplayIndex;
            for (int c = 0; c < relativeIndex; c++)
            {
                if (this[c].Visible)
                {
                    leftWidth += this[c].Width;
                }
            }
            return leftWidth;
        }

        public override string ToString()
        {
            return Text;
        }

        public int CompareTo(DataGridViewColumnGroup other)
        {
            return string.Compare(ID, other.ID, true);
        }

        public bool Equals(DataGridViewColumnGroup x, DataGridViewColumnGroup y)
        {
            return string.Compare(ID, y.ID, true) == 0;
        }

        public int GetHashCode(DataGridViewColumnGroup obj)
        {
            return obj.ID.GetHashCode();
        }
    }
}
