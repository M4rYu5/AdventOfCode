using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Day11
{
    internal class CustomComparerInt : IComparer<KeyValuePair<int, int>>
    {
        public int Compare(KeyValuePair<int, int> x, KeyValuePair<int, int> y)
        {
            if (x.Value < y.Value) return -1;
            if (x.Value > y.Value) return 1;
            return 0;
        }
    }
}
