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
    public class Day10A : IDay
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

            // How many steps along the loop does it take to get from the starting position to the point farthest from the starting position?
            int output = giantLoop.Count / 2;

            Console.WriteLine("Solution: {0}.", output);
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
