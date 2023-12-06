namespace AdventOfCode2023.Day5
{
    public class Map
    {
        // Rather than list every source number and its corresponding destination number one by one, the maps describe entire ranges of numbers that can be converted.
        private readonly List<Range> Ranges;

        public Map(string input)
        {
            Ranges = input
                .Split("\r\n")
                .Skip(1)
                .Select(rangeInput => new Range(rangeInput))
                .OrderBy(range => range.GetRangeStart())
                .ToList();
        }

        public long Apply(long number)
        {
            Range? matchingRange = Ranges.FirstOrDefault(range => range.IsIn(number));
            // Any source numbers that aren't mapped correspond to the same destination number.
            return matchingRange is null ? number : matchingRange.Apply(number);
        }
    }
}
