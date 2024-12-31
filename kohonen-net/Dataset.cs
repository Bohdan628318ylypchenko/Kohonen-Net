using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KohonenNet
{
    public struct Dataset
    {
        public struct DatasetComparationUnit
        {
            public Vector<double> Sample { get; set; }
            public int ExpectedGroup { get; set; }
            public int ActualGroup { get; set; }

            public override string? ToString()
            {
                return $"sample {string.Join(" ", Sample)} | expected group = {ExpectedGroup}, actual group = {ActualGroup}";
            }
        }

        private readonly int propertyCount;
        private readonly int groupCount;

        private readonly Vector<double>[] samples;
        private readonly int[] groups;

        public int PropertyCount => propertyCount;
        public int GroupCount => groupCount;
        public Vector<double>[] Samples => samples;
        public int[] Groups => groups;
        public int SampleCount => samples.Length;

        public Dataset(int propertyCount, int groupCount, Vector<double>[] samples, int[] groups)
        {
            this.propertyCount = propertyCount;
            this.groupCount = groupCount;
            this.samples = samples;
            this.groups = groups;
        }

        public (Vector<double> sample, int group) this[int i] => (samples[i], groups[i]);

        public IEnumerable<DatasetComparationUnit> CompareDatasets(Dataset other)
        {
            if (PropertyCount != other.PropertyCount || GroupCount != other.GroupCount || SampleCount != other.SampleCount)
                throw new ArgumentException(
                    $"Can't compare dataset {GetHashCode()} (property count = {PropertyCount}, group count = {GroupCount}, sample count = {SampleCount}) and dataset {GetHashCode()} (property count = {PropertyCount}, group count = {GroupCount}, sample count = {SampleCount})"
                );

            var difference = new List<DatasetComparationUnit>();            
            for (var i = 0; i < SampleCount; i++)
            {
                if (groups[i] != other.Groups[i])
                    difference.Add(new DatasetComparationUnit()
                    {
                        Sample = samples[i],
                        ActualGroup = groups[i],
                        ExpectedGroup = other.Groups[i]
                    });
            }

            return difference;
        }

        public override string? ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Dataset {GetHashCode()}");
            stringBuilder.AppendLine($"Sample count = {SampleCount}");
            for (var i = 0; i < SampleCount; i++)
                stringBuilder.AppendLine($"sample {string.Join(" ", samples[i])} | group = {groups[i]}");
            stringBuilder.Remove(stringBuilder.Length - 1, 1);

            return stringBuilder.ToString();
        }

        public Dataset Clone()
        {
            var copiedSamples = samples.Select(s => s.Clone()).ToArray();

            var copiedGroups = new int[groups.Length];
            Array.Copy(groups, copiedGroups, groups.Length);

            return new Dataset(propertyCount, groupCount, copiedSamples, copiedGroups);
        }
    }
}
