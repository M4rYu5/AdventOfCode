using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Day11
{

    public record struct MonkeyI(int mod, int throwMonkeyIdTrue, int throwMonkeyIdFalse, Queue<int> Items, Func<int, int> operation);


    internal class FirstPart
    {
        private static MonkeyI[] monkeysTest = new[]
            {
                new MonkeyI(23, 2, 3, new Queue<int> { 79, 98 }, (m) => m * 19),
                new MonkeyI(19, 2, 0, new Queue<int> { 54, 65, 75, 74}, (m) => m + 6),
                new MonkeyI(13, 1, 3, new Queue<int> { 79, 60, 97}, (m) => m * m),
                new MonkeyI(17, 0, 1, new Queue<int> { 74}, (m) => m + 3)
            };

        private static MonkeyI[] monkeysInput = new[] 
            {
                new MonkeyI(17, 2, 7, new Queue<int> { 83, 97, 95, 67 }, (m) => m * 19),
                new MonkeyI(19, 7, 0, new Queue<int> { 71, 70, 79, 88, 56, 70 }, (m) => m + 2),
                new MonkeyI(7, 4, 3, new Queue<int> { 98, 51, 51, 63, 80, 85, 84, 95 }, (m) => m + 7),
                new MonkeyI(11, 6, 4, new Queue<int> { 77, 90, 82, 80, 79 }, (m) => m + 1),
                new MonkeyI(13, 6, 5, new Queue<int> { 68 }, (m) => m * 5),
                new MonkeyI(3, 1, 0, new Queue<int> { 60, 94 }, (m) => m + 5),
                new MonkeyI(5, 5, 1, new Queue<int> { 81, 51, 85 }, (m) => m * m),
                new MonkeyI(2, 2, 3, new Queue<int> { 98, 81, 63, 65, 84, 71, 84 }, (m) => m + 3)
            };



        public static string Run()
        {
            var monkeys = monkeysInput;
            Dictionary<int, int> monkeyTransactionCount = new Dictionary<int, int>();

            for (int i = 0; i < 20; i++)
            {
                for (int mokIndex = 0; mokIndex < monkeys.Length; mokIndex++)
                {
                    var mok = monkeys[mokIndex];
                    while (mok.Items.TryDequeue(out var item))
                    {
                        IncrementCount(monkeyTransactionCount, mokIndex);

                        // run the tests for each monky
                        item = mok.operation(item);
                        item /= 3;

                        // run
                        if (item % mok.mod == 0)
                            monkeys[mok.throwMonkeyIdTrue].Items.Enqueue(item);
                        else
                            monkeys[mok.throwMonkeyIdFalse].Items.Enqueue(item);
                    }
                }

                PrintItemsPerMonkey(monkeys, i);
            }

            PrintScore(monkeyTransactionCount, out int score);

            bool check = true;
            if (check)
            {
                int testResult = 10605;
                int inputResult = 108240;
                if (score != inputResult && score != testResult)
                    throw new Exception("Bad output");
            }

            return "";
        }



        private static void PrintItemsPerMonkey(MonkeyI[] monkeys, int i)
        {
            // LOG
            Console.WriteLine();
            Console.WriteLine($"After round {i}:");
            PrintWorryLevels(monkeys);
        }

        private static void PrintScore(Dictionary<int, int> monkeyTransactionCount, out int score)
        {
            Console.WriteLine();

            // Print monkeys transaction counts
            Console.WriteLine("Monkey ID and TransactionCount:");
            foreach (var run in monkeyTransactionCount)
                Console.WriteLine(run.ToString());

            Console.WriteLine();

            // monkey business indicator
            var comparer = new Day11.CustomComparerInt();
            var sortedDictionary = monkeyTransactionCount.ToImmutableSortedSet(comparer).Reverse();
            var first = sortedDictionary.First().Value;
            var second = sortedDictionary.Skip(1).First().Value;
            score = first * second;
            Console.Write($"First two most active monkeys multiplied scores: {first}*{second}= {score}");
        }

        private static void IncrementCount(Dictionary<int, int> monkeyTransactionCount, int mokIndex)
        {
            // count transactions
            if (monkeyTransactionCount.TryGetValue(mokIndex, out int monkeyRun))
                monkeyTransactionCount[mokIndex]++;
            else
                monkeyTransactionCount[mokIndex] = 1;
        }


        private static void PrintWorryLevels(MonkeyI[] monkeys)
        {
            for (int i = 0; i < monkeys.Length; i++)
            {
                Console.Write($"Monkey {i}:");
                foreach (var item in monkeys[i].Items)
                {
                    Console.Write($" {item},");
                }
                Console.WriteLine();
            }
        }

    }
}
