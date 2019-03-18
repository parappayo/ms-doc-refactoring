using System;

namespace CancellationTokenExample.Math
{
    public interface IIntegerSequence
    {
        int Next();
    }

    public class Math
    {
        public static double Mean(int[] values)
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
    }

    public class SharedRandom
    {
        private Random Random = new Random();
        private object Lock = new object();

        public int Next(int minValue, int maxValue)
        {
            lock (this.Lock)
            {
                return this.Random.Next(minValue, maxValue);
            }
        }
    }

    public class RandomIntegers : IIntegerSequence
    {
        private SharedRandom SharedRandom = new SharedRandom();
        public readonly int Min;
        public readonly int Max;

        public RandomIntegers(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        public int Next()
        {
            return this.SharedRandom.Next(this.Min, this.Max + 1);
        }
    }

}
