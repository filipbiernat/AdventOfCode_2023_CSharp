using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2023.Day4
{
    public class Day4A : IDay
    {
        public void Run()
        {
            // The Elf leads you over to the pile of colorful cards.
            // There, you discover dozens of scratchcards, all with their opaque covering already scratched off.
            // You organize the information into a table (your puzzle input).
            List<string> input = File.ReadAllLines(@"..\..\..\Day4\Day4.txt").ToList();

            List<int> countOfMyWinningNumbers = input.Select(CountMyWinningNumbers).ToList();

            // The first match makes the card worth one point and each match after the first doubles the point value of that card.
            // Take a seat in the large pile of colorful cards. How many points are they worth in total?
            int output = countOfMyWinningNumbers
                .Select(numbersCount => numbersCount > 0 ? (int)Math.Pow(2, numbersCount - 1) : 0)
                .Sum();

            Console.WriteLine("Solution: {0}.", output);
        }

        private int CountMyWinningNumbers(string cardInput)
        {
            // Picking one up, it looks like each card has two lists of numbers separated by a vertical bar (|):
            // A list of winning numbers and then a list of numbers you have.
            string[] splitCardInput = Regex.Split(cardInput, @"\s*:\s*|\s*\|\s*");
            HashSet<int> winningNumbers = ParseSeriesOfNumbers(splitCardInput[1]);
            HashSet<int> myNumbers = ParseSeriesOfNumbers(splitCardInput[2]);

            // You have to figure out which of the numbers you have appear in the list of winning numbers.
            return myNumbers.Intersect(winningNumbers).Count();
        }

        private static HashSet<int> ParseSeriesOfNumbers(string seriesOfNumbers) => Regex
            .Split(seriesOfNumbers, @"\s+")
            .Select(int.Parse)
            .ToHashSet();
    }
}
