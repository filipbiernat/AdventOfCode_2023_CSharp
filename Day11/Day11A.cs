using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Collections;

namespace AdventOfCode2023.Day11
{
    public class Day11A : IDay
    {
        public void Run()
        {
            // The researcher has collected a bunch of data and compiled the data into a single giant image (your puzzle input).
            List<string> input = File.ReadAllLines(@"..\..\..\Day11\Day11.txt").ToList();

            // The image includes empty space (.) and galaxies (#).
            HashSet<Coords> galaxies = input
                .SelectMany((row, rowIndex) => row
                    .ToCharArray()
                    .Select((value, colIndex) => new Tuple<Coords, char>(new Coords(rowIndex, colIndex), value))
                    .Where(coordsAndValue => coordsAndValue.Item2 == '#')
                    .Select(coordsAndValue => coordsAndValue.Item1))
                .ToHashSet();

            // Expand the universe.
            galaxies = ExpandTheUniverse(galaxies);

            // Find the length of the shortest path between every pair of galaxies. What is the sum of these lengths?
            long output = galaxies
                .SelectMany((lhs, index) => galaxies.Skip(index + 1), (lhs, rhs) => new Tuple<Coords, Coords>(lhs, rhs))
                .Select(pairOfGalaxies => Coords.ManhattanDistance(pairOfGalaxies.Item1, pairOfGalaxies.Item2))
                .Sum();

            Console.WriteLine("Solution: {0}.", output);
        }

        // There's a catch: the universe expanded in the time it took the light from those galaxies to reach the observatory.
        // Due to something involving gravitational effects, only some space expands.
        // In fact, the result is that any rows or columns that contain no galaxies should all actually be twice as big.
        private static HashSet<Coords> ExpandTheUniverse(HashSet<Coords> galaxies)
        {
            Coords deltaRow = new(1, 0);
            Coords deltaColumn = new(0, 1);

            HashSet<long> rowsWithGalaxies = galaxies.Select(coords => coords.Row).ToHashSet();
            HashSet<long> columnsWithGalaxies = galaxies.Select(coords => coords.Column).ToHashSet();
            HashSet<long> rowsToExpand = CalculateRowsOrColumnsToExpand(rowsWithGalaxies);
            HashSet<long> columnsToExpand = CalculateRowsOrColumnsToExpand(columnsWithGalaxies);

            // Rows and columns need to be twice as big.
            return galaxies
                .Select(coords => coords +
                    deltaRow * rowsToExpand.Count(row => row < coords.Row) +
                    deltaColumn * columnsToExpand.Count(column => column < coords.Column))
                .ToHashSet();
        }

        private static HashSet<long> CalculateRowsOrColumnsToExpand(HashSet<long> rowsOrColumnsWithGalaxies) => Enumerable
            .Range(0, (int)rowsOrColumnsWithGalaxies.Max() + 1)
            .Select(rowOrColumn => (long)rowOrColumn)
            .Except(rowsOrColumnsWithGalaxies)
            .ToHashSet();
    }
}
