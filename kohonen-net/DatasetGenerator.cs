using MathNet.Numerics.LinearAlgebra;
using System;
using System.Linq;

namespace KohonenNet
{
    public class DatasetGenerator
    {
        private readonly int groupCount;

        public DatasetGenerator(int groupCount)
        {
            this.groupCount = groupCount;
        }

        public Dataset GenerateDataset(int propertyCount, int sampleCount)
        {
            if (!IsPropertyCountGroupCountRelationValid(propertyCount))
            {
                throw new PropertyCountGroupCountRelationViolationException(propertyCount, groupCount);
            }

            var classifiedSamples = new Tuple<Vector<double>, int>[sampleCount];
            for (var i = 0; i < sampleCount; i++)
                 classifiedSamples[i] = GenerateClassifiedSample(propertyCount, i % groupCount);
            RandomSingleton.Generator.Shuffle(classifiedSamples);

            var samples = classifiedSamples.Select(p => p.Item1).ToArray();
            var groups = classifiedSamples.Select(p => p.Item2).ToArray();
            return new Dataset(propertyCount, groupCount, samples, groups);
        }

        private Tuple<Vector<double>, int> GenerateClassifiedSample(int propertyCount, int group)
        {
            int k = propertyCount / groupCount;

            var sample = Vector<double>.Build.Dense(propertyCount);

            int gk = group * k;
            sample[gk] = 1;

            int a = gk + 1;
            int b = k - 1;
            sample.SetSubVector(
                a, b,
                sample
                    .SubVector(a, b)
                    .Map(s => RandomSingleton.ZeroOne())
            );

            return new Tuple<Vector<double>, int>(sample, group);
        }

        private bool IsPropertyCountGroupCountRelationValid(int propertyCount)
        {
            return propertyCount % groupCount == 0;
        }
    }
}
