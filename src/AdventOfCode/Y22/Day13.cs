using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace AdventOfCode.Y22
{
    internal sealed class Day13
    {
        private record CustomIndex(int index, bool needIncrementation);

        public static string FirstPart()
        {
            var inputs = File.ReadAllLines("Y22/day13_input.txt");

            var count = 0;
            for (int i = 0; i < inputs.Length; i += 3)
            {
                var isCorrect = IsCorrectOrder(inputs[i], inputs[i + 1]);
                if (isCorrect)
                    count += i / 3 + 1;
            }


            return count.ToString();
        }


        public static string SecondPart()
        {
            var item1 = "[[2]]";
            var item2 = "[[6]]";

            var inputs = File.ReadAllLines("Y22/day13_input.txt").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            inputs.AddRange(new string[] { item1, item2 });

            for (int i = 0; i + 1 < inputs.Count; i++)
            {
                var isCorrect = IsCorrectOrder(inputs[i], inputs[i + 1]);
                if (!isCorrect)
                {
                    // basic sort algorithm
                    //(inputs[i], inputs[i + 1]) = (inputs[i + 1], inputs[i]);
                    string coup = inputs[i];
                    inputs[i] = inputs[i + 1];
                    inputs[i + 1] = coup;

                    i = i == 0 ? -1 : i - 2;
                }
            }

            var position1 = inputs.IndexOf(item1) + 1;
            var position2 = inputs.IndexOf(item2) + 1;
            return (position1 * position2).ToString();
        }


        private static bool IsCorrectOrder(string line1, string line2)
        {
            var a = Newtonsoft.Json.JsonConvert.DeserializeObject(line1) ?? throw new ArgumentException();
            var b = Newtonsoft.Json.JsonConvert.DeserializeObject(line2) ?? throw new ArgumentException();

            return CheckOrder(a, b) ?? true;
        }

        // will return true if the values are in the right order or null if indecisive
        private static bool? CheckOrder(object left, object right)
        {
            //var isLeftInteger = left is int leftInt;
            //var isRightInteger = right is int rightInt;


            // if both values are integers
            if (left is JValue leftValue && right is JValue rightValue)
            {
                var leftInt = (int)leftValue;
                var rightInt = (int)rightValue;
                //var isLeftArray = left is Array leftArray;
                //var isRightArray = right is Array rightArray;
                if (leftInt == rightInt)
                    return null;
                else if (leftInt < rightInt)
                    return true;
                else
                    return false;
            }


            // both are lists
            else if (left is JArray leftArray && right is JArray rightArray)
            {
                // - compare each values (seems recursive)
                // - if left run out of values, is in right order
                int minLength = Math.Min(leftArray.Count, rightArray.Count);
                for (int i = 0; i < minLength; i++)
                {
                    var checkResult = CheckOrder(leftArray[i], rightArray[i]);
                    if (checkResult != null)
                        return checkResult;
                }
                if (leftArray.Count == rightArray.Count)
                    return null;
                return leftArray.Count == minLength;
            }


            // if one of the values is integer
            else if (left is JArray && right is JValue rightValueSecond)
            {
                // - convert integer to a list and compare
                var rightIntSecond = (int)rightValueSecond;
                return CheckOrder(left, new JArray { rightIntSecond });
            }
            else if (left is JValue leftValueSecond && right is JArray)
            {
                // - convert integer to a list and compare
                var leftIntSecond = (int)leftValueSecond;
                return CheckOrder(new JArray { leftIntSecond }, right);
            }

            throw new UnreachableException("You shoud've check all possible values at this point, or the input was invalid");
        }

    }
}
