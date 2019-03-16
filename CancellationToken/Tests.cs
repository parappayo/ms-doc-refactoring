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
        public void CreateTasks_PopulatesTasks()
        {
            var rnd = new Random();
            var lockObj = new object();
            var tokenSource = new CancellationTokenSource();
            var factory = new TaskFactory(tokenSource.Token);

            var tasks = Program.CreateTasks(tokenSource, rnd, lockObj, factory);
            Assert.IsNotEmpty(tasks);
        }

        [Test]
        public void GenerateValues_ReturnsValues()
        {
            var rnd = new Random();
            var lockObj = new object();
            var tokenSource = new CancellationTokenSource();
            var factory = new TaskFactory(tokenSource.Token);

            var values = Program.GenerateValues(tokenSource, rnd, lockObj);
            Assert.IsNotEmpty(values);
        }
    }
}
