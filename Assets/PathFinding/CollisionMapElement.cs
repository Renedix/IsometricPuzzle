using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFinding
{
    class CollisionMapElement
    {
        public bool Blocked { get; set; }
        public List<CollisionMapElement> Neighbours { get; set; }
        public List<CollisionMapElement> getUnblockedNeighbours()
        {
            return Neighbours.Where(coord => coord.Blocked == false).ToList();
        }

        public Coordinate ElementCoordinate { get; set; }

        public CollisionMapElement(int row, int column)
        {
            this.ElementCoordinate = new Coordinate(row, column);
        }

        public int DistanceFromGoal { get; set; }

        public void addNeighbour(CollisionMapElement newNeighbour)
        {
            if (Neighbours == null)
            {
                Neighbours = new List<CollisionMapElement>();
            }
            Neighbours.Add(newNeighbour);
        }

    }
}
