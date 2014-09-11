using System;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Windows.Forms.Design;
using System.ComponentModel;
using KCS.Common.Shared;

namespace KCS.Common.Controls
{
    /// <summary>
    /// Textbox that allows only numeric values.
    /// </summary>
    [DefaultEvent("TextChanged")]
    public class NumericToolStripTextBox : KCSToolStripTextBox
    {
        private string _PreviousText;
        private static NumberFormatInfo _nfi = Thread.CurrentThread.CurrentUICulture.NumberFormat;
        private static string[] _FirstChars = new string[] { _nfi.NumberDecimalSeparator, _nfi.NegativeSign, string.Empty };

        public enum ValueTypeEnum
        {
            Short,
            Integer,
            Long,
            Double
        }

        [Description("Gets or sets the type of value being held."), Category("Data"), DefaultValue(ValueTypeEnum.Integer)]
        public ValueTypeEnum ValueType { get; set; }

        #region Value Limits
        [Description("Minimum long value."), Category("Data Limits"), DefaultValue(long.MinValue)]
        public long LongMinValue { get; set; }
        [Description("Maximum long value."), Category("Data Limits"), DefaultValue(long.MaxValue)]
        public long LongMaxValue { get; set; }

        [Description("Minimum int value."), Category("Data Limits"), DefaultValue(int.MinValue)]
        public int IntMinValue { get; set; }
        [Description("Maximum int value."), Category("Data Limits"), DefaultValue(int.MaxValue)]
        public int IntMaxValue { get; set; }

        [Description("Minimum short value."), Category("Data Limits"), DefaultValue(short.MinValue)]
        public short ShortMinValue { get; set; }
        [Description("Maximum short value."), Category("Data Limits"), DefaultValue(short.MaxValue)]
        public short ShortMaxValue { get; set; }

        [Description("Minimum double value."), Category("Data Limits"), DefaultValue(double.MinValue)]
        public double DoubleMinValue { get; set; }
        [Description("Maximum double value."), Category("Data Limits"), DefaultValue(double.MaxValue)]
        public double DoubleMaxValue { get; set; }
        #endregion

        #region Value accessors
        /// <summary>
        /// Contains the Long representation of the value.
        /// </summary>
        [Browsable(true), DefaultValue(0L)]
        public long LongValue
        {
            get
            {
                return Utility.GetInt64(Text);
            }
            set
            {
                if (value < LongMinValue)
                {
                    value = LongMinValue;
                }
                if (value > LongMaxValue)
                {
                    value = LongMaxValue;
                }
                Text = value.ToString();
            }
        }

        /// <summary>
        /// Contains the Integer representation of the value.
        /// </summary>
        [Browsable(true), DefaultValue(0)]
        public int IntValue
        {
            get
            {
                return Utility.GetInt32(Text);
            }
            set
            {
                if (value < IntMinValue)
                {
                    value = IntMinValue;
                }
                if (value > IntMaxValue)
                {
                    value = IntMaxValue;
                }
                Text = value.ToString();
            }
        }

        /// <summary>
        /// Contains the Short representation of the value.
        /// </summary>
        [Browsable(true), DefaultValue((short)0)]
        public short ShortValue
        {
            get
            {
                return Utility.GetInt16(Text);
            }
            set
            {
                if (value < ShortMinValue)
                {
                    value = ShortMinValue;
                }
                if (value > ShortMaxValue)
                {
                    value = ShortMaxValue;
                }
                Text = value.ToString();
            }
        }

        /// <summary>
        /// Contains the Double representation of the value.
        /// </summary>
        [Browsable(true), DefaultValue(0.0)]
        public double DoubleValue
        {
            get
            {
                return Utility.GetDoubleValue(Text);
            }
            set
            {
                if (value < DoubleMinValue)
                {
                    value = DoubleMinValue;
                }
                if (value > DoubleMaxValue)
                {
                    value = DoubleMaxValue;
                }
                Text = value.ToString();
            }
        }
        #endregion

        /// <summary>
        /// Indicates that the Text value has changed.
        /// </summary>
        [Description("Indicates that the Text value has changed."),Category("Data"), DefaultValue(false)]
        public bool IsTextChanged { get; set; }

        /// <summary>
        /// Indicates that the control allows negative values.
        /// </summary>
        [Description("Indicates that the toolStripItem allows negative values."), Category("Behavior"), DefaultValue(true)]
        public bool AllowNegatives { get; set; }

        /// <summary>
        /// Indicates that the control allows decimal values.
        /// </summary>
        [Description("Indicates that the toolStripItem allows decimal values."), Category("Behavior"), DefaultValue(true)]
        public bool AllowDecimals { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public NumericToolStripTextBox()
        {
            this.ValueType = ValueTypeEnum.Integer;
            #region Min and Max
            LongMinValue = long.MinValue;
            LongMaxValue = long.MaxValue;

            IntMinValue = int.MinValue;
            IntMaxValue = int.MaxValue;

            ShortMinValue = short.MinValue;
            ShortMaxValue = short.MaxValue;

            DoubleMinValue = double.MinValue;
            DoubleMaxValue = double.MaxValue;
            #endregion

            AllowNegatives = true;
            AllowDecimals = true;

            MaxLength = 9;
            IsTextChanged = false;

            this.Leave += new EventHandler(NumericToolStripTextBox_Leave);
        }

        private void NumericToolStripTextBox_Leave(object sender, EventArgs e)
        {
            SetValue();
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
        //protected override void OnTextChanged(EventArgs e)
        protected void SetValue()
        {
            double result;
            IsTextChanged = false;
            if (Double.TryParse(Text, NumberStyles.Any, _nfi, out result))
            {
                if (ValueIsAllowed(result) && (AllowDecimals || (!AllowDecimals && ConvertToIntegral())) && string.Compare(_PreviousText, Text, true) != 0)
                {
                    _PreviousText = Text;
                    SetValueFromText();
                    IsTextChanged = true;                    
                    //base.OnTextChanged(e);
                }
            }
            else
            {
                if (Array.IndexOf(_FirstChars, Text) == -1)
                {
                    int selStart = SelectionStart;
                    Text = _PreviousText;
                    SelectionStart = selStart;
                }
                else
                {
                    if ((AllowDecimals || (!AllowDecimals && ConvertToIntegral())) && string.Compare(_PreviousText, Text, true) != 0)
                    {
                        _PreviousText = Text;
                        SetValueFromText();
                        IsTextChanged = true;
                        //base.OnTextChanged(e);                        
                    }
                }
            }
        }

        private void SetValueFromText()
        {
            switch(ValueType)
            {
                case ValueTypeEnum.Long: LongValue = Utility.GetInt64(Text); break;
                case ValueTypeEnum.Integer: IntValue = Utility.GetInt32(Text); break;
                case ValueTypeEnum.Short: ShortValue = Utility.GetInt16(Text); break;
                case ValueTypeEnum.Double: DoubleValue = Utility.GetDoubleValue(Text); break;
            }
        }
    }
}
