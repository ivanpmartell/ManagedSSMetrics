using SecondaryStructure.Regions;

namespace SecondaryStructure.Metrics.Interfaces
{
    internal abstract class IMetric
    {
        internal readonly Dictionary<char, int> refLengthForSS = new();
        internal readonly Dictionary<char, int> predLengthForSS = new();
        internal readonly string refSequence;
        internal readonly string predSequence;
        internal readonly HashSet<char> secondaryStructureClasses;
        protected IMetric(string refPath, string predPath)
        {
            refSequence = ReadSingleEntryFastaSequence(refPath);
            predSequence = ReadSingleEntryFastaSequence(predPath);
            if (refSequence.Length != predSequence.Length)
                throw new Exception("Reference and Prediction sequences are not the same length");
            secondaryStructureClasses = new HashSet<char>(refSequence);
            foreach (char secondaryStructure in secondaryStructureClasses)
            {
                refLengthForSS.Add(secondaryStructure, SequenceLengthForSS(secondaryStructure, ref refSequence));
                predLengthForSS.Add(secondaryStructure, SequenceLengthForSS(secondaryStructure, ref predSequence));
            }
        }
        public abstract double ComputeAllClasses();

        public abstract double ComputeOneClass(char secondaryStructure);

        public abstract double CommonComputation(char secondaryStructure);

        public abstract void Precomputation();

        internal int I(int idx, char secondaryStructure, OverlapRegion region)
        {
            if (region.refRegion.Sequence[idx] == secondaryStructure && region.predRegion.Sequence[idx] == secondaryStructure)
                return 1;
            else
                return 0;
        }

        internal int I(int idx, char secondaryStructure)
        {
            if (refSequence[idx] == secondaryStructure && predSequence[idx] == secondaryStructure)
                return 1;
            else
                return 0;
        }

        internal int T(int idx, char secondaryStructure, OverlapRegion region)
        {
            if (region.refRegion.Sequence[idx] == secondaryStructure || region.predRegion.Sequence[idx] == secondaryStructure)
                return 1;
            else
                return 0;
        }

        internal int T(int idx, char secondaryStructure)
        {
            if (refSequence[idx] == secondaryStructure || predSequence[idx] == secondaryStructure)
                return 1;
            else
                return 0;
        }

        internal int Sum(int from, int to, Func<int, char, int> function, char secondaryStructure)
        {
            int summation = 0;
            for (int i = from; i <= to; i++)
            {
                summation += function(i, secondaryStructure);
            }
            return summation;
        }

        internal int Sum(OverlapRegion region, Func<int, char, OverlapRegion, int> function, char secondaryStructure)
        {
            int summation = 0;
            for (int i = 0; i < region.Length; i++)
            {
                summation += function(i, secondaryStructure, region);
            }
            return summation;
        }

        private static int SequenceLengthForSS(char secondaryStructure, ref string sequence)
        {
            int length = 0;
            for (int i = 0; i < sequence.Length; i++)
            {
                if (sequence[i] == secondaryStructure)
                    length++;
            }
            return length;
        }

        private static string ReadSingleEntryFastaSequence(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Fasta file does not exist: {path}");
            using (StreamReader reader = new StreamReader(path))
            {
                string? line;
                string sequence = "";
                int headers = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (headers > 1)
                        throw new Exception($"Not a single entry fasta file: {path}");
                    if (line.StartsWith(">"))
                    {
                        headers++;
                    }
                    else
                    {
                        sequence += line;
                    }
                }
                if (string.IsNullOrEmpty(sequence))
                    throw new Exception($"Fasta sequence is empty for file: {path}");
                return sequence;
            }
        }
    }
}
