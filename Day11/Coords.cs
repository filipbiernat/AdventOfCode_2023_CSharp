namespace AdventOfCode2023.Day11
{
    public class Coords
    {
        public long Row;
        public long Column;

        public Coords(long row = 0, long column = 0)
        {
            Row = row;
            Column = column;
        }

        public static long ManhattanDistance(Coords lhs, Coords rhs) => Math.Abs(lhs.Row - rhs.Row) + Math.Abs(lhs.Column - rhs.Column);
        public static Coords operator +(Coords lhs, Coords rhs) => new(lhs.Row + rhs.Row, lhs.Column + rhs.Column);
        public static Coords operator *(Coords lhs, long rhs) => new(lhs.Row * rhs, lhs.Column * rhs);
        public override int GetHashCode() => (Row.GetHashCode() << 2) ^ Column.GetHashCode();
        public override bool Equals(object? obj) =>
            (obj != null) &&
            (Row == ((Coords)obj).Row) &&
            (Column == ((Coords)obj).Column);
    }
}
