using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Y23
{

    public static class Day4
    {
        public static string FirstPart()
        {
            var input = File.ReadAllLines("Y23/day4_input.txt");

            var sum = 0;
            foreach (var line in input)
            {
                var localPoints = 0; // 1 for first match then doubles
                List<int> winningNumbers = [];
                List<int> myNumbers = [];
                {
                    var linesNumbers = line.Split(": ")[1].Split(" | ");
                    var winningNumbersString = linesNumbers[0].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    var myNumbersString = linesNumbers[1].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    foreach (var winningNumberString in winningNumbersString)
                    {
                        winningNumbers.Add(int.Parse(winningNumberString));
                    }
                    foreach (var myNumberString in myNumbersString)
                    {
                        myNumbers.Add(int.Parse(myNumberString));
                    }

                }
                foreach (var number in myNumbers)
                {
                    if (winningNumbers.Contains(number))
                    {
                        if (localPoints == 0)
                            localPoints = 1;
                        else
                            localPoints *= 2;
                    }
                }
                sum += localPoints;
            }

            return sum.ToString();
        }


        public static string SecondPart()
        {
            var input = File.ReadAllLines("Y23/day4_input.txt");

            int[] copyOfCards = new int[input.Length];
            Array.Fill(copyOfCards, 1);
            for (var index = 0; index < input.Length; index++)
            {
                var line = input[index];

                var wonTickets = 0; // 1 for first match then doubles
                List<int> winningNumbers = [];
                List<int> myNumbers = [];
                {
                    var linesNumbers = line.Split(": ")[1].Split(" | ");
                    var winningNumbersString = linesNumbers[0].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    var myNumbersString = linesNumbers[1].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    foreach (var winningNumberString in winningNumbersString)
                    {
                        winningNumbers.Add(int.Parse(winningNumberString));
                    }
                    foreach (var myNumberString in myNumbersString)
                    {
                        myNumbers.Add(int.Parse(myNumberString));
                    }

                }

                wonTickets = myNumbers.Where(x => winningNumbers.Contains(x)).Count();
                for (int i = index + 1; i < index + wonTickets + 1; i++)
                {
                    copyOfCards[i] += copyOfCards[index];
                }
            }

            return copyOfCards.Sum().ToString();
        }
    }
}
