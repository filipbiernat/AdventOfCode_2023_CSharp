namespace AdventOfCode2023.Day14
{
    public class Coords
    {
        public int Row;
        public int Column;

        public Coords(int row = 0, int column = 0)
        {
            Row = row;
            Column = column;
        }

        public override string ToString() => $"({Row}, {Column})";
        public override int GetHashCode() => (Row.GetHashCode() << 2) ^ Column.GetHashCode();
        public override bool Equals(object? obj) =>
            (obj != null) &&
            (Row == ((Coords)obj).Row) &&
            (Column == ((Coords)obj).Column);
    }
}
