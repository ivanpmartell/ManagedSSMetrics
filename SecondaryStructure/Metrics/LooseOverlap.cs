using SecondaryStructure.Metrics.Interfaces;
using SecondaryStructure.Regions;

namespace SecondaryStructure.Metrics
{
    internal class LooseOverlap : IOverlap
    {
        public LooseOverlap(string refPath, string predPath) : base(refPath, predPath) { }

        internal override double Theta(char secondaryStructure, OverlapRegion overlappingRegion)
        {
            int overlapCount = Utils.OverlapAmount(overlappingRegion);
            if (secondaryStructure != 'C')
            {
                if (overlapCount >= Math.Ceiling(overlappingRegion.refRegion.Length / 2.0))
                    return 1;
                else
                    return 0;
            }
            else
            {
                if (overlapCount >= 2)
                    return 1;
                else
                    return 0;
            }
        }
    }
}
