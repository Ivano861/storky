using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flyer.Structures
{
    public sealed class Subscriptions : IList<ISubscription>, IReadOnlyList<ISubscription>
    {
        private List<ISubscription> _list;

        public Subscriptions()
        {
            _list = new List<ISubscription>();
        }

        public ISubscription this[int index] { get => ((IList<ISubscription>)_list)[index]; set => ((IList<ISubscription>)_list)[index] = value; }

        public int Count => ((IList<ISubscription>)_list).Count;

        public bool IsReadOnly => ((IList<ISubscription>)_list).IsReadOnly;

        public void Add(ISubscription item)
        {
            ((IList<ISubscription>)_list).Add(item);
        }

        public void Clear()
        {
            ((IList<ISubscription>)_list).Clear();
        }

        public bool Contains(ISubscription item)
        {
            return ((IList<ISubscription>)_list).Contains(item);
        }

        public void CopyTo(ISubscription[] array, int arrayIndex)
        {
            ((IList<ISubscription>)_list).CopyTo(array, arrayIndex);
        }

        public int IndexOf(ISubscription item)
        {
            return ((IList<ISubscription>)_list).IndexOf(item);
        }

        public void Insert(int index, ISubscription item)
        {
            ((IList<ISubscription>)_list).Insert(index, item);
        }

        public bool Remove(ISubscription item)
        {
            return ((IList<ISubscription>)_list).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<ISubscription>)_list).RemoveAt(index);
        }

        public IEnumerator<ISubscription> GetEnumerator()
        {
            return ((IList<ISubscription>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<ISubscription>)_list).GetEnumerator();
        }
    }
}
