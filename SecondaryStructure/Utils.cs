using SecondaryStructure.Regions;

namespace SecondaryStructure
{
    internal static class Utils
    {
        internal static void AddToDictSSRegionList(Dictionary<char, List<Region>> dict, char key, Region value)
        {
            if (dict.ContainsKey(key))
                dict[key].Add(value);
            else
                dict.Add(key, new List<Region>() { value });
        }

        internal static List<SSRegion> GetRegionsForSequence(string sequence, char secondaryStructure)
        {
            List<SSRegion> SSRegionsForSequence = new();
            for (int i = 0; i < sequence.Length; i++)
            {
                if (sequence[i] == secondaryStructure)
                {
                    if (i < 1)
                        SSRegionsForSequence.Add(new SSRegion(i, i));
                    else
                    {
                        if (sequence[i - 1] == secondaryStructure)
                        {
                            var lastRegion = SSRegionsForSequence[^1];
                            lastRegion.To = i;
                        }
                        else
                        {
                            SSRegionsForSequence.Add(new SSRegion(i, i));
                        }
                    }
                }
            }
            foreach (SSRegion region in SSRegionsForSequence)
            {
                region.Sequence = sequence.Substring(region.From, region.Length);
            }
            return SSRegionsForSequence;
        }

        internal static int OverlapAmount(OverlapRegion overlapRegion)
        {
            return Math.Min(overlapRegion.refRegion.To, overlapRegion.predRegion.To) - Math.Max(overlapRegion.refRegion.From, overlapRegion.predRegion.From) + 1;
        }
    }
}
