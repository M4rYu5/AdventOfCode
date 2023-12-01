using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Y22
{
    internal sealed class Day9
    {
        public static string FirstPart()
        {
            var inputs = File.ReadAllLines("Y22/day9_input.txt");

            // the display will be in the 4th quadrant
            // 0,0 will be left top,
            // and X goes to the right, Y down

            var headPosition = new Point(0, 0);
            var tailPosition = new Point(0, 0);
            var tailHistory = new HashSet<Point>
            {
                tailPosition
            };

            foreach (var direction in GetNextDirection(inputs))
            {
                headPosition = MoveTowards(headPosition, direction);
                MoveTail(headPosition, ref tailPosition);
                // update history
                if (!tailHistory.Contains(tailPosition))
                    tailHistory.Add(tailPosition);
            }


            return tailHistory.Count.ToString();
        }

        public static string SecondPart()
        {
            var inputs = File.ReadAllLines("Y22/day9_input.txt");

            // the display will be in the 4th quadrant
            // 0,0 will be left top,
            // and X goes to the right, Y down

            var knotPositions = new Point[10];
            for (int i = 0; i < knotPositions.Length; i++)
                knotPositions[i] = new Point(0, 0);
            var tailHistory = new HashSet<Point>
            {
                knotPositions[^1]
            };

            foreach (var direction in GetNextDirection(inputs))
            {
                knotPositions[0] = MoveTowards(knotPositions[0], direction);
                for (int i = 0; i + 1 < knotPositions.Length; i++)
                    MoveTail(knotPositions[i], ref knotPositions[i + 1]);
                // update history
                if (!tailHistory.Contains(knotPositions[^1]))
                    tailHistory.Add(knotPositions[^1]);
            }


            return tailHistory.Count.ToString();
        }

        private static void MoveTail(Point headPosition, ref Point tailPosition)
        {
            Point distance = new() { X = headPosition.X - tailPosition.X, Y = headPosition.Y - tailPosition.Y };

            if (Math.Abs(distance.X) > 1 || Math.Abs(distance.Y) > 1)
            {
                tailPosition.X += Math.Clamp(distance.X, -1, 1);
                tailPosition.Y += Math.Clamp(distance.Y, -1, 1);
            }
        }

        private static Point MoveTowards(Point headPosition, Direction direction)
        {
            return direction switch
            {
                Direction.Down => headPosition with { Y = headPosition.Y + 1 },
                Direction.Up => headPosition with { Y = headPosition.Y - 1 },
                Direction.Right => headPosition with { X = headPosition.X + 1 },
                Direction.Left => headPosition with { X = headPosition.X - 1 },
                _ => throw new UnreachableException()
            };
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }

        public static IEnumerable<Direction> GetNextDirection(string[] directions)
        {
            foreach (var direction in directions)
            {
                (Direction direction, int count) dirCount = GetDirCount(direction);
                for (int i = dirCount.count; i > 0; i--)
                {
                    yield return dirCount.direction;
                }
            }
        }

        private static (Direction direction, int count) GetDirCount(string direction)
        {
            var dir = direction[0] switch
            {
                'R' => Direction.Right,
                'D' => Direction.Down,
                'L' => Direction.Left,
                'U' => Direction.Up,
                _ => throw new UnreachableException()
            };
            var count = int.Parse(direction.Split(' ')[1]);
            return (dir, count);
        }

    }
}
