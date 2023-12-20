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
    public class Day19B : IDay
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

            // One of the Elves comes up with a new plan: rather than sort parts individually through all of these workflows,
            // maybe you can figure out in advance which combinations of ratings will be accepted or rejected.
            List<Condition> conditions = workflows
                .Values
                .SelectMany(workflow => workflow.ConditionsWithDestinations
                    .Select(condition => condition.Cond))
                .ToList();

            // Each of the four ratings (x, m, a, s) can have an integer value ranging from a minimum of 1 to a maximum of 4000.
            List<(int value, int delta)> xAndDeltaX = FindValuesToInvestigateAndCalculateDeltas(property: "x", conditions);
            List<(int value, int delta)> mAndDeltaM = FindValuesToInvestigateAndCalculateDeltas(property: "m", conditions);
            List<(int value, int delta)> aAndDeltaA = FindValuesToInvestigateAndCalculateDeltas(property: "a", conditions);
            List<(int value, int delta)> sAndDeltaS = FindValuesToInvestigateAndCalculateDeltas(property: "s", conditions);

            // Of all possible distinct combinations of ratings, your job is to figure out which ones will be accepted.
            long sum = 0;
            object lockObj = new();
            Parallel.ForEach(xAndDeltaX, (xAndDeltaXItem) =>
            {
                long partialSum = 0;
                int x = xAndDeltaXItem.value;
                int deltaX = xAndDeltaXItem.delta;

                foreach ((int m, int deltaM) in mAndDeltaM)
                {
                    foreach ((int a, int deltaA) in aAndDeltaA)
                    {
                        foreach ((int s, int deltaS) in sAndDeltaS)
                        {
                            Dictionary<string, int> part = new() { { "x", x }, { "m", m }, { "a", a }, { "s", s } };
                            if (EvaluateWorkflow(workflows, part))
                            {
                                partialSum += (long)deltaX * deltaM * deltaA * deltaS;
                            }
                        }
                    }
                }
                lock (lockObj)
                {
                    sum += partialSum;
                }
            });

            // How many distinct combinations of ratings will be accepted by the Elves' workflows?
            long output = sum;

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

        private static List<(int Value, int Delta)> FindValuesToInvestigateAndCalculateDeltas(string property, List<Condition> conditions)
        {
            List<int> valuesToInvestigate = conditions
                .Where(condition => condition.Property == property)
                .Select(condition => condition.Operator == "<" ? (condition.Value - 1) : condition.Value)
                .Concat(new List<int>() { 0, 4000 })
                .OrderBy(valueToInvestigate => valueToInvestigate)
                .Distinct()
                .ToList();

            List<(int Value, int Delta)> valuesToInvestigateWithDeltas =  valuesToInvestigate
                .Skip(1)
                .Zip(valuesToInvestigate, (lhs, rhs) => (Value: lhs, Delta: lhs - rhs))
                .ToList();

            return valuesToInvestigateWithDeltas;
        }
    }
}
