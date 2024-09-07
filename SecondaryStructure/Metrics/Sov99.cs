using SecondaryStructure.Metrics.Interfaces;
using SecondaryStructure.Regions;

namespace SecondaryStructure.Metrics
{
    internal class Sov99 : ISov
    {
        public Sov99(string refPath, string predPath) : base(refPath, predPath) { }

        public override double ComputeAllClasses()
        {
            double summation = 0;
            int N_sum = 0;
            foreach (char secondaryStructure in secondaryStructureClasses)
            {
                summation += PartialComputation[secondaryStructure];
                N_sum += Normalization[secondaryStructure];
            }
            return summation / N_sum;
        }

        public override double ComputeOneClass(char secondaryStructure)
        {
            return PartialComputation[secondaryStructure] / Normalization[secondaryStructure];
        }

        internal override double Delta(char secondaryStructure, OverlapRegion region)
        {
            int[] choices = {
                MaxOv(secondaryStructure, region) - MinOv(secondaryStructure, region),
                MinOv(secondaryStructure, region),
                region.refRegion.Length / 2,
                region.predRegion.Length / 2 };
            return choices.Min();
        }
    }
}
