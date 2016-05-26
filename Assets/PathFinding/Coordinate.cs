using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFinding
{
    public class Coordinate
    {
        public Coordinate(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }

        public Coordinate(Vector3 position)
        {
            this.Row = coordinateToCell(Mathf.Round(position.z)) - 1;
            this.Column = coordinateToCell(Mathf.Round(position.x)) - 1;
        }

        public Coordinate(Coordinate previousCoord, char direction)
        {

            switch (direction)
            {
                case 'N':
                    this.Row = previousCoord.Row;
                    this.Column = previousCoord.Column + 1;
                    break;

                case 'S':
                    this.Row = previousCoord.Row;
                    this.Column = previousCoord.Column - 1;
                    break;

                case 'E':
                    this.Row = previousCoord.Row - 1;
                    this.Column = previousCoord.Column;
                    break;

                case 'W':
                    this.Row = previousCoord.Row + 1;
                    this.Column = previousCoord.Column;
                    break;

                default:
                    break;
            }


        }

        public int Row { get; set; }
        public int Column { get; set; }

        public int distanceFromCoord(int row, int column)
        {
            return Math.Abs(row - this.Row) + Math.Abs(column - this.Column);
        }

        public override bool Equals(System.Object coordinate)
        {
            if (coordinate == null) return false;
            return ((Coordinate)coordinate).Column == this.Column && ((Coordinate)coordinate).Row == this.Row;
        }

        public override string ToString()
        {
            return "[" + Row + "," + Column + "]";
        }

        public Vector3 toVector3D(float yValue)
        {
            int moveToX = cellToCoordinate(Column + 1);
            int moveToY = cellToCoordinate(Row + 1);

            return new Vector3(moveToX, yValue, moveToY);
        }

        public char getDirection(Coordinate coordinate)
        {
            if (coordinate.Column > this.Column)
                return 'N';

            if (coordinate.Column < this.Column)
                return 'S';

            if (coordinate.Row < this.Row)
                return 'E';

            if (coordinate.Row > this.Row)
                return 'W';

            return ' ';
        }

        public bool isWithinBoundaries(int gridSize)
        {
            if (this.Row <= 0)
                return false;

            if (this.Column <= 0)
                return false;

            if (this.Row > gridSize)
                return false;

            if (this.Column > gridSize)
                return false;

            return true;
        }

        public static int cellToCoordinate(int val)
        {
            return val - (10 - (val - 1));
        }

        public static int coordinateToCell(float val)
        {
            int roundedVal = (int)Mathf.Round(val);
            return (int)(roundedVal + 10) - ((int)Mathf.Round((roundedVal + 10) / 2));
        }
    }
}
