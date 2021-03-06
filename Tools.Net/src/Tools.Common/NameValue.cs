using System;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Tools.Common
{
	/// <summary>
	/// Purpose of this class is to cover a blank space for NameValueCollection
	/// serialization issues.
	/// </summary>
    [DataContract()]
	[Serializable]
	public class NameValue<TKey, TValue>
	{
		#region Global declarations

        private TKey _name;
        private TValue _value;

		#endregion

		#region Properties

        [DataMember()]
		[XmlAttribute]
        public TKey Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}
        [DataMember()]
		[XmlAttribute]
        public TValue Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}


		#endregion

		#region Constuctors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public NameValue()
		{
		}

        public NameValue(TKey name, TValue value)
		{
			_name = name;
			_value = value;
		}

        public override bool Equals(object obj)
        {
            NameValue<TKey, TValue> nv = obj as NameValue<TKey, TValue>;


            if (nv == null)
                return false;
            return Name.Equals(nv.Name) && Value.Equals(nv.Value); 
        }
        //public static bool operator ==(NameValue<TKey, TValue> a, NameValue<TKey, TValue> b)
        //{
        //    if (System.Object.ReferenceEquals(a, b))
        //    {
        //        return true;
        //    }


        //    return (a.Name.Equals(b.Name)) && (a.Value.Equals(b.Value));
        //}
        //public static bool operator !=(NameValue<TKey, TValue> a, NameValue<TKey, TValue> b)
        //{
        //    return !(a==b);
        //}
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Value.GetHashCode();
        }


		#endregion
	}
}
