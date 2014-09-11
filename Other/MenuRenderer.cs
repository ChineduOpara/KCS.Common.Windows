using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KCS.Common.Controls
{
    public class MenuRenderer : ToolStripRenderer
    {
        //private SolidBrush _backBrush;
        //private SolidBrush _highlightBrush;
        public Color BackColor { get; private set; }

        public MenuRenderer() : base()
        {
            //BackColor = Color.Transparent;
        }

        public MenuRenderer(Color backColor) : this()
        {
            this.BackColor = backColor;
        }

        //protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        //{
        //    base.OnRenderToolStripBackground(e);
        //    //if (BackColor != Color.Transparent)
        //    //{
        //    //    if (_backBrush == null)
        //    //    {
        //    //        _backBrush = new SolidBrush(BackColor);
        //    //    }
        //    //    e.Graphics.FillRectangle(_backBrush, e.AffectedBounds);
        //    //}
        //    //else
        //    //{
        //    //    base.OnRenderToolStripBackground(e);
        //    //}
        //}

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            base.OnRenderToolStripBorder(e);
            ControlPaint.DrawBorder(e.Graphics, e.AffectedBounds, SystemColors.ControlDarkDark, ButtonBorderStyle.Solid);
        }

        //protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        //{
        //    if (_highlightBrush == null)
        //    {
        //        _highlightBrush = new SolidBrush(SystemColors.Highlight);
        //    }
        //    if (e.Item.Selected)
        //    {
        //        ControlPaint.DrawBorder(e.Graphics, e.Item.Bounds, SystemColors.ControlDarkDark, ButtonBorderStyle.Solid);
        //        //e.Graphics.FillRectangle(_highlightBrush, e.Item.Bounds);
        //    }
        //}

    }
}
