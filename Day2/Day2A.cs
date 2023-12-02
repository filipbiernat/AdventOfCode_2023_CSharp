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
    public class Day2A : IDay
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

            // Determine which games would have been possible if the bag had been loaded with only 12 red cubes, 13 green cubes, and 14 blue cubes.
            // What is the sum of the IDs of those games?
            int output = input
                .Where(IsGamePossible)
                .Select(GetGameId)
                .Sum();

            Console.WriteLine("Solution: {0}.", output);
        }

        // Each game is listed with its ID number.
        private static int GetGameId(string gameInput) => ParseMatch(Regex.Match(gameInput, "Game (\\d+)"));

        // The Elf would first like to know which games would have been possible if the bag contained only 12 red cubes, 13 green cubes, and 14 blue cubes?
        private static bool IsGamePossible(string gameInput) =>
            FindTheHighestNumberOfDrawnBalls("red", gameInput) <= 12 &&
            FindTheHighestNumberOfDrawnBalls("green", gameInput) <= 13 &&
            FindTheHighestNumberOfDrawnBalls("blue", gameInput) <= 14;

        private static int FindTheHighestNumberOfDrawnBalls(string color, string gameInput) => Regex
            .Matches(gameInput, "(\\d+) " + color)
            .Select(ParseMatch)
            .Max();

        private static int ParseMatch(Match match) => int.Parse(match.Groups[1].Value);
    }
}
