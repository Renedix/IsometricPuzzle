using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFinding
{
    class PlayerMovementPath
    {
        public List<Coordinate> CoordinatePath { get; set; }
        public int HeuristicValue { get; set; }
        public int DistanceTravelled { get { return getDistanceTravelled(); } }
        public Coordinate LastCoordinate { get { return getLastCoordinate(); } }

        public Coordinate StartCoordinate { get; set; }
        public Coordinate GoalCoordinate { get; set; }

        public int PathIterator;

        public PlayerMovementPath()
        {
            this.CoordinatePath = new List<Coordinate>();
            resetIterator();
        }

        public int getDistanceTravelled()
        {
            if (CoordinatePath == null)
                return 0;
            else
                return this.CoordinatePath.Count();
        }

        public Coordinate getLastCoordinate()
        {
            return CoordinatePath[CoordinatePath.Count() - 1];
        }

        public Coordinate getSecondToLastCoordinate()
        {
            if (CoordinatePath.Count() > 1)
            {
                return CoordinatePath[CoordinatePath.Count() - 2];
            }
            else
            {
                return null;
            }
        }

        public PlayerMovementPath Clone()
        {
            PlayerMovementPath newPath = new PlayerMovementPath();

            foreach (Coordinate coord in CoordinatePath)
            {
                newPath.CoordinatePath.Add(coord);
            }

            return newPath;
        }

        public Coordinate getNext()
        {
            if (this.CoordinatePath.Count<=PathIterator){
                return null;
            }

            PathIterator++;
            return this.CoordinatePath[PathIterator-1];
        }

        public void resetIterator()
        {
            this.PathIterator = 0;
        }
    }
}
