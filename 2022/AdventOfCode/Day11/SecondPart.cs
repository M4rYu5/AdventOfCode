using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Day11
{
    public record struct MonkeyII(int Mod, int ThrowMonkeyIdTrue, int ThrowMonkeyIdFalse, Queue<Item> Items, Func<Item, Item> Operation);

    internal class SecondPart
    {
        private static MonkeyII[] monkeysTest = new[] {
                new MonkeyII(23, 2, 3, new Queue<Item> { 79, 98 }, (m) => m * 19),
                new MonkeyII(19, 2, 0, new Queue<Item> { 54, 65, 75, 74}, (m) => m + 6),
                new MonkeyII(13, 1, 3, new Queue<Item> { 79, 60, 97}, (m) => m.MultiplySelf()),
                new MonkeyII(17, 0, 1, new Queue<Item> { 74}, (m) => m + 3)
            };
        private static MonkeyII[] monkeysInput = new[] {
                new MonkeyII(17, 2, 7, new Queue<Item> { 83, 97, 95, 67 }, (m) => m * 19),
                new MonkeyII(19, 7, 0, new Queue<Item> { 71, 70, 79, 88, 56, 70 }, (m) => m + 2),
                new MonkeyII(7, 4, 3, new Queue<Item> { 98, 51, 51, 63, 80, 85, 84, 95 }, (m) => m + 7),
                new MonkeyII(11, 6, 4, new Queue<Item> { 77, 90, 82, 80, 79 }, (m) => m + 1),
                new MonkeyII(13, 6, 5, new Queue<Item> { 68 }, (m) => m * 5),
                new MonkeyII(3, 1, 0, new Queue<Item> { 60, 94 }, (m) => m + 5),
                new MonkeyII(5, 5, 1, new Queue<Item> { 81, 51, 85 }, (m) => m.MultiplySelf()),
                new MonkeyII(2, 2, 3, new Queue<Item> { 98, 81, 63, 65, 84, 71, 84 }, (m) => m + 3)
            };

        public static string Run()
        {
            var monkeys = monkeysInput;
            Dictionary<int, long> monkeyTransactionCount = new Dictionary<int, long>();

            for (int i = 0; i < 10000; i++)
            {
                for (int mokIndex = 0; mokIndex < monkeys.Length; mokIndex++)
                {
                    var mok = monkeys[mokIndex];
                    while (mok.Items.TryDequeue(out var item))
                    {
                        IncrementCount(monkeyTransactionCount, mokIndex);

                        // run the tests for each monky
                        item = mok.Operation(item);

                        // run
                        var mod = item.Mod(mok.Mod);

                        if (mod == 0)
                            monkeys[mok.ThrowMonkeyIdTrue].Items.Enqueue(item);
                        else
                            monkeys[mok.ThrowMonkeyIdFalse].Items.Enqueue(item);
                    }
                }

                //PrintItemsPerMonkey(monkeys, i);
            }

            PrintScore(monkeyTransactionCount, out Int128 score);

            bool check = true;
            if (check)
            {
                var testResult = 2713310158;
                var inputResult = 25712998901;
                if (score != inputResult && score != testResult)
                    throw new Exception("Bad output");
            }

            return "";

        }

        private static void PrintItemsPerMonkey(MonkeyII[] monkeys, int i)
        {
            // LOG
            Console.WriteLine();
            Console.WriteLine($"After round {i}:");
            PrintWorryLevels(monkeys);
        }

        private static void PrintScore(Dictionary<int, long> monkeyTransactionCount, out Int128 score)
        {
            Console.WriteLine();

            // Print monkeys transaction counts
            Console.WriteLine("Monkey ID and TransactionCount:");
            foreach (var run in monkeyTransactionCount)
                Console.WriteLine(run.ToString());

            Console.WriteLine();

            // monkey business indicator
            var comparer = new CustomComparerLong();
            var sortedDictionary = monkeyTransactionCount.ToImmutableSortedSet(comparer).Reverse();
            var first = sortedDictionary.First().Value;
            var second = sortedDictionary.Skip(1).First().Value;
            score = first * second;
            Console.Write($"First two most active monkeys multiplied scores: {first}*{second}= {score}");
        }

        private static void IncrementCount(Dictionary<int, long> monkeyTransactionCount, int mokIndex)
        {
            // count transactions
            if (monkeyTransactionCount.TryGetValue(mokIndex, out long monkeyRun))
                monkeyTransactionCount[mokIndex]++;
            else
                monkeyTransactionCount[mokIndex] = 1;
        }


        private static void PrintWorryLevels(MonkeyII[] monkeys)
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
