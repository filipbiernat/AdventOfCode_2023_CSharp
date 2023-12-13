using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2023.Day12
{
    public class Day12A : IDay
    {
        public void Run()
        {
            // Many of the springs have fallen into disrepair, so they're not actually sure which springs would even be safe to use!
            // Worse yet, their condition records of which springs are damaged (your puzzle input) are also damaged! 
            List<string> input = File.ReadAllLines(@"..\..\..\Day12\Day12.txt").ToList();

            // In the giant field just outside, the springs are arranged into rows.
            List<(string Layout, List<int> Condition)> rowsOfSprings = input
                .Select(ParseRowOfSprings)
                .ToList();

            // For each row, count all of the different arrangements of operational and broken springs that meet the given criteria.
            // What is the sum of those counts?
            long output = rowsOfSprings
                .Select(rowOfSprings => CountCombinations(rowOfSprings.Layout + ".", rowOfSprings.Condition, numOfConsecutiveDamagedSprings: 0))
                .Sum();

            Console.WriteLine("Solution: {0}.", output);
        }

        private static (string Layout, List<int> Condition) ParseRowOfSprings(string input)
        {
            string[] splitInput = input.Split();

            // For each row, the condition records show every spring and whether it is operational (.) or damaged (#).
            // This is the part of the condition records that is itself damaged; for some springs, it is simply unknown (?) whether the spring is operational or damaged.
            string layout = splitInput[0];

            // The engineer that produced the condition records also duplicated some of this information in a different format!
            // After the list of springs for a given row, the size of each contiguous group of damaged springs is listed in the order those groups appear in the row.
            // This list always accounts for every damaged spring, and each number is the entire size of its contiguous group.
            List<int> condition = splitInput[1]
                .Split(",")
                .Select(int.Parse)
                .ToList();

            return new(layout, condition);
        }

        private static long CountCombinations(string layout, List<int> condition, long numOfConsecutiveDamagedSprings)
        {
            if (!layout.Any()) // Stop if there are no more springs to process.
            {
                return condition.Any() ? 0 : 1;
            }

            Tuple<string, string, long> cacheKey = new(layout, string.Join(",", condition), numOfConsecutiveDamagedSprings);
            if (Cache.ContainsKey(cacheKey)) // Check if the same entry has not been already processed.
            {
                return Cache[cacheKey];
            }

            List<char> springTypesToCheck = new() { '.', '#' }; // Find next possible spring type(s).
            if (layout.First() != '?')
            {
                springTypesToCheck = layout.Take(1).ToList();
            }

            long totalNumOfCombinations = springTypesToCheck // Sum combinations for the next possible spring type(s).
                .Select(springTypeToCheck =>
                {
                    string nextLayout = layout[1..];
                    long numOfCombinations = 0;

                    if (springTypeToCheck == '#') // Encountered a damaged spring.
                    {
                        ++numOfConsecutiveDamagedSprings;
                        numOfCombinations = CountCombinations(nextLayout, condition, numOfConsecutiveDamagedSprings);
                    }
                    else if (numOfConsecutiveDamagedSprings == 0) // Encountered an operational or unknown spring.
                    {
                        numOfCombinations = CountCombinations(nextLayout, condition, numOfConsecutiveDamagedSprings);
                    }
                    else if (condition.Any() && numOfConsecutiveDamagedSprings == condition.First()) // Encountered spring matching the current condition.
                    {
                        List<int> nextCondition = condition.Skip(1).ToList();
                        int nextNumOfConsecutiveDamagedSprings = 0;
                        numOfCombinations = CountCombinations(nextLayout, nextCondition, nextNumOfConsecutiveDamagedSprings);
                    }
                    return numOfCombinations;
                })
                .Sum();

            Cache[cacheKey] = totalNumOfCombinations; // Store the result in cache.
            return totalNumOfCombinations;
        }

        private static readonly Dictionary<Tuple<string, string, long>, long> Cache = new();
    }
}
