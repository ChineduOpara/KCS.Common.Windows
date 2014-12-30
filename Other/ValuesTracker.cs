using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.IO.IsolatedStorage;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization.Formatters.Binary;
//using System.ComponentModel;
//using System.ComponentModel.Design;
using KCS.Common.Shared;

namespace KCS.Common.Controls
{
    /// <summary>
	/// Extends the KCS.Common.Shared ValuesTracker to support saving a form's position and values of the child controls (user-entered values only).
	/// Data is loaded in the associated form's Load event, and saved in the form's Closing event.
    /// </summary>
	/// <remarks>
	/// Future Improvements:
	/// 1. The first version only works with a Form, but it could potentially be adapted to work with
	///    User Controls as well.
	/// 2. It enumerates ALL the controls on a Form (even nested ones), and looks at ALL controls.
	///	   However, only a subset of controls accept user input, so only those are automatically tracked.
	///	3. Any ComboBox (or subclass of same) must have their ValueMember set, in order to work with this
	///	   component.
	/// </remarks>
	/// <example>
	/// To use it with a Form ("Form mode"), at design-time:
	///		1. Drop the component on the Form. THAT'S ALL.
	///		To add additional values before the controls are restored, do so in this component's Loaded event handler.
	/// </example>
    //[
    //    Designer(typeof(ValuesTrackerDesigner), typeof(IDesigner)),
    //    DefaultProperty("ControlNames"),
    //    //ToolboxBitmap(typeof(ValuesTracker), "Resources.ValuesTracker") // Does not work. Well-known bug in VS.Net
    //    ToolboxBitmap(@"C:\DaVinci\DV2\KCS.Common.Controls\Resources\ValuesTracker.gif")
    //]
	public class ValuesTracker : KCS.Common.Shared.ValuesTracker
    {
		#region Events
		/// <summary>
		/// Raised just before controls values are batch-restored.
		/// </summary>
		public event EventHandler Restoring;

		/// <summary>
		/// Raised just after controls values are batch-restored.
		/// </summary>
		public event EventHandler<SuccessEventArgs> Restored;
		#endregion

		#region Properties
		/// <summary>
		/// Contains TRUE while the values are being auto-restored.
		/// </summary>
		public bool IsRestoring { get; private set; }

		/// <summary>
		/// List of fully-qualified control names for which values will be auto- saved and restored.
		/// </summary>
		public List<string> ControlNames { get; private set; }

        /// <summary>
        /// Gets or sets the associated form.
        /// </summary>
        public Form Form {get; private set;}

        /// <summary>
		/// If set, the form's state is automatically saved or restored after being closed or shown.
        /// </summary>
        public bool Auto { get; set; }
        #endregion		

		/// <summary>
		/// Alternate constructor - sets the associated Form and StorageKey.
		/// </summary>
		/// <param name="form">Form to be associated with this control.</param>
		public ValuesTracker(Form form, bool auto = true) : base(form.GetType().FullName)
		{
            if (Form != null)
            {
                throw new Exception("The parent Form is required.");
            }

            ControlNames = new List<string>();
            Form = form;
            Auto = auto;

            SubscribeToEventHandlers();
		}

		/// <summary>
		/// Automatically populate the ControlNames property with ALL the trackable controls
		/// on the associated Form.
		/// </summary>
		private void SetControlsToTrack()
		{
			var controls = Form.GetControls(true, new List<string>());
            ControlNames = controls.Select(x => x.GetFullyQualifiedName()).ToList();
		}

		/// <summary>
		/// Subscribes to the Shown, FormClosing, Move, and Resize events of the associated form.
		/// This allows auto-save and auto-restore, as well as tracking the size and position of the Form.
		/// </summary>
		private void SubscribeToEventHandlers()
		{
			Form.Load += new EventHandler(FormLoad);
			Form.FormClosing += new FormClosingEventHandler(FormClosing);
			Form.Move += new EventHandler(FormMovedOrResized);
			Form.Resize += new EventHandler(FormMovedOrResized);
		}				

        #region Form event handlers
		/// <summary>
		/// After the associated form is loaded, restores the form's control values (if Auto is true).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void FormLoad(object sender, EventArgs e)
		{
			try
			{
				base.Load();		// Always load the last-saved values.

				if (Auto)
				{
					Restore();
				}
			}
			catch (Exception ex)
			{
				Exceptions.Add(ex);
			}
		}

        /// <summary>
        /// When the form is closing, saves all the values (if Auto is true).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Auto)
            {
                Save();
            }
        }

        /// <summary>
        /// Saves the form's size and location when it is moved or resized.
		/// Does not apply when the form is minimized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMovedOrResized(object sender, EventArgs e)
        {
            if (!IsRestoring && Form.WindowState != FormWindowState.Minimized)
            {
                TrackFormPosition();
            }
        }

        /// <summary>
        /// Tracks the form's size and position.
        /// </summary>
		private void TrackFormPosition()
        {
            AddValue(Shared.Constants.FormProperties.Top, Form.Top);
			AddValue(Shared.Constants.FormProperties.Left, Form.Left);
			AddValue(Shared.Constants.FormProperties.Width, Form.Width);
			AddValue(Shared.Constants.FormProperties.Height, Form.Height);
        }

		/// <summary>
		/// Restores all of the form's state. This method is only valid if there is an associated Form.
		/// </summary>
		public void Restore()
		{
			// Indicate that the component is auto-restoring values.
			IsRestoring = true;

			// Always load the form's data from isolated storage
			try
			{
				// Raise the Restoring event
				OnRestoring();

				// Restore the form's values and position
				SetControlsToTrack();
				RestoreFormBounds();
				Form.SetValues(Dictionary, true, ControlNames);				
				TrackFormPosition();

				// Raise the Restored event, indicating success
				OnRestored(true);
			}
			catch (Exception ex)
			{
				Exceptions.Add(ex);

				// Raise the Restored event, indicating failure
				OnRestored(false);
			}
			finally
			{
				// Indicate that the component is no longer auto-restoring the values.
				IsRestoring = false;
			}
		}

        /// <summary>
        /// Restores ONLY the location of the associated form.
		/// If the form falls complete outside the bounds of all the available monitors, this method
        /// adjusts the form's position.
        /// </summary>
        public void RestoreFormBounds()
        {
            Rectangle formBounds;
            int intersections = 0;
			int intersectMargin = 100;

            var top = GetValue(Shared.Constants.FormProperties.Top, Form.Top);
            var left = GetValue(Shared.Constants.FormProperties.Left, Form.Left);
            var width = GetValue(Shared.Constants.FormProperties.Width, Form.Width);
            var height = GetValue(Shared.Constants.FormProperties.Height, Form.Height);

            Form.Top = top;
			Form.Left = left;
			Form.Width = width;
			Form.Height = height;

            formBounds = new Rectangle(Form.Location, Form.Size);
			formBounds.Inflate(-intersectMargin, -intersectMargin);

            // If the form's bounds intersects with at least one monitor, it's ok.
            // If not, reposition the form.
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(formBounds))
                {
                    intersections++;
                }
            }

            // If there were no intersections, perform location adjustment to top left corner.
            if (intersections == 0)
            {
				Form.Top = intersectMargin;
				Form.Left = intersectMargin;
            }
        }
        #endregion

        #region Methods for working with the dictionary (getting and setting values)
		/// <summary>
		/// Adds a value associated with a control's default property. If the key already exists, the value is set instead.
		/// </summary>
		/// <param name="ctrl">
		/// Control whose value will be stored. The Fully Qualified Name is used as the value's key.
		/// </param>
		public void AddValue(Control ctrl)
		{
			string key = ctrl.GetFullyQualifiedName();
            base.AddValue(key, ctrl.GetValue());
		}

		/// <summary> 
		/// Gets a value previously stored for a Control. If the key does not exists, returns the default.
		/// </summary>
		/// <param name="ctrl">
		/// Control whose value will be retrieved. The Fully Qualified Name is used as the value's key.
		/// </param>
		/// <param name="default">Default value.</param>
		/// <returns>Object value, or default.</returns>
		public T GetValue<T>(Control ctrl, T @default)
		{
			string key = ctrl.GetFullyQualifiedName();
            return (T)base.GetValue(key, @default);
		}
        
        /// <summary> 
        /// Adds the default values for the form.
        /// </summary>
        private void AddDefaultValues()
        {
			if (Form != null)
			{
				AddValue(Shared.Constants.FormProperties.Top, Form.Top);
				AddValue(Shared.Constants.FormProperties.Left, Form.Left);
				AddValue(Shared.Constants.FormProperties.Width, Form.Width);
				AddValue(Shared.Constants.FormProperties.Height, Form.Height);
			}
        }
        #endregion

		#region Methods that raise events.
		/// <summary>
		/// Raises the Restoring event.
		/// </summary>
		protected void OnRestoring()
		{
			if (Restoring != null)
			{
				Restoring(this, new EventArgs());
			}
		}

		/// <summary>
		/// Raises the Restored event.
		/// </summary>
		/// <param name="success">Success flag.</param>
		protected void OnRestored(bool success)
		{
			if (Restored != null)
			{
				Restored(this, new SuccessEventArgs(success));
			}
		}
		#endregion
    }

	
}
