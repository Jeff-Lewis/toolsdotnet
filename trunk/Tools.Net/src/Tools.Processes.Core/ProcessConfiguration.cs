using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tools.Core;

namespace Tools.Processes.Core
{
	/// <summary>
	/// Summary description for ProcessConfiguration.
	/// </summary>
	[Serializable()]
	public class ProcessConfiguration : Descriptor, IEnabled
	{
		#region Globals

		private int _stopTimeout = -1;
		private uint _count = 1;
        private List<NameValue<string, string>> _extensibilityItems = new List<NameValue<string,string>>();		

		#endregion Globals

		#region IEnabled Implementation

		private bool _enabled = true;

		public event System.EventHandler EnabledChanged = null;

		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					OnEnabledChanged();
				}

			}
		}

		protected virtual void OnEnabledChanged()
		{
			if (EnabledChanged!=null)
			{
				EnabledChanged(this, System.EventArgs.Empty);
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Timeout that should be given to the process to stop in milliseconds.
		/// Value of minus one (default) will give the process an infinite timeout
		/// to stop.
		/// </summary>
		[XmlAttribute()]
		public int StopTimeout
		{
			get
			{
				return _stopTimeout;
			}
			set
			{
				_stopTimeout = value;
			}
		}
		/// <summary>
		/// Number of processes that should be instantiated.
		/// </summary>
		[XmlAttribute()]
		public uint Count
		{
			get
			{
				return _count;
			}
			set
			{
				_count = value;
			}
		}
		/// <summary>
		/// This property holds a collection of any extensibility configuration
		/// parameters that can be freely used by dynamic assemblies (like
		/// algorithms for submission delay, etc).
		/// That is the preffered mechanism for configuration extensibility before
		/// even categorizing it to the more specific classes. (preffered just to putting
		/// key/values in the config). (SD)
		/// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification="This is by design.")]
		[XmlArray()]
        public List<NameValue<string, string>> ExtensibilityItems
		{
			get
			{
				return _extensibilityItems;
			}
			set
			{
				_extensibilityItems = value;
			}
		}
		#endregion Properties

		#region Constructors

		public ProcessConfiguration()
		{
		}
		public ProcessConfiguration
			(
			string name,
			string description
			) : base
			(
			name,
			description
			)
		{
		}

		#endregion Constructors
	}
}
