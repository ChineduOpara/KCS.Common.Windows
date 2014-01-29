using System;
using System.ComponentModel;
using System.Threading;
using System.Globalization;
using System.Windows.Forms;
using System.Text;

namespace KCS.Common.Controls
{
    /// <summary>
    /// Textbox that allows only numeric values.
    /// </summary>
    public class NumericTextBox : KCSTextBox
    {
        private string _previousText;
        private static NumberFormatInfo _nfi = System.Threading.Thread.CurrentThread.CurrentUICulture.NumberFormat;
        private static string[] _firstChars = new string[] { _nfi.NumberDecimalSeparator, _nfi.NegativeSign, string.Empty };

        #region Value properties
        /// <summary>
        /// Contains the Integer representation of the value.
        /// </summary>
        [Browsable(false)]
        public int IntValue
        {
            get
            {
                try
                {
                    return Convert.ToInt32(Text);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                Text = value.ToString();
            }
        }

        /// <summary>
        /// Contains the Short representation of the value.
        /// </summary>
        [Browsable(false)]
        public short ShortValue
        {
            get
            {
                try
                {
                    return Convert.ToInt16(Text);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                Text = value.ToString();
            }
        }

        /// <summary>
        /// Contains the Double representation of the value.
        /// </summary>
        [Browsable(false)]
        public double DoubleValue
        {
            get
            {
                try
                {
                    return Convert.ToDouble(Text);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                Text = value.ToString();
            }
        }

        /// <summary>
        /// Contains the Decimal representation of the value.
        /// </summary>
        [Browsable(false)]
        public decimal DecimalValue
        {
            get
            {
                try
                {
                    return Convert.ToDecimal(Text);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                Text = value.ToString();
            }
        }
        #endregion

        /// <summary>
        /// Indicates that the Text value has changed.
        /// </summary>
        [Description("Indicates that the Text value has changed."), Category("Data"), DefaultValue(false)]
        public bool IsTextChanged { get; set; }

        /// <summary>
        /// Indicates that the control allows negative values.
        /// </summary>
        [Description("Indicates that the toolStripItem allows negative values."), Category("Behavior"), DefaultValue(true)]
        public bool AllowNegatives { get; set; }

        /// <summary>
        /// Indicates that the control allows decimal values.
        /// </summary>
        [Description("Indicates that the toolStripItem allows decimal values."),Category("Behavior"),DefaultValue(true)]
        public bool AllowDecimals { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public NumericTextBox()
        {
            AllowNegatives = true;
            AllowDecimals = true;

            MaxLength = 9;
            IsTextChanged = false;
        }

        /// <summary>
        /// Checks to see if the value's sign is allowed.
        /// </summary>
        /// <param name="value">Value whose sign will be checked.</param>
        /// <returns>TRUE if the value's sign is allowed.</returns>
        private bool ValueIsAllowed(double value)
        {
            return AllowNegatives || (value >= 0);
        }

        /// <summary>
        /// Attempts to convert the value entered to an integer.
        /// </summary>
        /// <returns>TRUE if the value was successfuly converted to an Integer.</returns>
        private bool ConvertToIntegral()
        {
            long result;
            if (long.TryParse(Text, NumberStyles.Any, _nfi, out result))
            {
                Text = Convert.ToInt64(result).ToString();
                return true;
            }
            else
            {
                IntValue = 0;
                return false;
            }
        }

        /// <summary>
        /// Raised when the Text changes. This method applies any necessary conversions based on the properties. If the
        /// conversions failed, the original text is restored.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnTextChanged(EventArgs e)
        {
            double result;
            IsTextChanged = false;
            if (Double.TryParse(Text, NumberStyles.Any, _nfi, out result))
            {
                if (ValueIsAllowed(result) && (AllowDecimals || (!AllowDecimals && ConvertToIntegral())) && string.Compare(_previousText, Text, true) != 0)
                {
                    _previousText = Text;
                    IsTextChanged = true;
                    base.OnTextChanged(e);
                }
            }
            else
            {
                if (Array.IndexOf(_firstChars, Text) == -1)
                {
                    int selStart = SelectionStart;
                    Text = _previousText;
                    SelectionStart = selStart;
                }
                else
                {
                    if ((AllowDecimals || (!AllowDecimals && ConvertToIntegral())) && string.Compare(_previousText, Text, true) != 0)
                    {
                        _previousText = Text;
                        IsTextChanged = true;
                        base.OnTextChanged(e);                        
                    }
                }
            }
        }
    }
}
