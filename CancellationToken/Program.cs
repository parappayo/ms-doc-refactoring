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
            var tokenSource = new CancellationTokenSource();
            var sharedRandom = new SharedRandom();
            int taskCount = 10;

            try
            {
                double result = RunTasks(
                    () =>
                    {
                        return GenerateValues(tokenSource, sharedRandom);
                    },
                    (results) =>
                    {
                        Console.WriteLine("Calculating overall mean...");
                        return CalculateMean(Flatten(results));
                    },
                    tokenSource,
                    taskCount);

                Console.WriteLine("The mean is {0}.", result);
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
        }

        public static T[] Flatten<T>(Task<T[]>[] tasks)
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

        public static double CalculateMean(int[] values)
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

        public static TResult RunTasks<T, TResult>(
            Func<T> taskFunc,
            Func<Task<T>[], TResult> continuationFunc,
            CancellationTokenSource tokenSource,
            int taskCount)
        {
            var taskFactory = new TaskFactory(tokenSource.Token);

            try
            {
                Task<TResult> allTasks = taskFactory.ContinueWhenAll(
                    StartTasks(taskFunc, taskCount, taskFactory, tokenSource).ToArray(),
                    continuationFunc,
                    tokenSource.Token);

                return allTasks.Result;
            }
            finally
            {
                tokenSource.Dispose();
            }
        }

        public static List<Task<T>> StartTasks<T>(
            Func<T> taskFunc,
            int count,
            TaskFactory factory,
            CancellationTokenSource tokenSource)
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
