using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using KCS.Common.Shared;

namespace KCS.Common.Controls
{
	/// <summary>
	/// This comboxbox preserves the history if items typed into it (after user presses enter).
	/// It uses an instance of the ValuesTracker component (in Standalone mode).
	/// </summary>
	public class HistoryComboBox : KCSComboBox
	{
		private string _uniqueName;
		private const string ValueKey = "History";

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
        /// Constructor.
        /// </summary>
		public HistoryComboBox() : base()
        {
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
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			if (DesignMode)
			{
				return;
			}

			_uniqueName = this.GetFullyQualifiedName();
			ValuesTracker vt = new ValuesTracker(_uniqueName);
			vt.Load();
			List<string> history = vt.GetValue<List<string>>(ValueKey, new List<string>());

			Items.AddRange(history.ToArray());
		}

		/// <summary>
		/// When the control is destroyed, save the items to isolated storage.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (!DesignMode)
			{
				ValuesTracker vt = new ValuesTracker(_uniqueName);
				List<string> history = new List<string>(Items.Count);
				history.AddRange(Items.Cast<string>());
				vt.AddValue<List<string>>(ValueKey, history);
				vt.Save();
			}
			base.OnHandleDestroyed(e);
		}

		/// <summary>
		/// Adds the given Text to the Items collection.
		/// </summary>
		/// <param name="text"></param>
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
