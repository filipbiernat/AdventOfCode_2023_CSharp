using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2023.Day5
{
    public class Day5A : IDay
    {
        public void Run()
        {
            // The almanac (your puzzle input) lists all of the seeds that need to be planted.
            // It also lists what type of soil to use with each kind of seed, what type of fertilizer to use with each kind of soil,
            // what type of water to use with each kind of fertilizer, and so on.
            // Every type of seed, soil, fertilizer and so on is identified with a number, but numbers are reused by each category.
            List<string> input = File.ReadAllText(@"..\..\..\Day5\Day5.txt").Split("\r\n\r\n").ToList();

            // The almanac starts by listing which seeds need to be planted.
            List<long> seeds = Regex
                .Matches(input.First(), "\\d+")
                .Select(match => match.Value)
                .Select(long.Parse)
                .ToList();

            // The rest of the almanac contains a list of maps which describe how to convert numbers from a source category into numbers in a destination category.
            List<Map> maps = input
                .Skip(1)
                .Select(mapInput => new Map(mapInput))
                .ToList();

            // The gardener and his team want to get started as soon as possible, so they'd like to know the closest location that needs a seed.
            // Using these maps, find the lowest location number that corresponds to any of the initial seeds. 
            // What is the lowest location number that corresponds to any of the initial seed numbers?
            long output = seeds
                .Select(seed => ApplySeriesOfMaps(seed, maps))
                .Min();

            Console.WriteLine("Solution: {0}.", output);
        }

        // You'll need to convert each seed number through other categories until you can find its corresponding location number. 
        private static long ApplySeriesOfMaps(long seed, List<Map> maps) => maps
            .Aggregate(seed, (nextTypeInTheSeries, map) => map
                .Apply(nextTypeInTheSeries));
    }
}
