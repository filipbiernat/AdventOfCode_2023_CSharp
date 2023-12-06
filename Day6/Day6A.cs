using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2023.Day6
{
    public class Day6A : IDay
    {
        public void Run()
        {
            // As part of signing up, you get a sheet of paper (your puzzle input) that lists the time allowed for each race and also the best distance ever recorded in that race. 
            List<string> input = File.ReadAllLines(@"..\..\..\Day6\Day6.txt").ToList();
            List<long> raceTimes = ParseInputLine(input.First()).ToList();
            List<long> raceDistances = ParseInputLine(input.Last()).ToList();

            // Determine the number of ways you could beat the record in each race.
            // What do you get if you multiply these numbers together?
            long output = Enumerable
                .Range(0, raceTimes.Count)
                .Select(raceId => DetermineNumberOfWaysToWin(raceTimes[raceId], raceDistances[raceId]))
                .Aggregate((lhs, rhs) => lhs * rhs);

            Console.WriteLine("Solution: {0}.", output);
        }

        // To see how much margin of error you have, determine the number of ways you can beat the record in each race.
        private static long DetermineNumberOfWaysToWin(long raceTime, long raceDistance) => Enumerable
            .Range(1, (int) raceTime - 1)
            .Select(holdingTime => CalculateTotalDistance(raceTime, holdingTime))
            .Count(totalDistance => totalDistance > raceDistance);

        // Holding down the button charges the boat, and releasing the button allows the boat to move.
        // Boats move faster if their button was held longer, but time spent holding the button counts against the total race time.
        // You can only hold the button at the start of the race, and boats don't move until the button is released.
        private static long CalculateTotalDistance(long raceTime, long holdingTime) => (raceTime - holdingTime) * holdingTime;

        private static List<long> ParseInputLine(string line) => line
            .Split(new string[] { ":", " " }, StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(long.Parse)
            .ToList();
    }
}
