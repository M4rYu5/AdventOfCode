using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Y22.Day11
{
    internal static class Extensions
    {
        // create Add method extension for Queue to be able initialize the queue with q = new[] { new T }
        public static void Add<T>(this Queue<T> queue, T item) => queue.Enqueue(item);

    }
}
