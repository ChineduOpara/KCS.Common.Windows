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

        //[
        //    Category("Data"),
        //    Description("Gets or sets the property to display.")
        //]
        //public override string DisplayMember
        //{
        //    get { return base.ComboBox.DisplayMember; }
        //    set { base.ComboBox.DisplayMember = value; }
        //}

        //[
        //    Category("Data"),
        //    Description("Gets or sets the property to use as the actual value.")
        //]
        //public override string ValueMember
        //{
        //    get { return base.ComboBox.ValueMember; }
        //    set { base.ComboBox.ValueMember = value; }
        //}

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
            var vt = new Shared.ValuesTracker(_uniqueName);
            vt.Load();
            var history = vt.GetValue<List<string>>(ValueKey, new List<string>());
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
                var vt = new Shared.ValuesTracker(_uniqueName);
                var history = new List<string>(Items.Count);
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

                // We can't use Contains, because it is case-sensitive. So do an old-school loop.
                foreach (string item in Items)
                {
                    found = string.Compare(item.Trim(), txt, true) == 0;
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
