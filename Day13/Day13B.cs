using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;
using AdventOfCode2023.Day5;
using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;

namespace AdventOfCode2023.Day13
{
    public class Day13B : IDay
    {
        public void Run()
        {
            // You note down the patterns of ash (.) and rocks (#) that you see as you walk (your puzzle input).
            List<string> input = File.ReadAllText(@"..\..\..\Day13\Day13.txt").Split("\r\n\r\n").ToList();

            // In each pattern, fix the smudge and find the different line of reflection.
            // What number do you get after summarizing the new reflection line in each pattern in your notes?
            int output = input.Select(FindTheReflectionInPattern).Sum();

            Console.WriteLine("Solution: {0}.", output);

        }
        
        // To find the reflection in each pattern, you need to find a perfect reflection across either
        // a horizontal line between two rows or across a vertical line between two columns.
        private static int FindTheReflectionInPattern(string input)
        {
            // You note down the patterns of ash (.) and rocks (#) that you see as you walk;
            // perhaps by carefully analyzing these patterns, you can figure out where the mirrors are!
            HashSet<(Coords Coords, char Value)> rocksAndAshes = input
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                .SelectMany((row, rowIndex) => row
                    .ToCharArray()
                    .Select((value, colIndex) => (Coords: new Coords(rowIndex, colIndex), Value: value)))
                .ToHashSet();

            HashSet<Coords> rocks = rocksAndAshes
                .Where(coordsAndValue => coordsAndValue.Value == '#')
                .Select(coordsAndValue => coordsAndValue.Coords)
                .ToHashSet();

            // Upon closer inspection, you discover that every mirror has exactly one smudge: exactly one . or # should be the opposite type.
            // In each pattern, you'll need to locate and fix the smudge that causes a different reflection line to be valid.
            // (The old reflection line won't necessarily continue being valid after the smudge is fixed.)
            int oldReflectionLine = FindLineOfReflectionAndCalculateScore(rocks);
            int newReflectionLine = 0;

            foreach (Coords smudge in rocks)
            {
                HashSet<Coords> rocksWithoutSmudge = rocks.Where(rock => rock != smudge).ToHashSet();
                newReflectionLine = FindLineOfReflectionAndCalculateScore(rocksWithoutSmudge, lineToIgnore: oldReflectionLine);
                if (newReflectionLine > 0)
                {
                    return newReflectionLine;
                }
            }

            HashSet<Coords> ashes = rocksAndAshes
                .Where(coordsAndValue => coordsAndValue.Value == '.')
                .Select(coordsAndValue => coordsAndValue.Coords)
                .ToHashSet();

            foreach (Coords smudge in ashes)
            {
                HashSet<Coords> rocksWithSmudge = new(rocks) { smudge };
                newReflectionLine = FindLineOfReflectionAndCalculateScore(rocksWithSmudge, lineToIgnore: oldReflectionLine);
                if (newReflectionLine > 0)
                {
                    return newReflectionLine;
                }
            }

            return 0;
        }

        private static int FindLineOfReflectionAndCalculateScore(HashSet<Coords> rocks, int lineToIgnore = int.MaxValue)
        {
            // To find the reflection in each pattern, you need to find a perfect reflection across either
            // a horizontal line between two rows or across a vertical line between two columns.
            // To summarize your pattern notes, add up the number of columns to the left of each vertical line of reflection;
            // to that, also add 100 multiplied by the number of rows above each horizontal line of reflection. 
            return FindVerticalLineOfReflection(rocks, lineToIgnore) + 100 * FindHorizontalLineOfReflection(rocks, lineToIgnore / 100);
        }

        private static int FindVerticalLineOfReflection(HashSet<Coords> rocks, int lineToIgnore)
        {
            int maxColumn = rocks.Max(coords => coords.Column);

            Dictionary<int, HashSet<int>> groupedRocks = rocks
                .GroupBy(rock => rock.Column)
                .Select(group => new KeyValuePair<int, HashSet<int>>(
                    group.Key,
                    group.Select(rock => rock.Row).ToHashSet()))
                .ToDictionary(groupedRocks => groupedRocks.Key, groupedRocks => groupedRocks.Value);

            return FindLineOfReflection(groupedRocks, maxColumn, lineToIgnore);
        }

        private static int FindHorizontalLineOfReflection(HashSet<Coords> rocks, int lineToIgnore)
        {
            int maxRow = rocks.Max(coords => coords.Row);

            Dictionary<int, HashSet<int>> groupedRocks = rocks
                .GroupBy(rock => rock.Row)
                .Select(group => new KeyValuePair<int, HashSet<int>>(
                    group.Key,
                    group.Select(rock => rock.Column).ToHashSet()))
                .ToDictionary(groupedRocks => groupedRocks.Key, groupedRocks => groupedRocks.Value);

            return FindLineOfReflection(groupedRocks, maxRow, lineToIgnore);
        }

        private static int FindLineOfReflection(Dictionary<int, HashSet<int>> groupedRocks, int max, int lineToIgnore)
        {
            for (int lineCandidate = 0; lineCandidate <= max; ++lineCandidate)
            {
                if (lineCandidate + 1 == lineToIgnore)
                {
                    continue;
                }

                for (int delta = 0; ; ++delta)
                {
                    int left = lineCandidate - delta;
                    int right = lineCandidate + delta + 1;

                    if (left < 0 || right > max)
                    {
                        return delta > 0 ? (lineCandidate + 1) : 0;
                    }

                    if (!groupedRocks.ContainsKey(left) || !groupedRocks.ContainsKey(right))
                    {
                        return 0;
                    }
                    else if (!groupedRocks[left].SetEquals(groupedRocks[right]))
                    {
                        break;
                    }
                }
            }
            return 0;
        }
    }
}
