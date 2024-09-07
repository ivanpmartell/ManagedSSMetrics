using SecondaryStructure.Metrics.Interfaces;
using SecondaryStructure.Regions;

namespace SecondaryStructure.Metrics
{
    internal class Sov94 : ISov, IAdjustableDelta
    {
        private bool? _zeroDelta;
        public bool ZeroDelta
        {
            set
            {
                if (_zeroDelta == null)
                    _zeroDelta = value;
            }
            get { return (bool)(_zeroDelta != null ? _zeroDelta : false); }
        }

        public Sov94(string refPath, string predPath) : base(refPath, predPath) { }

        public override double ComputeAllClasses()
        {
            return PartialComputation.Sum(x => x.Value) / Normalization.Sum(x => x.Value);
        }

        public override double ComputeOneClass(char secondaryStructure)
        {
            return PartialComputation[secondaryStructure] / Normalization[secondaryStructure];
        }

        internal override int N(char secondaryStructure)
        {
            return refLengthForSS[secondaryStructure];
        }

        internal override double Delta(char secondaryStructure, OverlapRegion region)
        {
            if (ZeroDelta)
                return 0;
            int[] choices = {
                MaxOv(secondaryStructure, region) - MinOv(secondaryStructure, region),
                MinOv(secondaryStructure, region),
                region.refRegion.Length / 2 };
            return choices.Min();
        }
    }
}
