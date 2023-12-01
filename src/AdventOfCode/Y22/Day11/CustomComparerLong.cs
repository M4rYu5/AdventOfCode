using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Y22.Day11
{
    internal class CustomComparerLong : IComparer<KeyValuePair<int, long>>
    {
        public int Compare(KeyValuePair<int, long> x, KeyValuePair<int, long> y)
        {
            if (x.Value < y.Value) return -1;
            if (x.Value > y.Value) return 1;
            return 0;
        }
    }
}
