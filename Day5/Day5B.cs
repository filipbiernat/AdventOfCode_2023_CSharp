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

    public class Day5B : IDay
    {
        public void Run()
        {
            // The almanac (your puzzle input) lists all of the seeds that need to be planted.
            // It also lists what type of soil to use with each kind of seed, what type of fertilizer to use with each kind of soil,
            // what type of water to use with each kind of fertilizer, and so on.
            // Every type of seed, soil, fertilizer and so on is identified with a number, but numbers are reused by each category.
            List<string> input = File.ReadAllText(@"..\..\..\Day5\Day5.txt").Split("\r\n\r\n").ToList();

            // Everyone will starve if you only plant such a small number of seeds.
            // Re-reading the almanac, it looks like the seeds: line actually describes ranges of seed numbers.
            // The values on the initial seeds: line come in pairs.
            List<(long, long)> rangesOfSeeds = Regex
                .Matches(input.First(), "(\\d+) (\\d+)")
                .Select(match => (long.Parse(match.Groups[1].Value), long.Parse(match.Groups[2].Value)))
                .ToList();

            // The rest of the almanac contains a list of maps which describe how to convert numbers from a source category into numbers in a destination category.
            List<Map> maps = input
                .Skip(1)
                .Select(mapInput => new Map(mapInput))
                .ToList();

            int approximationStep = 50000; // For starters, use step 25000 to narrow down the neighboring seeds to the closest seed.

            // Within each pair, the first value is the start of the range and the second value is the length of the range.
            IEnumerable<List<long>> generatedListsOfSeeds = rangesOfSeeds
                .Select(seedRange => GenerateListOfSeeds(beginning: seedRange.Item1, count: seedRange.Item2, approximationStep));

            // Consider all of the initial seed numbers listed in the ranges on the first line of the almanac. 
            long approximatelyClosestSeed = generatedListsOfSeeds
                .Select(listOfSeeds => FindClosestSeedAndItsLocation(listOfSeeds, maps))
                .OrderBy(seedAndLocation => seedAndLocation.Location)
                .First()
                .Seed;
            List<long> listOfNeighborSeeds = GenerateListOfSeeds(beginning: approximatelyClosestSeed - approximationStep, count: 2 * approximationStep);

            // What is the lowest location number that corresponds to any of the initial seed numbers?
            long output = FindClosestSeedAndItsLocation(listOfNeighborSeeds, maps).Location;

            Console.WriteLine("Solution: {0}.", output);
        }

        // You'll need to convert each seed number through other categories until you can find its corresponding location number. 
        private static long ApplySeriesOfMaps(long seed, List<Map> maps) => maps
            .Aggregate(seed, (nextTypeInTheSeries, map) => map
                .Apply(nextTypeInTheSeries));

        private static List<long> GenerateListOfSeeds(long beginning, long count, long step = 1) => Enumerable
            .Range(0, (int)(count / step) + 1)
            .Select(item => beginning + item * step)
            .ToList();

        private static (long Seed, long Location) FindClosestSeedAndItsLocation(List<long> seeds, List<Map> maps) => seeds
            .Select(seed => (Seed: seed, Location: ApplySeriesOfMaps(seed, maps)))
            .OrderBy(seedAndLocation => seedAndLocation.Location)
            .First();
    }
}
