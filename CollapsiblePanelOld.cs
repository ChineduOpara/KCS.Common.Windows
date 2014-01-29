using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OVT.CustomControls
{
    
    public class CollapsiblePanelOld : System.Windows.Forms.Panel
    {

        public CollapsiblePanelOld()
        {
            InitializeComponent();
            headerFont = new Font(Font, FontStyle.Bold);
            headerTextColor = Color.Black;
        }

        private bool headerTextAutoEllipsis;

        public bool HeaderTextAutoEllipsis
        {
            get { return headerTextAutoEllipsis; }
            set
            {
                headerTextAutoEllipsis = value;
                Refresh();
            }
        }

        private string headerText;

        public string HeaderText
        {
            get { return headerText; }
            set 
            { 
                headerText = value;
                Refresh();
            }
        }

        private Color headerTextColor;

        public Color HeaderTextColor
        {
            get { return headerTextColor; }
            set
            {
                headerTextColor = value;
                Refresh();
            }
        }


        private Image headerImage;

        public Image HeaderImage
        {
            get { return headerImage; }
            set
            {
                headerImage = value;
                Refresh();
            }
        }


        private Font headerFont;

        public Font HeaderFont
        {
            get { return headerFont; }
            set
            {
                headerFont = value;
                Refresh();
            }
        }



        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            // Draw panel's header.
            int headerRectHeight = e.ClipRectangle.Height / 3;
            if (headerRectHeight > 40)
                headerRectHeight = 40;
            if (headerRectHeight < 30)
                headerRectHeight = 30;

            Rectangle headerRect = new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width,
                headerRectHeight);

            LinearGradientBrush headerBrush = new LinearGradientBrush(
                headerRect.Location,
                new Point(headerRect.X+headerRect.Width, headerRect.Y+headerRect.Height),
                Color.Snow, Color.LightBlue);

            e.Graphics.FillRectangle(headerBrush, headerRect);

            // Draw header image.
            if (headerImage != null)
            {
                Size headerImageSize = new Size(headerRectHeight,headerRectHeight);
                Rectangle imageRect = new Rectangle(headerRect.Location, headerImageSize);
                e.Graphics.DrawImage(headerImage, imageRect);
            }


            // Calculate header string position.
            if (!String.IsNullOrEmpty(headerText))
            {
                int delta = 0;
                if (headerImage != null)
                {
                    delta = headerRectHeight;
                }
                PointF headerTextPosition = new PointF();
                Size headerTextSize = TextRenderer.MeasureText(headerText, headerFont);

                if (headerTextAutoEllipsis)
                {
                    if (headerTextSize.Width >= headerRect.Width - delta)
                    {
                        RectangleF rectLayout =
                            new RectangleF((float)headerRect.X + delta, (float)(headerRect.Height - headerTextSize.Height) / 2, (float)headerRect.Width - delta, (float)headerTextSize.Height);
                        StringFormat format = new StringFormat();
                        format.Trimming = StringTrimming.EllipsisWord;
                        e.Graphics.DrawString(headerText, headerFont, new SolidBrush(headerTextColor),
                            rectLayout, format);

                    }
                    else
                    {
                        headerTextPosition.X = (delta + headerRect.Width - headerTextSize.Width) / 2;
                        headerTextPosition.Y = (headerRect.Height - headerTextSize.Height) / 2;
                        e.Graphics.DrawString(headerText, headerFont, new SolidBrush(headerTextColor),
                            headerTextPosition);
                    }
                }
                else
                {
                    headerTextPosition.X = (delta + headerRect.Width - headerTextSize.Width) / 2;
                    headerTextPosition.Y = (headerRect.Height - headerTextSize.Height) / 2;
                    e.Graphics.DrawString(headerText, headerFont, new SolidBrush(headerTextColor),
                        headerTextPosition);
                }
            }

            // Draw right button.
            
        }


        

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }

    

    }
}
