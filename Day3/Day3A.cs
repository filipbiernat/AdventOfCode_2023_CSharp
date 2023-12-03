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
    public class Day3A : IDay
    {
        public void Run()
        {
            // The engine schematic (your puzzle input) consists of a visual representation of the engine.
            List<string> input = File.ReadAllLines(@"..\..\..\Day3\Day3.txt").ToList();

            // There are lots of numbers and symbols you don't really understand.
            // Apparently any number adjacent to a symbol, even diagonally, is a "part number" and should be included in your sum.
            Dictionary<Coords, string> numbers = ParseMap(input, "\\d+");
            IEnumerable<Coords> symbols = ParseMap(input, "[^\\w\\s.]").Keys;

            IEnumerable<int> partNumbers = symbols
                .SelectMany(symbol => numbers
                    .Where(number => symbol.IsAdjacent(number.Key, number.Value.Length))
                    .Select(ParseNumber));

            //  What is the sum of all of the part numbers in the engine schematic?
            int output = partNumbers.Sum();

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
