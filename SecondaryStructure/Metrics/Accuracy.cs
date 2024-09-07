using SecondaryStructure.Metrics.Interfaces;
using SecondaryStructure.Regions;

namespace SecondaryStructure.Metrics
{
    internal class Accuracy : IMetric
    {
        internal readonly Dictionary<char, double> PartialComputation = new();
        public Accuracy(string refPath, string predPath) : base(refPath, predPath) { }

        public override double ComputeAllClasses()
        {
            double summation = 0;
            foreach (char secondaryStructure in secondaryStructureClasses)
            {
                summation += PartialComputation[secondaryStructure] / refSequence.Length;
            }
            return summation;
        }

        public override double ComputeOneClass(char secondaryStructure)
        {
            return PartialComputation[secondaryStructure] / refLengthForSS[secondaryStructure];
        }

        public override double CommonComputation(char secondaryStructure)
        {
            return Sum(0, refSequence.Length - 1, I, secondaryStructure);
        }

        public override void Precomputation()
        {
            foreach (char secondaryStructure in secondaryStructureClasses)
            {
                PartialComputation.Add(secondaryStructure, CommonComputation(secondaryStructure));
            }
        }
    }
}
