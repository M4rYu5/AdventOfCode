using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Y23
{
    using NumberFouded = (bool found, int number);
    using NumberFoudedIndex = (bool found, int number, int index);

    public static class Day1
    {
        public static string FirstPart()
        {
            var input = File.ReadAllLines("Y23/day1_input.txt");

            int sum = 0;

            foreach (var line in input)
            {
                var leftNuber = new NumberFouded(false, 0);
                var rightNuber = new NumberFouded(false, 0);
                for (var i = 0; i < line.Length; i++)
                {
                    if (!leftNuber.found)
                    {
                        var lChar = line[i];
                        if (char.IsNumber(lChar))
                        {
                            leftNuber.found = true;
                            leftNuber.number = lChar - '0';
                        }
                    }
                    if (!rightNuber.found)
                    {
                        var rChar = line[^(i + 1)];
                        if (char.IsNumber(rChar))
                        {
                            rightNuber.found = true;
                            rightNuber.number = rChar - '0';
                        }
                    }
                    if (leftNuber.found && rightNuber.found)
                    {
                        break;
                    }
                }
                // right number will always be found if first is found,
                // so no need to worry about numbers of types X0
                sum += leftNuber.number * 10 + rightNuber.number;
            }

            return sum.ToString();
        }


        public static string SecondPart()
        {
            var input = File.ReadAllLines("Y23/day1_input.txt");

            int sum = 0;

            foreach (var line in input)
            {
                var leftNuber = new NumberFoudedIndex(false, 0, int.MaxValue);
                var rightNuber = new NumberFoudedIndex(false, 0, int.MinValue);
                for (var i = 0; i < line.Length; i++)
                {
                    if (!leftNuber.found)
                    {
                        var lChar = line[i];
                        if (char.IsNumber(lChar))
                        {
                            leftNuber = (true, lChar - '0', i);
                        }
                    }
                    if (!rightNuber.found)
                    {
                        var rChar = line[^(i + 1)];
                        if (char.IsNumber(rChar))
                        {
                            rightNuber = (true, rChar - '0', line.Length - i - 1);
                        }
                    }
                    if (leftNuber.found && rightNuber.found)
                    {
                        break;
                    }
                }
                var leftSpelledNumber = FirstSpelledNumber(line);
                var rightSpelledNumber = LastSpelledNumber(line);

                if (leftSpelledNumber.found && leftNuber.index > leftSpelledNumber.index)
                    leftNuber = leftSpelledNumber;
                if (rightSpelledNumber.found && rightNuber.index < rightSpelledNumber.index)
                    rightNuber = rightSpelledNumber;

                // right number will always be found if first is found,
                // so no need to worry about numbers of types X0 
                sum += leftNuber.number * 10 + rightNuber.number;
            }

            return sum.ToString(); // 53221
        }

        private static readonly string[] spelledNumbers = [
            "zero",
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine"
        ];

        private static NumberFoudedIndex FirstSpelledNumber(string text)
        {
            NumberFoudedIndex number = (false, 0, int.MaxValue);
            for (var i = 0; i < spelledNumbers.Length; i++)
            {
                var index = text.IndexOf(spelledNumbers[i]);
                if (index > -1 && index < number.index)
                {
                    number = (true, i, index);
                }
            }
            return number;
        }
        
        private static NumberFoudedIndex LastSpelledNumber(string text)
        {
            NumberFoudedIndex number = (false, text.Length, int.MinValue);
            for (var i = 0; i < spelledNumbers.Length; i++)
            {
                var index = text.LastIndexOf(spelledNumbers[i]);
                if (index > -1 && index > number.index)
                {
                    number = (true, i, index);
                }
            }
            return number;
        }

    }
}
