using System;
using System.Collections;

namespace Tools.Failover
{
    /// <summary>
    ///     <para>
    ///       A collection that stores <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/> objects.
    ///    </para>
    /// </summary>
    /// <seealso cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/>
    [Serializable]
    public class FailoverConfigurationCollection : CollectionBase
    {
        #region Constructors

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/>.
        ///    </para>
        /// </summary>
        public FailoverConfigurationCollection()
        {
        }

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/> based on another <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/>.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/> from which the contents are copied
        /// </param>
        public FailoverConfigurationCollection(FailoverConfigurationCollection value)
        {
            AddRange(value);
        }

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/> containing any array of <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/> objects.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A array of <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/> objects with which to intialize the collection
        /// </param>
        public FailoverConfigurationCollection(FailoverConfiguration[] value)
        {
            AddRange(value);
        }

        #endregion

        #region Indexers

        /// <summary>
        /// <para>Represents the entry at the specified index of the <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/>.</para>
        /// </summary>
        /// <param name='index'><para>The zero-based index of the entry to locate in the collection.</para></param>
        /// <value>
        ///    <para> The entry at the specified index of the collection.</para>
        /// </value>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> 
        /// is outside the valid range of indexes for the collection.
        /// </exception>
        public FailoverConfiguration this[int index]
        {
            get { return ((FailoverConfiguration) (List[index])); }
            set { List[index] = value; }
        }

        public FailoverConfiguration this[string name]
        {
            get
            {
                foreach (FailoverConfiguration fdf in this)
                {
                    if (fdf.Name == name)
                    {
                        return fdf;
                    }
                }
                return null;
            }
            set
            {
                for (int i = 0; i < List.Count; i++)
                {
                    if (((FailoverConfiguration) List[i]).Name == name)
                    {
                        List[i] = value;
                        return;
                    }
                }
                Add(value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///    <para>Adds a <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/> with the specified value to the 
        ///    <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/> to add.</param>
        /// <returns>
        ///    <para>The index at which the new element was inserted.</para>
        /// </returns>
        /// <seealso cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection.AddRange'/>
        public int Add(FailoverConfiguration value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// <para>Merges the elements of an array to the end of the <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/>.</para>
        /// </summary>
        /// <param name='value'>
        ///    An array of type <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <seealso cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection.Add'/>
        public void AddRange(FailoverConfiguration[] value)
        {
            for (int i = 0; (i < value.Length); i = (i + 1))
            {
                FailoverConfiguration nv = GetEntry(value[i].Name);
                if (nv != null)
                {
                    nv = value[i];
                }
                else
                {
                    Add(value[i]);
                }
            }
        }

        /// <summary>
        ///     <para>
        ///       Adds the contents of another <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/> to the end of the collection.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///    A <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <seealso cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection.Add'/>
        public void AddRange(FailoverConfigurationCollection value)
        {
            for (int i = 0; (i < value.Count); i = (i + 1))
            {
                Add(value[i]);
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether the 
        ///    <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/> contains the specified <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/>.</para>
        /// </summary>
        /// <param name='value'>The <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/> to locate.</param>
        /// <returns>
        /// <para><see langword='true'/> if the <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/> is contained in the collection; 
        ///   otherwise, <see langword='false'/>.</para>
        /// </returns>
        /// <seealso cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection.IndexOf'/>
        public bool Contains(FailoverConfiguration value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// <para>Copies the <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </summary>
        /// <param name='array'><para>The one-dimensional <see cref='System.Array'/> that is the destination of the values copied from <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/> .</para></param>
        /// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <exception cref='System.ArgumentException'><para><paramref name='array'/> is multidimensional.</para> <para>-or-</para> <para>The number of elements in the <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/> is greater than the available space between <paramref name='arrayIndex'/> and the end of <paramref name='array'/>.</para></exception>
        /// <exception cref='System.ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='arrayIndex'/> is less than <paramref name='array'/>'s lowbound. </exception>
        /// <seealso cref='System.Array'/>
        public void CopyTo(FailoverConfiguration[] array, int index)
        {
            List.CopyTo(array, index);
        }

        /// <summary>
        ///    <para>Returns the index of a <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/> in 
        ///       the <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/> to locate.</param>
        /// <returns>
        /// <para>The index of the <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/> of <paramref name='value'/> in the 
        /// <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/>, if found; otherwise, -1.</para>
        /// </returns>
        /// <seealso cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection.Contains'/>
        public int IndexOf(FailoverConfiguration value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// <para>Inserts a <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/> into the <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/> at the specified index.</para>
        /// </summary>
        /// <param name='index'>The zero-based index where <paramref name='value'/> should be inserted.</param>
        /// <param name=' value'>The <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/> to insert.</param>
        /// <returns><para>None.</para></returns>
        /// <seealso cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection.Add'/>
        public void Insert(int index, FailoverConfiguration value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        ///    <para>Returns an enumerator that can iterate through 
        ///       the <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/> .</para>
        /// </summary>
        /// <returns><para>None.</para></returns>
        /// <seealso cref='System.Collections.IEnumerator'/>
        public new FailoverConfigurationEnumerator GetEnumerator()
        {
            return new FailoverConfigurationEnumerator(this);
        }

        /// <summary>
        ///    <para> Removes a specific <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/> from the 
        ///    <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfiguration'/> to remove from the <see cref='eurocommerce.ie.architecture.root.configuration.FailoverConfigurationCollection'/> .</param>
        /// <returns><para>None.</para></returns>
        /// <exception cref='System.ArgumentException'><paramref name='value'/> is not found in the Collection. </exception>
        public void Remove(FailoverConfiguration value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Gets an entry for the supplied name.
        /// </summary>
        /// <param name="name">Entry name.</param>
        /// <returns>Entry if exists or null otherwise.</returns>
        public FailoverConfiguration GetEntry(string name)
        {
            FailoverConfigurationEnumerator ce = GetEnumerator();
            while (ce.MoveNext())
            {
                if (ce.Current.Name == name) return ce.Current;
            }
            return null;
        }

        #endregion

        #region FailoverConfigurationEnumerator class

        public class FailoverConfigurationEnumerator : object, IEnumerator
        {
            #region Fields

            private readonly IEnumerator baseEnumerator;
            private readonly IEnumerable temp;

            #endregion

            #region Constructors

            public FailoverConfigurationEnumerator(FailoverConfigurationCollection mappings)
            {
                temp = ((mappings));
                baseEnumerator = temp.GetEnumerator();
            }

            #endregion

            #region Properties

            public FailoverConfiguration Current
            {
                get { return ((FailoverConfiguration) (baseEnumerator.Current)); }
            }

            #endregion

            #region IEnumerator implementation

            object IEnumerator.Current
            {
                get { return baseEnumerator.Current; }
            }

            bool IEnumerator.MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            void IEnumerator.Reset()
            {
                baseEnumerator.Reset();
            }

            #endregion

            #region Methods

            public bool MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            public void Reset()
            {
                baseEnumerator.Reset();
            }

            #endregion
        }

        #endregion
    }
}