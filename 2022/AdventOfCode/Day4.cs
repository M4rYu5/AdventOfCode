using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal sealed class Day4
    {
        public static string FirstPart()
        {
            var inputs = File.ReadAllLines("day4_input.txt");

            int fullyContainedCount = 0;
            foreach(var input in inputs)
            {
                var elfSection = input.Split(',');
                int[] pairs = new int[4];
                var firstPairRange = elfSection[0].Split('-');
                pairs[0] = int.Parse(firstPairRange[0]);
                pairs[1] = int.Parse(firstPairRange[1]);

                var secondPairRange = elfSection[1].Split('-');
                pairs[2] = int.Parse(secondPairRange[0]);
                pairs[3] = int.Parse(secondPairRange[1]);

                if (pairs[0] <= pairs[2] && pairs[1] >= pairs[3]
                    || (pairs[0] >= pairs[2] && pairs[1] <= pairs[3]))
                    fullyContainedCount++;
            }
            

            return fullyContainedCount.ToString();
        }



        public static string SecondPart()
        {
            var inputs = File.ReadAllLines("day4_input.txt");

            int fullyContainedCount = 0;
            foreach (var input in inputs)
            {
                var elfSection = input.Split(',');
                int[] pairs = new int[4];
                var firstPairRange = elfSection[0].Split('-');
                pairs[0] = int.Parse(firstPairRange[0]);
                pairs[1] = int.Parse(firstPairRange[1]);

                var secondPairRange = elfSection[1].Split('-');
                pairs[2] = int.Parse(secondPairRange[0]);
                pairs[3] = int.Parse(secondPairRange[1]);


                if (IsFirstIntersectingSecondOnTheEndOfFirst(pairs[0], pairs[1], pairs[2], pairs[3])
                    || IsFirstIntersectingSecondOnTheStartOfFirst(pairs[0], pairs[1], pairs[2], pairs[3])
                    || IsFirstInsideSecond(pairs[0], pairs[1], pairs[2], pairs[3])
                    || IsSecondInsideFirst(pairs[0], pairs[1], pairs[2], pairs[3]))
                    fullyContainedCount++;
            }


            return fullyContainedCount.ToString();
        }

        private static bool IsFirstIntersectingSecondOnTheEndOfFirst(int firstStart, int firstEnd, int secondStart, int secondEnd)
        {
            return firstStart <= secondStart && firstEnd >= secondStart;
        }

        private static bool IsFirstIntersectingSecondOnTheStartOfFirst(int firstStart, int firstEnd, int secondStart, int secondEnd)
        {
            return secondStart <= firstStart && secondEnd >= firstStart;
        }

        private static bool IsFirstInsideSecond(int firstStart, int firstEnd, int secondStart, int secondEnd)
        {
            return firstStart >= secondStart && firstEnd <= secondEnd;
        }

        private static bool IsSecondInsideFirst(int firstStart, int firstEnd, int secondStart, int secondEnd)
        {
            return firstStart <= secondStart && firstEnd >= secondEnd;
        }

    }
}
