namespace AdventOfCode2023.Day5
{
    public class Range
    {
        // Each line within a map contains three numbers: the destination range start, the source range start, and the range length.
        private readonly long DestinationStart;
        private readonly long RangeStart;
        private readonly long RangeLength;

        public Range(string input)
        {
            string[] splitInput = input.Split();
            DestinationStart = long.Parse(splitInput[0]);
            RangeStart = long.Parse(splitInput[1]);
            RangeLength = long.Parse(splitInput[2]);
        }

        public bool IsIn(long number) => number >= RangeStart && number <= GetRangeEnd();
        public long Apply(long number) => number + GetDelta();
        public long GetRangeStart() => RangeStart;

        private long GetRangeEnd() => RangeStart + RangeLength - 1;
        private long GetDelta() => DestinationStart - RangeStart;
    }
}
