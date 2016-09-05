using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Core
{
    public class Metadata : IList<Metadata.Entry>
    {
        public Entry this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Add(Entry item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(Entry item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Entry[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Entry> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(Entry item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, Entry item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Entry item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public class Entry
        {
        }
    }
}
