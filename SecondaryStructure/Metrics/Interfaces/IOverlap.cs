using SecondaryStructure.Regions;

namespace SecondaryStructure.Metrics.Interfaces
{
    internal abstract class IOverlap : IMetric
    {
        internal readonly Dictionary<char, List<Region>> OverlappingRegions = new();
        internal readonly Dictionary<char, double> PartialComputation = new();

        protected IOverlap(string refPath, string predPath) : base(refPath, predPath)
        {
            CollectOverlapRegions();
        }

        public override void Precomputation()
        {
            foreach (char secondaryStructure in secondaryStructureClasses)
            {
                PartialComputation.Add(secondaryStructure, CommonComputation(secondaryStructure));
            }
        }

        public override double ComputeAllClasses()
        {
            return PartialComputation.Sum(x => x.Value) / refLengthForSS.Sum(x => x.Value);
        }

        public override double ComputeOneClass(char secondaryStructure)
        {
            return PartialComputation[secondaryStructure] / refLengthForSS[secondaryStructure];
        }

        public override double CommonComputation(char secondaryStructure)
        {
            double summation = 0;
            if (OverlappingRegions.ContainsKey(secondaryStructure))
            {
                foreach (OverlapRegion overlappingRegion in OverlappingRegions[secondaryStructure])
                {
                    summation += Theta(secondaryStructure, overlappingRegion) * overlappingRegion.refRegion.Length;
                }
            }
            return summation;
        }

        internal abstract double Theta(char secondaryStructure, OverlapRegion overlappingRegion);

        internal virtual int MinOv(OverlapRegion region)
        {
            return Utils.OverlapAmount(region);
        }

        internal virtual int MaxOv(OverlapRegion region)
        {
            return region.Length;
        }

        internal virtual double DeltaSov(OverlapRegion region)
        {
            double[] choices = {
                MaxOv(region) - MinOv(region),
                MinOv(region),
                region.refRegion.Length / 2.0 };
            return choices.Min();
        }

        private void CollectOverlapRegions()
        {
            foreach (char secondaryStructure in secondaryStructureClasses)
            {
                var refSequenceRegions = Utils.GetRegionsForSequence(refSequence, secondaryStructure);
                var predSequenceRegions = Utils.GetRegionsForSequence(predSequence, secondaryStructure);

                foreach (var refRegion in refSequenceRegions)
                {
                    foreach (var predRegion in predSequenceRegions)
                    {
                        if (predRegion.From <= refRegion.To && refRegion.From <= predRegion.To)
                        {
                            var overlapRegion = new OverlapRegion(refRegion, predRegion);
                            Utils.AddToDictSSRegionList(OverlappingRegions, secondaryStructure, overlapRegion);
                        }
                    }
                }
            }
        }
    }
}
