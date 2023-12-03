namespace AdventOfCode2023.Day3
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

        public bool IsAdjacent(Coords other, int length)
        {
            bool isAdjacentVertically = (Row - other.Row <= 1) && (other.Row - Row <= 1);
            bool isAdjacentHorizontally = (Column - other.Column <= length) && (other.Column - Column <= 1);
            return isAdjacentVertically && isAdjacentHorizontally;
        }

        public override int GetHashCode() => (Row.GetHashCode() << 2) ^ Column.GetHashCode();
        public override bool Equals(object? obj) =>
            (obj != null) &&
            (Row == ((Coords)obj).Row) &&
            (Column == ((Coords)obj).Column);
    }
}
