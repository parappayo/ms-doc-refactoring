using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CancellationTokenExample.Tasks
{
    public class Tasks
    {
        public static T[] FlattenResults<T>(Task<T[]>[] tasks)
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

        public static List<Task<T>> Start<T>(
            Func<T> taskFunc,
            int count,
            TaskFactory factory,
            CancellationTokenSource cancellationTokenSource)
        {
            var tasks = new List<Task<T>>();

            for (int i = 0; i <= count; i++)
            {
                tasks.Add(
                    factory.StartNew(
                        taskFunc,
                        cancellationTokenSource.Token));
            }

            return tasks;
        }

        public static TResult RunAll<T, TResult>(
            Func<T> taskFunc,
            Func<Task<T>[], TResult> continuationFunc,
            CancellationTokenSource cancellationTokenSource,
            int taskCount)
        {
            var taskFactory = new TaskFactory(cancellationTokenSource.Token);

            try
            {
                Task<TResult> allTasks = taskFactory.ContinueWhenAll(
                    Start(taskFunc, taskCount, taskFactory, cancellationTokenSource).ToArray(),
                    continuationFunc,
                    cancellationTokenSource.Token);

                return allTasks.Result;
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }
    }
}
