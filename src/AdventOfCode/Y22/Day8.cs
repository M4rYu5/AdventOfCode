using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Y22
{
    internal sealed class Day8
    {
        public static string FirstPart()
        {
            var inputs = File.ReadAllLines("Y22/day8_input.txt");

            var visibleTrees = inputs.Length * 2 + inputs[0].Length + inputs[^1].Length - 4;
            for (int i = 1; i + 1 < inputs.Length; i++)
            {
                for (int j = 1; j < inputs[i].Length - 1; j++)
                {
                    if (IsTreeVisible(inputs, i, j))
                        visibleTrees++;
                }
            }

            return visibleTrees.ToString();
        }

        private static bool IsTreeVisible(string[] inputs, int row, int column)
        {
            return IsVisibleFromTop(inputs, row, column) || IsVisibleFromBottom(inputs, row, column) || IsVisibleFromLeft(inputs, row, column) || IsVisibleFromRight(inputs, row, column);
        }

        private static bool IsVisibleFromRight(string[] inputs, int row, int column)
        {
            for (int i = column + 1; i < inputs[row].Length; i++)
            {
                if (int.Parse(inputs[row][i].ToString()) >= int.Parse(inputs[row][column].ToString()))
                    return false;
            }
            return true;
        }

        private static bool IsVisibleFromLeft(string[] inputs, int row, int column)
        {
            for (int i = 0; i < column; i++)
            {
                if (int.Parse(inputs[row][i].ToString()) >= int.Parse(inputs[row][column].ToString()))
                    return false;
            }
            return true;
        }

        private static bool IsVisibleFromBottom(string[] inputs, int row, int column)
        {
            for (int i = row + 1; i < inputs.Length; i++)
            {
                if (int.Parse(inputs[i][column].ToString()) >= int.Parse(inputs[row][column].ToString()))
                    return false;
            }
            return true;
        }

        private static bool IsVisibleFromTop(string[] inputs, int row, int column)
        {
            for (int i = 0; i < row; i++)
            {
                if (int.Parse(inputs[i][column].ToString()) >= int.Parse(inputs[row][column].ToString()))
                    return false;
            }
            return true;
        }





        public static string SecondPart()
        {
            var inputs = File.ReadAllLines("Y22/day8_input.txt");

            var bestScenicScore = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs[i].Length; j++)
                {
                    var scenicScore = GetScenicScore(inputs, i, j);
                    bestScenicScore = bestScenicScore < scenicScore ? scenicScore : bestScenicScore;
                }
            }

            return bestScenicScore.ToString();
        }

        private static int GetScenicScore(string[] inputs, int row, int column)
        {
            var top = GetScenicScoreTop(inputs, row, column);
            var bottom = GetScenicScoreBottom(inputs, row, column);
            var left = GetScenicScoreLeft(inputs, row, column);
            var right = GetScenicScoreRight(inputs, row, column);
            return top * bottom * left * right;
        }

        private static int GetScenicScoreRight(string[] inputs, int row, int column)
        {
            var count = 0;
            var currentTree = int.Parse(inputs[row][column].ToString());
            for (int i = column + 1; i < inputs[row].Length; i++)
            {
                var tree = int.Parse(inputs[row][i].ToString());
                if (tree < currentTree)
                    count++;
                else
                {
                    count++;
                    break;
                }
            }
            return count;
        }

        private static int GetScenicScoreLeft(string[] inputs, int row, int column)
        {
            var count = 0;
            var currentTree = int.Parse(inputs[row][column].ToString());
            for (int i = column - 1; i >= 0; i--)
            {
                var tree = int.Parse(inputs[row][i].ToString());
                if (tree < currentTree)
                    count++;
                else
                {
                    count++;
                    break;
                }
            }
            return count;
        }

        private static int GetScenicScoreBottom(string[] inputs, int row, int column)
        {
            var count = 0;
            var currentTree = int.Parse(inputs[row][column].ToString());
            for (int i = row + 1; i < inputs.Length; i++)
            {
                var tree = int.Parse(inputs[i][column].ToString());
                if (tree < currentTree)
                    count++;
                else
                {
                    count++;
                    break;
                }
            }
            return count;
        }

        private static int GetScenicScoreTop(string[] inputs, int row, int column)
        {
            var count = 0;
            var currentTree = int.Parse(inputs[row][column].ToString());
            for (int i = row - 1; i >= 0; i--)
            {
                var tree = int.Parse(inputs[i][column].ToString());
                if (tree < currentTree)
                    count++;
                else
                {
                    count++;
                    break;
                }
            }
            return count;
        }


    }
}
