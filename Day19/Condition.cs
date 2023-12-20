using System.Text.RegularExpressions;

namespace AdventOfCode2023.Day19
{
    public class Condition
    {
        public string Property;
        public string Operator;
        public int Value;

        public Condition(string input)
        {
            List<string> splitCondition = Regex
                .Match(input, "([xmas])([<>=])(\\d+)", RegexOptions.Compiled)
                .Groups
                .Cast<Group>()
                .Select(group => group.Value)
                .Skip(1)
                .ToList();

            Property = splitCondition[0];
            Operator = splitCondition[1];
            Value = int.Parse(splitCondition[2]);
        }

        public bool Evaluate(Dictionary<string, int> part) => Operator switch
        {
            ">" => part[Property] > Value,
            _ => part[Property] < Value
        };
    }
}
