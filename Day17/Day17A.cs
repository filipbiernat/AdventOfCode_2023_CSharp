using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using AdventOfCode2023.Day5;
using System.Data.Common;

namespace AdventOfCode2023.Day17
{
    public class Day17A : IDay
    {
        public void Run()
        {
            // You'll need to find the best way to get the crucible from the lava pool to the machine parts factory.
            // You need to minimize heat loss while choosing a route that doesn't require the crucible to go in a straight line for too long.
            // Fortunately, the Elves here have a map (your puzzle input) that uses traffic patterns, ambient temperature, and hundreds of
            // other parameters to calculate exactly how much heat loss can be expected for a crucible entering any particular city block.
            List<string> input = File.ReadAllLines(@"..\..\..\Day17\Day17.txt").ToList();

            // Each city block is marked by a single digit that represents the amount of heat loss if the crucible enters that block.
            Dictionary<Coords, int> heatLossMap = input
                .SelectMany((row, rowIndex) => row
                    .ToCharArray()
                    .Select((value, colIndex) => new KeyValuePair<Coords, int>(
                        new Coords(rowIndex, colIndex), (int)char.GetNumericValue(value))))
                .ToDictionary(coordsWithValue => coordsWithValue.Key, pair => pair.Value);

            // The destination, the machine parts factory, is the bottom-right city block.
            Coords bottomRightBlock = new(
                heatLossMap.Max(blockWithHeatLoss => blockWithHeatLoss.Key.Row),
                heatLossMap.Max(blockWithHeatLoss => blockWithHeatLoss.Key.Column));

            // Directing the crucible from the lava pool to the machine parts factory, but not moving more than
            // three consecutive blocks in the same direction, what is the least heat loss it can incur?
            int output = FindShortestPath(heatLossMap, bottomRightBlock);

            Console.WriteLine("Solution: {0}.", output);
        }
        public int FindShortestPath(Dictionary<Coords, int> heatLossMap, Coords bottomRightBlock)
        {
            // The starting point, the lava pool, is the top-left city block.
            State startingPoint = new(block: new Coords(0, 0), direction: new Coords(0, 0), stepsInDirection: 0);

            // Because you already start in the top-left block, you don't incur that block's heat loss unless you leave that block and then return to it.
            Dictionary<State, int> distance = new() { [startingPoint] = 0 };

            PriorityQueue<State, int> statesToVisit = new();
            statesToVisit.Enqueue(startingPoint, heatLossMap[startingPoint.Block]);

            HashSet<State> visitedStates = new();
            while (statesToVisit.Count > 0) // Dijkstra algorithm.
            {
                State currentState = statesToVisit.Dequeue();

                // The crucible also can't reverse direction; after entering each city block, it may only turn left, continue straight, or turn right.
                List<Coords> availableDirections = Coords
                    .Directions
                    .Where(availableDirection => availableDirection != currentState.Direction.Opposite())
                    .ToList();

                foreach (Coords nextDirection in availableDirections)
                {
                    State nextState = new(currentState.Block + nextDirection, nextDirection, stepsInDirection: 1);
                    if (!IsWithinHeatLossMap(nextState.Block, bottomRightBlock))
                    {
                        continue;
                    }

                    // Because it is difficult to keep the top-heavy crucible going in a straight line for very long,
                    // it can move at most three blocks in a single direction before it must turn 90 degrees left or right.
                    if (currentState.Direction == nextDirection)
                    {
                        nextState.StepsInDirection = currentState.StepsInDirection + 1;
                        if (nextState.StepsInDirection > 3)
                        {
                            continue;
                        }
                    }

                    if (!visitedStates.Contains(nextState))
                    {
                        int nextDistance = distance[currentState] + heatLossMap[nextState.Block];
                        if (!distance.ContainsKey(nextState) || nextDistance < distance[nextState])
                        {
                            statesToVisit.Enqueue(nextState, nextDistance);
                            distance[nextState] = nextDistance;
                        }
                    }
                }
            }
            return distance
               .Where(stateWithDistance => stateWithDistance.Key.Block == bottomRightBlock)
               .Min(stateWithDistance => stateWithDistance.Value);
        }


        private static bool IsWithinHeatLossMap(Coords currentBlock, Coords bottomRightBlock) =>
            currentBlock.Row >= 0 && currentBlock.Row <= bottomRightBlock.Row &&
            currentBlock.Column >= 0 && currentBlock.Column <= bottomRightBlock.Column;
    }
}
