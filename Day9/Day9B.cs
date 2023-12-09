using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2023.Day9
{
    public class Day9B : IDay
    {
        public void Run()
        {
            // You pull out your handy Oasis And Sand Instability Sensor and analyze your surroundings.
            // The OASIS produces a report of many values and how they are changing over time (your puzzle input).
            List<string> input = File.ReadAllLines(@"..\..\..\Day9\Day9.txt").ToList();

            // Each line in the report contains the history of a single value.
            List<List<int>> reports = input.Select(ParseReport).ToList();

            // Of course, it would be nice to have even more history included in your report.
            // Surely it's safe to just extrapolate backwards as well, right?
            // Analyze your OASIS report again, this time extrapolating the previous value for each history.
            // What is the sum of these extrapolated values?
            int output = reports.Select(PredictThePreviousValue).Sum();

            Console.WriteLine("Solution: {0}.", output);
        }

        private static List<int> ParseReport(string input) => input
            .Split()
            .Select(int.Parse)
            .ToList();
        
        private int PredictThePreviousValue(List<int> inputSequence)
        {
            // Start by making a new sequence from the difference at each step of your history.
            List<int> newSequence = inputSequence
                .Skip(1)
                .Zip(inputSequence, (lhs, rhs) => lhs - rhs)
                .ToList();

            // For each history, repeat the process of finding differences until the sequence of differences is entirely zero
            // Then, rather than adding a zero to the end and filling in the next values of each previous sequence,
            // you should instead add a zero to the beginning of your sequence of zeroes.
            int previousValue = 0;

            // Then fill in new first values for each previous sequence.
            if (newSequence.Any(value => value != 0))
            {
                previousValue = PredictThePreviousValue(newSequence);
            }

            // Adding the new values on the left side of each sequence from bottom to top eventually reveals the new left-most history value.
            return inputSequence.First() - previousValue;
        }
    }
}
