using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Y23
{


    public static class Day2
    {
        /// keeps the cubes played in this round
        private class Round: Dictionary<string, int> { }
        /// keeps the rounds of a game
        private class Game : List<Round> { }

        public static string FirstPart()
        {
            var input = File.ReadAllLines("Y23/day2_input.txt");

            List<Game> games = ExtractGames(input);

            Dictionary<string, int> availableCubes = new() { 
                { "red", 12 },
                { "green", 13 },
                { "blue", 14 },
            };

            List<int> canPlay = [];
            for (var gameId = 0; gameId < games.Count; gameId++)
            {
                bool canPlayGame = true;
                foreach (var round in games[gameId])
                {
                    foreach (var cubeTypeCount in round)
                    {
                        if (!availableCubes.ContainsKey(cubeTypeCount.Key) || availableCubes[cubeTypeCount.Key] < cubeTypeCount.Value)
                        {
                            canPlayGame = false;
                            break;
                        }
                    }
                }
                if (canPlayGame)
                {
                    canPlay.Add(gameId + 1);
                }
            }

            int sum = 0;
            foreach (var game in canPlay) {
                sum += game;
            }

            return sum.ToString();
        }


        public static string SecondPart()
        {
            var input = File.ReadAllLines("Y23/day2_input.txt");

            List<Game> games = ExtractGames(input);

            Dictionary<string, int> availableCubes = new() {
                { "red", 12 },
                { "green", 13 },
                { "blue", 14 },
            };

            int sum = 0;
            for (var gameId = 0; gameId < games.Count; gameId++)
            {
                Round minReqCubes = new();
                foreach (var round in games[gameId])
                {
                    foreach (var cubeTypeCount in round)
                    {
                        if (minReqCubes.TryGetValue(cubeTypeCount.Key, out int minVal))
                        {
                            if (minVal < cubeTypeCount.Value)
                            {
                                minReqCubes[cubeTypeCount.Key] = cubeTypeCount.Value;
                            }
                        }
                        else
                        {
                            minReqCubes.Add(cubeTypeCount.Key, cubeTypeCount.Value);
                        }
                    }
                }
                int part = 1;
                foreach (var minAmountCube in minReqCubes)
                {
                    part *= minAmountCube.Value;
                }
                sum += part;
            }

            return sum.ToString();
        }


        private static List<Game> ExtractGames(string[] input)
        {
            List<Game> games = [];
            for (var gameId = 1; gameId <= input.Length; gameId++)
            {
                var game = new Game();
                games.Add(game);
                var gameInfo = input[gameId - 1].Split(": ")[1];
                var rounds = gameInfo.Split("; ");
                foreach (var roundString in rounds)
                {
                    var round = new Round();
                    game.Add(round);
                    var cubeWithNumbers = roundString.Split(", ");
                    foreach (var cube in cubeWithNumbers)
                    {
                        var numberCube = cube.Split(" ");
                        round.Add(numberCube[1], int.Parse(numberCube[0]));
                    }
                }
            }
            return games;
        }

    }
}
