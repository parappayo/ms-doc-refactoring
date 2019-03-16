using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

namespace CancellationTokenExample
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void StartTasks_PopulatesTasks()
        {
            var sharedRandom = new SharedRandom();
            var tokenSource = new CancellationTokenSource();
            var taskFactory = new TaskFactory(tokenSource.Token);
            int taskCount = 10;

            var tasks = Program.StartTasks(
                () => { return Program.GenerateValues(tokenSource, sharedRandom); },
                taskCount,
                taskFactory,
                tokenSource);
            Assert.IsNotEmpty(tasks);
        }

        [Test]
        public void GenerateValues_ReturnsValues()
        {
            var sharedRandom = new SharedRandom();
            var tokenSource = new CancellationTokenSource();
            var factory = new TaskFactory(tokenSource.Token);

            var values = Program.GenerateValues(tokenSource, sharedRandom);
            Assert.IsNotEmpty(values);
        }
    }
}
