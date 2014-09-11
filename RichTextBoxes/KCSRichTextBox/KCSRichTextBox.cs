using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using KCS.Common.Shared;

namespace KCS.Common.Controls
{
	/// <summary>
	/// Supports color-syntax highliting.
	/// </summary>
	public class KCSRichTextBox : System.Windows.Forms.RichTextBox
	{
		/// <summary>
		/// Helper class.
		/// </summary>
		private class HighlitingDefinition
		{
			public string Name { get; set; }
			public IEnumerable<string> Tokens {get; private set;}
			public bool UseWordBoundary {get; private set;}
			public Color Color {get; private set;}
			public Regex Regex {get; private set;}

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="tokens"></param>
			/// <param name="delimiter"></param>
			/// <param name="useWordBoundary"></param>
			/// <param name="color"></param>
			public HighlitingDefinition(string tokens, char delimiter, bool useWordBoundary, Color color)
			{
				this.Tokens = tokens.Split(new char[]{delimiter}, StringSplitOptions.RemoveEmptyEntries);
				this.UseWordBoundary = useWordBoundary;
				this.Color = color;

				// Get the words ready
				string[] array = new string[Tokens.Count()];
				string item;
				for (int i = 0; i < array.Length; i++)
				{
					item = Tokens.ElementAt(i).Replace("\r", "");
					item = item.Replace("\n", "");
					item = item.Replace("\r\n", "");
					array[i] = UseWordBoundary ? string.Format("\\b{0}\\b", item) : item;
				}
				this.Regex = new Regex("@|" + string.Join("|", array), RegexOptions.IgnoreCase);
			}
		}

		private Color _defaultTextColor = Color.FromKnownColor(KnownColor.WindowText);
		private List<HighlitingDefinition> _definitions = new List<HighlitingDefinition>(1);
		private Timer tmr;
		private IContainer components;
		private bool _hasTextChanged = false;
        private string _language = string.Empty;

		/// <summary>
		/// Raised just before highliting starts.
		/// </summary>
		public event EventHandler SyntaxHighliting;

		/// <summary>
		/// Raised just after highliting finishes.
		/// </summary>
		public event EventHandler SyntaxHighlited;

		#region Properties
		/// <summary>
		/// Enables or disables color syntax highlighting.
		/// </summary>
		[
			Description("Enables or disables color syntax highlighting."),
			DefaultValue(true),
			Category("Behavior")
		]
		public bool EnableSyntaxHighliting
		{
			get;
			set;
		}

        public string Language
        {
            get { return _language; }
            set
            {
                // Attempt to load the appropriate language
            }
        }
		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public KCSRichTextBox()
		{
			InitializeComponent();
			EnableSyntaxHighliting = true;
		}

		/// <summary>
		/// Adds a syntax highlighting definition.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="tokens"></param>
		/// <param name="delimiter"></param>
		/// <param name="useWordBoundary"></param>
		/// <param name="color"></param>
		public void AddDefinition(string name, string tokens, char delimiter, bool useWordBoundary, Color color)
		{
			var hd = new HighlitingDefinition(tokens, delimiter, useWordBoundary, color);
			hd.Name = name;
			_definitions.Add(hd);
		}

		/// <summary>
		/// Enable the interrnal timer.
		/// </summary>
		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			tmr.Enabled = true;
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			tmr.Enabled = false;
			base.OnHandleDestroyed(e);
		}

		private void tmr_Tick(object sender, EventArgs e)
		{
			int systemUptime = Environment.TickCount;		// Get the system uptime
			int lastInputTicks = 0;							// The tick at which the last input was recorded
			int idleTicks = 0;								// The number of ticks that passed since last input
			int idleSeconds = 0;							// Idle time, in seconds

			// Set the struct
			Win32API.User32.LASTINPUTINFO lpi = new Win32API.User32.LASTINPUTINFO();
			lpi.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lpi);
			lpi.dwTime = 0;

			// If we have a value from the function
			if (Win32API.User32.GetLastInputInfo(ref lpi))
			{
				lastInputTicks = (int)lpi.dwTime;			// Get the number of ticks at the point when the last activity was seen				
				idleTicks = systemUptime - lastInputTicks;	// Number of idle ticks = system uptime ticks - number of ticks at last input
				idleSeconds = idleTicks / 1000;				// Get the number of seconds there's been no input

				if (_hasTextChanged && idleSeconds > 1)
				{
					tmr.Enabled = false;
					ApplySyntaxHighliting();
					tmr.Enabled = true;
					_hasTextChanged = false;
				}
			}
		}

		/// <summary>
		/// When the text is changed, set a flag indicating so.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnTextChanged(EventArgs e)
		{	
			base.OnTextChanged(e);
			_hasTextChanged = true;
		}

		/// <summary>
		/// Clears the syntaxt highlighting.
		/// </summary>
		public void ClearSyntaxHighliting()
		{
			SelectAll();
			SelectionColor = _defaultTextColor;
			DeselectAll();
		}

		public void ApplySyntaxHighliting()
		{
			if (!EnableSyntaxHighliting || IsDisposed)
				return;

			try
			{
				OnHighliting();

				Win32API.User32.LockWindowUpdate(Handle);

				// Get the last cursor position in the RichTextBox			
				int selPos = SelectionStart;

				ClearSyntaxHighliting();

				foreach (HighlitingDefinition hd in _definitions)
				{
					foreach (Match m in hd.Regex.Matches(Text))
					{
						Select(m.Index, m.Length);
						SelectionColor = hd.Color;
					}
				}
				SelectionStart = selPos;
				DeselectAll();
			}
			finally
			{
				Win32API.User32.LockWindowUpdate(IntPtr.Zero);
				OnHighlited();
			}
		}

		/// <summary>
		/// Raises the Highliting event.
		/// </summary>
		protected virtual void OnHighliting()
		{
			if (SyntaxHighliting != null)
			{
				SyntaxHighliting(this, new EventArgs());
			}
		}

		/// <summary>
		/// Raises the Highlited event.
		/// </summary>
		protected virtual void OnHighlited()
		{
			if (SyntaxHighlited != null)
			{
				SyntaxHighlited(this, new EventArgs());
			}
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tmr = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// tmr
			// 
			this.tmr.Interval = 500;
			this.tmr.Tick += new System.EventHandler(this.tmr_Tick);
			this.ResumeLayout(false);

		}		
	}
}
