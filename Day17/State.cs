namespace AdventOfCode2023.Day17
{
    public class State
    {
        public Coords Block;
        public Coords Direction;
        public int StepsInDirection;

        public State(Coords block, Coords direction, int stepsInDirection)
        {
            Block = block;
            Direction = direction;
            StepsInDirection = stepsInDirection;
        }

        public override int GetHashCode() => (Block.GetHashCode() << 16) ^ (Direction.GetHashCode() << 8) ^ StepsInDirection.GetHashCode();
        public override bool Equals(object? obj) =>
            (obj != null) &&
            (Block == ((State)obj).Block) &&
            (Direction == ((State)obj).Direction) &&
            (StepsInDirection == ((State)obj).StepsInDirection);
    }
}
