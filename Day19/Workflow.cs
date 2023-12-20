using MathNet.Numerics.LinearAlgebra.Solvers;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Day19
{
    // Each workflow has a name and contains a list of rules; each rule specifies a condition and where to send the part if the condition is true.
    // The first rule that matches the part being considered is applied immediately, and the part moves on to the destination described by the rule.
    // (The last rule in each workflow has no condition and always applies if reached.)
    public class Workflow
    {
        public List<(Condition Cond, string Dest)> ConditionsWithDestinations;
        public string FinalDestination;

        public Workflow(string input)
        {
            List<string> splitInput = input.Split(',').ToList();

            ConditionsWithDestinations = splitInput
                .SkipLast(1)
                .Select(conditionWithDestination => conditionWithDestination
                    .Split(':')
                    .ToList())
                .Select(conditionWithDestination => (Cond: new Condition(conditionWithDestination[0]), Dest: conditionWithDestination[1]))
                .ToList();

            FinalDestination = splitInput.Last();
        }

        // If a part is sent to another workflow, it immediately switches to the start of that workflow instead and never returns.
        // If a part is accepted (sent to A) or rejected (sent to R), the part immediately stops any further processing.
        public string Evaluate(Dictionary<string, int> part)
        {
            foreach ((Condition condition, string destination) in ConditionsWithDestinations)
            {
                if (condition.Evaluate(part))
                {
                    return destination;
                }
            }
            return FinalDestination;
        }
    }
}
