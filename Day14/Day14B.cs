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
    public class Day14B : IDay
    {
        public enum Directions { N, S, W, E };

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

            // This process should work if you leave it running long enough, but you're still worried about the north support beams.
            // To make sure they'll survive for a while, you need to calculate the total load on the north support beams after 1000000000 cycles.
            List<string> roundedRocksAfterEachSpin = new();
            for (int spinIndex = 0; spinIndex < 1000000000; ++spinIndex)
            {
                roundedRocks = SpinCycle(roundedRocks, cubeShapedRocks);

                string roundedRocksString = string.Join("", roundedRocks);
                if (roundedRocksAfterEachSpin.Contains(roundedRocksString))
                {
                    roundedRocksAfterEachSpin.Add(roundedRocksString);
                    break;
                }
                roundedRocksAfterEachSpin.Add(roundedRocksString);
            }

            List<int> duplicatedSpinIndices = roundedRocksAfterEachSpin
                .Select((rocks, index) => (Rocks: rocks, Index: index))
                .GroupBy(rocksWithIndex => rocksWithIndex.Rocks)
                .Where(group => group.Count() > 1)
                .First()
                .Select(group => group.Index)
                .ToList();

            int spinsLeft = (1000000000 - duplicatedSpinIndices[0]) % (duplicatedSpinIndices[1] - duplicatedSpinIndices[0]) - 1;
            for (int spinIndex = 0; spinIndex < spinsLeft; ++spinIndex)
            {
                roundedRocks = SpinCycle(roundedRocks, cubeShapedRocks);
            }

            // The amount of load caused by a single rounded rock (O) is equal to the number of rows from the rock to the
            // south edge of the platform, including the row the rock is on. (Cube-shaped rocks (#) don't contribute to load.)
            // What is the total load on the north support beams?
            int output = roundedRocks
                .Select(rock => BottomRightCorner.Row - rock.Row + 1)
                .Sum();

            Console.WriteLine("Solution: {0}.", output);
        }

        // Each cycle tilts the platform four times so that the rounded rocks roll north, then west, then south, then east.
        // After one cycle, the platform will have finished rolling the rounded rocks in those four directions in that order.
        private static HashSet<Coords> SpinCycle(HashSet<Coords> roundedRocks, HashSet<Coords> cubeShapedRocks)
        {
            HashSet<Coords> newRoundedRocks;
            newRoundedRocks = Tilt(Directions.N, roundedRocks, cubeShapedRocks);
            newRoundedRocks = Tilt(Directions.W, newRoundedRocks, cubeShapedRocks);
            newRoundedRocks = Tilt(Directions.S, newRoundedRocks, cubeShapedRocks);
            newRoundedRocks = Tilt(Directions.E, newRoundedRocks, cubeShapedRocks);
            return newRoundedRocks;
        }

        // After each tilt, the rounded rocks roll as far as they can before the platform tilts in the next direction.
        private static HashSet<Coords> Tilt(Directions direction, HashSet<Coords> roundedRocks, HashSet<Coords> cubeShapedRocks)
        {
            Dictionary<int, HashSet<int>> groupedRoundedRocks, groupedCubeShapedRocks;
            if (direction == Directions.N || direction == Directions.S)
            {
                groupedRoundedRocks = GroupRocksByColumn(roundedRocks);
                groupedCubeShapedRocks = GroupRocksByColumn(cubeShapedRocks);
            }
            else
            {
                groupedRoundedRocks = GroupRocksByRow(roundedRocks);
                groupedCubeShapedRocks = GroupRocksByRow(cubeShapedRocks);
            }

            return Enumerable
                .Range(0, BottomRightCorner.Column + 1)
                .Select(columnIndex => TiltSingleRowOrColumn(direction, columnIndex, groupedRoundedRocks, groupedCubeShapedRocks))
                .Aggregate((lhs, rhs) => lhs.Union(rhs).ToHashSet());
        }

        private static HashSet<Coords> TiltSingleRowOrColumn(
            Directions direction,
            int currentIndex,
            Dictionary<int, HashSet<int>> groupedRoundedRocks,
            Dictionary<int, HashSet<int>> groupedCubeShapedRocks)
        {
            if (!groupedRoundedRocks.ContainsKey(currentIndex))
            {
                return new();
            }

            HashSet<int> cubeShapedRocksInTheCurrentRowOrColumn = groupedCubeShapedRocks.ContainsKey(currentIndex) ?
                groupedCubeShapedRocks[currentIndex] :
                new();

            int upperLimit = ((direction == Directions.N || direction == Directions.S) ? BottomRightCorner.Column : BottomRightCorner.Row) + 1;
            IEnumerable<int> lowerLimits = LowerLimitHashset.Union(cubeShapedRocksInTheCurrentRowOrColumn);
            IEnumerable<int> upperLimits = cubeShapedRocksInTheCurrentRowOrColumn.Union(new HashSet<int>() { upperLimit });
            HashSet<(int LowerLimit, int UpperLimit)> ranges = lowerLimits
                .Zip(upperLimits, (lowerLimit, upperLimit) => (lowerLimit, upperLimit))
                .ToHashSet();

            IEnumerable<IGrouping<(int LowerLimit, int UpperLimit), int>> groupedRoundedRocksInEachRange = groupedRoundedRocks[currentIndex]
                .GroupBy(groupIndex => ranges
                    .FirstOrDefault(range => groupIndex > range.LowerLimit && groupIndex <= range.UpperLimit));

            IEnumerable<int> newRoundedRocksInTheCurrentRowOrColumn;
            if (direction == Directions.N || direction == Directions.W)
            {
                newRoundedRocksInTheCurrentRowOrColumn = groupedRoundedRocksInEachRange
                    .Select(group => (StartRange: group.Key.LowerLimit + 1, Count: group.Count()))
                    .SelectMany(startRangeAndCount => Enumerable
                        .Range(startRangeAndCount.StartRange, startRangeAndCount.Count));
            }
            else
            {
                newRoundedRocksInTheCurrentRowOrColumn = groupedRoundedRocksInEachRange
                    .Select(group => (StartRange: group.Key.UpperLimit - 1, Count: group.Count()))
                    .SelectMany(startRangeAndCount => Enumerable
                        .Range(0, startRangeAndCount.Count)
                        .Select(index => startRangeAndCount.StartRange - index));
            }

            return newRoundedRocksInTheCurrentRowOrColumn
                .ToHashSet()
                .Select(rowIndex => 
                    (direction == Directions.N || direction == Directions.S) ?
                    new Coords(rowIndex, currentIndex) :
                    new Coords(currentIndex, rowIndex))
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

        private static Dictionary<int, HashSet<int>> GroupRocksByRow(HashSet<Coords> rocks) => rocks
            .GroupBy(rock => rock.Row)
            .Select(group => new KeyValuePair<int, HashSet<int>>(
                group.Key,
                group.Select(rock => rock.Column).ToHashSet()))
            .ToDictionary(groupedRocks => groupedRocks.Key, groupedRocks => groupedRocks.Value);

        private static readonly HashSet<int> LowerLimitHashset = new() { -1 };
        private static Coords BottomRightCorner = new();
    }
}
