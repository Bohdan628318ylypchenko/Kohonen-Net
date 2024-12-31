using KohonenNet;
using MathNet.Numerics;
using System;
using System.Linq;
using System.Text;

namespace Playground
{
    internal class Program
    {
        private struct Experiment
        {
            public int EpochCount { get; set; }
            public double L {  get; set; }

            public String Execute(KNet knet, Dataset dataset)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"Experiment: EpochCount = {EpochCount}, L = {L}");

                knet.Train(dataset, EpochCount, L);
                stringBuilder.AppendLine(knet.ToString());
                stringBuilder.Append('\n');

                var difference = knet
                    .PredictOnDataset(dataset)
                    .CompareDatasets(dataset);
                stringBuilder.AppendLine($"Difference (count = {difference.Count()})");
                foreach (var d in difference)
                {
                    stringBuilder.AppendLine(d.ToString());
                }

                return stringBuilder.ToString();
            }
        }

        static void Main(string[] args)
        {
            RandomSingleton.Initialize(1450);

            Control.UseNativeMKL();
            Console.Write(
            $"""
            ===
            MathNet state:
            {Control.Describe()}
            ===


            """);

            Console.WriteLine("=== Research 8 groups ===");
            Research8Groups();
            Console.WriteLine();

            Console.WriteLine("=== Research 16 groups ===");
            Research16Groups();
            Console.WriteLine();
        }

        private static void Research8Groups()
        {
            const int propertyCount = 32;
            const int groupCount = 8;
            const int sampleCount = 25;

            var originalDataset = new DatasetGenerator(groupCount).GenerateDataset(propertyCount, sampleCount);
            var knet = new KNet(propertyCount, groupCount);

            Console.WriteLine("Original dataset:");
            Console.WriteLine(originalDataset.ToString());
            Console.WriteLine();

            var experiments = new Experiment[]
            {
                new Experiment() { EpochCount = 1, L = 0.0 },
                new Experiment() { EpochCount = 10, L = 0.05 },
                new Experiment() { EpochCount = 20, L = 0.01 }
            };
            foreach (var experiment in experiments)
            {
                Console.WriteLine(experiment.Execute(knet, originalDataset));
            }
        }

        private static void Research16Groups()
        {
            const int propertyCount = 32;
            const int groupCount = 16;
            const int sampleCount = 25;

            var originalDataset = new DatasetGenerator(groupCount).GenerateDataset(propertyCount, sampleCount);
            var knet = new KNet(propertyCount, groupCount);

            Console.WriteLine("Original dataset:");
            Console.WriteLine(originalDataset.ToString());
            Console.WriteLine();

            var experiments = new Experiment[]
            {
                new Experiment() { EpochCount = 1, L = 0.0 },
                new Experiment() { EpochCount = 10, L = 0.05 },
                new Experiment() { EpochCount = 20, L = 0.01 },
                new Experiment() { EpochCount = 20, L = 0.01 }
            };
            foreach (var experiment in experiments)
            {
                Console.WriteLine(experiment.Execute(knet, originalDataset));
            }
        }
    }
}
