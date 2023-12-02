using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2023.Day2
{
    public class Day2B : IDay
    {
        public void Run()
        {
            // As you walk, the Elf shows you a small bag and some cubes which are either red, green, or blue.
            // Each time you play this game, he will hide a secret number of cubes of each color in the bag,
            // and your goal is to figure out information about the number of cubes.
            // To get information, once a bag has been loaded with cubes, the Elf will reach into the bag,
            // grab a handful of random cubes, show them to you, and then put them back in the bag.
            // He'll do this a few times per game.
            // You play several games and record the information from each game (your puzzle input).
            List<string> input = File.ReadAllLines(@"..\..\..\Day2\Day2.txt").ToList();

            // For each game, find the minimum set of cubes that must have been present.
            // What is the sum of the power of these sets?
            int output = input
                .Select(CalculatePower)
                .Sum();

            Console.WriteLine("Solution: {0}.", output);
        }

        // The power of a set of cubes is equal to the numbers of red, green, and blue cubes multiplied together.
        private static int CalculatePower(string gameInput) =>
            FindTheHighestNumberOfDrawnBalls("red", gameInput) *
            FindTheHighestNumberOfDrawnBalls("green", gameInput) *
            FindTheHighestNumberOfDrawnBalls("blue", gameInput);

        private static int FindTheHighestNumberOfDrawnBalls(string color, string gameInput) => Regex
            .Matches(gameInput, "(\\d+) " + color)
            .Select(ParseMatch)
            .Max();

        private static int ParseMatch(Match match) => int.Parse(match.Groups[1].Value);
    }
}
