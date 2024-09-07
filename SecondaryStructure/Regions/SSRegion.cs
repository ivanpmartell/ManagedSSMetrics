namespace SecondaryStructure.Regions
{
    internal class SSRegion : Region
    {
        private string _sequence = "";
        internal string Sequence
        {
            get
            {
                if (_sequence.Trim().Length != Length)
                    throw new Exception("Wrong sequence for region");
                return _sequence;
            }
            set
            {
                if (String.IsNullOrEmpty(_sequence))
                    _sequence = value;
            }
        }
        internal SSRegion(int from, int to)
        {
            From = from;
            To = to;
        }
    }
}
