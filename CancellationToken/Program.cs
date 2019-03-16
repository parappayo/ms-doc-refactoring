﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CancellationTokenExample
{
    public class Program
    {
        public static void Main()
        {
            var rnd = new Random();
            var lockObj = new object();

            var tokenSource = new CancellationTokenSource();
            var factory = new TaskFactory(tokenSource.Token);
            var tasks = new List<Task<int[]>>();

            CreateTasks(tokenSource, rnd, lockObj, tasks, factory);

            try
            {
                Task<double> fTask = factory.ContinueWhenAll(tasks.ToArray(),
                   (results) =>
                   {
                       Console.WriteLine("Calculating overall mean...");
                       long sum = 0;
                       int n = 0;
                       foreach (var t in results)
                       {
                           foreach (var r in t.Result)
                           {
                               sum += r;
                               n++;
                           }
                       }
                       return sum / (double)n;
                   }, tokenSource.Token);
                Console.WriteLine("The mean is {0}.", fTask.Result);
            }
            catch (AggregateException ae)
            {
                foreach (Exception e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException)
                    {
                        Console.WriteLine("Unable to compute mean: {0}",
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

        public static void CreateTasks(CancellationTokenSource tokenSource, Random rnd, object lockObj, List<Task<int[]>> tasks, TaskFactory factory)
        {
            for (int taskCtr = 0; taskCtr <= 10; taskCtr++)
            {
                int iteration = taskCtr + 1;
                tasks.Add(factory.StartNew(() =>
                {
                    int value;
                    int[] values = new int[10];
                    for (int ctr = 1; ctr <= 10; ctr++)
                    {
                        lock (lockObj)
                        {
                            value = rnd.Next(0, 101);
                        }
                        if (value == 0)
                        {
                            tokenSource.Cancel();
                            Console.WriteLine("Cancelling at task {0}", iteration);
                            break;
                        }
                        values[ctr - 1] = value;
                    }
                    return values;
                }, tokenSource.Token));
            }
        }
    }
}
