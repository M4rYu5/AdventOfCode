using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal sealed class Day12
    {
        private record struct Point(int RowIndex, int ColumnIndex);


        public static string FirstPart()
        {
            var inputs = File.ReadAllLines("day12_input.txt");
            var map = GetMap(inputs, out Point start, out Point end);

            var bottomRight = new Point(map.GetLength(0) - 1, map.GetLength(1) - 1);
            var path = RadialSearch(map, start, end, bottomRight);


            return (path.Count() - 1).ToString();
        }

        public static string SecondPart()
        {
            var inputs = File.ReadAllLines("day12_input.txt");
            var map = GetMap(inputs, out Point start, out Point end);

            var bottomRight = new Point(map.GetLength(0) - 1, map.GetLength(1) - 1);

            var minDistance = int.MaxValue;
            for(int i = 0; i <= bottomRight.RowIndex; i++)
                for(int j = 0; j <= bottomRight.ColumnIndex; j++)
                {
                    if (map[i, j] == 'a')
                    {
                        var distanceToTop = RadialSearch(map, new(i, j), end, bottomRight).Count() - 1;
                        if(distanceToTop >= 0 && distanceToTop < minDistance)
                            minDistance = distanceToTop;
                    }
                }

            return minDistance.ToString();
        }


        // I didn't needed the road
        private record Tile(Point Position, Tile? Parent)
        {
            public Tile? Top { get; set; } = null;
            public Tile? Left { get; set; } = null;
            public Tile? Right { get; set; } = null;
            public Tile? Bottom { get; set; } = null;
        }

        private static IEnumerable<Point> RadialSearch(char[,] map, Point start, Point end, Point bottomRight)
        {
            HashSet<Point> traversedPoints = new();
            Dictionary<Point, Tile> processingQueue;
            Dictionary<Point, Tile> tilesToBeQueued = new()
            {
                { start, new Tile(start, null) }
            };

            Tile? endTile = null;
            while (endTile == null)
            {
                processingQueue = tilesToBeQueued;
                tilesToBeQueued = new();
                //PrintProgressionMap(bottomRight, traversedPoints, processingQueue);

                if (processingQueue.Count == 0)
                    return new List<Point>();

                foreach (var tile in processingQueue)
                {
                    // if found
                    if (tile.Value.Position == end)
                    {
                        endTile = tile.Value;
                        break;
                    }

                    traversedPoints.Add(tile.Value.Position);

                    // search
                    { // left
                        var leftPoint = tile.Value.Position with { ColumnIndex = tile.Value.Position.ColumnIndex - 1 };
                        if (CanReach(tile.Value.Position, leftPoint, bottomRight, map) && !traversedPoints.Contains(leftPoint))
                        {
                            if (!tilesToBeQueued.ContainsKey(leftPoint))
                            {
                                var leftTile = new Tile(Position: leftPoint, Parent: tile.Value);
                                tile.Value.Left = leftTile;
                                tilesToBeQueued.Add(leftPoint, leftTile);
                            }
                        }
                    }
                    { // top
                        var topPoint = tile.Value.Position with { RowIndex = tile.Value.Position.RowIndex - 1 };
                        if (CanReach(tile.Value.Position, topPoint, bottomRight, map) && !traversedPoints.Contains(topPoint))
                        {
                            if (!tilesToBeQueued.ContainsKey(topPoint))
                            {
                                var topTile = new Tile(Position: topPoint, Parent: tile.Value);
                                tile.Value.Top = topTile;
                                tilesToBeQueued.Add(topPoint, topTile);
                            }
                        }
                    }
                    { // right
                        var rightPoint = tile.Value.Position with { ColumnIndex = tile.Value.Position.ColumnIndex + 1 };
                        if (CanReach(tile.Value.Position, rightPoint, bottomRight, map) && !traversedPoints.Contains(rightPoint))
                        {
                            if (!tilesToBeQueued.ContainsKey(rightPoint))
                            {
                                var rithTile = new Tile(rightPoint, Parent: tile.Value);
                                tile.Value.Right = rithTile;
                                tilesToBeQueued.Add(rightPoint, rithTile);
                            }
                        }
                    }
                    { // bottom
                        var bottomPoint = tile.Value.Position with { RowIndex = tile.Value.Position.RowIndex + 1 };
                        if (CanReach(tile.Value.Position, bottomPoint, bottomRight, map) && !traversedPoints.Contains(bottomPoint))
                        {
                            if (!tilesToBeQueued.ContainsKey(bottomPoint))
                            {
                                var bottomTile = new Tile(bottomPoint, Parent: tile.Value);
                                tile.Value.Bottom = bottomTile;
                                tilesToBeQueued.Add(bottomPoint, bottomTile);

                            }
                        }
                    }
                }
            }

            // this wasn't needed, but I'll keep it
            var list = new List<Point>();
            var current = endTile;
            while (current != null)
            {
                list.Insert(0, current.Position);
                current = current.Parent;
            }
            return list;
        }

        private static void PrintProgressionMap(Point bottomRight, HashSet<Point> traversedPoints, Dictionary<Point, Tile> processingQueue)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int j = 0; j <= bottomRight.RowIndex; j++)
            {
                for (int i = 0; i <= bottomRight.ColumnIndex; i++)
                {
                    if (traversedPoints.Contains(new Point(j, i)))
                        stringBuilder.Append("#");
                    else if (processingQueue.ContainsKey(new Point(j, i)))
                        stringBuilder.Append('*');
                    else
                        stringBuilder.Append('.');
                }
                stringBuilder.Append(Environment.NewLine);
            }
            Console.Clear();
            Console.WriteLine(stringBuilder.ToString());
        }

        //private static HashSet<Point>? BruteForceSearch(char[,] map, HashSet<Point>? path, Point end, Point bottomRight)
        //{
        //    if (path == null || !path.Any())
        //        return null;


        //    HashSet<Point>?[] results = new HashSet<Point>[4];
        //    var last = path.Last();

        //    // if found
        //    if (last == end)
        //        return path;

        //    // brute force search
        //    { // left
        //        var leftPoint = last with { ColumnIndex = last.ColumnIndex - 1 };
        //        if (CanReach(last, leftPoint, bottomRight, map) && !path.Contains(leftPoint))
        //            results[0] = BruteForceSearch(map, new HashSet<Point>(path) { leftPoint }, end, bottomRight);
        //        else
        //            results[0] = new HashSet<Point>();
        //    }
        //    { // top
        //        var topPoint = last with { LineIndex = last.LineIndex - 1 };
        //        if (CanReach(last, topPoint, bottomRight, map) && !path.Contains(topPoint))
        //            results[1] = BruteForceSearch(map, new HashSet<Point>(path) { topPoint }, end, bottomRight);
        //        else
        //            results[1] = new HashSet<Point>();
        //    }
        //    { // right
        //        var rightPoint = last with { ColumnIndex = last.ColumnIndex + 1 };
        //        if (CanReach(last, rightPoint, bottomRight, map) && !path.Contains(rightPoint))
        //            results[2] = BruteForceSearch(map, new HashSet<Point>(path) { rightPoint }, end, bottomRight);
        //        else
        //            results[2] = new HashSet<Point>();
        //    }
        //    { // bottom
        //        var bottomPoint = last with { LineIndex = last.LineIndex + 1 };
        //        if (CanReach(last, bottomPoint, bottomRight, map) && !path.Contains(bottomPoint))
        //            results[3] = BruteForceSearch(map, new HashSet<Point>(path) { bottomPoint }, end, bottomRight);
        //        else
        //            results[3] = new HashSet<Point>();
        //    }


        //    // here we should have at least 1 solution;
        //    return results.ToImmutableSortedSet(new CustomComparer()).SkipWhile(x => x == null || !x.Any()).FirstOrDefault();
        //}


        private class CustomComparer : IComparer<HashSet<Point>>
        {
            public int Compare(HashSet<Point>? x, HashSet<Point>? y)
            {
                if (x == null && y != null) return -1;
                if (x != null && y == null) return 1;
                if (x == null && y == null) return 0;
                if (x.Count() < y.Count()) return -1;
                if (x.Count() > y.Count()) return 1;
                return 0;
            }
        }
        private static bool CanReach(Point current, Point desired, Point bottomRightMargin, char[,] map)
        {
            // out of map
            if (desired.ColumnIndex < 0
                || desired.RowIndex < 0
                || desired.ColumnIndex > bottomRightMargin.ColumnIndex
                || desired.RowIndex > bottomRightMargin.RowIndex)
                return false;

            // too high or too low
            if (map[desired.RowIndex, desired.ColumnIndex] - map[current.RowIndex, current.ColumnIndex] > 1)
                return false;

            // can't see why not :)
            return true;
        }


        private static char[,] GetMap(string[] inputs, out Point start, out Point end)
        {
            Point? startPoint = null;
            Point? endPoint = null;
            var map = new char[inputs.Length, inputs[0].Length];
            for (int lineIndex = 0; lineIndex < inputs.Length; lineIndex++)
            {
                for (int columnIndex = 0; columnIndex < inputs[lineIndex].Length; columnIndex++)
                {
                    var value = inputs[lineIndex][columnIndex];
                    map[lineIndex, columnIndex] = value;
                    if (value == 'S')
                    {
                        startPoint = new Point(lineIndex, columnIndex);
                        map[lineIndex, columnIndex] = 'a';
                    }
                    if (value == 'E')
                    {
                        endPoint = new Point(lineIndex, columnIndex);
                        map[lineIndex, columnIndex] = 'z';
                    }
                }
            }

            start = startPoint ?? throw new Exception("The map does not contains an start 'S' point"); ;
            end = endPoint ?? throw new Exception("The map does not contains an start 'S' point"); ;
            return map;
        }

    }
}
