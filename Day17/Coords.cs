namespace AdventOfCode2023.Day17
{
    public class Coords
    {
        public static readonly List<Coords> Directions = new()
        {
            new Coords(-1, 0),
            new Coords(1, 0),
            new Coords(0, -1),
            new Coords(0, 1),
        };

        public int Row;
        public int Column;

        public Coords(int row = 0, int column = 0)
        {
            Row = row;
            Column = column;
        }

        public Coords Opposite() => new(-Row, -Column);

        public static Coords operator +(Coords lhs, Coords rhs) => new(lhs.Row + rhs.Row, lhs.Column + rhs.Column);
        public static Coords operator -(Coords lhs, Coords rhs) => new(lhs.Row - rhs.Row, lhs.Column - rhs.Column);
        public static bool operator ==(Coords lhs, Coords rhs) => lhs.Row == rhs.Row && lhs.Column == rhs.Column;
        public static bool operator !=(Coords lhs, Coords rhs) => lhs.Row != rhs.Row || lhs.Column != rhs.Column;
        public override int GetHashCode() => (Row.GetHashCode() << 2) ^ Column.GetHashCode();
        public override bool Equals(object? obj) =>
            (obj != null) &&
            (Row == ((Coords)obj).Row) &&
            (Column == ((Coords)obj).Column);
    }
}
