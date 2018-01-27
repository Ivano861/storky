using System.Collections;
using System.Collections.Generic;

namespace Storky
{
    internal class Couplings : IList<Coupling>
    {
        #region Private members
        private List<Coupling> _list;
        #endregion

        #region Constructors
        internal Couplings()
        {
            _list = new List<Coupling>();
        }

        public Coupling this[int index] { get => ((IList<Coupling>)_list)[index]; set => ((IList<Coupling>)_list)[index] = value; }
        #endregion

        #region IList<Coupling> interface implementation
        public int Count => ((ICollection<Coupling>)_list).Count;

        public bool IsReadOnly => ((ICollection<Coupling>)_list).IsReadOnly;

        public void Add(Coupling item)
        {
            ((ICollection<Coupling>)_list).Add(item);
        }

        public void Clear()
        {
            ((ICollection<Coupling>)_list).Clear();
        }

        public bool Contains(Coupling item)
        {
            return ((ICollection<Coupling>)_list).Contains(item);
        }

        public void CopyTo(Coupling[] array, int arrayIndex)
        {
            ((ICollection<Coupling>)_list).CopyTo(array, arrayIndex);
        }

        public IEnumerator<Coupling> GetEnumerator()
        {
            return ((ICollection<Coupling>)_list).GetEnumerator();
        }

        public int IndexOf(Coupling item)
        {
            return ((IList<Coupling>)_list).IndexOf(item);
        }

        public void Insert(int index, Coupling item)
        {
            ((IList<Coupling>)_list).Insert(index, item);
        }

        public bool Remove(Coupling item)
        {
            return ((ICollection<Coupling>)_list).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<Coupling>)_list).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<Coupling>)_list).GetEnumerator();
        }
        #endregion
    }
}
