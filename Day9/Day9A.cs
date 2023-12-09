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
    public class Day9A : IDay
    {
        public void Run()
        {
            // You pull out your handy Oasis And Sand Instability Sensor and analyze your surroundings.
            // The OASIS produces a report of many values and how they are changing over time (your puzzle input).
            List<string> input = File.ReadAllLines(@"..\..\..\Day9\Day9.txt").ToList();

            // Each line in the report contains the history of a single value.
            List<List<int>> reports = input.Select(ParseReport).ToList();

            // To best protect the oasis, your environmental report should include a prediction of the next value in each history.
            // Analyze your OASIS report and extrapolate the next value for each history.
            // What is the sum of these extrapolated values?
            int output = reports.Select(PredictTheNextValue).Sum();

            Console.WriteLine("Solution: {0}.", output);
        }

        private static List<int> ParseReport(string input) => input
            .Split()
            .Select(int.Parse)
            .ToList();
        
        private int PredictTheNextValue(List<int> inputSequence)
        {
            // Start by making a new sequence from the difference at each step of your history.
            List<int> newSequence = inputSequence
                .Skip(1)
                .Zip(inputSequence, (lhs, rhs) => lhs - rhs)
                .ToList();

            // Once all of the values in your latest sequence are zeroes, you can extrapolate what the next value of the original history should be.
            // To extrapolate, start by adding a new zero to the end of your list of zeroes.
            int nextValue = 0;

            // If that sequence is not all zeroes, repeat this process, using the sequence you just generated as the input sequence.
            if (newSequence.Any(value => value != 0))
            {
                nextValue = PredictTheNextValue(newSequence);
            }

            // There is now a placeholder in every sequence above it. You can then start filling in placeholders from the bottom up.
            return inputSequence.Last() + nextValue;
        }
    }
}
