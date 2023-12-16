using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2023.Day16
{
    public class Day16A : IDay
    {

        public void Run()
        {
            // You note the layout of the contraption (your puzzle input).
            List<string> input = File.ReadAllLines(@"..\..\..\Day16\Day16.txt").ToList();

            Dictionary<Coords, char> layoutOfContraption = input
                .SelectMany((row, rowIndex) => row
                    .ToCharArray()
                    .Select((value, colIndex) => new KeyValuePair<Coords, char>(
                        new Coords(rowIndex, colIndex), value)))
                .ToDictionary(coordsWithValue => coordsWithValue.Key, pair => pair.Value);

            // With the beam starting in the top-left heading right, how many tiles end up being energized?
            int output = CountEnergizedTiles(layoutOfContraption);

            Console.WriteLine("Solution: {0}.", output);
        }

        static int CountEnergizedTiles(Dictionary<Coords, char> layout)
        {
            Coords bottomRightCorner = new(layout.Keys.Max(tile => tile.Row), layout.Keys.Max(tile => tile.Column));

            // The beam enters in the top-left corner from the left and heading to the right. 
            HashSet<Beam> analysedBeams = new();
            Stack<Beam> beams = new();
            Coords tileRightBeforeEntry = new Coords(0, 0) - DirToCoords[Dir.Right];
            Beam beamRightBeforeEntry = new(tileRightBeforeEntry, Dir.Right); // In case the first tile is not an empty space.
            beams.Push(beamRightBeforeEntry);

            while (beams.Any())
            {
                Beam currentBeam = beams.Pop();
                if (analysedBeams.Contains(currentBeam))
                {
                    continue;
                }
                analysedBeams.Add(currentBeam);

                Coords newTile = currentBeam.Tile + DirToCoords[currentBeam.Direction];
                if (!IsInsideGrid(newTile, bottomRightCorner))
                {
                    continue;
                }

                // If the beam encounters empty space (.), it continues in the same direction.
                // If the beam encounters the pointy end of a splitter (| or -), the beam passes through the splitter as if the splitter were empty space.
                // For instance, a rightward-moving beam that encounters a - splitter would continue in the same direction.
                List<Dir> newDirections = new(){ currentBeam.Direction };

                char currentTileType = layout[newTile];
                if (currentTileType == '/' || currentTileType == '\\')
                {
                    // If the beam encounters a mirror (/ or \), the beam is reflected 90 degrees depending on the angle of the mirror.
                    // For instance, a rightward-moving beam that encounters a / mirror would continue upward in the mirror's column,
                    // while a rightward-moving beam that encounters a \ mirror would continue downward from the mirror's column.
                    if (currentTileType == '/')
                    {
                        newDirections[0] = currentBeam.Direction switch
                        {
                            Dir.Right => Dir.Up,
                            Dir.Up => Dir.Right,
                            Dir.Left => Dir.Down,
                            _ => Dir.Left,
                        };
                    }
                    else
                    {
                        newDirections[0] = currentBeam.Direction switch
                        {
                            Dir.Right => Dir.Down,
                            Dir.Down => Dir.Right,
                            Dir.Left => Dir.Up,
                            _ => Dir.Left
                        };
                    }
                }
                else if ((currentTileType == '-' && (currentBeam.Direction == Dir.Up || currentBeam.Direction == Dir.Down)) ||
                    (currentTileType == '|' && (currentBeam.Direction == Dir.Right || currentBeam.Direction == Dir.Left)))
                {
                    // If the beam encounters the flat side of a splitter (| or -),
                    // the beam is split into two beams going in each of the two directions the splitter's pointy ends are pointing.
                    // For instance, a rightward-moving beam that encounters a | splitter would split into two beams:
                    // one that continues upward from the splitter's column and one that continues downward from the splitter's column.
                    newDirections = currentBeam.Direction switch
                    {
                        Dir.Right or Dir.Left => new List<Dir>() { Dir.Down, Dir.Up },
                        _ => new List<Dir>() {  Dir.Right, Dir.Left }
                    };
                }

                newDirections.ForEach(newDirection => beams.Push(new Beam(newTile, newDirection)));
            }

            // A tile is energized if that tile has at least one beam pass through it, reflect in it, or split in it.
            // With the beam starting in the top-left heading right, how many tiles end up being energized?
            return analysedBeams
                .Where(beam => IsInsideGrid(beam.Tile, bottomRightCorner))
                .Select(beam => beam.Tile)
                .Distinct()
                .Count();
        }

        static bool IsInsideGrid(Coords tile, Coords bottomRightCorner) =>
            tile.Row >= 0 &&
            tile.Column >= 0 &&
            tile.Row <= bottomRightCorner.Row &&
            tile.Column <= bottomRightCorner.Column;

        public enum Dir { Up, Down, Left, Right };

        public static readonly Dictionary<Dir, Coords> DirToCoords = new()
        {
            { Dir.Up, new Coords(-1, 0) },
            { Dir.Down, new Coords(1, 0) },
            { Dir.Left, new Coords(0, -1) },
            { Dir.Right, new Coords(0, 1) },
        };

        public class Beam
        {
            public Coords Tile;
            public Dir Direction;

            public Beam(Coords tile, Dir direction)
            {
                Tile = tile;
                Direction = direction;
            }

            public override int GetHashCode() => (Tile.GetHashCode() << 16) ^ Direction.GetHashCode();
            public override bool Equals(object? obj) =>
                (obj != null) &&
                Tile.Equals(((Beam)obj).Tile) &&
                (Direction == ((Beam)obj).Direction);
        }
    }
}
