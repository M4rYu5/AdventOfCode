using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal sealed class Day10
    {
        public static string FirstPart()
        {
            var inputs = File.ReadAllLines("day10_input.txt");

            var cycle = 0;
            var sum = 0;
            var xReg = 1;
            foreach(var operation in inputs)
            {
                if (operation == "noop")
                    IncrementCycle();
                else if (operation.StartsWith("addx "))
                {
                    for(int i = 0; i < 2; i ++)
                        IncrementCycle();
                    var number = operation[5..];
                    xReg += int.Parse(number);
                }
            }
            

            return sum.ToString();

            void IncrementCycle()
            {
                cycle++;
                if((cycle - 20) % 40 == 0 )
                {
                    sum += xReg * cycle;
                }
            }
        }



        public static string SecondPart()
        {
            var inputs = File.ReadAllLines("day10_input.txt");

            var cycle = 0;
            var sum = 0;
            var xReg = 1;
            foreach (var operation in inputs)
            {
                if (operation == "noop")
                    IncrementCycle();
                else if (operation.StartsWith("addx "))
                {
                    for (int i = 0; i < 2; i++)
                        IncrementCycle();
                    var number = operation[5..];
                    xReg += int.Parse(number);
                }
            }


            return "";

            void IncrementCycle()
            {
                var crtPosition = cycle % 40;
                cycle++;


                if ((cycle - 1) % 40 == 0)
                    Console.WriteLine();

                if (crtPosition <= xReg + 1 && crtPosition >= xReg - 1)
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write(".");
                }
            }
        }

    }
}
