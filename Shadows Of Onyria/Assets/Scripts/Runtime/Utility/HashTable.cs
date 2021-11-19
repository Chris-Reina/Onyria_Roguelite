using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoaT.Experimental
{
    [Serializable]
    public class HashTable<TKey, TValue>: IEnumerable<KeyValuePair<TKey,TValue>>
    {
        private const float LOAD_FACTOR_LIMIT = 0.75f;
            
        public int Count { get; private set; } = 0;
        public List<TKey> Keys => GetKeys();
        public List<TValue> Values => GetValues();
        private float LoadFactor => (float) _occupiedBucketEntries / _numberOfBuckets;

        [NonSerialized] private LinkedList<KeyValuePair<TKey, TValue>>[] _buckets;
        private int _occupiedBucketEntries = 0;
        private int _numberOfBuckets = 10;

        public HashTable()
        {
            _buckets = new LinkedList<KeyValuePair<TKey, TValue>>[_numberOfBuckets];
            for (int i = 0; i < _numberOfBuckets; i++)
            {
                _buckets[i] = new LinkedList<KeyValuePair<TKey, TValue>>();
            }
        }

        public HashTable(int initialCapacity)
        {
            _numberOfBuckets = initialCapacity;
            _buckets = new LinkedList<KeyValuePair<TKey, TValue>>[_numberOfBuckets];
            for (int i = 0; i < _numberOfBuckets; i++)
            {
                _buckets[i] = new LinkedList<KeyValuePair<TKey, TValue>>();
            }
        }
        
        public void Add(TKey key, TValue value)
        {
            if (key == null || value == null)
                throw new ArgumentNullException();
            
            if (LoadFactor >= LOAD_FACTOR_LIMIT)
            {
                ExpandTable();
            }

            InternalAdd(_buckets, key, value);
        }
        
        public void Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException();

            var index = Mathf.Abs(key.GetHashCode() % _numberOfBuckets);
            var buckets = _buckets[index];

            if (buckets == null || buckets.Count == 0) return;

            KeyValuePair<TKey, TValue> toRemove;
            foreach (var keyValuePair in buckets)
            {
                if (!Equals(keyValuePair.Key, key)) continue;
                
                toRemove = keyValuePair;
                break;
            }
            
            buckets.Remove(toRemove);
            Count -= 1;
        }

        public void Clear()
        {
            Count = 0;
            _occupiedBucketEntries = 0;
            _numberOfBuckets = 10;
            _buckets = new LinkedList<KeyValuePair<TKey, TValue>>[_numberOfBuckets];
        }

        public bool ContainsKey(TKey key)
        {
            if (_occupiedBucketEntries == 0) return false;
            
            var index = Mathf.Abs(key.GetHashCode() % _numberOfBuckets);

            if (_buckets[index] == null) return false;
            
            foreach (var keyValuePair in _buckets[index])
            {
                if (!Equals(keyValuePair.Key, key)) continue;
                return true;
            }

            return false;
        }

        public bool ContainsValue(TValue value)
        {
            if (_occupiedBucketEntries == 0) return false;
            
            foreach (var bucket in _buckets)
            {
                if(bucket.Count == 0) continue;
                
                foreach (var keyValuePair in bucket)
                {
                    if (Equals(keyValuePair.Value, value))
                    {
                        return true;
                    }
                }
            }

            return true;
        }

        private void ExpandTable()
        {
            Count = 0;
            _numberOfBuckets *= 2;
            _occupiedBucketEntries = 0;
            var newTable = new LinkedList<KeyValuePair<TKey, TValue>>[_numberOfBuckets];

            foreach (var bucket in _buckets)
            {
                if(bucket == null || bucket.Count == 0) continue;
                
                foreach (var kvp in bucket)
                {
                    InternalAdd(newTable, kvp.Key, kvp.Value);
                }
            }

            _buckets = newTable;
        }

        private void InternalAdd(LinkedList<KeyValuePair<TKey, TValue>>[] collection, TKey key, TValue value)
        {
            InternalAdd(collection, new KeyValuePair<TKey, TValue>(key,value));
        }
        private void InternalAdd(LinkedList<KeyValuePair<TKey, TValue>>[] collection, KeyValuePair<TKey, TValue> kvp)
        {
            var index = Mathf.Abs(kvp.Key.GetHashCode() % collection.Length);
            //index = index < 0 ? -index : index;
            var indexedLikenList = collection[index];
            
            
            if (indexedLikenList.Count == 0) _occupiedBucketEntries += 1;
            else if (indexedLikenList.Any(x => x.Equals(kvp.Key)))
                throw new ArgumentException("Duplicate Key Found");
                
            indexedLikenList.AddLast(kvp);
            Count += 1;
        }
        
        private List<TKey> GetKeys()
        {
            if (Count == 0 || _occupiedBucketEntries == 0) return new List<TKey>();

            var newList = new List<TKey>();
            
            foreach (var bucket in _buckets)
            {
                if (bucket == null || bucket.Count == 0) continue;

                foreach (var keyValuePair in bucket)
                {
                    newList.Add(keyValuePair.Key);
                }
            }
            return newList;
        }
        private List<TValue> GetValues()
        {
            if (Count == 0 || _occupiedBucketEntries == 0) return new List<TValue>();

            var newList = new List<TValue>();
            
            foreach (var bucket in _buckets)
            {
                if (bucket == null || bucket.Count == 0) continue;

                foreach (var keyValuePair in bucket)
                {
                    var value = keyValuePair.Value;
                    if(value == null) continue;
                    newList.Add(value);
                }
            }
            return newList;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var bucket in _buckets)
            {
                if (bucket == null || bucket.Count == 0) continue;
                foreach (var keyValuePair in bucket)
                {
                    yield return keyValuePair;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var str = "";

            foreach (var bucket in _buckets)
            {
                if (bucket == null || bucket.Count == 0) continue;
                foreach (var keyValuePair in bucket)
                {
                    str += keyValuePair + "\n";
                }
            }
            
            return str;
        }
    }
}