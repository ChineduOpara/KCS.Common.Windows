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
using System.ComponentModel;
using System.ComponentModel.Design;
using KCS.Common.Shared;

namespace KCS.Common.Controls
{
    /// <summary>
	/// Facilitates saving a form's position and values of the child controls (user-entered values only).
	/// It can also be used standalone (without associating it with a Form). That is, it supports adding and retrieving
	/// arbitrary values manually (via the overloaded AddValue and GetValue methods).
	/// In "Form" mode, the data is loaded in the associated form's Load event, and saved in the form's Closing event.
	/// Data is serialized and saved in an IsolatedStorage file, scoped to the User &amp; Application.
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
	/// 
	/// To use it WITHOUT a form ("Standalone mode"):
	///		1. Instantiate the control with the "storageKey" constructor.
	///		2. Use AddValue() as needed.
	///		3. Call Save() as needed. This persists the values to disk.
	///		4. To reload the values into memory, call Load().
	///		5. Call GetValue() as needed, to retrieve specific values by key (the same keys used in the calls
	///		    to AddValue() ). You can also call Restore() to simply restore all the stored values to their
	///		    matching controls.
	/// </example>
    //[
    //    Designer(typeof(ValuesTrackerDesigner), typeof(IDesigner)),
    //    DefaultProperty("ControlNames"),
    //    //ToolboxBitmap(typeof(ValuesTracker), "Resources.ValuesTracker") // Does not work. Well-known bug in VS.Net
    //    ToolboxBitmap(@"C:\DaVinci\DV2\KCS.Common.Controls\Resources\ValuesTracker.gif")
    //]
	public class ValuesTracker : Component
    {
        #region Members
		private Form _ownerForm;
		private IContainer _Components;		
		private bool _SavingCancelled;
		private bool _LoadingCancelled;
        #endregion

		#region Events
		/// <summary>
		/// Raised just before values are loaded from the datastore.
		/// </summary>
		[
			Category("Behavior"),
			Description("Raised just before values are loaded from the datastore.")
		]
		public event EventHandler<CancelEventArgs> Loading;

		/// <summary>
		/// Raised just after values are loaded from the datastore.
		/// </summary>
		[
			Category("Behavior"),
			Description("Raised just after values are loaded from the datastore.")
		]
		public event EventHandler<SuccessEventArgs> Loaded;

		/// <summary>
		/// Raised just before controls values are batch-restored.
		/// </summary>
		[
			Category("Behavior"),
			Description("Raised just before controls' values are batch-restored.")
		]
		public event EventHandler Restoring;

		/// <summary>
		/// Raised just after controls values are batch-restored.
		/// </summary>
		[
			Category("Behavior"),
			Description("Raised just after controls values are batch-restored.")
		]
		public event EventHandler<SuccessEventArgs> Restored;

		/// <summary>
		/// Raised just before controls values are batch-saved.
		/// </summary>
		[
			Category("Behavior"),
			Description("Raised just before controls values are batch-saved.")
		]
		public event EventHandler<CancelEventArgs> Saving;

		/// <summary>
		/// Raised just after controls values are batch-saved.
		/// </summary>
		[
			Category("Behavior"),
			Description("Raised just after controls values are batch-saved.")
		]
		public event EventHandler<SuccessEventArgs> Saved;
		#endregion

		#region Properties
		/// <summary>
		/// Contains TRUE while the values are being auto-restored.
		/// </summary>
		[
			Category("Behavior"),
			Description("Contains TRUE while the values are being auto-restored."),
			Browsable(false)
		]
		public bool IsRestoring { get; private set; }

		/// <summary>
		/// List of fully-qualified control names for which values will be auto- saved and restored.
		/// </summary>
		[
			Category("Controls"),
			Description("List of fully-qualified toolStripItem names for which values will be auto- saved and restored."),
            //Editor(typeof(ValuesTrackerEditor), typeof(UITypeEditor)),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
			Browsable(false)
		]
		public List<string> ControlNames { get; private set; }

		///// <summary>
		///// List of control types that will participate in tracking.
		///// </summary>
		//[
		//    Category("Controls"),
		//    Description("List of control types that will participate in tracking"),
		//    //Editor(typeof(ValuesTrackerEditor), typeof(UITypeEditor)),
		//    DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		//    Browsable(false)
		//]
		//private List<Type> ControlTypes { get; set; }

		/// <summary>
		/// Contains all Exceptions generated during the lifetime of the component.
		/// </summary>
		[
			Browsable(false)
		]
		public List<Exception> Exceptions { get; private set; }

        /// <summary>
        /// Gets or sets the associated form.
        /// </summary>
        [
			Description("Gets or sets the associated form."),
            Category("Design"),
			Browsable(false)
        ]
        public Form Form
		{
			get
			{
				return _ownerForm;
			}
			set
			{				
				_ownerForm = value;

				// Extract the form name and use it as the StorageKey.
				if (DesignMode)
				{					
					IDesignerHost designerHost = (IDesignerHost)Container;
					Form form = (Form)designerHost.RootComponent;
					string[] parts = designerHost.RootComponentClassName.Split('.');
					StorageKey = parts[parts.Length - 1];
				}
				else
				{
					StorageKey = _ownerForm.GetType().Name;

					// At runtime, subscribe to the Form's events.
					SubscribeToEventHandlers();
				}
			}
		}

		/// <summary>
		/// Gets or sets the storage key, which is used to uniquely identify the associated form in the
		/// data store.
		/// </summary>
		[
			Description("Gets or sets the storage key, which is used to uniquely identify the associated form in the imageData store."),
			Category("Data"),
			Browsable(true),
			DefaultValue("")
		]
		internal string StorageKey { get; private set; }

        /// <summary>
        /// Contains the dictionary of values being tracked.
        /// </summary>
        [
            Browsable(false)
        ]
        private Dictionary<string, object> Dictionary { get; set; }

        /// <summary>
		/// If set, the form's state is automatically saved or restored after being closed or shown.
        /// </summary>
        [
			Description("If set, the form's state is automatically saved or restored after being closed or shown."),
            DefaultValue(true),
            Category("Behavior")
        ]
        public bool Auto { get; set; }
        #endregion		

		/// <summary>
		/// Default constructor.
		/// </summary>
		private ValuesTracker()
		{
			InitializeComponent();
			Dictionary = new Dictionary<string, object>();
			Exceptions = new List<Exception>();
			ControlNames = new List<string>();
			Auto = true;
			//ControlTypes = new List<Type>();			

			// Build the list of control types that will participate in tracking
			//ControlTypes.Clear();
			//ControlTypes.Add(typeof(CheckedListBox));
			//ControlTypes.Add(typeof(TextBox));
			//ControlTypes.Add(typeof(CheckBox));
			//ControlTypes.Add(typeof(NumericUpDown));
			//ControlTypes.Add(typeof(ComboBox));
			//ControlTypes.Add(typeof(ListView));
			//ControlTypes.Add(typeof(TreeView));
			//ControlTypes.Add(typeof(DateTimePicker));
			//ControlTypes.Add(typeof(MonthCalendar));
			//ControlTypes.Add(typeof(ListBox));
			//ControlTypes.Add(typeof(RadioButton));
			//ControlTypes.Add(typeof(RichTextBox));
			//ControlTypes.Add(typeof(WebBrowser));
			//ControlTypes.Add(typeof(SplitContainer));
			//ControlTypes.Add(typeof(TabControl));
		}		

		/// <summary>
		/// Alternate constructor - sets the associated Form and StorageKey.
		/// </summary>
		/// <param name="form">Form to be associated with this control.</param>
		public ValuesTracker(Form form) : this()
		{
			Form = form;
			StorageKey = form.Name;
		}

		/// <summary>
		/// Alternate constructor - sets the associated Form and StorageKey, as well as the Auto flag.
		/// </summary>
		/// <param name="form">Form to be associated with this control.</param>
		/// <param name="auto">Initial value of the Auto property.</param>
		public ValuesTracker(Form form, bool auto) : this(form)
		{
			Auto = auto;
		}

		/// <summary>
		/// Alternate constructor - sets the Auto flag to FALSE.
		/// </summary>
		/// <param name="storageKey">Storage key, for persistent storage.
		/// All instances of this component that share the same key will contain the same values at startup.</param>
		public ValuesTracker(string storageKey) : this()
		{
			StorageKey = storageKey;
			Auto = false;
		}

		/// <summary>   
		/// Initializes a new instance of the class with the specified container.
		/// Required by the WinForms designer.
		/// </summary>   
		/// <param name="container">An System.ComponentModel.IContainer that represents the container for the Component control.</param>  
		public ValuesTracker(IContainer container) : this()
		{			
			if (container != null)
			{
				container.Add(this);
			}
			Auto = true;
		}

		/// <summary>
		/// Called when the component initializes.
		/// </summary>
		private void InitializeComponent()
		{
			_Components = new System.ComponentModel.Container();
			ComponentResourceManager resources = new ComponentResourceManager(typeof(ValuesTracker));
		}

		/// <summary>
		/// Sets a byte array representation of the internal dictionary.
		/// </summary>
		/// <param name="data">Data to set.</param>
		public void SetDictionaryData(byte[] data)
		{
			MemoryStream stream;
			BinaryFormatter bf = new BinaryFormatter();

			// Save the dictionary as raw binary data. If it fails, the Dictionary is created, but it is empty.
			try
			{
				if (data == null)
				{
					Dictionary = new Dictionary<string, object>();
				}
				else
				{
					stream = new MemoryStream(data);
					Dictionary = (Dictionary<string, object>)bf.Deserialize(stream);
				}
			}
			catch (Exception ex)
			{
				Exceptions.Add(ex);
				Dictionary = new Dictionary<string, object>();
			}
		}

		/// <summary>
		/// Gets a byte array representation of the internal dictionary.
		/// </summary>
		/// <returns>Byte array.</returns>
		public byte[] GetDictionaryData()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatter bf = new BinaryFormatter();

			// Save the dictionary as raw binary data
			bf.Serialize(stream, Dictionary);
			return stream.GetBuffer();
		}

		/// <summary>
		/// Automatically populate the ControlNames property with ALL the trackable controls
		/// on the associated Form.
		/// </summary>
		private void SetControlsToTrack()
		{
			List<Control> controls = Form.GetControls(true, new List<string>());
			ControlNames = controls.ConvertAll<string>
			(
				delegate(Control ctrl)
				{
					return ctrl.GetFullyQualifiedName();
				}
			);
		}

		/// <summary>
		/// Subscribes to the Shown, FormClosing, Move, and Resize events of the associated form.
		/// This allows auto-save and auto-restore, as well as tracking the size and position of the Form.
		/// </summary>
		private void SubscribeToEventHandlers()
		{
			if (Form != null && !DesignMode)
			{
				Form.Load += new EventHandler(FormLoad);
				Form.FormClosing += new FormClosingEventHandler(FormClosing);
				Form.Move += new EventHandler(FormMovedOrResized);
				Form.Resize += new EventHandler(FormMovedOrResized);
			}
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
				Load();		// Always load the last-saved values.

				if (Auto)
				{
					Restore();
				}
			}
			catch (Exception ex)
			{
				Exceptions.Add(ex);
			}
			finally
			{
				//
			}
		}

        /// <summary>
        /// When the form is closing, saves all the values (if Auto is true).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (Auto)
                {
                    Save();
                }
            }
            finally
            {
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
            if (Form != null && !IsRestoring && Form.WindowState != FormWindowState.Minimized)
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
        /// Saves the values to the data store. If a Form is available, it first saves the child controls values.
        /// </summary>
        public void Save()
        {
			byte[] buffer;
			IsolatedStorageFile ifs = IsolatedStorageFile.GetUserStoreForAssembly();
			IsolatedStorageFileStream isStream = null;

			// Get all the values and values from all the controls.
			if (Form != null)
			{
				// Assemble the list of Controls that will be tracked.
				SetControlsToTrack();
				AddValues(Form.GetValues(true, ControlNames));
			}

			// Persist the data to isolated storage
			try
			{
				// Raise the Saving event. If the event was not cancelled, proceed to save the data to
				// isoloated storage
				OnSaving();
				if (!_SavingCancelled)
				{
					buffer = GetDictionaryData();
					isStream = new IsolatedStorageFileStream(StorageKey, FileMode.Create, FileAccess.Write, ifs);
					isStream.Write(buffer, 0, buffer.Length);
				}
				
				// Raise the Saved event, indicating success
				OnSaved(true);
			}
			catch (Exception ex)
			{
				Exceptions.Add(ex);

				// Raise the Saved event, indicating failure
				OnSaved(false);
			}
			finally
			{
				if (isStream != null)
				{
					isStream.Close();
					isStream = null;
				}
			}            
        }

        /// <summary>
        /// Loads all the previously-saved values from isolated storage.
        /// </summary>
        public void Load()
        {
			BinaryFormatter bf = new BinaryFormatter();
			IsolatedStorageFileStream isStream = null;
			IsolatedStorageFile ifs = IsolatedStorageFile.GetUserStoreForAssembly();

            // Load the data from isolated storage
			try
			{
				// Raise the Loading event. If it was not cancelled, proceed to get the data from isolated storage.
				OnLoading();
				if (!_LoadingCancelled)
				{
					isStream = new IsolatedStorageFileStream(StorageKey, FileMode.OpenOrCreate, FileAccess.Read, ifs);
					if (isStream.Length > 0)
					{
						Dictionary = (Dictionary<string, object>)bf.Deserialize(isStream);
					}
				}

				// Raise the Loaded event, indicating success
				OnLoaded(true);
			}
			catch (Exception ex)
			{
				Exceptions.Add(ex);

				// Raise the Loaded event, indicating failure
				OnLoaded(false);
			}
			finally
			{
				if (isStream != null)
				{
					isStream.Close();
					isStream = null;
				}
			}

            // Make sure that the values dictionary exists and is populated with the minimum items
            if (Dictionary == null)
            {
                Dictionary = new Dictionary<string, object>();
            }

            if (Dictionary.Count == 0)
            {
                AddDefaultvalues();
            }
        }

		/// <summary>
		/// Restores all of the form's state. This method is only valid if there is an associated Form.
		/// </summary>
		public void Restore()
		{
			if (Form == null)
			{
				throw new NotSupportedException("There is no associated Form. Is the component in Standalone mode? If so, please use GetValue() or GetValues() and perform manual restoration.");
			}

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

            Form.Top = GetValue(Shared.Constants.FormProperties.Top, Form.Top);
			Form.Left = GetValue(Shared.Constants.FormProperties.Left, Form.Left);
			Form.Width = GetValue(Shared.Constants.FormProperties.Width, Form.Width);
			Form.Height = GetValue(Shared.Constants.FormProperties.Height, Form.Height);

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
		/// Removes a value from the collection
		/// </summary>
		/// <param name="key">Key of the value to remove.</param>
		public void RemoveValue(string key)
		{
			if (Dictionary.ContainsKey(key))
			{
				Dictionary.Remove(key);
			}
		}

		/// <summary> 
		/// Removes a value previously stored for a Control.
		/// </summary>
		/// <param name="ctrl">
		/// Control whose value will be removed. The Fully Qualified Name is used as the value's key.
		/// </param>
		/// <returns>Object value, or default.</returns>
		public void RemoveValue(Control ctrl)
		{
			string key = ctrl.GetFullyQualifiedName();
			if (Dictionary.ContainsKey(key))
			{
				Dictionary.Remove(key);
			}
		}

		///// <summary>
		///// Adds an int value. If the key already exists, the value is set instead.
		///// </summary>
		///// <param name="key">Value's key.</param>
		///// <param name="value">Value to set.</param>
		//public void AddValue(string key, int value)
		//{
		//    if (Dictionary.ContainsKey(key))
		//    {
		//        Dictionary[key] = value;
		//    }
		//    else
		//    {
		//        Dictionary.Add(key, value);
		//    }
		//}

		///// <summary>
		///// Adds a long value. If the key already exists, the value is set instead.
		///// </summary>
		///// <param name="key">Value's key.</param>
		///// <param name="value">Value to set.</param>
		//public void AddValue(string key, long value)
		//{
		//    if (Dictionary.ContainsKey(key))
		//    {
		//        Dictionary[key] = value;
		//    }
		//    else
		//    {
		//        Dictionary.Add(key, value);
		//    }
		//}

		///// <summary>
		///// Adds a Decimal value. If the key already exists, the value is set instead.
		///// </summary>
		///// <param name="key">Value's key.</param>
		///// <param name="value">Value to set.</param>
		//public void AddValue(string key, Decimal value)
		//{
		//    if (Dictionary.ContainsKey(key))
		//    {
		//        Dictionary[key] = value;
		//    }
		//    else
		//    {
		//        Dictionary.Add(key, value);
		//    }
		//}

		///// <summary>
		///// Adds a string value. If the key already exists, the value is set instead.
		///// </summary>
		///// <param name="key">Value's key.</param>
		///// <param name="value">Value to set.</param>
		//public void AddValue(string key, string value)
		//{
		//    if (Dictionary.ContainsKey(key))
		//    {
		//        Dictionary[key] = value;
		//    }
		//    else
		//    {
		//        Dictionary.Add(key, value);
		//    }
		//}

		///// <summary>
		///// Adds a boolean value. If the key already exists, the value is set instead.
		///// </summary>
		///// <param name="key">Value's key.</param>
		///// <param name="value">Value to set.</param>
		//public void AddValue(string key, bool value)
		//{
		//    if (Dictionary.ContainsKey(key))
		//    {
		//        Dictionary[key] = value;
		//    }
		//    else
		//    {
		//        Dictionary.Add(key, value);
		//    }
		//}

        /// <summary>
        /// Adds an object value. If the key already exists, the value is set instead.
        /// </summary>
        /// <param name="key">Value's key.</param>
        /// <param name="value">Value to set.</param>
        public void AddValue<T>(string key, T value)
        {
            if (Dictionary.ContainsKey(key))
            {
                Dictionary[key] = value;
            }
            else
            {
                Dictionary.Add(key, value);
            }
        }

		/// <summary>
		/// Adds a value associated with a control's default property. If the key already exists, the value is set instead.
		/// </summary>
		/// <param name="ctrl">
		/// Control whose value will be stored. The Fully Qualified Name is used as the value's key.
		/// </param>
		public void AddValue(Control ctrl)
		{
			string key = ctrl.GetFullyQualifiedName();

			if (Dictionary.ContainsKey(key))
			{
				Dictionary[key] = ctrl.GetValue();
			}
			else
			{
				Dictionary.Add(key, ctrl.GetValue());
			}
		}

		/// <summary>
		/// Adds multiple values to the internal dictionary.
		/// </summary>
		/// <param name="collection">Collection to be added.</param>
		public void AddValues<T>(Dictionary<string, T> collection)
		{
		    foreach(KeyValuePair<string, T> pair in collection)
		    {
				if (pair.Value != null)
				{
					AddValue(pair.Key, pair.Value);
				}
				else
				{
					RemoveValue(pair.Key);
				}
		    }
		}

		/// <summary> 
		/// Gets an object value. If the key does not exists, returns the default.
		/// </summary>
		/// <param name="key">Key of value to retrieve.</param>
		/// <param name="default">Default value.</param>
		/// <returns>Object value.</returns>
		public T GetValue<T>(string key, T @default)
		{
			if (Dictionary.ContainsKey(key))
			{
				return (T)Dictionary[key];
			}
			else
			{
				return @default;
			}
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
			if (Dictionary.ContainsKey(key))
			{
				return (T)Dictionary[key];
			}
			else
			{
				return (T)@default;
			}
		}

		/// <summary> 
		/// Gets all values.
		/// </summary>
		/// <returns>Dictionary of all the values.</returns>
		public Dictionary<string, object> GetValues()
		{
			return new Dictionary<string, object>(this.Dictionary);
		}

        /// <summary> 
        /// Clears all the values and adds the default values.
        /// </summary>
        public void Clear(bool addDefaultValues = true)
        {
            Dictionary.Clear();
            if (addDefaultValues)
            {
                AddDefaultvalues();
            }
        }

		/// <summary> 
		/// Deletes the isolated storage file used to store values.
		/// </summary>
		public void DeleteDataStore()
		{
			try
			{
				var ifs = IsolatedStorageFile.GetUserStoreForAssembly();
				
				var files = ifs.GetFileNames("*.*");
				if (files.Contains(StorageKey))
				{
					ifs.DeleteFile(StorageKey);
				}
			}
			catch (Exception ex)
			{
				Exceptions.Add(ex);
			}
			finally
			{
			}
		}

        /// <summary> 
        /// Adds the default values.
        /// </summary>
        private void AddDefaultvalues()
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
		/// Raises the Loading event.
		/// </summary>
		protected void OnLoading()
		{
			CancelEventArgs args = new CancelEventArgs();
			if (Loading != null)
			{
				Loading(this, args);
				_LoadingCancelled = args.Cancel;
			}
		}

		/// <summary>
		/// Raises the Loaded event.
		/// </summary>
		/// <param name="success">Success flag.</param>
		protected void OnLoaded(bool success)
		{
			if (Loaded != null)
			{
				Loaded(this, new SuccessEventArgs(success));
			}
		}

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

		/// <summary>
		/// Raises the Saving event.
		/// </summary>
		protected void OnSaving()
		{
			CancelEventArgs args = new CancelEventArgs();
			if (Saving != null)
			{
				Saving(this, args);
				_SavingCancelled = args.Cancel;
			}
		}

		/// <summary>
		/// Raises the Saveded event.
		/// </summary>
		/// <param name="success">Success flag.</param>
		protected void OnSaved(bool success)
		{
			if (Saved != null)
			{
				Saved(this, new SuccessEventArgs(success));
			}
		}
		#endregion
	}

	
}
