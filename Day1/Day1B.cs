using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Day1
{
    public class Day1B : IDay
    {
        public void Run()
        {
            // The newly-improved calibration document consists of lines of text;
            // each line originally contained a specific calibration value that the Elves now need to recover.
            List<string> input = File.ReadAllLines(@"..\..\..\Day1\Day1.txt").ToList();

            // Consider your entire calibration document. What is the sum of all of the calibration values?
            int output = input.Select(FindCalibrationValue).Sum();

            Console.WriteLine("Solution: {0}.", output);
        }

        // On each line, the calibration value can be found by combining the first digit and the last digit
        // (in that order) to form a single two-digit number.
        private static int FindCalibrationValue(string line)
        {
            string regex = "(" + string.Join("|", Digits.Keys) + "|\\d)";
            int firstDigit = ParseDigit(Regex.Match(line, regex).Value);
            int lastDigit = ParseDigit(Regex.Match(line, regex, RegexOptions.RightToLeft).Value);
            return 10 * firstDigit + lastDigit;
        }

        // It looks like some of the digits are actually spelled out with letters:
        // one, two, three, four, five, six, seven, eight, and nine also count as valid "digits".
        private static int ParseDigit(string expression) => expression.All(char.IsDigit) ? int.Parse(expression) : Digits[expression];

        private static readonly Dictionary<string, int> Digits = new()
        {
            { "zero", 0 },
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 }
        };
    }
}
