using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;

namespace AdventOfCode2023.Day19
{
    public class Day19A : IDay
    {
        public void Run()
        {
            // The Elves ask if you can help sort a few parts and give you the list of workflows and some part ratings (your puzzle input).
            // The workflows are listed first, followed by a blank line, then the ratings of the parts the Elves would like you to sort. 
            List<string> input = File.ReadAllText(@"..\..\..\Day19\Day19.txt").Split("\r\n\r\n").ToList();

            // Each part is sent through a series of workflows that will ultimately accept or reject the part.
            // Each workflow has a name and contains a list of rules.
            Dictionary<string, Workflow> workflows = input
                .First()
                .Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(workFlowInput => workFlowInput
                    .Split(new[] { "{", "}" }, StringSplitOptions.RemoveEmptyEntries))
                .Select(splitWorkflow => new KeyValuePair<string, Workflow>(splitWorkflow[0], new Workflow(splitWorkflow[1])))
                .ToDictionary(nameWithWorkflow => nameWithWorkflow.Key, workflowWithValue => workflowWithValue.Value);

            List<Dictionary<string, int>> parts = input
                .Last()
                .Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(partInput => partInput
                    .Replace("{", "")
                    .Replace("}", "")
                    .Split(',')
                    .Select(splitInput => splitInput.Split('='))
                    .Select(splitInput => new KeyValuePair<string, int>(splitInput[0], int.Parse(splitInput[1])))
                    .ToDictionary(propertyWithValue => propertyWithValue.Key, propertyWithValue => propertyWithValue.Value))
                .ToList();

            // What do you get if you add together all of the rating numbers for all of the parts that ultimately get accepted?
            int output = parts.Select(part => (Part: part, Accepted: EvaluateWorkflow(workflows, part)))
                .Where(partWithAccepted => partWithAccepted.Accepted)
                .Select(partWithAccepted => partWithAccepted.Part)
                .Sum(part => part.Values.Sum());

            Console.WriteLine("Solution: {0}.", output);
        }

        private static bool EvaluateWorkflow(Dictionary<string, Workflow> workflows, Dictionary<string, int> part)
        {
            // All parts begin in the workflow named in.
            string workflowName = "in";

            // If a part is accepted (sent to A) or rejected (sent to R), the part immediately stops any further processing.
            while (workflowName != "A" && workflowName != "R")
            {
                workflowName = workflows[workflowName].Evaluate(part);
            }
            return workflowName == "A";
        }
    }
}
