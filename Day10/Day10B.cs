using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2023.Day10
{
    public class Day10B : IDay
    {
        public enum Directions { N, S, W, E };

        public static readonly Dictionary<Directions, Coords> DirectionsToCoords = new()
        {
            { Directions.N, new Coords(-1, 0) },
            { Directions.S, new Coords(1, 0) },
            { Directions.W, new Coords(0, -1) },
            { Directions.E, new Coords(0, 1) },
        };

        public void Run()
        {
            // Scanning the area, you discover that the entire field you're standing on is densely packed with pipes;
            // it was hard to tell at first because they're the same metallic silver color as the "ground".
            // You make a quick sketch of all of the surface pipes you can see (your puzzle input).
            List<string> input = File.ReadAllLines(@"..\..\..\Day10\Day10.txt").ToList();

            Dictionary<Coords, char> map = input
                .SelectMany((row, rowIndex) => row
                    .ToCharArray()
                    .Select((value, colIndex) => new KeyValuePair<Coords, char>(
                        new Coords(rowIndex, colIndex), value)))
                .ToDictionary(coordsWithValue => coordsWithValue.Key, pair => pair.Value);

            // The pipes are arranged in a two-dimensional grid of tiles:
            // - | is a vertical pipe connecting north and south.
            // - - is a horizontal pipe connecting east and west.
            // - L is a 90-degree bend connecting north and east.
            // - J is a 90-degree bend connecting north and west.
            // - 7 is a 90-degree bend connecting south and west.
            // - F is a 90-degree bend connecting south and east.
            Dictionary<char, List<Directions>> directionsOfPipes = new()
            {
                { '|', new List<Directions>() { Directions.N, Directions.S } },
                { '-', new List<Directions>() { Directions.E, Directions.W } },
                { 'L', new List<Directions>() { Directions.N, Directions.E } },
                { 'J', new List<Directions>() { Directions.N, Directions.W } },
                { '7', new List<Directions>() { Directions.S, Directions.W } },
                { 'F', new List<Directions>() { Directions.S, Directions.E } },
            };

            // - S is the starting position of the animal; there is a pipe on this tile, but your sketch doesn't show what shape the pipe has.
            Coords startingPosition = map.FirstOrDefault(coordsWithValue => coordsWithValue.Value == 'S').Key;

            // - . is ground; there is no pipe in this tile.
            Dictionary<Coords, char> mapWithoutGround = map
                .Where(coordsWithValue => coordsWithValue.Value != '.')
                .ToDictionary(coordsWithValue => coordsWithValue.Key, coordsWithValue => coordsWithValue.Value);

            // Find the single giant loop starting at S.
            HashSet<Coords> giantLoop = new();

            // You can still figure out which pipes form the main loop: they're the ones connected to S, pipes those pipes connect to,
            // pipes those pipes connect to, and so on. Every pipe in the main loop connects to its two neighbors
            // (including S, which will have exactly two pipes connecting to it, and which is assumed to connect back to those two pipes).
            Queue<Coords> pipesToProcess = new();
            pipesToProcess.Enqueue(startingPosition);

            while (pipesToProcess.Count > 0) // Perform breadth-first search (BFS).
            {
                Coords currentPipe = pipesToProcess.Dequeue();
                giantLoop.Add(currentPipe);

                char currentPipeType = mapWithoutGround[currentPipe];
                List<Directions> neighborDirections = DirectionsToCoords.Keys.ToList();
                if (currentPipeType != 'S')
                {
                    neighborDirections = directionsOfPipes[currentPipeType];
                }

                foreach (Directions nextDirections in neighborDirections)
                {
                    Coords nextPipe = currentPipe + DirectionsToCoords[nextDirections];
                    if (mapWithoutGround.ContainsKey(nextPipe) &&
                        !giantLoop.Contains(nextPipe) &&
                        directionsOfPipes[mapWithoutGround[nextPipe]].Contains(OppositeDirection(nextDirections)))
                    {
                        pipesToProcess.Enqueue(nextPipe);
                    }
                }
            }

            // You quickly reach the farthest point of the loop, but the animal never emerges. Maybe its nest is within the area enclosed by the loop?
            // To determine whether it's even worth taking the time to search for such a nest, you should calculate how many tiles are contained within the loop.
            // Figure out whether you have time to search for the nest by calculating the area within the loop. How many tiles are enclosed by the loop?
            int output = map
                .Select(coordsWithValue => coordsWithValue.Key)
                .Where(coords => !giantLoop.Contains(coords))
                .Where(outOfLoopCoords => IsInside(outOfLoopCoords, map, giantLoop))
                .Count();

            Console.WriteLine("Solution: {0}.", output);
        }

        private static bool IsInside(Coords coords, Dictionary<Coords, char> map, HashSet<Coords> giantLoop)
        { // Check if the number of vertially enclosing pipe elelents (|, J, L) to the left is odd.
            int numOfVerticallyEnclosingPipesToTheLeft = Enumerable 
                .Range(0, coords.Column)
                .Select(column => new Coords(coords.Row, column))
                .Where(coordsToTheLeft => giantLoop.Contains(coordsToTheLeft))
                .Select(pipeElementsToTheLeft => map[pipeElementsToTheLeft])
                .Count(pipeToTheLeft => pipeToTheLeft == '|' || pipeToTheLeft == 'J' || pipeToTheLeft == 'L');
            return numOfVerticallyEnclosingPipesToTheLeft % 2 == 1;
        }

        private static Directions OppositeDirection(Directions direction) => direction switch
        {
            Directions.N => Directions.S,
            Directions.S => Directions.N,
            Directions.W => Directions.E,
            Directions.E => Directions.W,
            _ => throw new ArgumentException("Invalid direction"),
        };
    }
}
