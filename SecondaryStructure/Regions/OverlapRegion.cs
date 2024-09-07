namespace SecondaryStructure.Regions
{
    internal class OverlapRegion : Region
    {
        internal SSRegion refRegion;
        internal SSRegion predRegion;
        internal OverlapRegion(SSRegion refRegion, SSRegion predRegion)
        {
            From = Math.Min(refRegion.From, predRegion.From);
            To = Math.Max(refRegion.To, predRegion.To);
            this.refRegion = new SSRegion(refRegion.From, refRegion.To);
            this.refRegion.Sequence = PadRegionSequence(refRegion, From, To);
            this.predRegion = new SSRegion(predRegion.From, predRegion.To);
            this.predRegion.Sequence = PadRegionSequence(predRegion, From, To);
            if (this.refRegion.Sequence.Length != this.predRegion.Sequence.Length)
                throw new Exception("Padded regions do not have equal lengths");
        }

        private string PadRegionSequence(SSRegion region, int from, int to)
        {
            string paddedSequence;
            int padLeftAmount = region.From - from;
            if (padLeftAmount > 0)
                paddedSequence = region.Sequence.PadLeft(region.Sequence.Length + padLeftAmount);
            else
                paddedSequence = region.Sequence;
            int padRightAmount = to - region.To;
            if (padRightAmount > 0)
                paddedSequence = region.Sequence.PadRight(paddedSequence.Length + padRightAmount);
            return paddedSequence;
        }
    }
}
