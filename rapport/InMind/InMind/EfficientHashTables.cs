/*
 *
 * Copyright (C) Carnegie Mellon University - All Rights Reserved.
 * Unauthorized copying of this file, via any medium is strictly prohibited.
 * This is proprietary and confidential.
 * Written by members of the ArticuLab, directed by Justine Cassell, 2014.
 * 
 * Author: Alexandros Papangelis, apapa@cs.cmu.edu
 * 
 */

/*
 * REFERENCES: http://pastebin.com/Aq2L7NxQ
 *             http://stackoverflow.com/questions/6013420/a-sparse-map-data-type-rle-like-for-c-c-or-net
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMind
{
    public class Efficient64bitHashTable<T>
    {
        private class element
        {
            public ulong _key;
            public T _value;
        };
        private element[][] _buckets;
        private uint _capacity;

        public Efficient64bitHashTable()
        {
            _capacity = 214373;    // some prime number
            _buckets = new element[_capacity][];
        }
        public Efficient64bitHashTable(uint capacity)
        {
            _capacity = capacity;
            _buckets = new element[_capacity][];
        }

        public uint hash(ulong key)
        {
            return (uint)(key % _capacity);
        }

        public void Add(ulong key, T value)
        {
            uint hsh = hash(key);
            element[] e;
            if (_buckets[hsh] == null)
                _buckets[hsh] = e = new element[1];
            else
            {
                foreach (var elem in _buckets[hsh])
                    if (elem._key == key)
                    {
                        elem._value = value;
                        return;
                    }
                e = new element[_buckets[hsh].Length + 1];
                Array.Copy(_buckets[hsh], 0, e, 1, _buckets[hsh].Length);
                _buckets[hsh] = e;
            }
            e[0] = new element { _key = key, _value = value };
        }

        public T Get(ulong key)
        {
            uint hsh = hash(key);
            element[] e = _buckets[hsh];
            if (e == null) return default(T);
            foreach (var f in e)
                if (f._key == key)
                    return f._value;
            return default(T);
        }

        public bool Has(ulong key)
        {
            uint hsh = hash(key);
            element[] e = _buckets[hsh];
            if (e == null) return false;
            foreach (var f in e)
                if (f._key == key)
                    return true;
            return false;
        }

        public int Count()
        {
            int r = 0;
            foreach (var e in _buckets)
                if (e != null)
                    r += e.Length;
            return r;
        }
    }

    public class Efficient32bitHashTableInt
    {
        private class element
        {
            public uint _key;
            public int _value;
        };
        private element[][] _buckets;
        private uint _capacity;

        public Efficient32bitHashTableInt()
        {
            _capacity = 463;    // some prime number
            _buckets = new element[_capacity][];
        }
        public Efficient32bitHashTableInt(uint capacity)
        {
            _capacity = capacity;
            _buckets = new element[_capacity][];
        }

        public uint hash(uint key)
        {
            return (uint)(key % _capacity);
        }

        public void Add(uint key, int value)
        {
            uint hsh = hash(key);
            element[] e;
            if (_buckets[hsh] == null)
                _buckets[hsh] = e = new element[1];
            else
            {
                foreach (var elem in _buckets[hsh])
                    if (elem._key == key)
                    {
                        elem._value = value;
                        return;
                    }
                e = new element[_buckets[hsh].Length + 1];
                Array.Copy(_buckets[hsh], 0, e, 1, _buckets[hsh].Length);
                _buckets[hsh] = e;
            }
            e[0] = new element { _key = key, _value = value };
        }

        public void Inc(uint key)
        {
            uint hsh = hash(key);
            if (_buckets[hsh] == null)
            {
                _buckets[hsh] = new element[1] { new element { _key = key, _value = 1 } };
                return;
            }

            foreach (var elem in _buckets[hsh])
                if (elem._key == key)
                {
                    elem._value++;
                    return;
                }

            var e = new element[_buckets[hsh].Length + 1];
            Array.Copy(_buckets[hsh], 0, e, 1, _buckets[hsh].Length);
            _buckets[hsh] = e;
            e[0] = new element { _key = key, _value = 1 };
        }

        public int Get(uint key)
        {
            uint hsh = hash(key);
            element[] e = _buckets[hsh];
            if (e == null) return 0;
            foreach (var f in e)
                if (f._key == key)
                    return f._value;
            return 0;
        }

        public int Count()
        {
            int r = 0;
            foreach (var e in _buckets)
                if (e != null)
                    r += e.Length;
            return r;
        }

        public uint Max()
        {
            uint maxKey = 0;
            int maxValue = 0;
            for (int i = 0; i < _buckets.Length; i++)
                if (_buckets[i] != null)
                    for (int j = 0; j < _buckets[i].Length; j++)
                        if (_buckets[i][j]._value > maxValue)
                        {
                            maxValue = _buckets[i][j]._value;
                            maxKey = _buckets[i][j]._key;
                        }
            return maxKey;
        }
    }
}