using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2023.Day3
{
    public class Day3B : IDay
    {
        public void Run()
        {
            // The engine schematic (your puzzle input) consists of a visual representation of the engine.
            List<string> input = File.ReadAllLines(@"..\..\..\Day3\Day3.txt").ToList();

            // The missing part wasn't the only issue - one of the gears in the engine is wrong.
            // A gear is any * symbol that is adjacent to exactly two part numbers.
            // Its gear ratio is the result of multiplying those two numbers together.
            Dictionary<Coords, string> numbers = ParseMap(input, "\\d+");
            IEnumerable<Coords> asterisks = ParseMap(input, "\\*").Keys;

            IEnumerable<int> gearRatios = asterisks
                .Select(symbol => numbers
                    .Where(number => symbol.IsAdjacent(number.Key, number.Value.Length)))
                .Where(adjacentNumbers => adjacentNumbers.Count() == 2)
                .Select(twoAdjacentNumbers => ParseNumber(twoAdjacentNumbers.First()) * ParseNumber(twoAdjacentNumbers.Last()));

            //  What is the sum of all of the gear ratios in your engine schematic?
            int output = gearRatios.Sum();

            Console.WriteLine("Solution: {0}.", output);
        }

        private static Dictionary<Coords, string> ParseMap(List<string> map, string regex) => map
            .SelectMany((row, rowIndex) => Regex
                .Matches(row, regex)
                .Select(match => new KeyValuePair<Coords, string>(
                    new Coords(rowIndex, match.Index),
                    match.Value)))
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        private static int ParseNumber(KeyValuePair<Coords, string> number) => int.Parse(number.Value);
    }
}
