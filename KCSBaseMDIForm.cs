using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KCS.Common.Controls
{
    public partial class KCSBaseMDIForm : Form
    {
        private bool _showStatusStrip;
        private bool _hasPainted;
        protected delegate void SetStatusMessageDelegate(string message);
        protected delegate string GetStatusMessageDelegate();
        protected event EventHandler FormShown;

        [Browsable(false)]
        public bool IsBusy { get; protected set; }

        [DefaultValue(true), Category("Behavior"), Description("Disable the main menu when the form is busy.")]
        public bool DisableMenuWhenBusy
        {
            get;
            set;
        }

        [DefaultValue(true), Category("Appearance")]
        public bool ShowStatusStrip
        {
            get { return _showStatusStrip; }
            set
            {
                //if (this.statusStrip.Visible != value)
                {
                    this.statusStrip.Visible = value;
                    _showStatusStrip = value;
                }
            }
        }

        [Category("Appearance")]
        public virtual string StatusMessage
        {
            get { return GetStatusMessage(); }
            set
            {
                SetStatusMessage(value);
            }
        }

        private string GetStatusMessage()
        {
            if (this.InvokeRequired)
            {
                var d = new GetStatusMessageDelegate(GetStatusMessage);
                return Shared.Strings.ConvertToString(this.Invoke(d));
            }
            else
            {
                return lblStatus.Text;
            }
        }

        private void SetStatusMessage(string message)
        {
            if (this.InvokeRequired)
            {
                var d = new SetStatusMessageDelegate(SetStatusMessage);
                this.Invoke(d, new object[] { message });
            }
            else
            {
                lblStatus.Text = string.IsNullOrEmpty(message) ? "Ready" : message;
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public KCSBaseMDIForm()
        {
            InitializeComponent();
            Application.Idle += new EventHandler(ApplicationIdle);
            _showStatusStrip = true;
            DisableMenuWhenBusy = true;
        }

        protected override void DestroyHandle()
        {
            Application.Idle -= new EventHandler(ApplicationIdle);
            base.DestroyHandle();
        }

        /// <summary>
        /// Idle processing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ApplicationIdle(object sender, EventArgs e)
        {
            if (tsProgressBar != null)
            {
                tsProgressBar.Visible = IsBusy;
            }
        }

        /// <summary>
        /// Raised when the form paints for the first time.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!_hasPainted)
            {
                _hasPainted = true;
                OnFormShown(e);
            }
        }

        /// <summary>
        /// This is called ONLY the first time the form is painted.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFormShown(EventArgs e)
        {
            if (FormShown != null)
            {
                FormShown(this, e);
            }
        }

        public DialogResult ShowMessage(string message, MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            return MessageBox.Show(this, message, "Message", buttons, icon);
        }

        /// <summary>
        /// Shows a non-blocking dialog window.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="icon"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public DialogResult ShowNonModalMessage(string message, MessageBoxIcon icon = MessageBoxIcon.Information, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            return MessageBox.Show(this, message, "Message", buttons, icon);
        }

        protected virtual void SetBusy(bool busy)
        {
            SetBusy(busy, string.Empty);
        }

        protected virtual void SetBusy(bool busy, string message)
        {            
            if (DisableMenuWhenBusy && this.MainMenuStrip != null)
            {
                this.MainMenuStrip.Enabled = !busy;
            }
            tsProgressBar.Visible = busy;
            StatusMessage = message;
            System.Diagnostics.Debug.WriteLine(message);
            IsBusy = busy;
            this.UseWaitCursor = busy;
            Application.DoEvents();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (IsBusy)
            {
                MessageBox.Show(this, "The application is busy. Please wait some moments and try again...", "Application Busy", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                e.Cancel = true;
                return;
            }
            else
            {
                base.OnClosing(e);
            }
        }
    }
}
