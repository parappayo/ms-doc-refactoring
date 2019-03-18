using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using CancellationTokenExample.Math;

namespace CancellationTokenExample
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void StartTasks_PopulatesTasks()
        {
            var randomIntegers = new RandomIntegers(0, 100);
            var tokenSource = new CancellationTokenSource();
            var taskFactory = new TaskFactory(tokenSource.Token);
            int taskCount = 10;

            var tasks = Tasks.Tasks.Start(
                () => { return Program.GenerateValues(tokenSource, randomIntegers); },
                taskCount,
                taskFactory,
                tokenSource);
            Assert.IsNotEmpty(tasks);
        }

        [Test]
        public void GenerateValues_ReturnsValues()
        {
            var randomIntegers = new RandomIntegers(0, 100);
            var tokenSource = new CancellationTokenSource();
            var factory = new TaskFactory(tokenSource.Token);

            var values = Program.GenerateValues(tokenSource, randomIntegers);
            Assert.IsNotEmpty(values);
        }
    }
}
