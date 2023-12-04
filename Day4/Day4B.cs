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
    public class Day4B : IDay
    {
        public void Run()
        {
            // The Elf leads you over to the pile of colorful cards.
            // There, you discover dozens of scratchcards, all with their opaque covering already scratched off.
            // You organize the information into a table (your puzzle input).
            List<string> input = File.ReadAllLines(@"..\..\..\Day4\Day4.txt").ToList();

            List<int> countOfMyWinningNumbers = input.Select(CountMyWinningNumbers).ToList();

            // Scratchcards only cause you to win more scratchcards equal to the number of winning numbers you have.
            List<int> countOfAllMyCards = countOfMyWinningNumbers.Select(count => 1).ToList();

            // You win copies of the scratchcards below the winning card equal to the number of matches.
            // Copies of scratchcards are scored like normal scratchcards and have the same card number as the card they copied.
            // Process all of the original and copied scratchcards until no more scratchcards are won.
            Enumerable.Range(1, countOfMyWinningNumbers.Count)
                .ToList()
                .ForEach(processedCardId => Enumerable.Range(processedCardId + 1, countOfMyWinningNumbers[processedCardId - 1])
                    .ToList()
                    .ForEach(wonCardId => countOfAllMyCards[wonCardId - 1] += countOfAllMyCards[processedCardId - 1]));

            // Including the original set of scratchcards, how many total scratchcards do you end up with?
            int output = countOfAllMyCards.Sum();

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
