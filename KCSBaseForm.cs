using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KCS.Common.Controls
{
    public partial class KCSBaseForm : KCSBaseMDIForm
    {
        private System.Diagnostics.Stopwatch _stopWatch { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public KCSBaseForm()
        {
            InitializeComponent();

            _stopWatch = new System.Diagnostics.Stopwatch();
        }

        /// <summary>
        /// Start stopwatch.
        /// </summary>
        protected void StopWatchRestart()
        {
            _stopWatch.Reset();
            _stopWatch.Start();
        }

        /// <summary>
        /// Stop stopwatch.
        /// </summary>
        /// <returns></returns>
        protected TimeSpan StopWatchStop()
        {
            _stopWatch.Stop();
            return _stopWatch.Elapsed;
        }

        protected override void SetBusy(bool busy)
        {
            pnlMain.Enabled = !busy;
            base.SetBusy(busy);            
        }

        protected override void SetBusy(bool busy, string message)
        {
            pnlMain.Enabled = !busy;
            base.SetBusy(busy, message);            
        }
    }
}
