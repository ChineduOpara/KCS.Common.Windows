namespace KCS.Common.Controls
{
    partial class AsyncProgressBar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AsyncProgressBar));
            this.lblHeading = new System.Windows.Forms.Label();
            this.pnl = new System.Windows.Forms.Panel();
            this.lblSubtext = new System.Windows.Forms.Label();
            this.neProgressBar = new KCS.Common.Controls.AsyncProgressBarControl();
            this.pnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHeading
            // 
            this.lblHeading.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.Location = new System.Drawing.Point(0, 0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(330, 38);
            this.lblHeading.TabIndex = 1;
            this.lblHeading.Text = "Processing...";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnl
            // 
            this.pnl.BackColor = System.Drawing.Color.Transparent;
            this.pnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl.Controls.Add(this.lblSubtext);
            this.pnl.Controls.Add(this.neProgressBar);
            this.pnl.Controls.Add(this.lblHeading);
            this.pnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl.Location = new System.Drawing.Point(0, 0);
            this.pnl.Name = "pnl";
            this.pnl.Size = new System.Drawing.Size(332, 94);
            this.pnl.TabIndex = 3;
            // 
            // lblSubtext
            // 
            this.lblSubtext.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblSubtext.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtext.Location = new System.Drawing.Point(0, 64);
            this.lblSubtext.Name = "lblSubtext";
            this.lblSubtext.Size = new System.Drawing.Size(330, 28);
            this.lblSubtext.TabIndex = 3;
            this.lblSubtext.Text = "Please Wait";
            this.lblSubtext.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // neProgressBar
            // 
            this.neProgressBar.AutoProgress = true;
            this.neProgressBar.AutoProgressSpeed = ((byte)(70));
            this.neProgressBar.BackColor = System.Drawing.Color.Transparent;
            this.neProgressBar.IndicatorColor = System.Drawing.Color.Red;
            this.neProgressBar.Location = new System.Drawing.Point(17, 41);
            this.neProgressBar.Name = "neProgressBar";
            this.neProgressBar.NormalImage = ((System.Drawing.Image)(resources.GetObject("neProgressBar.NormalImage")));
            this.neProgressBar.PointImage = ((System.Drawing.Image)(resources.GetObject("neProgressBar.PointImage")));
            this.neProgressBar.Position = 7;
            this.neProgressBar.ProgressBoxStyle = KCS.Common.Controls.AsyncProgressBarControl.OSProgressBoxStyle.SolidSameSize;
            this.neProgressBar.ProgressStyle = KCS.Common.Controls.AsyncProgressBarControl.OSProgressStyle.LeftOrRight;
            this.neProgressBar.ProgressType = KCS.Common.Controls.AsyncProgressBarControl.OSProgressType.Graphic;
            this.neProgressBar.ShowBorder = false;
            this.neProgressBar.Size = new System.Drawing.Size(288, 20);
            this.neProgressBar.TabIndex = 2;
            // 
            // AsyncProgressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(332, 94);
            this.ControlBox = false;
            this.Controls.Add(this.pnl);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AsyncProgressBar";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "progressBar";
            this.pnl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Panel pnl;
        private AsyncProgressBarControl neProgressBar;
        private System.Windows.Forms.Label lblSubtext;
    }
}