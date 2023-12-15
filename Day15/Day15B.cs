using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2023.Day15
{
    public class Day15B : IDay
    {
        // The book goes on to describe a series of 256 boxes numbered 0 through 255.
        // The boxes are arranged in a line starting from the point where light enters the facility. 
        private static readonly Dictionary<int, List<(string Label, int LenseFocalLength)>> Boxes = Enumerable
            .Range(0, 256)
            .Select(boxId => new KeyValuePair<int, List<(string Label, int Number)>>(boxId, new()))
            .ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);

        public void Run()
        {
            // The initialization sequence (your puzzle input) is a comma-separated list of steps to start the Lava Production Facility.
            // Ignore newline characters when parsing the initialization sequence.
            List<string> input = File.ReadAllText(@"..\..\..\Day15\Day15.txt").Split(",").ToList();

            // Follow the initialization sequence.
            input.ForEach(PerformStep);

            // To confirm that all of the lenses are installed correctly, add up the focusing power of all of the lenses.
            // What is the focusing power of the resulting lens configuration?
            int output = Boxes.Select(CalculateFocusingPower).Sum();

            Console.WriteLine("Solution: {0}.", output);
        }

        // The book goes on to explain how to perform each step in the initialization sequence,
        // a process it calls the Holiday ASCII String Helper Manual Arrangement Procedure, or HASHMAP for short.
        private void PerformStep(string description)
        {
            // The label will be immediately followed by a character that indicates the operation to perform: either an equals sign (=) or a dash (-).
            List<string> operands = description
                .Split(new char[] { '=', '-' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            // Each step begins with a sequence of letters that indicate the label of the lens on which the step operates.
            string label = operands[0];

            // The result of running the HASH algorithm on the label indicates the correct box for that step.
            int boxId = RunHashAlgorithm(label);
            bool currentBoxAlreadyHasCurrentLabel = Boxes[boxId].Any(lens => lens.Label == label);

            if (operands.Count == 1)
            {
                // If the operation character is a dash (-), go to the relevant box and remove the lens with the given label if it is present in the box.
                // Then, move any remaining lenses as far forward in the box as they can go without changing their order,
                // filling any space made by removing the indicated lens. (If no lens in that box has the given label, nothing happens.)
                if (currentBoxAlreadyHasCurrentLabel)
                {
                    Boxes[boxId].Remove(Boxes[boxId].Find(lens => lens.Label == label));
                }
            }
            else
            {
                // If the operation character is an equals sign (=), it will be followed by a number indicating the focal length of the lens that needs to go into
                // the relevant box; be sure to use the label maker to mark the lens with the label given in the beginning of the step so you can find it later.
                // There are two possible situations:
                if (currentBoxAlreadyHasCurrentLabel)
                {
                    // - If there is already a lens in the box with the same label, replace the old lens with the new lens:
                    //   remove the old lens and put the new lens in its place, not moving any other lenses in the box.
                    int lensId = Boxes[boxId].FindIndex(lens => lens.Label == label);
                    Boxes[boxId][lensId] = (Boxes[boxId][lensId].Label, int.Parse(operands[1]));
                }
                else
                {
                    // - If there is not already a lens in the box with the same label, add the lens to the box immediately behind any lenses already in the box.
                    //   Don't move any of the other lenses when you do this. If there aren't any lenses in the box, the new lens goes all the way to the front of the box.
                    Boxes[boxId].Add((label, int.Parse(operands[1])));
                }
            }
        }

        // The focusing power of a single lens is the result of multiplying together:
        // - One plus the box number of the lens in question.
        // - The slot number of the lens within the box: 1 for the first lens, 2 for the second lens, and so on.
        // - The focal length of the lens.
        private static int CalculateFocusingPower(KeyValuePair<int, List<(string Label, int LenseFocalLength)>> box) => box
            .Value
            .Select((lenseFocalLength, lensIndex) =>
                (1 + box.Key) *
                (lensIndex + 1) *
                lenseFocalLength.LenseFocalLength)
            .Sum();

        // The HASH algorithm is a way to turn any string of characters into a single number in the range 0 to 255.
        // To run the HASH algorithm on a string, start with a current value of 0.
        // Then, for each character in the string starting from the beginning:
        //  - Determine the ASCII code for the current character of the string.
        //  - Increase the current value by the ASCII code you just determined.
        //  - Set the current value to itself multiplied by 17.
        //  - Set the current value to the remainder of dividing itself by 256.
        private static int RunHashAlgorithm(string stringOfCharacters) => stringOfCharacters
            .Aggregate(0, (currentValue, asciiCode) =>
            {
                int increasedValue = currentValue + asciiCode;
                int multipliedValue = increasedValue * 17;
                return multipliedValue & 0xFF; // Modulo 256.
            });
    }
}
