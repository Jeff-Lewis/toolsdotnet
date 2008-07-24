using System;
using System.Xml.Serialization;

namespace Tools.Core.Context
{
	// TODO: this class to be re-factored into inheritance of the minimum of two classes:
	// One oriented onto more abstract ContextHolderId, Guid and InternalId
	// And another one oriented onto more transactional an parent related context environment. (SD)
	/// <summary>
	/// Summary description for ContextIdentifier.
	/// 
	/// </summary>
	[Serializable()]
	public class ContextIdentifier : ICloneable
	{
		
		#region Fields
		
		/// <summary>
		/// Unique id that corresponds to the transaction in the 
		/// third party system.
		/// </summary>
		private object	_externalId			= 0;
		/// <summary>
		/// Id of the record or entity
		/// </summary>
		private object	_externalParentId	= 0;

		private object	_externalReference	= 0;
		// Autogenerated
		private Guid	_contextGuid		= Guid.NewGuid();

		private decimal _internalId			= 0;

		private decimal _internalParentId	= 0;

		private int		_contextHolderId	= 0;

		private int _authenticationTokenId	= 0;

		#endregion

		#region Properties

		//[XmlAttribute()]
		public object ExternalId
		{
			
			get
			{
				return _externalId;
			}

			set
			{
				_externalId = value;
			}

		}
		//[XmlAttribute()]
		public object ExternalParentId
		{
			
			get
			{
				return _externalParentId;
			}

			set
			{
				_externalParentId = value;
			}

		}
		//[XmlAttribute()]
		public object ExternalReference
		{
			
			get
			{
				return _externalReference;
			}

			set
			{
				_externalReference = value;
			}

		}
		[XmlAttribute()]
		public Guid ContextGuid
		{
			
			get
			{
				return _contextGuid;
			}
			set
			{
				_contextGuid = value;
			}

		}
		[XmlAttribute()]
		public decimal InternalId
		{
			
			get
			{
				return _internalId;
			}

			set
			{
				_internalId = value;
			}

		}
		[XmlAttribute()]
		public decimal InternalParentId
		{
			
			get
			{
				return _internalParentId;
			}

			set
			{
				_internalParentId = value;
			}

		}
		[XmlAttribute()]
		public int ContextHolderId
		{
			
			get
			{
				return _contextHolderId;
			}

			set
			{
				_contextHolderId = value;
			}

		}
		[XmlAttribute()]
		public int AuthenticationTokenId
		{
			
			get
			{
				return _authenticationTokenId;
			}

			set
			{
				_authenticationTokenId = value;
			}

		}
		#endregion

		#region Constructors

		public ContextIdentifier()
		{

		}

		public ContextIdentifier
			(
			Guid contextGuid
			)
		{
			this._contextGuid = contextGuid;
		}
		private ContextIdentifier
			(
			object	externalId,
			object	externalParentId,
			object	externalReference,
			Guid	contextGuid,
			decimal internalId,
			decimal internalParentId,
			int		contextHolderId,
			int authenticationTokenId
			)
		{
			_externalId = externalId;
			_externalParentId = externalParentId;
			_externalReference = externalReference;
			_contextGuid = contextGuid;
			_internalId = internalId;
			_internalParentId = internalParentId;
			_contextHolderId = contextHolderId;
			_authenticationTokenId = authenticationTokenId;
		}

		#endregion

		#region ICloneable Members

		/// <summary>
		/// Provides deep copy
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new ContextIdentifier
				(
				_externalId,
				_externalParentId,
				_externalReference,
				_contextGuid,
				_internalId,
				_internalParentId,
				_contextHolderId,
				_authenticationTokenId
				);
		}

		#endregion

        public static ContextIdentifier ProbeForContextIdentifier
            (
            IContextIdentifierHolder contextIdentifierHolder
            )
        {
            if (contextIdentifierHolder == null || contextIdentifierHolder.ContextIdentifier == null)
            {
                return new ContextIdentifier();
            }
            return contextIdentifierHolder.ContextIdentifier;
        }
	}
}
