using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal sealed class Day14
    {
        // draw rock as #
        // draw air as .
        // sand spawn is represented with +
        // sand at rest is represented with o


        // from lower left corner:
        //  -> x - iterate columns; y - iterate rows;
        // draw rock in lines x, y are separated by ,
        // corners are separated with ->


        // sand 'pouring' 1 unit at a time (and instantly dropping into place)
        // each step a new unit of sand is created and follow the following rules:
        // is going down
        // if lock then diagonally down left
        // if down left block then diagonally down right
        // if all locked rest;


        // I might end up not needing to know the structure type at a coordinate
        private enum RoomType
        {
            Air,
            Rock,
            Sand,
        }

        public static string FirstPart()
        {
            var inputs = File.ReadAllLines("day14_input.txt");

            int abysStart = 0;
            (int Column, int Row) sandSpawn = (500, 0);

            // I'll save all non air 'rooms' by rows, then columns
            Dictionary<int, Dictionary<int, RoomType>> nonAirRooms = new();

            FillWithRock(inputs, nonAirRooms, out abysStart);
            FillWithSand(nonAirRooms, abysStart, sandSpawn);

            int points = 0;
            foreach (var i in nonAirRooms)
            {
                foreach (var j in i.Value)
                    if (j.Value == RoomType.Sand)
                        points++;
            }

            return points.ToString();
        }

        public static string SecondPart()
        {
            var inputs = File.ReadAllLines("day14_input.txt");

            int abysStart = 0;
            (int Column, int Row) sandSpawn = (500, 0);

            // I'll save all non air 'rooms' by rows, then columns
            Dictionary<int, Dictionary<int, RoomType>> nonAirRooms = new();

            FillWithRock(inputs, nonAirRooms, out abysStart);
            var highestRow = nonAirRooms.Select(x => x.Key).OrderDescending().First();
            MakeFloor(nonAirRooms, highestRow + 2, sandSpawn.Column);
            abysStart = abysStart + 2;
            FillWithSand(nonAirRooms, abysStart, sandSpawn);

            int points = 0;
            foreach(var i in nonAirRooms)
            {
                foreach (var j in i.Value)
                    if (j.Value == RoomType.Sand)
                        points++;
            }

            return points.ToString();
        }

        private static void MakeFloor(Dictionary<int, Dictionary<int, RoomType>> nonAirRooms, int floorAtRow, int spawnColumn)
        {
           for(int i = spawnColumn - floorAtRow; i <= spawnColumn + floorAtRow; i++)
            {
                if (!nonAirRooms.ContainsKey(floorAtRow))
                    nonAirRooms[floorAtRow] = new Dictionary<int, RoomType>();
                nonAirRooms[floorAtRow][i] = RoomType.Rock;
            }
        }

        private static void FillWithSand(Dictionary<int, Dictionary<int, RoomType>> nonAirRooms, int abysStart, (int Column, int Row) sandSpawn)
        {
            while (true)
            {
                int currentColumn = sandSpawn.Column;
                if (RoomType.Air != GetRoomType(nonAirRooms, sandSpawn.Row, sandSpawn.Column))
                    return;

                for (int row = sandSpawn.Row; row < abysStart; row++)
                {
                    if (row + 1 >= abysStart)
                        return;

                    if (RoomType.Air == GetRoomType(nonAirRooms, row + 1, currentColumn))
                        continue;

                    if (RoomType.Air == GetRoomType(nonAirRooms, row + 1, currentColumn - 1))
                    {
                        currentColumn--;
                        continue;
                    }
                    if (RoomType.Air == GetRoomType(nonAirRooms, row + 1, currentColumn + 1))
                    {
                        currentColumn++;
                        continue;
                    }

                    if (!nonAirRooms.ContainsKey(row))
                        nonAirRooms[row] = new Dictionary<int, RoomType>();
                    nonAirRooms[row][currentColumn] = RoomType.Sand;
                    break;
                }
            }
        }

        private static RoomType GetRoomType(Dictionary<int, Dictionary<int, RoomType>> nonAirRooms, int Row, int Column)
        {
            if (nonAirRooms.TryGetValue(Row, out var columns))
                if (columns.TryGetValue(Column, out var type))
                    return type;
            return RoomType.Air;
        }

        private static void FillWithRock(string[] inputs, Dictionary<int, Dictionary<int, RoomType>> nonAirRooms, out int abysStart)
        {
            abysStart = 0;
            foreach (var inputRow in inputs)
            {
                (int Column, int Row)? lastCorner = null;
                foreach (var cornersCoordinates in inputRow.Split("->", StringSplitOptions.RemoveEmptyEntries))
                {
                    var coordinates = cornersCoordinates.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (coordinates.Length != 2) throw new UnreachableException();

                    var column = int.Parse(coordinates[0]);
                    var row = int.Parse(coordinates[1]);
                    abysStart = abysStart > row + 1 ? abysStart : row + 1;

                    if (!nonAirRooms.ContainsKey(row))
                        nonAirRooms[row] = new Dictionary<int, RoomType>();

                    nonAirRooms[row][column] = RoomType.Rock;

                    if (lastCorner != null)
                    {
                        if (row != lastCorner.Value.Row)
                        {
                            var minRow = Math.Min(row, lastCorner.Value.Row);
                            var maxRow = Math.Max(row, lastCorner.Value.Row);

                            for (int i = minRow + 1; i < maxRow; i++)
                            {
                                if (!nonAirRooms.ContainsKey(i))
                                    nonAirRooms[i] = new Dictionary<int, RoomType>();
                                nonAirRooms[i][column] = RoomType.Rock;
                            }
                        }
                        if (column != lastCorner.Value.Column)
                        {
                            var minColumn = Math.Min(column, lastCorner.Value.Column);
                            var maxColumn = Math.Max(column, lastCorner.Value.Column);
                            for (int i = minColumn + 1; i < maxColumn; i++)
                                nonAirRooms[row][i] = RoomType.Rock;
                        }
                    }
                    lastCorner = (column, row);
                }
            }
        }

    }
}
