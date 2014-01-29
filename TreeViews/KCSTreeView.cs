using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace KCS.Common.Controls
{
    /// <summary>
    /// Base class for specialized treeview controls. This class can control the treeview width based on all expanded nodes width.
    /// </summary>
    public class KCSTreeView : System.Windows.Forms.TreeView
    {
        #region Fields
            //bool _IsCopyEnabled;
            //int _ExpandedTreeWidth=0;
        #endregion

        #region Properties
        /// <summary>
        /// Verifies if any nodes have been checked
        /// </summary>
        public bool HasCheckedNodes
        {
            get { return Shared.ControlsExtensions.IsAnyNodeChecked(this); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Still working on it.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawNode(System.Windows.Forms.DrawTreeNodeEventArgs e)
        {            
            if (!(e.Node is KCSTreeNode) || !CheckBoxes)
            {
                e.DrawDefault = true;
                base.OnDrawNode(e);
                return;
            }

            var tn = (KCSTreeNode)e.Node;
            if (tn.Checkbox)
            {
                e.DrawDefault = true;
                base.OnDrawNode(e);
            }
            else
            {
                // Get size of the checkbox button
                var glyphSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, CheckBoxState.UncheckedNormal);

                var brush = new SolidBrush(this.ForeColor);
                e.Graphics.DrawString(tn.Text, this.Font, brush, glyphSize.Width + e.Bounds.X, e.Bounds.Y);
            }
        }

        /// <summary>
        /// Check or uncheck items.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="checkedFlag"></param>
        /// <param name="includeNested"></param>
        public void CheckAll(TreeNode parent, bool checkedFlag, bool includeNested = false)
        {
            TreeNodeCollection nodes = parent == null ? this.Nodes : parent.Nodes;

            foreach (TreeNode node in nodes)
            {
                node.Checked = checkedFlag;
                if (includeNested && node.Nodes.Count > 0)
                {
                    CheckAll(node, checkedFlag, includeNested);
                }
            }
        }

        ///// <summary>
        ///// Expand or collapse items.
        ///// </summary>
        ///// <param name="parent"></param>
        ///// <param name="collapse"></param>
        ///// <param name="includeNested"></param>
        //public void ExpandAll(TreeNode parent, bool collapse, bool includeNested = false)
        //{
        //    TreeNodeCollection nodes = parent == null ? this.Nodes : parent.Nodes;

        //    foreach (TreeNode node in nodes)
        //    {
        //        if (collapse)
        //        {
        //            node.Collapse(!includeNested);
        //        }
        //        else
        //        {
        //            node.Expand();
        //        }
        //        if (includeNested && node.Nodes.Count > 0)
        //        {
        //            CheckAll(node, collapse, includeNested);
        //        }
        //    }
        //    base.colla
        //}

        /// <summary>
        /// Gets all checked nodes, through all levels.
        /// </summary>
        /// <param name="checkedFlag"></param>
        /// <returns></returns>
        public IEnumerable<TreeNode> GetCheckedNodes(TreeNodeCollection nodes = null)
        {
            List<TreeNode> list = new List<TreeNode>();

            if (nodes == null)
            {
                nodes = this.Nodes;
            }

            foreach (TreeNode node in nodes)
            {
                if (node.Checked)
                {
                    list.Add(node);
                }
                foreach (TreeNode innerNode in GetCheckedNodes(node.Nodes))
                {
                    list.Add(innerNode);
                }
            }

            return list;
        }
        #endregion
        
        #region Methods to salvage later.
        /*
        /// <summary>
        /// Copies a TreeView.
        /// </summary>
        /// <param name="source">Source TreeView.</param>
        public void CopyFrom(TreeView source)
        {
            this.Nodes.Clear();
			foreach (TreeNode originalNode in source.Nodes)
            {
                var newNode = new TreeNode(originalNode.Text);
                newNode.Tag = originalNode.Tag;
                newNode.Checked = originalNode.Checked;                    
                this.Nodes.Add(newNode);
				IterateTreeNodes(originalNode, newNode, source.CheckBoxes);
               
            }
        }

        /// <summary>
        /// Copies a TreeView.
        /// </summary>
        /// <param name="target">Target TreeView.</param>
        public void CopyTo(TreeView target)
        {
            target.Nodes.Clear();
            foreach (TreeNode originalNode in this.Nodes)
            {
                TreeNode newNode = new TreeNode(originalNode.Text);
                newNode.Tag = originalNode.Tag;
                newNode.Checked = originalNode.Checked;
                target.Nodes.Add(newNode);
                IterateTreeNodes(target, originalNode, newNode, target.CheckBoxes);

            }
        } 

        /// <summary>
        /// Iterates TreeView, including checked TreeViews. Used by the Copy method.
        /// </summary>
        /// <param name="originalNode"></param>
        /// <param name="rootNode"></param>
		/// <param name="IsCheckNodes">Needs comment.</param>
        private void IterateTreeNodes(TreeNode originalNode, TreeNode rootNode, bool IsCheckNodes)
        {
            foreach (TreeNode childNode in originalNode.Nodes)
            {   
                if (IsCheckNodes)
                {
                    this.CheckBoxes = true;
                    TreeNode newNode = new TreeNode(childNode.Text);
                    newNode.Tag = childNode.Tag;
                    newNode.Checked = childNode.Checked;
                    if (originalNode.Checked) newNode.Expand();
                    this.SelectedNode = rootNode;
                    this.SelectedNode.Nodes.Add(newNode);
                    IterateTreeNodes(childNode, newNode, IsCheckNodes);
                    //childNode.Bounds.Width
                    
                }
                else
                {
                    TreeNode newNode = new TreeNode(childNode.Text);
                    newNode.Tag = childNode.Tag;
                    if (childNode.IsExpanded) newNode.Expand();
                    this.SelectedNode = rootNode;
                    this.SelectedNode.Nodes.Add(newNode);
                    IterateTreeNodes(childNode, newNode, IsCheckNodes);
                    //childNode.Bounds.Width
                }

            }
        }

        /// <summary>
        /// Iterates TreeView, including checked TreeViews. Used by the CopyTo method.
        /// </summary>
        private void IterateTreeNodes(TreeView tvw, TreeNode originalNode, TreeNode rootNode, bool IsCheckNodes)
        {
            foreach (TreeNode childNode in originalNode.Nodes)
            {
                if (IsCheckNodes)
                {
                    tvw.CheckBoxes = true;
                    TreeNode newNode = new TreeNode(childNode.Text);
                    newNode.Tag = childNode.Tag;
                    newNode.Checked = childNode.Checked;
                    if (originalNode.Checked) newNode.Expand();
                    tvw.SelectedNode = rootNode;
                    tvw.SelectedNode.Nodes.Add(newNode);
                    IterateTreeNodes(tvw, childNode, newNode, IsCheckNodes);
                    //childNode.Bounds.Width

                }
                else
                {
                    TreeNode newNode = new TreeNode(childNode.Text);
                    newNode.Tag = childNode.Tag;
                    if (childNode.IsExpanded) newNode.Expand();
                    tvw.SelectedNode = rootNode;
                    tvw.SelectedNode.Nodes.Add(newNode);
                    IterateTreeNodes(tvw, childNode, newNode, IsCheckNodes);
                    //childNode.Bounds.Width
                }

            }
        }

		/// <summary>
		/// Retrieves all checked nodes' tag values as a CSV. Written by Chinedu.
		/// </summary>
		/// <param name="delimiter">Delimiter.</param>
		/// <returns>Delimited string of node tag values.</returns>
		public string GetCheckedValues(string delimiter)
		{
			var nodes = Shared.ControlsExtensions.GetCheckedNodes(this);

			// Get the tags from all the checked nodes.
			var query = from n in nodes
						where n.Checked && n.Tag != null
						select n.Tag.ToString();

			// Include delimiter and return the result
			return String.Join(delimiter, query.ToArray());
		}

        /// <summary>
        /// Retrieves a list of TreeView checked items, using recursion. Written by Georgi.
        /// </summary>
        /// <param name="parent">Parent Node.</param>
        /// <param name="parentChecked">If true, one of the parents was checked.</param>
        /// <returns>CSV of node tag values.</returns>
		/// <remarks>
		/// Additional documentation by Chinedu: This method accepts a node as long as ANY of the 
		/// parents - all the way to the root - is checked.
		/// </remarks>
        public string GetCheckedValues(TreeNode parent, bool parentChecked)
        {
			string s = string.Empty;

			if (parentChecked) s = parent.Tag.ToString();

			foreach (TreeNode tn in parent.Nodes)
			{
				string s2 = GetCheckedValues(tn, parentChecked || tn.Checked);
				if (s2 != string.Empty) s += ((s.Length > 0) ? "," : "") + s2;
			}
            return s;
        }        
        **/
        #endregion        
    }
}
