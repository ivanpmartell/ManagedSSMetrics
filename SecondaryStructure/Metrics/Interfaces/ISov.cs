using SecondaryStructure.Regions;

namespace SecondaryStructure.Metrics.Interfaces
{
    internal abstract class ISov : IMetric
    {
        internal readonly Dictionary<char, int> Normalization = new();
        internal readonly Dictionary<char, double> PartialComputation = new();
        internal readonly Dictionary<char, List<Region>> OverlappingRegions = new();
        internal readonly Dictionary<char, List<Region>> NonOverlappingRegions = new();
        internal readonly Dictionary<char, List<SSRegion>> AllRefRegions = new();
        internal readonly Dictionary<char, List<SSRegion>> AllPredRegions = new();

        protected ISov(string refPath, string predPath) : base(refPath, predPath)
        {
            CollectAllRegions();
        }

        public override void Precomputation()
        {
            foreach (char secondaryStructure in secondaryStructureClasses)
            {
                PartialComputation.Add(secondaryStructure, CommonComputation(secondaryStructure));
                Normalization.Add(secondaryStructure, N(secondaryStructure));
            }
        }

        public override double CommonComputation(char secondaryStructure)
        {
            double summation = 0;
            if (OverlappingRegions.ContainsKey(secondaryStructure))
            {
                foreach (OverlapRegion overlappingRegion in OverlappingRegions[secondaryStructure])
                {
                    summation += (MinOv(secondaryStructure, overlappingRegion) + Delta(secondaryStructure, overlappingRegion)) / MaxOv(secondaryStructure, overlappingRegion) * overlappingRegion.refRegion.Length;
                }
            }
            return summation;
        }

        internal virtual int N(char secondaryStructure)
        {
            int summation = 0;
            if (OverlappingRegions.ContainsKey(secondaryStructure))
            {
                foreach (OverlapRegion overlappingSegment in OverlappingRegions[secondaryStructure])
                {
                    summation += overlappingSegment.refRegion.Length;
                }
            }
            if (NonOverlappingRegions.ContainsKey(secondaryStructure))
            {
                foreach (SSRegion nonOverlappingSegment in NonOverlappingRegions[secondaryStructure])
                {
                    summation += nonOverlappingSegment.Length;
                }
            }
            return summation;
        }

        internal virtual int MinOv(char secondaryStructure, OverlapRegion region)
        {
            return Sum(region, I, secondaryStructure);
        }

        internal virtual int MaxOv(char secondaryStructure, OverlapRegion region)
        {
            return Sum(region, T, secondaryStructure);
        }

        internal abstract double Delta(char secondaryStructure, OverlapRegion region);

        private void CollectAllRegions()
        {
            foreach (char secondaryStructure in secondaryStructureClasses)
            {
                AllRefRegions.Add(secondaryStructure, Utils.GetRegionsForSequence(refSequence, secondaryStructure));
                AllPredRegions.Add(secondaryStructure, Utils.GetRegionsForSequence(predSequence, secondaryStructure));

                foreach (var refRegion in AllRefRegions[secondaryStructure])
                {
                    var hasOverlap = false;
                    foreach (var predRegion in AllPredRegions[secondaryStructure])
                    {
                        if (predRegion.From <= refRegion.To && refRegion.From <= predRegion.To)
                        {
                            hasOverlap = true;
                            var overlapRegion = new OverlapRegion(refRegion, predRegion);
                            Utils.AddToDictSSRegionList(OverlappingRegions, secondaryStructure, overlapRegion);
                        }
                    }
                    if (!hasOverlap)
                    {
                        Utils.AddToDictSSRegionList(NonOverlappingRegions, secondaryStructure, refRegion);
                    }
                }
            }
        }
    }
}
