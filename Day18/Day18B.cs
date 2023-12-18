using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2023.Day18
{
    public class Day18B : IDay
    {
        public void Run()
        {
            // They aren't sure the lagoon will be big enough; they've asked you to take a look at the dig plan(your puzzle input).
            List<string> input = File.ReadAllLines(@"..\..\..\Day18\Day18.txt").ToList();

            List<(string Direction, long Steps)> digPlan = input.Select(ParseDigPlanEntry).ToList();

            // The digger starts in a 1 meter cube hole in the ground.
            Coords currentCube = new(0, 0);

            // To catch up with the large backlog of parts requests, the factory will also need a large supply of lava for a while;
            // the Elves have already started creating a large lagoon nearby for this purpose.
            List<Coords> lagoonPoints = new() { currentCube };
            long lagoonBoundaryPoints = 0;

            foreach ((string direction, long steps) in digPlan)
            {
                currentCube += Directions[direction] * steps;
                lagoonPoints.Add(currentCube);

                lagoonBoundaryPoints += steps;
            }

            long polygonArea = CalculatePolygonAreaWithShoelaceFormula(lagoonPoints);
            long interiorPoints = CalculateNumOfInteriorPointsUsingPicksTheorem(polygonArea, lagoonBoundaryPoints);

            // Elves follow this new dig plan, how many cubic meters of lava could the lagoon hold?
            long output = interiorPoints + lagoonBoundaryPoints;

            Console.WriteLine("Solution: {0}.", output);
        }

        private static long CalculatePolygonAreaWithShoelaceFormula(List<Coords> points)
        {
            long polygonArea = Enumerable
                .Range(0, points.Count - 1)
                .Sum(index => points[index].Row * points[index + 1].Column - points[index + 1].Row * points[index].Column);
            polygonArea += points[points.Count - 1].Row * points[0].Column - points[0].Row * points[points.Count - 1].Column;
            return Math.Abs(polygonArea) / 2;
        }

        private static long CalculateNumOfInteriorPointsUsingPicksTheorem(long polygonArea, long boundaryPoints) =>
            polygonArea - (boundaryPoints / 2) + 1;

        private (string Direction, long Steps) ParseDigPlanEntry(string entry)
        {
            string[] splitEntry = entry.Split();
            string color = splitEntry[2];

            // The first five hexadecimal digits encode the distance in meters as a five-digit hexadecimal number. 
            long steps = long.Parse(color.Substring(2, 5), System.Globalization.NumberStyles.HexNumber);

            // The last hexadecimal digit encodes the direction to dig: 0 means R, 1 means D, 2 means L, and 3 means U.
            string direction = color[color.Length - 2] switch
            {
                '0' => "R",
                '1' => "D",
                '2' => "L",
                _ => "U",
            };
            return (direction, steps);
        }

        // They then dig the specified number of meters up (U), down (D), left (L), or right (R), clearing full 1 meter cubes as they go.
        // The directions are given as seen from above, so if "up" were north, then "right" would be east, and so on.
        private static readonly Dictionary<string, Coords> Directions = new()
        {
            { "U", new Coords(-1, 0) },
            { "D", new Coords(1, 0) },
            { "L", new Coords(0, -1) },
            { "R", new Coords(0, 1) },
        };
    }
}
