using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Y22
{
    internal sealed class Day6
    {
        public static string FirstPart()
        {
            var inputs = File.ReadAllLines("Y22/day6_input.txt");
            int index = 0;

            foreach (var input in inputs)
            {
                Queue<char> chars = new Queue<char>();
                foreach (var c in input)
                {
                    if (chars.Count >= 4)
                        chars.Dequeue();
                    index++;
                    chars.Enqueue(c);
                    if (chars.Count == 4 && chars.Distinct().Count() == chars.Count)
                    {
                        return index.ToString();
                    }
                }
            }


            return index.ToString();
        }



        public static string SecondPart()
        {
            var inputs = File.ReadAllLines("Y22/day6_input.txt");
            int index = 0;

            foreach (var input in inputs)
            {
                Queue<char> chars = new Queue<char>();
                foreach (var c in input)
                {
                    if (chars.Count >= 14)
                        chars.Dequeue();
                    index++;
                    chars.Enqueue(c);
                    if (chars.Count == 14 && chars.Distinct().Count() == chars.Count)
                    {
                        return index.ToString();
                    }
                }
            }


            return index.ToString();
        }

    }
}
