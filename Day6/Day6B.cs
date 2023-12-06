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
    public class Day6B : IDay
    {
        public void Run()
        {
            // As part of signing up, you get a sheet of paper (your puzzle input) that lists the time allowed for each race and also the best distance ever recorded in that race. 
            List<string> input = File.ReadAllLines(@"..\..\..\Day6\Day6.txt").ToList();
            long raceTime = ParseInputLine(input.First());
            long raceDistance = ParseInputLine(input.Last());

            // Now, you have to figure out how many ways there are to win this single race.
            // How many ways can you beat the record in this one much longer race?
            long output = DetermineNumberOfWaysToWin(raceTime, raceDistance);

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

        // As the race is about to start, you realize the piece of paper with race times and record distances you got earlier actually just has very bad kerning.
        // There's really only one race - ignore the spaces between the numbers on each line.
        private static long ParseInputLine(string line) => long
            .Parse(new string(line
                .Where(char.IsDigit)
                .ToArray()));
    }
}
