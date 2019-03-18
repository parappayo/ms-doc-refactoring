using System;
using System.Threading;
using System.Threading.Tasks;

using CancellationTokenExample.Math;

namespace CancellationTokenExample
{
    public class Program
    {
        public static void Main()
        {
            var tokenSource = new CancellationTokenSource();
            var randomIntegers = new RandomIntegers(0, 100);
            int taskCount = 10;

            try
            {
                int[] result = Tasks.Tasks.RunAll(
                    () =>
                    {
                        return GenerateValues(tokenSource, randomIntegers);
                    },
                    (results) =>
                    {
                        return Tasks.Tasks.FlattenResults(results);
                    },
                    tokenSource,
                    taskCount);

                Console.WriteLine("Calculating overall mean...");
                Console.WriteLine("The mean is {0}.", Math.Math.Mean(result));
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


        public static int[] GenerateValues(CancellationTokenSource cancellationToken, IIntegerSequence integerSequence)
        {
            int value;
            int[] values = new int[10];

            Console.WriteLine("Starting task {0}", Task.CurrentId);
            for (int ctr = 1; ctr <= 10; ctr++)
            {
                value = integerSequence.Next();

                if (value == 0)
                {
                    cancellationToken.Cancel();
                    Console.WriteLine("Cancelling at task {0}", Task.CurrentId);
                    break;
                }

                values[ctr - 1] = value;
            }

            return values;
        }
    }
}
