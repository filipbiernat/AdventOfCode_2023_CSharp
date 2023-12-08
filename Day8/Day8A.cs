using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Reflection.PortableExecutable;
using AdventOfCode2023.Day3;

namespace AdventOfCode2023.Day8
{
    public class Day8A : IDay
    {
        public void Run()
        {
            // One of the camel's pouches is labeled "maps" - sure enough, it's full of documents (your puzzle input) about how to navigate the desert.
            List<string> input = File.ReadAllLines(@"..\..\..\Day8\Day8.txt").ToList();

            // One of the documents contains a list of left/right instructions.
            List<char> instructions = input.First().ToList();

            // The rest of the documents seem to describe some kind of network of labeled nodes.
            Dictionary<string, Dictionary<char, string>> network = input
                .Skip(2)
                .Select(inputLine => Regex
                    .Matches(inputLine, "\\w{3}")
                    .Select(match => match.Value)
                    .ToList())
                .Select(nodesFromInputLine => new KeyValuePair<string, Dictionary<char, string>>(
                    nodesFromInputLine[0],
                    new Dictionary<char, string>
                    {
                        { 'L', nodesFromInputLine[1] },
                        { 'R', nodesFromInputLine[2] },
                    }))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            // After examining the maps for a bit, two nodes stick out: AAA and ZZZ.
            // You feel like AAA is where you are now, and you have to follow the left/right instructions until you reach ZZZ.
            int stepId = 0;
            for (string currentNode = "AAA"; currentNode != "ZZZ"; ++stepId)
            {
                // It seems like you're meant to use the left/right instructions to navigate the network.
                char instruction = instructions[stepId % instructions.Count];

                // Perhaps if you have the camel follow the same instructions, you can escape the haunted wasteland!
                currentNode = network[currentNode][instruction];
            }

            // How many steps are required to reach ZZZ?
            int output = stepId;

            Console.WriteLine("Solution: {0}.", output);
        }
    }
}
