using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Collections.Immutable;
using System.Xml.Linq;
using System.Reflection.Metadata;

namespace AdventOfCode2023.Day14
{
    public class Day14A : IDay
    {
        public void Run()
        {
            // You note the positions of all of the empty spaces (.) and rocks (your puzzle input).
            List<string> input = File.ReadAllLines(@"..\..\..\Day14\Day14.txt").ToList();

            HashSet<(Coords Coords, char Value)> rocks = input
                .SelectMany((row, rowIndex) => row
                    .ToCharArray()
                    .Select((value, colIndex) => (Coords: new Coords(rowIndex, colIndex), Value: value)))
                .ToHashSet();
            BottomRightCorner = new(rocks.Max(rock => rock.Coords.Row), rocks.Max(rock => rock.Coords.Column));

            // The rounded rocks (O) will roll when the platform is tilted, while the cube-shaped rocks (#) will stay in place.
            HashSet<Coords> roundedRocks = FilterRocks(rocks, 'O');
            HashSet<Coords> cubeShapedRocks = FilterRocks(rocks, '#');

            // Tilt the platform so that the rounded rocks all roll north.
            roundedRocks = Tilt(roundedRocks, cubeShapedRocks);

            // The amount of load caused by a single rounded rock (O) is equal to the number of rows from the rock to the
            // south edge of the platform, including the row the rock is on. (Cube-shaped rocks (#) don't contribute to load.)
            // What is the total load on the north support beams?
            int output = roundedRocks
                .Select(rock => BottomRightCorner.Row - rock.Row + 1)
                .Sum();

            Console.WriteLine("Solution: {0}.", output);
        }

        private static HashSet<Coords> Tilt(HashSet<Coords> roundedRocks, HashSet<Coords> cubeShapedRocks)
        {
            Dictionary<int, HashSet<int>> groupedRoundedRocks = GroupRocksByColumn(roundedRocks);
            Dictionary<int, HashSet<int>> groupedCubeShapedRocks = GroupRocksByColumn(cubeShapedRocks);

            return Enumerable
                .Range(0, BottomRightCorner.Column + 1)
                .Select(columnIndex => TiltSingleColumn(columnIndex, groupedRoundedRocks, groupedCubeShapedRocks))
                .Aggregate((lhs, rhs) => lhs.Union(rhs).ToHashSet());
        }

        private static HashSet<Coords> TiltSingleColumn(
            int columnIndex,
            Dictionary<int, HashSet<int>> groupedRoundedRocks,
            Dictionary<int, HashSet<int>> groupedCubeShapedRocks)
        {
            if (!groupedRoundedRocks.ContainsKey(columnIndex))
            {
                return new();
            }

            HashSet<int> cubeShapedRocksInTheCurrentColumn = groupedCubeShapedRocks.ContainsKey(columnIndex) ?
                groupedCubeShapedRocks[columnIndex] :
                new();

            int upperLimit = BottomRightCorner.Column + 1;
            IEnumerable<int> lowerLimits = LowerLimitHashset.Union(cubeShapedRocksInTheCurrentColumn);
            IEnumerable<int> upperLimits = cubeShapedRocksInTheCurrentColumn.Union(new HashSet<int>() { upperLimit });
            HashSet<(int LowerLimit, int UpperLimit)> ranges = lowerLimits
                .Zip(upperLimits, (lowerLimit, upperLimit) => (lowerLimit, upperLimit))
                .ToHashSet();

            IEnumerable<int> newRoundedRocksInTheCurrentColumn = groupedRoundedRocks[columnIndex]
                .GroupBy(rowIndex => ranges
                    .FirstOrDefault(range => rowIndex > range.LowerLimit && rowIndex <= range.UpperLimit))
                .Select(group => (StartRange: group.Key.LowerLimit + 1, Count: group.Count()))
                .SelectMany(startRangeAndCount => Enumerable
                    .Range(startRangeAndCount.StartRange, startRangeAndCount.Count));

            return newRoundedRocksInTheCurrentColumn
                .ToHashSet()
                .Select(rowIndex => new Coords(rowIndex, columnIndex))
                .ToHashSet();
        }

        private static HashSet<Coords> FilterRocks(HashSet<(Coords Coords, char Value)> rocks, char value) => rocks
            .Where(coordsAndValue => coordsAndValue.Value == value)
            .Select(coordsAndValue => coordsAndValue.Coords)
            .ToHashSet();

        private static Dictionary<int, HashSet<int>> GroupRocksByColumn(HashSet<Coords> rocks) => rocks
            .GroupBy(rock => rock.Column)
            .Select(group => new KeyValuePair<int, HashSet<int>>(
                group.Key,
                group.Select(rock => rock.Row).ToHashSet()))
            .ToDictionary(groupedRocks => groupedRocks.Key, groupedRocks => groupedRocks.Value);

        private static readonly HashSet<int> LowerLimitHashset = new() { -1 };
        private static Coords BottomRightCorner = new();
    }
}
