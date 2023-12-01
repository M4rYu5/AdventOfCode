using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Y22
{
    internal sealed class Day1
    {
        public static string FirstPart()
        {
            var input = File.ReadAllLines("Y22/day1_input.txt");

            int max = 0;
            int currentCount = 0;
            foreach (var line in input)
            {
                if (line == "")
                {
                    if (currentCount > max)
                        max = currentCount;

                    currentCount = 0;
                    continue;
                }

                if (int.TryParse(line, out int itemCalories))
                {
                    currentCount += itemCalories;
                }
            }


            return max.ToString();
        }

        public static string SecondPart()
        {
            var input = File.ReadAllLines("Y22/day1_input.txt");

            SortedSet<int> max = new SortedSet<int>();

            int currentCount = 0;
            foreach (var line in input)
            {
                if (line == "")
                {
                    max.Add(currentCount);
                    currentCount = 0;
                    continue;
                }

                if (int.TryParse(line, out int itemCalories))
                {
                    currentCount += itemCalories;
                }
            }

            int first3Added = 0;
            foreach (var item in max.OrderDescending().Take(..3))
                first3Added += item;

            return first3Added.ToString();
        }

    }
}
