using SecondaryStructure.Metrics.Interfaces;
using SecondaryStructure.Regions;

namespace SecondaryStructure.Metrics
{
    internal class SovRefine : ISov
    {
        private double? _lambda;
        internal double Lambda {
            set
            {
                if (_lambda == null)
                    _lambda = value;
            }
            get { return (double)(_lambda != null ? _lambda : 1); }
        }
        public SovRefine(string refPath, string predPath) : base(refPath, predPath) { }

        public override double ComputeOneClass(char secondaryStructure)
        {
            return PartialComputation[secondaryStructure] / Normalization[secondaryStructure];
        }

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

        internal override double Delta(char secondaryStructure, OverlapRegion overlappingRegion)
        {
            var deltaValue = DeltaAll() * (overlappingRegion.refRegion.Length / (double)refSequence.Length) * (MinOv(secondaryStructure, overlappingRegion) / (double)MaxOv(secondaryStructure, overlappingRegion));
            int threshold = MaxOv(secondaryStructure, overlappingRegion) - MinOv(secondaryStructure, overlappingRegion);
            if (deltaValue > threshold) { 
                return threshold;
            }
            return deltaValue;
        }

        private double DeltaAll()
        {
            double summation = 0;
            foreach (char secondaryStructure in secondaryStructureClasses)
            {
                foreach (SSRegion region in AllRefRegions[secondaryStructure])
                {
                    summation += Math.Pow(region.Length / (double)refSequence.Length, 2);
                }
            }
            return Lambda * (secondaryStructureClasses.Count / summation);
        }
    }
}
