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

    public static class Day3
    {
        /// <summary>
        /// (0, 0) on top-right
        /// </summary>
        record struct Point_D3(int HorizontalIndex, int VerticalIndex);
        record PointValue_D3(Point_D3 Point, int Value);
        record Size_D3(int Widht, int Height)
        {
            public List<Point_D3> GetAdjent(Point_D3 point)
            {
                List<Point_D3> validPoints = [];
                // left
                if (point.HorizontalIndex > 0) validPoints.Add(new(point.HorizontalIndex - 1, point.VerticalIndex));
                // left-top
                if (point.HorizontalIndex > 0 && point.VerticalIndex > 0) validPoints.Add(new(point.HorizontalIndex - 1, point.VerticalIndex - 1));
                // top
                if (point.VerticalIndex > 0) validPoints.Add(new(point.HorizontalIndex, point.VerticalIndex - 1));
                // top-right
                if (point.HorizontalIndex < Widht && point.VerticalIndex > 0) validPoints.Add(new(point.HorizontalIndex + 1, point.VerticalIndex - 1));
                // right
                if (point.HorizontalIndex < Widht) validPoints.Add(new(point.HorizontalIndex + 1, point.VerticalIndex));
                // right-bottom
                if (point.HorizontalIndex < Widht && point.VerticalIndex < Height) validPoints.Add(new(point.HorizontalIndex + 1, point.VerticalIndex + 1));
                // bottom
                if (point.VerticalIndex < Height) validPoints.Add(new(point.HorizontalIndex, point.VerticalIndex + 1));
                // bottom-left
                if (point.HorizontalIndex > 0 && point.VerticalIndex < Height) validPoints.Add(new(point.HorizontalIndex - 1, point.VerticalIndex + 1));

                return validPoints;
            }
        };



        public static string FirstPart()
        {
            var input = File.ReadAllLines("Y23/day3_input.txt");

            Dictionary<Point_D3, int> pointValue = [];

            Size_D3 world = new Size_D3(input[0].Length, input.Length);

            for (int verticalIndex = 0; verticalIndex < input.Length; verticalIndex++)
            {
                var line = input[verticalIndex];
                for (int horizontalIndex = 0; horizontalIndex < line.Length; horizontalIndex++)
                {
                    var character = line[horizontalIndex];
                    if (character != '.' && !char.IsDigit(character))
                    {
                        var adjents = world.GetAdjent(new(horizontalIndex, verticalIndex));
                        foreach (var adjent in adjents)
                        {
                            PointValue_D3? value = GetPointValue(input, adjent);
                            if (value is not null && !pointValue.ContainsKey(value.Point))
                            {
                                pointValue.Add(value.Point, value.Value);
                            }
                        }
                    }
                }
            }

            var sum = 0;
            foreach (var pv in pointValue)
            {
                sum += pv.Value;
            }
            
            return sum.ToString();
        }


        public static string SecondPart()
        {
            var input = File.ReadAllLines("Y23/day3_input.txt");

            Size_D3 world = new Size_D3(input[0].Length, input.Length);

            var sum = 0;
            for (int verticalIndex = 0; verticalIndex < input.Length; verticalIndex++)
            {
                var line = input[verticalIndex];
                for (int horizontalIndex = 0; horizontalIndex < line.Length; horizontalIndex++)
                {
                    var character = line[horizontalIndex];
                    if (character == '*')
                    {
                        var adjents = world.GetAdjent(new(horizontalIndex, verticalIndex));
                        var count = 0;
                        var product = 1;
                        Dictionary<Point_D3, int> pointValue = [];
                        foreach (var adjent in adjents)
                        {
                            PointValue_D3? value = GetPointValue(input, adjent);
                            if (value is not null && !pointValue.ContainsKey(value.Point))
                            {
                                count++;
                                product *= value.Value;
                                pointValue.Add(value.Point, value.Value);
                            }
                        }
                        if (count == 2)
                        {
                            sum += product;
                        }
                    }
                }
            }

            return sum.ToString();
        }



        private static PointValue_D3? GetPointValue(string[] input, Point_D3 point)
        {
            if (char.IsDigit(input[point.VerticalIndex][point.HorizontalIndex]))
            {
                int numberIndex = point.HorizontalIndex;

                int digit = 1;
                int value = 0;

                value = digit * int.Parse(input[point.VerticalIndex][point.HorizontalIndex].ToString());
                digit *= 10;
                //to the right
                for (int rightOffset = 1; point.HorizontalIndex + rightOffset < input[point.VerticalIndex].Length && char.IsDigit(input[point.VerticalIndex][point.HorizontalIndex + rightOffset]); rightOffset++)
                {
                    value = value * 10 + int.Parse(input[point.VerticalIndex][point.HorizontalIndex + rightOffset].ToString());
                    digit *= 10;
                }
                for (int leftOffset = 1; point.HorizontalIndex - leftOffset >= 0 && char.IsDigit(input[point.VerticalIndex][point.HorizontalIndex - leftOffset]); leftOffset++, numberIndex--)
                {
                    value += digit * int.Parse(input[point.VerticalIndex][point.HorizontalIndex - leftOffset].ToString());
                    digit *= 10;
                }

                return new PointValue_D3(new Point_D3(numberIndex, point.VerticalIndex), value);
            }
            return null;
        }

    }
}
