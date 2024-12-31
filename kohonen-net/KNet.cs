using MathNet.Numerics.LinearAlgebra;
using System;
using System.Linq;
using System.Text;

namespace KohonenNet
{
    public class KNet
    {
        private struct StateIK
        {
            public int Idx { get; set; }
            public int K { get; set; }
        }

        private Matrix<double> weights;
        private int[] groupOrder;

        private KNet(Matrix<double> weights, int[] groupOrder)
        {
            this.weights = weights;
            this.groupOrder = groupOrder;
        }

        public KNet(int propertyCount, int groupCount)
        {
            weights = Matrix<double>.Build.Dense(propertyCount, groupCount);
            weights.MapInplace(w => RandomSingleton.ZeroOne());
            groupOrder = new int[groupCount];
        }

        public int PropertyCount => weights.RowCount;
        public int GroupCount => weights.ColumnCount;

        public int Predict(Vector<double> sample)
        {
            return groupOrder[PredictAsInternal(sample)];
        }

        public void Train(Dataset dataset, int epochCount, double l)
        {
            var shuffledSamples = new Vector<double>[dataset.SampleCount];
            Array.Copy(dataset.Samples, shuffledSamples, dataset.SampleCount);
            for (var i = 0; i < epochCount; i++)
            {
                RandomSingleton.Generator.Shuffle(shuffledSamples);
                foreach (var sample in shuffledSamples)
                {
                    int indexOfGroup = PredictAsInternal(sample);
                    var columnOfGroup = weights.Column(indexOfGroup);
                    weights.SetColumn(indexOfGroup, columnOfGroup + (sample - columnOfGroup).Multiply(l));
                }
            }
            UpdateGroupOrder();
        }

        public Dataset PredictOnDataset(Dataset original)
        {
            int[] predictions = original.Samples.Select(sample => Predict(sample)).ToArray();
            return new Dataset(PropertyCount, GroupCount, original.Samples, predictions);
        }

        private int PredictAsInternal(Vector<double> sample)
        {
            Vector<double> knetOutput = weights.LeftMultiply(sample);
            return Array.IndexOf(knetOutput.AsArray(), knetOutput.Maximum());
        }

        private void UpdateGroupOrder()
        {
            for (var i = 0; i < GroupCount; i++)
            {
                groupOrder[i] = IdentifyGroupOfVector(weights.Column(i));
            }
        }

        private int IdentifyGroupOfVector(Vector<double> vector)
        {
            double[] sums = new double[GroupCount];
            for (var state = new StateIK { Idx = 0, K = vector.Count / GroupCount };
                 state.Idx < GroupCount; state.Idx++)
            {
                sums[state.Idx] = vector.SubVector(state.Idx * state.K, state.K).Sum();
            }
            return Array.IndexOf(sums, sums.Max());
        }

        public override string? ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"KNet {GetHashCode()}");
            stringBuilder.AppendLine($"Property count = {PropertyCount}, Group count = {GroupCount}");
            stringBuilder.AppendLine(weights.ToString(PropertyCount, GroupCount));
            stringBuilder.Append($"Group order = {string.Join(" ", groupOrder)}");

            return stringBuilder.ToString();
        }

        public KNet Clone()
        {
            var copiedWeights = weights.Clone();

            var copiedGroupOrder = new int[GroupCount];
            Array.Copy(groupOrder, copiedGroupOrder, GroupCount);

            return new KNet(copiedWeights, copiedGroupOrder);
        }
    }
}
