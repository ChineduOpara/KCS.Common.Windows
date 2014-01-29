using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;

namespace KCS.Common.Controls
{
    public class KCSTreeNode : System.Windows.Forms.TreeNode
	{
		/// <summary>
		/// Is the node meant to be enabled or disabled?
		/// </summary>
		[
			DefaultValue(true),
			Category("Behavior"),
			Description("Is the node meant to be enabled or disabled?")
		]
		public bool Enabled { get; set; }

        /// <summary>
        /// Is the node checked?
        /// </summary>
        [
            DefaultValue(true),
            Category("Appearance"),
            Description("Can node be checked/unchecked?")
        ]
        public bool Checkbox { get; set; }

		/// <summary>
		/// Unique ID for the node.
		/// </summary>
		[
			DefaultValue(0),
			Category("Data"),
			Description("Unique ID for the node.")
		]
		public int UniqueID { get; set; }

		public KCSTreeNode() : base()
		{
			UniqueID = 0;
			Enabled = true;
            Checkbox = true;
		}

        public KCSTreeNode(string text) : this()
        {
            base.Text = text;
        }
	}
}
