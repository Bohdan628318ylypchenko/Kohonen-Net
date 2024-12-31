using System;

namespace KohonenNet
{
    public static class RandomSingleton
    {
        private static Random generator = new Random();
        public static Random Generator => generator;

        public static void Initialize(int seed)
        {
            generator = new Random(seed);
        }

        public static double ZeroOne()
        {
            return Generator.NextDouble() >= 0.5 ? 1.0 : 0.0;
        }
    }
}
