using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using KCS.Common.Shared;

namespace KCS.Common.Controls
{
    /// <summary>
    /// Extends the ToolStripComboBox, allowing control of the SelectedIndexChanged event.
    /// </summary>
    public class HistoryToolStripComboBox : ToolStripComboBox
    {
        private bool _readOnly;
        private string _uniqueName;
        private const string ValueKey = "History";

        /// <summary>
        /// If TRUE, the control does not allow selection.
        /// </summary>
        [
            Category("Behavior"),
            DefaultValue(false),
            Description("If TRUE, the toolStripItem does not allow selection.")
        ]
        public bool ReadOnly
        {
            get
            {
                return _readOnly;
            }
            set
            {
                _readOnly = value;
                base.Enabled = !value;
            }
        }

        /// <summary>
        /// If set, the ENTER key attempts to add the current text, if any, to the history list.
        /// </summary>
        [
            Description("If set, the ENTER key attempts to add the current text, if any, to the history list."),
            Category("Behavior"),
            DefaultValue(false)
        ]
        public bool AutoAddTextOnEnterKey
        {
            get;
            set;
        }

        /// <summary>
        /// If TRUE, the control raises the SelectedIndexChanged event.
        /// </summary>
        [
            Category("Behavior"),
            DefaultValue(true),
            Description("If TRUE, the toolStripItem raises the SelectedIndexChanged event.")
        ]
        public bool RaiseSelectedIndexChangedEvent { get; set; }

        [
            Category("Data"),
            Description("Gets or sets the property to display.")
        ]
        public string DisplayMember
        {
            get { return base.ComboBox.DisplayMember; }
            set { base.ComboBox.DisplayMember = value; }
        }

        [
            Category("Data"),
            Description("Gets or sets the property to use as the actual value.")
        ]
        public string ValueMember
        {
            get { return base.ComboBox.ValueMember; }
            set { base.ComboBox.ValueMember = value; }
        }

        /// <summary>
        /// Constructor. Sets some default property values.
        /// </summary>
        public HistoryToolStripComboBox()
        {
            ComboBox.HandleCreated += new EventHandler(ComboBox_HandleCreated);
            ComboBox.HandleDestroyed += new EventHandler(ComboBox_HandleDestroyed);

            RaiseSelectedIndexChangedEvent = true;

            DropDownStyle = ComboBoxStyle.DropDown;
            RaiseSelectedIndexChangedEvent = true;
            ReadOnly = false;
            AutoCompleteMode = AutoCompleteMode.Suggest;
            AutoCompleteSource = AutoCompleteSource.ListItems;
            AutoAddTextOnEnterKey = false;            
        }        

        /// <summary>
        /// When the handle is created, get the history from isolated storage.
        /// </summary>
        /// <param name="e"></param>
        private void ComboBox_HandleCreated(object sender, EventArgs e)
        {
            if (DesignMode)
            {
                return;
            }

            _uniqueName = this.GetFullyQualifiedName();
            ValuesTracker vt = new ValuesTracker(_uniqueName);
            vt.Load();
            List<string> history = vt.GetValue<List<string>>(ValueKey, new List<string>());
            history = history.Distinct().ToList();

            Items.AddRange(history.ToArray());
        }

        /// <summary>
        /// When the control is destroyed, save the items to isolated storage.
        /// </summary>
        /// <param name="e"></param>
        private void ComboBox_HandleDestroyed(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                ValuesTracker vt = new ValuesTracker(_uniqueName);
                List<string> history = new List<string>(Items.Count);
                history.AddRange(Items.Cast<string>().Distinct());
                vt.AddValue<List<string>>(ValueKey, history);
                vt.Save();
            }
        }

        /// <summary>
        /// Raised when the selection is changed. The event is only bubbled up if RaiseSelectedIndexChangedEvent is TRUE.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (RaiseSelectedIndexChangedEvent)
            {
                base.OnSelectedIndexChanged(e);
            }
        }

        /// <summary>
        /// Adds the current Text to the Items collection.
        /// </summary>
        public void Add()
        {
            Add(this.Text);
        }

        /// <summary>
        /// Adds the given Text to the Items collection.
        /// </summary>
        /// <param name="text">Text to add.</param>
        public void Add(string text)
        {
            bool found = false;
            if (!string.IsNullOrEmpty(Text.Trim()))
            {
                string txt = Text.Trim();

                // We can't use Contains, because it is case-sensitive. So do a hardcore loop.
                foreach (string item in Items)
                {
                    found = string.Compare(item, txt, true) == 0;
                    if (found)
                    {
                        break;
                    }
                }
                if (!found)
                {
                    Items.Insert(0, txt);
                }
            }
        }

        /// <summary>
        /// The ENTER key attempts to add the current Text (if any) to the history list.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (AutoAddTextOnEnterKey && e.KeyCode == Keys.Enter)
            {
                Add(Text);
            }
            base.OnKeyDown(e);
        }
    }
}
