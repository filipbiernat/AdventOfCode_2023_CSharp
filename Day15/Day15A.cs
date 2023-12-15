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
    public class Day15A : IDay
    {
        public void Run()
        {
            // The initialization sequence (your puzzle input) is a comma-separated list of steps to start the Lava Production Facility.
            // Ignore newline characters when parsing the initialization sequence.
            List<string> input = File.ReadAllText(@"..\..\..\Day15\Day15.txt").Split(",").ToList();

            // Run the HASH algorithm on each step in the initialization sequence. What is the sum of the results? 
            int output = input.Select(RunHashAlgorithm).Sum();

            Console.WriteLine("Solution: {0}.", output);
        }

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
