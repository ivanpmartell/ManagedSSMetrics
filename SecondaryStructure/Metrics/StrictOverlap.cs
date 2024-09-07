using SecondaryStructure.Metrics.Interfaces;
using SecondaryStructure.Regions;

namespace SecondaryStructure.Metrics
{
    internal class StrictOverlap : IOverlap
    {

        public StrictOverlap(string refPath, string predPath) : base(refPath, predPath) { }

        internal override double Theta(char secondaryStructure, OverlapRegion overlappingRegion)
        {
            if (Math.Abs(overlappingRegion.refRegion.Length - overlappingRegion.predRegion.Length) <= DeltaSov(overlappingRegion) &&
                Math.Abs(overlappingRegion.refRegion.From - overlappingRegion.predRegion.From) <= DeltaStrict(overlappingRegion) &&
                Math.Abs(overlappingRegion.refRegion.To - overlappingRegion.predRegion.To) <= DeltaStrict(overlappingRegion))
                return 1;
            else
                return 0;
        }

        private int DeltaStrict(OverlapRegion overlappingRegion)
        {
            if (overlappingRegion.refRegion.Length <= 5)
                return 1;
            else if (overlappingRegion.refRegion.Length <= 10)
                return 2;
            else
                return 3;
        }
    }
}
