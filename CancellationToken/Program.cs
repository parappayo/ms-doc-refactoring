using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CancellationTokenExample
{
    public class SharedRandom
    {
        private Random Random = new Random();
        private object Lock = new object();

        public int Next(int minValue, int maxValue)
        {
            lock (Lock)
            {
                return Random.Next(minValue, maxValue);
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            var sharedRandom = new SharedRandom();
            var tokenSource = new CancellationTokenSource();
            var taskFactory = new TaskFactory(tokenSource.Token);
            int taskCount = 10;

            var tasks = CreateTasks(
                () => { return GenerateValues(tokenSource, sharedRandom); },
                taskCount,
                taskFactory,
                tokenSource);

            try
            {
                Task<double> allTasks = taskFactory.ContinueWhenAll(
                    tasks.ToArray(),
                    (results) =>
                    {
                        Console.WriteLine("Calculating overall mean...");
                        return CalculateMean(Flatten(results));
                    },
                    tokenSource.Token);

                Console.WriteLine("The mean is {0}.", allTasks.Result);
            }
            catch (AggregateException ae)
            {
                foreach (Exception e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException)
                    {
                        Console.WriteLine(
                            "Unable to compute mean: {0}",
                            ((TaskCanceledException)e).Message);
                    }
                    else
                    {
                        Console.WriteLine("Exception: " + e.GetType().Name);
                    }
                }
            }
            finally
            {
                tokenSource.Dispose();
            }
        }

        private static T[] Flatten<T>(Task<T[]>[] tasks)
        {
            var flatResults = new List<T>();

            foreach (var task in tasks)
            {
                foreach (var result in task.Result)
                {
                    flatResults.Add(result);
                }
            }

            return flatResults.ToArray();
        }

        private static double CalculateMean(int[] values)
        {
            long sum = 0;
            int n = 0;

            foreach (var v in values)
            {
                sum += v;
                n++;
            }

            return sum / (double)n;
        }

        public static List<Task<T>> CreateTasks<T>(Func<T> taskFunc, int count, TaskFactory factory, CancellationTokenSource tokenSource)
        {
            var tasks = new List<Task<T>>();

            for (int i = 0; i <= count; i++)
            {
                tasks.Add(
                    factory.StartNew(
                        taskFunc,
                        tokenSource.Token));
            }

            return tasks;
        }

        public static int[] GenerateValues(CancellationTokenSource tokenSource, SharedRandom sharedRandom)
        {
            int value;
            int[] values = new int[10];

            Console.WriteLine("Starting task {0}", Task.CurrentId);
            for (int ctr = 1; ctr <= 10; ctr++)
            {
                value = sharedRandom.Next(0, 101);

                if (value == 0)
                {
                    tokenSource.Cancel();
                    Console.WriteLine("Cancelling at task {0}", Task.CurrentId);
                    break;
                }

                values[ctr - 1] = value;
            }

            return values;
        }
    }
}
