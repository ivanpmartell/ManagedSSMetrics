using SecondaryStructure.Metrics.Interfaces;
using SecondaryStructure.Regions;

namespace SecondaryStructure.Metrics
{
    internal class FractionalOverlap : IOverlap, IAdjustableDelta
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

        public FractionalOverlap(string refPath, string predPath) : base(refPath, predPath) { }

        internal override double Theta(char secondaryStructure, OverlapRegion overlappingRegion)
        {
            var deltaResult = ZeroDelta ? 0 : DeltaSov(overlappingRegion);
            return (Utils.OverlapAmount(overlappingRegion) + deltaResult) / overlappingRegion.Length;
        }
    }
}
