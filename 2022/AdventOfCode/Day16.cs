using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AdventOfCode
{
    internal sealed class Day16
    {

        private record struct Pipe(string Name, int FlowRate, List<string> PathToPipes);
        private record struct CacheKeyDay16(int minuteH, int minuteE, Int128 humanCurrentPipe, Int128 elephantCurrentPipe, Int128 openedPipesFlag);


        /// <summary>
        /// Adds a Flag for every new element. The flags grouped (and incremented) by the Type of TElement. <br/>
        /// </summary>
        private struct FlaggedElement<TElement>
        {
            private static Dictionary<Type, Int128> idGen = new(); // used to generate unique flags for each pipe
            private static Dictionary<Type, Int128> allFlags = new();

            public TElement Element { get; set; }

            public FlaggedElement(TElement element)
            {
                Element = element;
                IdFlag = GenId(element.GetType());
            }

            public Int128 IdFlag { get; private set; }

            private static Int128 GenId(Type elementType)
            {
                if (!idGen.ContainsKey(elementType))
                {
                    idGen[elementType] = 1;
                    allFlags[elementType] = 1;
                }

                var id = idGen[elementType];
                idGen[elementType] <<= 1;
                allFlags[elementType] = (allFlags[elementType] << 1) + 1;
                return id;
            }

            public static Int128 GetAllFlang()
            {
                if (allFlags.TryGetValue(typeof(TElement), out var val))
                    return val;
                return 0;
            }

            public Int128 GetAllFlag() => GetAllFlang();
        }


        public static string FirstPart()
        {
            var inputs = File.ReadAllLines("day16_input.txt");
            List<Pipe> pipes = new();

            FillPipesFromInput(inputs, pipes);
            //var start = Stopwatch.GetTimestamp();
            var pressureReleased = ReleasePressure30m(pipes);
            //Console.WriteLine("Elapsed: " + Stopwatch.GetElapsedTime(start));
            return pressureReleased.ToString();
        }



        // takes about 5 minutes, but whatever
        public static string SecondPart()
        {
            var inputs = File.ReadAllLines("day16_input.txt");
            List<Pipe> pipes = new();

            FillPipesFromInput(inputs, pipes);
            //var start = Stopwatch.GetTimestamp();
            var pressureReleased = ReleasePressure30m(pipes, true);
            //Console.WriteLine("Elapsed: " + Stopwatch.GetElapsedTime(start));
            Console.Beep();
            return pressureReleased.ToString();
        }



        private static int ReleasePressure30m(List<Pipe> pipes, bool withHelp = false)
        {
            var pipeToPipeDistance = new int[pipes.Count, pipes.Count];
            FillDistances(pipes, pipeToPipeDistance);

            Dictionary<CacheKeyDay16, int> cache = new Dictionary<CacheKeyDay16, int>();

            int aaPipeId = pipes.IndexOf(pipes.First(x => x.Name == "AA")); // took me a while to find that AA isn't always at 0
            if (!withHelp)
                return OpenRecursive1(-1, aaPipeId, pipeToPipeDistance, 0, pipes.Select(x => new FlaggedElement<Pipe>(x)).ToList(), 30, cache);
            else
            {
                // takes about 5 minutes, but whatever
                var ws = new WorkingPipeSet(-1, aaPipeId, 26);
                return OpenRecursive2_Parallel(ws, ws, pipeToPipeDistance, 0, pipes.Select(x => new FlaggedElement<Pipe>(x)).ToList());
            }
        }




        private static int OpenRecursive1(int lastPipeId, int currentPipeId, int[,] pipeToPipeDistance, Int128 openedPipes, List<FlaggedElement<Pipe>> pipes, int minutesLeft, Dictionary<CacheKeyDay16, int> cache)
        {
            if (minutesLeft < 1)
                return 0;


            int currentRelease = 0;

            // open if not first
            if (lastPipeId >= 0)
            {
                var distance = pipeToPipeDistance[lastPipeId, currentPipeId] + 1;
                minutesLeft -= distance;
                currentRelease = (minutesLeft) * pipes[currentPipeId].Element.FlowRate;
            }
            var flags = FlaggedElement<Pipe>.GetAllFlang();
            if (openedPipes == flags)
                return currentRelease;


            var releasedByNext = 0;
            for (int i = 0; i < pipes.Count; i++)
            {
                var nextPipe = pipes[i];

                if (ShouldSkip(nextPipe.IdFlag, nextPipe.Element, openedPipes))
                    continue;

                int nextRelease;
                var nextOpenedPipes = openedPipes | nextPipe.IdFlag;
                //var key = new CacheKeyDay16(minutesLeft, nextPipe.Element.Name, nextOpenedPipes);

                //if (cache.TryGetValue(key, out var val))
                //    nextRelease = val;
                //else
                //{
                nextRelease = OpenRecursive1(currentPipeId, i, pipeToPipeDistance, nextOpenedPipes, pipes, minutesLeft, cache);
                //    cache[key] = nextRelease;
                //}
                

                if (nextRelease > releasedByNext)
                    releasedByNext = nextRelease;
            }

            return currentRelease + releasedByNext;
        }

        


        private record struct WorkingPipeSet(int lastPipeId, int currentPipeId, int minutesLeft);

        // takes about 5 minutes, but whatever
        private static int OpenRecursive2_Parallel(WorkingPipeSet human, WorkingPipeSet elephant, int[,] pipeToPipeDistance, Int128 openedPipes, List<FlaggedElement<Pipe>> pipes)
        {
            List<Task<int>> results = new List<Task<int>>();

            for (int humanNextIndex = 0; humanNextIndex < pipes.Count; humanNextIndex++)
            {
                var nextPipe = pipes[humanNextIndex];

                if (ShouldSkip(nextPipe.IdFlag, nextPipe.Element, openedPipes))
                    continue;

                // store task variables
                var threadHumanNextIndex = humanNextIndex;
                var threadPipe = nextPipe;
                var threadPipes = pipes.ToList();
                var x = pipeToPipeDistance.GetLength(0);
                var y = pipeToPipeDistance.GetLength(1);
                var threadPipeToPipeDistance = new int[x, y];
                {
                    for (int i = 0; i < x; i++)
                        for (int j = 0; j < y; j++)
                        {
                            var a = pipeToPipeDistance[i, j];
                            threadPipeToPipeDistance[i, j] = a;
                        }
                }

                // spin a task
                results.Add(Task.Factory.StartNew(() => Worker(human, elephant, pipeToPipeDistance, openedPipes, threadHumanNextIndex, threadPipe, threadPipes)));
            }


            // task worker
            static int Worker(WorkingPipeSet human, WorkingPipeSet elephant, int[,] pipeToPipeDistance, Int128 openedPipes, int threadHumanNextIndex, FlaggedElement<Pipe> threadPipe, List<FlaggedElement<Pipe>> threadPipes)
            {
                int releasedByNext = 0;
                for (int elephantNextIndex = 0; elephantNextIndex < threadPipes.Count; elephantNextIndex++)
                {
                    var openedPipesAfterHuman = openedPipes | threadPipe.IdFlag;
                    var nextPipeE = threadPipes[elephantNextIndex];
                    if (ShouldSkip(nextPipeE.IdFlag, nextPipeE.Element, openedPipesAfterHuman))
                        continue;

                    int nextRelease;
                    var nextOpenedPipes = openedPipesAfterHuman | nextPipeE.IdFlag;

                    nextRelease = OpenRecursive2(new WorkingPipeSet(human.currentPipeId, threadHumanNextIndex, human.minutesLeft),
                                                 new WorkingPipeSet(elephant.currentPipeId, elephantNextIndex, elephant.minutesLeft),
                                                 pipeToPipeDistance, nextOpenedPipes, threadPipes);

                    if (nextRelease > releasedByNext)
                        releasedByNext = nextRelease;
                }
                return releasedByNext;
            }

            // wait for resultS and compare
            Task.WaitAll(results.ToArray());
            var max = 0;
            foreach (var t in results)
            {
                if (t.Result > max)
                    max = t.Result;
            }
            return max;

        }



        private static int OpenRecursive2(WorkingPipeSet human, WorkingPipeSet elephant, int[,] pipeToPipeDistance, Int128 openedPipes, List<FlaggedElement<Pipe>> pipes)
        {
            if (human.minutesLeft < 1 && elephant.minutesLeft < 1)
                return 0;


            int currentRelease = 0;

            // open if not first
            if (human.lastPipeId >= 0)
            {
                var distanceForHuman = pipeToPipeDistance[human.lastPipeId, human.currentPipeId] + 1;
                var distanceForElephant = pipeToPipeDistance[elephant.lastPipeId, elephant.currentPipeId] + 1;

                human.minutesLeft -= distanceForHuman;
                elephant.minutesLeft -= distanceForElephant;

                if (human.minutesLeft > 0)
                    currentRelease += human.minutesLeft * pipes[human.currentPipeId].Element.FlowRate;
                if (elephant.minutesLeft > 0)
                    currentRelease += elephant.minutesLeft * pipes[elephant.currentPipeId].Element.FlowRate;
            }

            var flags = FlaggedElement<Pipe>.GetAllFlang();
            if (openedPipes == flags)
                return currentRelease;
            if (human.minutesLeft < 3 && elephant.minutesLeft < 3)
                return currentRelease;

            var releasedByNext = 0;

            for (int humanNextIndex = 0; humanNextIndex < pipes.Count; humanNextIndex++)
            {
                var nextPipe = pipes[humanNextIndex];

                if (ShouldSkip(nextPipe.IdFlag, nextPipe.Element, openedPipes))
                    continue;



                    var openedPipesAfterHuman = openedPipes | nextPipe.IdFlag;
                for (int elephantNextIndex = 0; elephantNextIndex < pipes.Count; elephantNextIndex++)
                {
                    var nextPipeE = pipes[elephantNextIndex];
                    if (ShouldSkip(nextPipeE.IdFlag, nextPipeE.Element, openedPipesAfterHuman))
                        continue;

                    int nextRelease;
                    var nextOpenedPipes = openedPipesAfterHuman | nextPipeE.IdFlag;
                    var key = new CacheKeyDay16(human.minutesLeft, elephant.minutesLeft, nextPipe.IdFlag, nextPipeE.IdFlag, nextOpenedPipes);


                        nextRelease = OpenRecursive2(new WorkingPipeSet(human.currentPipeId, humanNextIndex, human.minutesLeft),
                                                 new WorkingPipeSet(elephant.currentPipeId, elephantNextIndex, elephant.minutesLeft),
                                                 pipeToPipeDistance, nextOpenedPipes, pipes);

                    if (nextRelease > releasedByNext)
                        releasedByNext = nextRelease;
                }
            }

            return currentRelease + releasedByNext;
        }





        private static bool ShouldSkip(Int128 pipeFlag, Pipe pipe, Int128 openedPipes)
        {
            // skip if already opened
            if ((pipeFlag & openedPipes) > 0)
                return true;
            if (pipe.FlowRate == 0)
                return true;
            return false;
        }

        private static void FillDistances(List<Pipe> pipes, int[,] pipeToPipeDistanceMap)
        {
            pipeToPipeDistanceMap ??= new int[pipes.Count, pipes.Count];

            for (int i = 0; i < pipes.Count; i++)
            {
                Pipe from = pipes[i];
                for (int j = 0; j < pipes.Count; j++)
                {
                    Pipe to = pipes[j];
                    var distance = FindDistance(pipes, from, to);
                    pipeToPipeDistanceMap[i, j] = distance;
                }
            }
        }

        private static int FindDistance(List<Pipe> pipes, Pipe from, Pipe to)
        {
            List<Pipe> treeAtCurrentLevel = new() { from };
            for (int distance = 0; ; distance++)
            {
                var workingTree = treeAtCurrentLevel;
                treeAtCurrentLevel = new();
                if (workingTree.Count == 0)
                    throw new Exception("Closed Loop Detected");
                foreach (var currentPipe in workingTree)
                {
                    if (currentPipe.Name == to.Name)
                        return distance;
                    foreach (string pipeName in currentPipe.PathToPipes)
                    {
                        if (treeAtCurrentLevel.All(x => x.Name != pipeName))
                            treeAtCurrentLevel.Add(pipes.First(p => p.Name == pipeName));
                    }
                }
            }
        }

        private static void FillPipesFromInput(string[] inputs, List<Pipe> pipes)
        {
            foreach (var line in inputs)
            {
                var values = line.Split(new string[] { "Valve ", " has flow rate=", "; tunnels lead to valves ", "; tunnel leads to valve " }, StringSplitOptions.RemoveEmptyEntries);
                var pipe = new Pipe() { Name = values[0], FlowRate = int.Parse(values[1]), PathToPipes = values[2].Split(", ", StringSplitOptions.RemoveEmptyEntries).ToList() };
                pipes.Add(pipe);
            }
        }

    }
}
