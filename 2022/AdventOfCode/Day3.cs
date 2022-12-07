using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal sealed class Day3
    {
        public static string FirstPart()
        {
            var inputs = File.ReadAllLines("day3_input.txt");

            int points = 0;
            foreach(var input in inputs)
            {
                char c = GetCommonChar(input);
                int cPoint = GetPointsFromChar(c);
                points += cPoint;
            }


            return points.ToString();
        }



        public static string SecondPart()
        {
            var inputs = File.ReadAllLines("day3_input.txt");

            
            int points = 0;
            for(int i = 0; i + 3 <= inputs.Length; i += 3)
            {
                bool found = false;
                foreach(char c1 in inputs[i])
                {
                    foreach(char c2 in inputs[i + 1])
                    {
                        foreach(char c3 in inputs[i + 2])
                        {
                            if(c1 == c2 && c1 == c3)
                            {
                                points += GetPointsFromChar(c1);
                                found = true;
                                break;
                            }
                        }
                        if (found)
                            break;
                    }
                    if (found)
                        break;
                }
            }


            return points.ToString();
        }



        private static int GetPointsFromChar(char c)
        {
            if (char.IsLower(c))
                return (int)c - 96;
            if (char.IsUpper(c))
                return (int)c - 38;
            return 0;
        }


        private static char GetCommonChar(string input)
        {
            foreach (char c1 in input.Take(input.Length / 2))
            {
                foreach (char c2 in input.Skip(input.Length / 2))
                {
                    if (c1 == c2)
                        return c1;
                }
            }
            return '-';
        }

    }
}
