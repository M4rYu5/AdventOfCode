using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Y22
{
    internal sealed class Day5
    {
        public static string FirstPart()
        {
            var inputs = File.ReadAllLines("Y22/day5_input.txt");

            var (stacks, _, stackNameColumn) = GetInitialStacks(inputs);
            MoveStacks(inputs, stacks, stackNameColumn);


            string topStack = "";
            foreach (var stack in stacks)
            {
                topStack += stack.Peek();
            }

            return topStack;
        }
        public static string SecondPart()
        {
            var inputs = File.ReadAllLines("Y22/day5_input.txt");

            var (stacks, numOfStacks, stackNameColumn) = GetInitialStacks(inputs);
            MoveStacks2(inputs, stacks, stackNameColumn);


            string topStack = "";
            foreach (var stack in stacks)
            {
                topStack += stack.Peek();
            }

            return topStack;
        }

        private static void MoveStacks(string[] inputs, Stack<char>[] stacks, int stackNameColumn)
        {
            foreach (var move in inputs.Skip(stackNameColumn + 2))
            {
                var command = GetMoveCommand(move);
                for (int i = 0; i < command.count; i++)
                {
                    var item = stacks[command.fromIndex].Pop();
                    stacks[command.toIndex].Push(item);
                }
            }
        }

        private static void MoveStacks2(string[] inputs, Stack<char>[] stacks, int stackNameColumn)
        {
            foreach (var move in inputs.Skip(stackNameColumn + 2))
            {
                var command = GetMoveCommand(move);
                char[] partial = new char[command.count];
                for (int i = 0; i < command.count; i++)
                {
                    var item = stacks[command.fromIndex].Pop();
                    partial[i] = item;
                }
                for (int i = partial.Length - 1; i >= 0; i--)
                {
                    stacks[command.toIndex].Push(partial[i]);
                }
            }
        }

        private static (int fromIndex, int toIndex, int count) GetMoveCommand(string command)
        {
            var splittedCommand = command.Split(new string[] { "move ", " from ", " to " }, StringSplitOptions.RemoveEmptyEntries);
            return (int.Parse(splittedCommand[1]) - 1, int.Parse(splittedCommand[2]) - 1, int.Parse(splittedCommand[0]));
        }

        private static (Stack<char>[] stacks, int numOfStacks, int stackNameColumn) GetInitialStacks(string[] inputs)
        {
            (int numOfStacks, int stackNameColumn) = GetNumOfStacks(inputs);
            Stack<char>[] stacks = new Stack<char>[numOfStacks];
            for (int i = 0; i < stacks.Length; i++)
            {
                stacks[i] = new Stack<char>();
            }

            for (int i = stackNameColumn - 1; i >= 0; i--)
            {
                for (int j = 0; j <= inputs[i].Length / 4; j++)
                {
                    char chr = inputs[i][1 + j * 4];
                    if (char.IsLetter(chr))
                    {
                        stacks[j].Push(chr);
                    }
                }

            }
            return (stacks, numOfStacks, stackNameColumn);
        }

        private static (int numOfStacks, int columnIndex) GetNumOfStacks(string[] inputs)
        {
            var numOfStacks = 0;
            var index = 0;
            foreach (var input in inputs)
            {
                if (input.Length > 1 && int.TryParse(input[1].ToString(), out _))
                {
                    numOfStacks = input.Split("   ").Length;
                    break;
                }
                index++;
            }
            return (numOfStacks, index);
        }

    }
}
