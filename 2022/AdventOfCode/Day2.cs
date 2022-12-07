using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal sealed class Day2
    {

        enum RPS
        {
            rock = 1,
            paper = 2,
            scissors = 3,
        }

        enum Outcome
        {
            lose,
            draw,
            win,
        }


        public static string FirstPart()
        {
            var input = File.ReadAllLines("day2_input.txt");


            int playerPoints = 0;
            foreach(var line in input)
            {

                RPS playerHand = GetPlayerHand(line);
                RPS enemyHand = GetEnemyHand(line);

                playerPoints += TotalPointsForHand(playerHand, enemyHand);
            }


            return playerPoints.ToString();
        }


        public static string SecondPart()
        {
            var input = File.ReadAllLines("day2_input.txt");


            int playerPoints = 0;
            foreach (var line in input)
            {

                RPS enemyHand = GetEnemyHand(line);
                Outcome desiredOutcoem = GetDesiredOutcome(line);
                RPS playerHand = GetPlayerHandFromDesiredOutcome(desiredOutcoem, enemyHand);

                playerPoints += TotalPointsForHand(playerHand, enemyHand);
            }


            return playerPoints.ToString();
        }

        private static RPS GetPlayerHandFromDesiredOutcome(Outcome desiredOutcoem, RPS enemyHand)
        {
            return (desiredOutcoem, enemyHand) switch
            {
                (Outcome.win, RPS.rock) => RPS.paper,
                (Outcome.win, RPS.paper) => RPS.scissors,
                (Outcome.win, RPS.scissors) => RPS.rock,
                (Outcome.draw, RPS.rock) => RPS.rock,
                (Outcome.draw, RPS.paper) => RPS.paper,
                (Outcome.draw, RPS.scissors) => RPS.scissors,
                (Outcome.lose, RPS.rock) => RPS.scissors,
                (Outcome.lose, RPS.paper) => RPS.rock,
                (Outcome.lose, RPS.scissors) => RPS.paper,
                _ => throw new UnreachableException(),
            };
        }

        private static RPS GetEnemyHand(string line)
        {
            if (line.ToLower().Contains('a'))
            {
                return RPS.rock;
            }
            if (line.ToLower().Contains('b'))
            {
                return RPS.paper;
            }
            if (line.ToLower().Contains('c'))
            {
                return RPS.scissors;
            }
            throw new UnreachableException();
        }

        private static RPS GetPlayerHand(string line)
        {
            if (line.ToLower().Contains('x'))
            {
                return RPS.rock;
            }
            if (line.ToLower().Contains('y'))
            {
                return RPS.paper;
            }
            if (line.ToLower().Contains('z'))
            {
                return RPS.scissors;
            }
            throw new UnreachableException();
        }

        private static Outcome GetDesiredOutcome(string line)
        {
            if (line.ToLower().Contains('x'))
            {
                return Outcome.lose;
            }
            if (line.ToLower().Contains('y'))
            {
                return Outcome.draw;
            }
            if (line.ToLower().Contains('z'))
            {
                return Outcome.win;
            }
            throw new UnreachableException();
        }

        private static int TotalPointsForHand(RPS playerHand, RPS enemeyHand)
        {
            Outcome outcome = HandOutcome(playerHand, enemeyHand);
            int playerPoints = PointsForChosenHand(playerHand) + PointsForOutcome(outcome);
            return playerPoints;
        }

        private static int PointsForChosenHand(RPS player)
        {
            return player switch
            {
                RPS.rock => 1,
                RPS.paper => 2,
                RPS.scissors => 3,
                _ => throw new UnreachableException()
            };
        }

        private static int PointsForOutcome(Outcome outcome)
        {
            return outcome switch
            {
                Outcome.lose => 0,
                Outcome.draw => 3,
                Outcome.win => 6,
            };
        }

        private static Outcome HandOutcome(RPS player, RPS enemy)
        {
            return (player, enemy) switch
            {
                (RPS.rock, RPS.rock) or (RPS.paper, RPS.paper) or (RPS.scissors, RPS.scissors) => Outcome.draw,
                (RPS.rock, RPS.scissors) or (RPS.paper, RPS.rock) or (RPS.scissors, RPS.paper) => Outcome.win,
                (RPS.rock, RPS.paper) or (RPS.paper, RPS.scissors) or (RPS.scissors, RPS.rock) => Outcome.lose,
                _ => throw new UnreachableException(),
            };
        }

    

    }
}
