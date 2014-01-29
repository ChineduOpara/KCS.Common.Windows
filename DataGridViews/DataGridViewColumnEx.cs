using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KCS.Common.Controls
{
    public class DataGridViewColumnSpec
    {
        public Color BackColor { get; set; }
        public DataGridViewColumn Column { get; private set; }
        public Image Image { get; private set; }
        public bool UseImage { get; set; }

        public DataGridViewColumnSpec(DataGridViewColumn column)
        {
            BackColor = Color.Empty;
        }

        public DataGridViewColumnSpec(DataGridViewColumn column, Color backColor) : this(column)
        {
            Column = column;
            BackColor = backColor;
        }

        public DataGridViewColumnSpec(DataGridViewColumn column, Image image): this(column)
        {
            Column = column;
            this.Image = image;
        }

        public DataGridViewColumnSpec(DataGridViewColumn column, Color backColor, Image image, bool useImage) : this(column, image)
        {
            Column = column;
            this.Image = image;
            BackColor = backColor;
            UseImage = useImage;
        }
    }
}
