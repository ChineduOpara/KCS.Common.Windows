using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using KCS.Common.Shared;

namespace KCS.Common.Controls
{
	/// <summary>   
	/// Component Designer that supports the ValuesTracker componnet. It ensures that the component
	/// is associated with a Form.
	/// </summary>
	/// <remarks>
	/// In the future this might be modified to allow the component to be associatd with a User Control.
	/// </remarks>
	internal class ValuesTrackerDesigner : ComponentDesigner
	{
		private Form _ParentForm;

		/// <summary>   
		/// Prepares the designer to view, edit, and design the specified component.   
		/// </summary>   
		/// <param name="component">The component for this designer.</param>   
		public override void Initialize(System.ComponentModel.IComponent component)
		{
			base.Initialize(component);

			SetParentForm();
		}

		/// <summary>
		/// Called when a new ValuesTracker component is dropped onto the Form.
		/// </summary>
		/// <param name="defaultValues"></param>
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues)
		{
			base.InitializeNewComponent(defaultValues);
			SetParentForm();
		}

		/// <summary>
		/// Sets the parent form of the component.
		/// </summary>
		private void SetParentForm()
		{
			ValuesTracker tracker = (ValuesTracker)this.Component;
			IDesignerHost designerHost = (IDesignerHost)tracker.Container;			
			_ParentForm = (Form)designerHost.RootComponent;

			if (_ParentForm == null)
			{
				throw new ApplicationException("The host form must be a top-level form.");
			}

			// Set the form (and storage key).
			tracker.Form = _ParentForm;
		}
	}	
}
