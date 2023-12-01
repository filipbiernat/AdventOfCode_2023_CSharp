using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Day1
{
    public class Day1A : IDay
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
            string regex = "(\\d)";
            int firstDigit = int.Parse(Regex.Match(line, regex).Value);
            int lastDigit = int.Parse(Regex.Match(line, regex, RegexOptions.RightToLeft).Value);
            return 10 * firstDigit + lastDigit;
        }
    }
}
