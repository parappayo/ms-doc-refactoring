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
            try
            {
                int[] result = GenerateValuesInParallel(new RandomIntegers(0, 100));

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

        public static int[] GenerateValuesInParallel(IIntegerSequence integerSequence, int taskCount = 10)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            return Tasks.Tasks.RunAll(
                () =>
                {
                    return GenerateValues(cancellationTokenSource, integerSequence);
                },
                (results) =>
                {
                    return Tasks.Tasks.FlattenResults(results);
                },
                cancellationTokenSource,
                taskCount);
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
