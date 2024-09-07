namespace SecondaryStructure.Regions
{
    public abstract class Region
    {
        private int _to;
        private int _length;
        internal int To
        {
            get { return _to; }
            set
            {
                if (value < From)
                    throw new Exception("Region 'To' cannot be less than 'From'");
                _to = value;
                _length = _to - From + 1;
            }
        }

        internal int From { init; get; }
        internal int Length
        {
            get
            {
                return _length;
            }
        }
    }
}
