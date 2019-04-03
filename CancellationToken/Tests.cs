using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using CancellationTokenExample.Math;

namespace CancellationTokenExample
{
    public class AllZeroesSequence : IIntegerSequence
    {
        public int Next()
        {
            return 0;
        }
    }

    public class AllOnesSequence : IIntegerSequence
    {
        public int Next()
        {
            return 1;
        }
    }

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
        public void GenerateValues_HandlesAllZeroes()
        {
            // var exception = Assert.Throws<AggregateException>(
            //     delegate {
            //         Program.GenerateValues(
            //             new CancellationTokenSource(),
            //             new AllZeroesSequence());
            //     });

            var cancellationTokenSource = new CancellationTokenSource();

            var values = Program.GenerateValues(
                cancellationTokenSource,
                new AllZeroesSequence());

            Assert.IsTrue(cancellationTokenSource.IsCancellationRequested);
        }

        [Test]
        public void GenerateValues_HandlesAllOnes()
        {
            var values = Program.GenerateValues(
                new CancellationTokenSource(),
                new AllOnesSequence());

            Assert.IsNotEmpty(values);
            Array.ForEach(values, (value) => { Assert.AreEqual(value, 1); });
        }
    }
}
