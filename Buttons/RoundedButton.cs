using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Drawing.Drawing2D;

namespace KCS.Common.Controls
{
    public class RoundedButton : KCSButton
    {
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            GraphicsPath buttonPath = new GraphicsPath();

            // Set a new rectangle to the same size as the button's 
            // ClientRectangle property.
            System.Drawing.Rectangle newRectangle = ClientRectangle;

            // Decrease the size of the rectangle.
            newRectangle.Inflate(-10, -10);

            // Draw the button's border.
            e.Graphics.DrawEllipse(System.Drawing.Pens.Black, newRectangle);

            // Increase the size of the rectangle to include the border.
            newRectangle.Inflate(1, 1);

            // Create a circle within the new rectangle.
            buttonPath.AddEllipse(newRectangle);

            // Set the button's Region property to the newly created 
            // circle region.
            Region = new System.Drawing.Region(buttonPath);
        }
    }
}
