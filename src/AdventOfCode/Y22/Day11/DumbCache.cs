using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Y22.Day11
{

    internal class DumbItemCache
    {
        public record struct Value(int Index, long ModValue);
        public record struct Key(int mod);
        private readonly Dictionary<Key, Value> dumbCache = new();


        public void Cache(Key key, Value value)
        {
            dumbCache[key] = value;
        }

        public Value? Get(Key key)
        {
            if (dumbCache.TryGetValue(key, out Value value))
                return value;
            return null;
        }

    }
}
