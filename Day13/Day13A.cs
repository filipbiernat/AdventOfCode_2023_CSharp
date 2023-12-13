using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;
using AdventOfCode2023.Day5;

namespace AdventOfCode2023.Day13
{
    public class Day13A : IDay
    {
        public void Run()
        {
            // You note down the patterns of ash (.) and rocks (#) that you see as you walk (your puzzle input).
            List<string> input = File.ReadAllText(@"..\..\..\Day13\Day13.txt").Split("\r\n\r\n").ToList();

            // Find the line of reflection in each of the patterns in your notes.
            // What number do you get after summarizing all of your notes?
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

            return FindLineOfReflectionAndCalculateScore(rocks);
        }

        private static int FindLineOfReflectionAndCalculateScore(HashSet<Coords> rocks, int lineToIgnore = int.MaxValue)
        {
            // To find the reflection in each pattern, you need to find a perfect reflection across either
            // a horizontal line between two rows or across a vertical line between two columns.
            // To summarize your pattern notes, add up the number of columns to the left of each vertical line of reflection;
            // to that, also add 100 multiplied by the number of rows above each horizontal line of reflection. 
            return FindVerticalLineOfReflection(rocks) + 100 * FindHorizontalLineOfReflection(rocks);
        }

        private static int FindVerticalLineOfReflection(HashSet<Coords> rocks)
        {
            int maxColumn = rocks.Max(coords => coords.Column);

            Dictionary<int, HashSet<int>> groupedRocks = rocks
                .GroupBy(rock => rock.Column)
                .Select(group => new KeyValuePair<int, HashSet<int>>(
                    group.Key,
                    group.Select(rock => rock.Row).ToHashSet()))
                .ToDictionary(groupedRocks => groupedRocks.Key, groupedRocks => groupedRocks.Value);

            return FindLineOfReflection(groupedRocks, maxColumn);
        }

        private static int FindHorizontalLineOfReflection(HashSet<Coords> rocks)
        {
            int maxRow = rocks.Max(coords => coords.Row);

            Dictionary<int, HashSet<int>> groupedRocks = rocks
                .GroupBy(rock => rock.Row)
                .Select(group => new KeyValuePair<int, HashSet<int>>(
                    group.Key,
                    group.Select(rock => rock.Column).ToHashSet()))
                .ToDictionary(groupedRocks => groupedRocks.Key, groupedRocks => groupedRocks.Value);

            return FindLineOfReflection(groupedRocks, maxRow);
        }

        private static int FindLineOfReflection(Dictionary<int, HashSet<int>> groupedRocks, int max)
        {
            for (int lineCandidate = 0; lineCandidate <= max; ++lineCandidate)
            {
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
