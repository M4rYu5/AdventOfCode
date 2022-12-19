using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal sealed class Day15
    {
        // x goes right
        // y goes down
        private record struct Point(int Column, int Row)
        {
            public int DistanceTo(Point point)
            {
                return Math.Abs(this.Column - point.Column) + Math.Abs(this.Row - point.Row);
            }
            public int DistanceTo(int row, int column)
            {
                return Math.Abs(this.Column - column) + Math.Abs(this.Row - row);
            }
        }
        private record CustomSet(Point Sensor, Point Beacon, int Distance);


        public static string FirstPart()
        {
            var inputs = File.ReadAllLines("day15_input.txt");

            List<CustomSet> sensorInfo = new();
            FillInfo(sensorInfo, inputs);
            //Print(sensorInfo);
            int countCheckedCoordinates = GetCheckedCoordinatesAt(sensorInfo, 2000000);

            return countCheckedCoordinates.ToString();
        }


        /// <remarks>
        /// I spent a long time to find this (first) solution, however this is not the best one. <br/>
        /// A way much better one can be found here https://github.com/encse/adventofcode/blob/master/2022/Day15/Solution.cs
        /// </remarks>
        public static string SecondPart()
        {
            var inputs = File.ReadAllLines("day15_input.txt");

            List<CustomSet> sensorInfo = new();
            FillInfo(sensorInfo, inputs);
            //Print(sensorInfo);
            Point countCheckedCoordinates = GetUncheckedPoint(sensorInfo);


            return ((Int128)countCheckedCoordinates.Column * (Int128)4000000 + (Int128)countCheckedCoordinates.Row).ToString();
        }

        private record struct CustomRange(int Start, int End);
        private class MapRowRangeStorrage
        {
            // key is the row number
            private readonly Dictionary<int, List<CustomRange>> rangeStorage = new();

            public void Add(int row, CustomRange addRange)
            {
                if (!rangeStorage.ContainsKey(row))
                {
                    rangeStorage[row] = new List<CustomRange>() { addRange };
                    return;
                }
                var ranges = rangeStorage[row];

                int i = 0;
                bool wasInserted = false;
                // add at appropiate index, to keep the list sorted by start
                for (; i < ranges.Count; i++)
                {
                    var current = ranges[i];
                    if (addRange.Start < current.Start)
                    {
                        ranges.Insert(i, addRange);
                        wasInserted = true;
                        break;
                    }
                }
                if (!wasInserted)
                {
                    ranges.Add(addRange);
                }

                // combine ranges if needed
                // backward
                for (var b = i - 1; b >= 0; b--)
                {
                    var current = ranges[b];
                    var next = ranges[b + 1];
                    if (current.End >= next.Start)
                    {
                        var newRange = new CustomRange(current.Start, Math.Max(current.End, next.End));
                        ranges[b] = newRange;
                        ranges.RemoveAt(b + 1);
                        i--;
                    }
                }
                // forward
                for (var b = i + 1; b < ranges.Count; b++)
                {
                    var current = ranges[b - 1];
                    var next = ranges[b];
                    if (current.End >= next.Start)
                    {
                        var newRange = new CustomRange(current.Start, Math.Max(current.End, next.End));
                        ranges[b] = newRange;
                        ranges.RemoveAt(b - 1);
                        b--;
                        //i--;
                    }
                }
            }

            public Dictionary<int, List<CustomRange>> GetRanges() => rangeStorage;


            private static bool IsFirstIntersectingSecondOnTheEndOfFirst(int firstStart, int firstEnd, int secondStart, int secondEnd)
                => firstStart <= secondStart && firstEnd >= secondStart;
            private static bool IsFirstIntersectingSecondOnTheStartOfFirst(int firstStart, int firstEnd, int secondStart, int secondEnd)
                => secondStart <= firstStart && secondEnd >= firstStart;
            private static bool IsFirstInsideSecond(int firstStart, int firstEnd, int secondStart, int secondEnd)
                => firstStart >= secondStart && firstEnd <= secondEnd;
            private static bool IsSecondInsideFirst(int firstStart, int firstEnd, int secondStart, int secondEnd)
                => firstStart <= secondStart && firstEnd >= secondEnd;

        }


        private static Point GetUncheckedPoint(List<CustomSet> sensorsInfo)
        {
            // Rain Drop Algorithm
            // I didn't research if there's an algorithm like this, but I like the name and I'm going with it
            // to solve:
            // we need to eliminate elements from a huge map of posibilities
            // how it works:
            // havign a vector storing the ranges eliminated
            // we'll check for each 'drop' the area that it covers, generating ranges that will get stored in the vector
            // the vector should handle the process of combining those ranges into a single one, if intersecting


            var map = new MapRowRangeStorrage();

            foreach (var s in sensorsInfo)
            {
                int nrOfPointsOnLine = s.Distance * 2 + 1;
                int rowTopId = s.Sensor.Row;
                int rowBottomId = rowTopId;
                while (nrOfPointsOnLine > 0)
                {
                    int startColumnIndex = s.Sensor.Column - nrOfPointsOnLine / 2;
                    int endColumnIndex = s.Sensor.Column + nrOfPointsOnLine / 2;

                    if (rowTopId == rowBottomId)
                        map.Add(rowTopId, new CustomRange(startColumnIndex, endColumnIndex));
                    else
                    {
                        map.Add(rowTopId, new CustomRange(startColumnIndex, endColumnIndex));
                        map.Add(rowBottomId, new CustomRange(startColumnIndex, endColumnIndex));
                    }
                    rowTopId -= 1;
                    rowBottomId += 1;
                    nrOfPointsOnLine -= 2;
                }
            }

            var lowerLimit = 0;
            var higherLimit = 4000000;

            var rangesMap = map.GetRanges();
            var testRanges = new List<(int row, List<CustomRange> column)>();
            for (int rowId = lowerLimit; rowId <= higherLimit; rowId++)
            {
                if (rangesMap.TryGetValue(rowId, out var ranges))
                {
                    if (ranges.Any(x => x.Start > lowerLimit && x.Start < higherLimit || x.End > lowerLimit && x.End < higherLimit))
                        testRanges.Add(new(rowId, ranges));
                }
            }

            // those checks was added after the value was found
            // and I did not put extra effort to test those
            if (testRanges.Count != 1)
                throw new Exception("It should be just one row");

            var row = testRanges[0].row;
            var foundRanges = testRanges[0].column;
            if(foundRanges.Count == 2)
            {
                if(foundRanges[1].Start - foundRanges[0].End != 2)
                    throw new Exception("It should be a single value");
                return new Point(foundRanges[0].End + 1, row);
            }

            // this part was not tested
            if (foundRanges.Count == 1)
            {
                if (foundRanges[0].Start == lowerLimit + 1 && foundRanges[0].End > higherLimit)
                    return new Point(0, row);
                if (foundRanges[0].Start < lowerLimit && foundRanges[0].End == higherLimit - 1)
                    return new Point(higherLimit, row);
                        
            }
            
            throw new Exception("It should be a single value");
        }

        /// <summary>
        /// Will print the example output
        /// </summary>
        /// <remarks>
        /// Was made because I could not foud out why my values weren't good for first part,
        /// after tested each line of code I realized that I had checked for row 10, not 20_000
        /// </remarks>
        private static void Print(List<CustomSet> sensorInfo)
        {
            // store matrix limits
            int left = int.MaxValue,
                right = int.MinValue,
                top = int.MaxValue,
                down = int.MinValue;

            // define matrix limit
            sensorInfo.ForEach(x =>
            {
                UpdatePoint(x.Sensor, x.Distance, ref top, ref down, ref left, ref right);
                UpdatePoint(x.Beacon, x.Distance, ref top, ref down, ref left, ref right);
            });

            (int row, int column) indexOffset = new(-top, -left);

            char[,] map = new char[Math.Abs(down - top) + 1, Math.Abs(right - left) + 1];

            // fill the map with scanned portions
            foreach (var s in sensorInfo)
            {
                int nrOfPointsOnLine = s.Distance * 2 + 1;
                int rowTopId = s.Sensor.Row + indexOffset.row;
                int rowBottomId = rowTopId;
                while (nrOfPointsOnLine > 0)
                {
                    int startColumnIndex = s.Sensor.Column - nrOfPointsOnLine / 2 + indexOffset.column;
                    int endColumnIndex = s.Sensor.Column + nrOfPointsOnLine / 2 + indexOffset.column;
                    if (rowTopId == rowBottomId)
                        for (int i = startColumnIndex; i <= endColumnIndex; i++)
                        {
                            map[rowTopId, i] = '#';
                        }
                    else
                        for (int i = startColumnIndex; i <= endColumnIndex; i++)
                        {
                            map[rowTopId, i] = '#';
                            map[rowBottomId, i] = '#';
                        }
                    rowTopId -= 1;
                    rowBottomId += 1;
                    nrOfPointsOnLine -= 2;
                }
            }

            // add scanners and beacon
            sensorInfo.ForEach(x =>
            {
                map[x.Sensor.Row + indexOffset.row, x.Sensor.Column + indexOffset.column] = 'S';
                map[x.Beacon.Row + indexOffset.row, x.Beacon.Column + indexOffset.column] = 'B';
            });

            // print
            for (int columnIndex = 0; columnIndex < map.GetLength(1); columnIndex++)
                Console.Write((columnIndex - indexOffset.column).ToString()[^1].ToString());
            Console.WriteLine();
            for (int rowIndex = 0; rowIndex < map.GetLength(0); rowIndex++)
            {

                for (int columnIndex = 0; columnIndex < map.GetLength(0); columnIndex++)
                {
                    char c = map[rowIndex, columnIndex];
                    Console.Write(c == 0 ? '.' : c);
                }
                Console.Write(rowIndex - indexOffset.row);
                Console.WriteLine();
            }

            Console.WriteLine();

            // Util
            static void UpdatePoint(Point point, int offset, ref int top, ref int down, ref int left, ref int right)
            {
                UpdateAxys(point.Row, ref top, ref down, offset);
                UpdateAxys(point.Column, ref left, ref right, offset);
            }
            static void UpdateAxys(int value, ref int lowerLimit, ref int higherLimit, int offset = 0)
            {
                if (value - offset < lowerLimit)
                    lowerLimit = value - offset;
                if (value + offset > higherLimit)
                    higherLimit = value + offset;
            }
        }

        private static int GetCheckedCoordinatesAt(List<CustomSet> sensorsInfo, int rowToCheck)
        {
            HashSet<int> checkedLocations = new();

            // fill sensors & beacons
            sensorsInfo.ForEach(x =>
            {
                //if (x.Sensor.Row == rowToCheck)
                //{
                //    if (!checkedLocations.Contains(x.Sensor.Column))
                //        checkedLocations.Add(x.Sensor.Column);
                //}

                if (x.Beacon.Row == rowToCheck)
                {
                    if (!checkedLocations.Contains(x.Beacon.Column))
                        checkedLocations.Add(x.Beacon.Column);
                }
            });

            int numOfSensorsAndBaconsOnLine = checkedLocations.Count;

            // fill searched location
            foreach (CustomSet sensor in sensorsInfo)
            {
                var distanceToRow = Math.Abs(sensor.Sensor.Row - rowToCheck);
                if (distanceToRow <= sensor.Distance)
                {
                    int numOfPointsOnLine = (sensor.Distance - distanceToRow) * 2 + 1;
                    int startColumnIndex = sensor.Sensor.Column - numOfPointsOnLine / 2;
                    int endColumnIndex = sensor.Sensor.Column + numOfPointsOnLine / 2;
                    for (int i = startColumnIndex; i <= endColumnIndex; i++)
                    {
                        if (!checkedLocations.Contains(i))
                            checkedLocations.Add(i);
                    }
                }
            }

            return checkedLocations.Count - numOfSensorsAndBaconsOnLine;
        }

        private static void FillInfo(List<CustomSet> sensorInfo, string[] inputs)
        {
            string[] splits =
                {
                    "Sensor at x=",
                    ", y=",
                    ": closest beacon is at x=",
                };
            foreach (var input in inputs)
            {
                string[] points = input.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                Point sensor = new Point(int.Parse(points[0]), int.Parse(points[1]));
                Point beacon = new Point(int.Parse(points[2]), int.Parse(points[3]));
                int distance = sensor.DistanceTo(beacon);
                sensorInfo.Add(new CustomSet(sensor, beacon, distance));
            }
        }

    }
}
